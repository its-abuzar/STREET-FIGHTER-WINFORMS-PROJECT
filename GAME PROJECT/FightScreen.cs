using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Game_Project.Controllers;
using Game_Project.Interfaces;
using Game_Project.Managers;
using Game_Project.Models;

namespace Game_Project
{
    /// <summary>
    /// View / Orchestrator: owns the WinForms surface and wires up all managers.
    /// After refactoring this class only:
    ///   - Creates and injects dependencies
    ///   - Routes WinForms events (Paint, KeyDown, KeyUp, Tick) to the correct manager
    ///   - Implements IMatchObserver to react when the match ends
    /// All game logic lives in the Controllers and Managers folders.
    /// </summary>
    public partial class FightScreen : Form, IMatchObserver
    {
        // ── Injected / created dependencies ─────────────────────────────
        private readonly InputController    _input;
        private readonly MovementController _movement;
        private readonly CombatController   _combat;
        private readonly AIController       _ai;
        private readonly MatchStateManager  _matchState;
        private readonly StageManager       _stage;
        private readonly ProjectileManager  _projectiles;
        private readonly SoundManager       _sound;
        private readonly HUDRenderer        _hud;

        // ── Characters ─────────────────────────────────────────────────
        private readonly BaseCharacter _player;
        private readonly BaseCharacter _opponent;

        // ── UI controls ────────────────────────────────────────────────
        private PictureBox _canvas;
        private Panel      _playerHealthBar;
        private Panel      _opponentHealthBar;

        // ── Timers ─────────────────────────────────────────────────────
        private readonly Timer _gameLoopTimer;

        // ── Misc ───────────────────────────────────────────────────────
        private readonly bool   _vsMode;
        private readonly Random _rand = new Random();

        // Key bindings (static, reloaded each fight)
        public static KeyBindings CurrentBindings { get; private set; }
        static FightScreen() { CurrentBindings = new KeyBindings(); }

        // ─────────────────────────────────────────────────────────────────
        public FightScreen(string playerName, string opponentName, bool vsMode = false)
        {
            InitializeComponent();
            _vsMode = vsMode;

            // Reload bindings from file so Options changes take effect
            string configPath     = Path.Combine(Application.StartupPath, "Assets", "Config", "keybindings.txt");
            KeyBindings bindings  = new KeyBindings();
            if (File.Exists(configPath)) bindings.Load(configPath);
            CurrentBindings = bindings;

            // ── Build canvas (replaces the three Designer PictureBoxes) ─
            BuildCanvas();

            // ── Create characters ────────────────────────────────────────
            Point playerPos   = new Point(80, 0);
            Point opponentPos = new Point(ClientSize.Width - 200, 0);
            _player   = CharacterFactory.Create(playerName,   playerPos,   false);
            _opponent = CharacterFactory.Create(opponentName, opponentPos, true);

            // ── Model ────────────────────────────────────────────────────
            var matchModel = new MatchState();

            // ── Managers ─────────────────────────────────────────────────
            _sound       = new SoundManager();
            _stage       = new StageManager(_rand, () => _canvas?.Invalidate());
            _projectiles = new ProjectileManager(() => _canvas?.Invalidate());
            _hud         = new HUDRenderer();
            _matchState  = new MatchStateManager(matchModel);

            // ── Controllers (constructor injection) ──────────────────────
            _input    = new InputController(bindings, vsMode);
            _movement = new MovementController(_player, _opponent);
            _combat   = new CombatController(_player, _opponent, _matchState, _projectiles,
                                             () => _canvas?.Invalidate());
            _ai       = new AIController(_rand, _combat, _opponent);

            // ── Wire match-over observer ──────────────────────────────────
            _matchState.Initialise(_player.Name, _opponent.Name, this);

            // ── Load assets ───────────────────────────────────────────────
            _stage.Load();
            _projectiles.Load();
            _sound.StartStageMusic();

            // ── Build health bar panels ───────────────────────────────────
            BuildHealthBars();

            // ── Game-loop timer ───────────────────────────────────────────
            _gameLoopTimer          = new Timer { Interval = 16 };
            _gameLoopTimer.Tick    += GameLoop_Tick;
            _gameLoopTimer.Start();

            _matchState.StartTimer();

            // ── Input plumbing ────────────────────────────────────────────
            KeyPreview = true;
            Select();
            Focus();
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F1)
                    _matchState.State.DebugMode = !_matchState.State.DebugMode;
            };
        }

        // ─────────────────────────────────────────────────────────────────
        //  UI construction helpers
        // ─────────────────────────────────────────────────────────────────

        private void BuildCanvas()
        {
            // Remove the placeholder controls created by the Designer
            Controls.Remove(pbStage);
            Controls.Remove(pbPlayer);
            Controls.Remove(pbOpponent);
            pbStage.Dispose();
            pbPlayer.Dispose();
            pbOpponent.Dispose();

            _canvas             = new PictureBox();
            _canvas.Dock        = DockStyle.Fill;
            _canvas.BackColor   = Color.DarkGray;
            _canvas.Paint      += Canvas_Paint;
            Controls.Add(_canvas);
        }

        private void BuildHealthBars()
        {
            _playerHealthBar          = new Panel();
            _playerHealthBar.Location = new Point(50, 20);
            _playerHealthBar.Size     = new Size(280, 25);
            _playerHealthBar.BackColor = Color.Transparent;
            _playerHealthBar.Paint    += (s, e) =>
                _hud.DrawHealthBar(e.Graphics, _matchState.State.PlayerHealth,
                                   _playerHealthBar.Width, _playerHealthBar.Height);
            Controls.Add(_playerHealthBar);
            _playerHealthBar.BringToFront();

            _opponentHealthBar          = new Panel();
            _opponentHealthBar.Location = new Point(ClientSize.Width - 330, 20);
            _opponentHealthBar.Size     = new Size(280, 25);
            _opponentHealthBar.BackColor = Color.Transparent;
            _opponentHealthBar.Paint    += (s, e) =>
                _hud.DrawHealthBar(e.Graphics, _matchState.State.OpponentHealth,
                                   _opponentHealthBar.Width, _opponentHealthBar.Height);
            Controls.Add(_opponentHealthBar);
            _opponentHealthBar.BringToFront();

            // Invalidate health bars whenever match state changes
            _matchState.State.PlayerHealth   = 100;
            _matchState.State.OpponentHealth = 100;
        }

        // ─────────────────────────────────────────────────────────────────
        //  Paint
        // ─────────────────────────────────────────────────────────────────

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            // 1. Stage background
            _stage.Render(e.Graphics, _canvas.Width, _canvas.Height);

            // 2. Fireball
            _projectiles.Render(e.Graphics, _canvas.Width, _canvas.Height);

            // 3. Characters (back one first for correct layering)
            if (_player.Position.X > _opponent.Position.X)
            {
                DrawCharacter(e.Graphics, _opponent);
                DrawCharacter(e.Graphics, _player);
            }
            else
            {
                DrawCharacter(e.Graphics, _player);
                DrawCharacter(e.Graphics, _opponent);
            }

            // 4. Debug overlays
            if (_matchState.State.DebugMode)
                _hud.RenderDebug(e.Graphics, _player, _opponent, _combat.P1AttackLocked);

            // 5. HUD (names, timer, result)
            _hud.RenderHUD(e.Graphics, _canvas.Width, _matchState.State,
                           _player.Name, _opponent.Name);
        }

        private static void DrawCharacter(Graphics g, BaseCharacter c)
        {
            if (c?.CurrentFrame != null)
                g.DrawImage(c.CurrentFrame, c.Position);
        }

        // ─────────────────────────────────────────────────────────────────
        //  Game loop
        // ─────────────────────────────────────────────────────────────────

        private void GameLoop_Tick(object sender, EventArgs e)
        {
            if (_matchState.State.IsOver) return;

            int canvasW = ClientSize.Width;
            _movement.CanvasWidth  = canvasW;
            _combat.CanvasWidth    = canvasW;

            // Update fireball; pass opponent bounding rect for collision
            Rectangle oppRect = new Rectangle(_opponent.Position,
                _opponent.CurrentFrame?.Size ?? new Size(100, 120));
            _projectiles.Update(oppRect, canvasW);

            // Invalidate health bars so they repaint with latest values
            _playerHealthBar.Invalidate();
            _opponentHealthBar.Invalidate();

            // Player 1 movement
            if (!_combat.P1AttackLocked && !_player.IsHitStunned)
                _movement.MovePlayer(_input.P1.Left, _input.P1.Right);

            // Player 2 / AI
            if (_vsMode)
            {
                if (!_opponent.IsHitStunned && !_combat.P2AttackLocked)
                    _movement.MoveOpponentVs(_input.P2.Left, _input.P2.Right);
            }
            else
            {
                // AI state machine update
                if (!_opponent.IsHitStunned)
                    _ai.Update(_player.Position.X, _opponent.Position.X, _opponent.IsHitStunned);

                // AI movement
                if (!_opponent.IsHitStunned && !_combat.P2MovementBlocked)
                    _movement.MoveOpponentAi(_ai.AiDirection);
            }

            _canvas.Invalidate();
        }

        // ─────────────────────────────────────────────────────────────────
        //  Input routing
        // ─────────────────────────────────────────────────────────────────

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_matchState.State.IsOver) return base.ProcessCmdKey(ref msg, keyData);
            if (_player.IsHitStunned) return true;

            // Let InputController update its state first
            bool consumed = _input.HandleKeyDown(keyData);

            // Player 1 attacks – dispatch newly-pressed keys to CombatController
            if (!_combat.P1AttackLocked)
            {
                if (_input.P1.LightPunch && keyData == CurrentBindings.P1_LightPunch)
                    _combat.P1_LightPunch(_input.P1.Up);
                else if (_input.P1.HeavyPunch && keyData == CurrentBindings.P1_HeavyPunch)
                    _combat.P1_HeavyPunch(_input.P1.Up);
                else if (_input.P1.LightKick && keyData == CurrentBindings.P1_LightKick)
                    _combat.P1_LightKick(_input.P1.Down);
                else if (_input.P1.HeavyKick && keyData == CurrentBindings.P1_HeavyKick)
                    _combat.P1_HeavyKick(_input.P1.Down);
                else if (_input.P1.Special && keyData == CurrentBindings.P1_Special)
                    _combat.P1_Special();
            }

            // Player 2 attacks (VS mode)
            if (_vsMode && !_opponent.IsHitStunned && !_combat.P2AttackLocked)
            {
                if (_input.P2.LightPunch && keyData == CurrentBindings.P2_LightPunch)
                    _combat.P2_LightPunch(_input.P2.Up);
                else if (_input.P2.HeavyPunch && keyData == CurrentBindings.P2_HeavyPunch)
                    _combat.P2_HeavyPunch(_input.P2.Up);
                else if (_input.P2.LightKick && keyData == CurrentBindings.P2_LightKick)
                    _combat.P2_LightKick(_input.P2.Down);
                else if (_input.P2.HeavyKick && keyData == CurrentBindings.P2_HeavyKick)
                    _combat.P2_HeavyKick(_input.P2.Down);
                else if (_input.P2.Special && keyData == CurrentBindings.P2_Special)
                    _combat.P2_Special();
            }

            return consumed || base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            _input.HandleKeyUp(e.KeyCode);

            // If P1 is not moving and not attacking, return to idle
            if (!_input.P1.IsMoving && !_combat.P1AttackLocked && !_player.IsHitStunned)
                _player.ReturnToIdle();
        }

        // ─────────────────────────────────────────────────────────────────
        //  IMatchObserver implementation
        // ─────────────────────────────────────────────────────────────────

        public void OnMatchOver(string winnerName, bool isDraw)
        {
            _sound.StopStageMusic();
            _gameLoopTimer.Stop();

            MatchState state = _matchState.State;
            state.ShowResult = true;

            if (isDraw)
            {
                state.ResultText     = "DRAW";
                state.ResultSubtitle = "";
            }
            else
            {
                state.ResultText     = "K.O.";
                state.ResultSubtitle = $"{winnerName.ToUpper()} WINS!";
            }

            _sound.PlayResultSound(isDraw);

            // Force the loser's health bar to show exactly 0.
            // Invalidate() is async, so call Update() immediately after
            // to flush the paint synchronously before the result overlay appears.
            _playerHealthBar?.Invalidate();
            _playerHealthBar?.Update();
            _opponentHealthBar?.Invalidate();
            _opponentHealthBar?.Update();
            _canvas.Invalidate();
            _canvas.Update();

            Timer closeTimer = new Timer { Interval = 5000 };
            closeTimer.Tick += (s, ev) =>
            {
                closeTimer.Stop();
                closeTimer.Dispose();
                _sound.StopResultSound();

                MainWindow mainForm = ParentForm as MainWindow;
                if (mainForm != null)
                {
                    mainForm.LoadFormInPanel(new MainMenu());
                }
                else
                {
                    Close();
                    MainWindow mw = new MainWindow();
                    mw.LoadFormInPanel(new MainMenu());
                    mw.Show();
                }
                Close();
            };
            closeTimer.Start();
        }

        // ─────────────────────────────────────────────────────────────────
        //  Cleanup
        // ─────────────────────────────────────────────────────────────────

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _sound.Dispose();
            _stage.Dispose();
            _projectiles.Dispose();
            _combat.Dispose();
            _matchState.Dispose();
            _hud.Dispose();
            _player?.Dispose();
            _opponent?.Dispose();
            _gameLoopTimer?.Stop();
            _gameLoopTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
