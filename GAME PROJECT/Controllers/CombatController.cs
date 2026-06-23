using System;
using System.Drawing;
using System.Windows.Forms;
using Game_Project.Managers;
using Game_Project.Models;

namespace Game_Project.Controllers
{
    /// <summary>
    /// Single Responsibility: executes attack animations and hit detection for both players.
    /// All damage is delegated to MatchStateManager (Dependency Inversion).
    /// The ProjectileManager is injected for fireball creation (Dependency Injection).
    /// </summary>
    public class CombatController : IDisposable
    {
        // --- Injected dependencies ---
        private readonly BaseCharacter    _player;
        private readonly BaseCharacter    _opponent;
        private readonly MatchStateManager _matchState;
        private readonly ProjectileManager _projectiles;
        private readonly Action           _invalidate;

        // --- Revert timers (owned here, not in FightScreen) ---
        private readonly Timer _p1RevertTimer;
        private readonly Timer _p2RevertTimer;

        // --- Attack lock flags (mirrors the original attackPressed / p2AttackPressed) ---
        private bool _p1AttackLocked;
        private bool _p2AttackLocked;
        private bool _p2MovementBlocked;   // isAttacking equivalent for P2

        public bool P1AttackLocked => _p1AttackLocked;
        public bool P2AttackLocked => _p2AttackLocked;
        public bool P2MovementBlocked => _p2MovementBlocked;

        // Expose attack-locked state so InputController can read it
        public void SetP1AttackLocked(bool value) => _p1AttackLocked = value;
        public void SetP2AttackLocked(bool value) => _p2AttackLocked = value;

        // Canvas width needed for boundary clamping
        public int CanvasWidth { get; set; }

        public CombatController(
            BaseCharacter player,
            BaseCharacter opponent,
            MatchStateManager matchState,
            ProjectileManager projectiles,
            Action invalidateCallback)
        {
            _player      = player;
            _opponent    = opponent;
            _matchState  = matchState;
            _projectiles = projectiles;
            _invalidate  = invalidateCallback;

            _p1RevertTimer = new Timer { Interval = 800 };
            _p1RevertTimer.Tick += (s, e) =>
            {
                if (!_player.IsHitStunned) _player.ReturnToIdle();
                _p1RevertTimer.Stop();
                _p1AttackLocked = false;
                _invalidate();
            };

            _p2RevertTimer = new Timer { Interval = 800 };
            _p2RevertTimer.Tick += (s, e) =>
            {
                if (!_opponent.IsHitStunned) _opponent.ReturnToIdle();
                _p2RevertTimer.Stop();
                _p2AttackLocked    = false;
                _p2MovementBlocked = false;
                _invalidate();
            };
        }

        // ──────────────────────────────────────────────
        //  Player 1 attacks
        // ──────────────────────────────────────────────

        public void P1_LightPunch(bool upHeld)
        {
            if (_p1AttackLocked) return;
            if (upHeld) PerformP1Attack(() => _player.UppercutLeft(), 12, 250);
            else        PerformP1Attack(() => _player.PunchLeft(),    8,  200);
        }

        public void P1_HeavyPunch(bool upHeld)
        {
            if (_p1AttackLocked) return;
            if (upHeld) PerformP1Attack(() => _player.UppercutRight(), 12, 250);
            else        PerformP1Attack(() => _player.PunchRight(),     8, 200);
        }

        public void P1_LightKick(bool downHeld)
        {
            if (_p1AttackLocked) return;
            if (downHeld) PerformP1Attack(() => _player.CrouchKickLeft(),  9, 220);
            else          PerformP1Attack(() => _player.KickHighLeft(),    9, 220);
        }

        public void P1_HeavyKick(bool downHeld)
        {
            if (_p1AttackLocked) return;
            if (downHeld) PerformP1Attack(() => _player.CrouchKickRight(), 9, 220);
            else          PerformP1Attack(() => _player.KickHighRight(),   9, 220);
        }

        public void P1_Special()
        {
            if (_p1AttackLocked) return;
            _p1AttackLocked = true;
            if (_p1RevertTimer.Enabled) _p1RevertTimer.Stop();
            _player.DoSpecial();
            _p1RevertTimer.Start();

            if (_player is Ryu)
            {
                Timer delay = new Timer { Interval = 700 };
                delay.Tick += (s, ev) =>
                {
                    SpawnP1Fireball();
                    delay.Stop();
                    delay.Dispose();
                };
                delay.Start();
            }
            else if (_player is Blanka)
            {
                ApplySpecialHitDelayed(15, 20, 400, fullScreen: true, isPlayerAttacking: false);
            }
            else
            {
                ApplySpecialHitDelayed(15, 20, 400, fullScreen: false, isPlayerAttacking: false);
            }
            _invalidate();
        }

        private void SpawnP1Fireball()
        {
            Size playerSize = _player.CurrentFrame != null
                ? _player.CurrentFrame.Size
                : new Size(100, 120);
            _projectiles.Spawn(_player.Position, playerSize);
        }

        private void PerformP1Attack(Action attackAction, int damage, int hitDelayMs)
        {
            PerformUnifiedAttack(_player, _opponent, attackAction, damage, hitDelayMs,
                ref _p1AttackLocked, _p1RevertTimer,
                ref _p2MovementBlocked, isPlayerTwo: false);
        }

        // ──────────────────────────────────────────────
        //  Player 2 attacks
        // ──────────────────────────────────────────────

        public void P2_LightPunch(bool upHeld)
        {
            if (_p2AttackLocked) return;
            if (upHeld) PerformP2Attack(() => _opponent.UppercutLeft(), 12, 250);
            else        PerformP2Attack(() => _opponent.PunchLeft(),    8,  200);
        }

        public void P2_HeavyPunch(bool upHeld)
        {
            if (_p2AttackLocked) return;
            if (upHeld) PerformP2Attack(() => _opponent.UppercutRight(), 12, 250);
            else        PerformP2Attack(() => _opponent.PunchRight(),    8,  200);
        }

        public void P2_LightKick(bool downHeld)
        {
            if (_p2AttackLocked) return;
            if (downHeld) PerformP2Attack(() => _opponent.CrouchKickLeft(),  9, 220);
            else          PerformP2Attack(() => _opponent.KickHighLeft(),    9, 220);
        }

        public void P2_HeavyKick(bool downHeld)
        {
            if (_p2AttackLocked) return;
            if (downHeld) PerformP2Attack(() => _opponent.CrouchKickRight(), 9, 220);
            else          PerformP2Attack(() => _opponent.KickHighRight(),   9, 220);
        }

        public void P2_Special()
        {
            if (_p2AttackLocked) return;
            _p2AttackLocked    = true;
            _p2MovementBlocked = true;
            if (_p2RevertTimer.Enabled) _p2RevertTimer.Stop();
            _opponent.DoSpecial();
            _p2RevertTimer.Start();

            Timer specialHit = new Timer { Interval = 400 };
            specialHit.Tick += (s, ev) =>
            {
                if (!_player.IsInvincible)
                {
                    int hitDistance = _opponent.GetCurrentHitDistance();
                    if (hitDistance > 0)
                    {
                        Rectangle playerBox = new Rectangle(
                            _player.Position,
                            _player.CurrentFrame?.Size ?? new Size(100, 120));

                        Rectangle attackBox = BuildAttackBox(_opponent, hitDistance);

                        if (attackBox.IntersectsWith(playerBox))
                        {
                            _matchState.DamagePlayer(15);
                            _player.ApplyHitReaction(20, 300, 500);
                            ClampPosition(_player, -20);
                        }
                    }
                }
                specialHit.Dispose();
            };
            specialHit.Start();
            _invalidate();
        }

        private void PerformP2Attack(Action attackAction, int damage, int hitDelayMs)
        {
            PerformUnifiedAttack(_opponent, _player, attackAction, damage, hitDelayMs,
                ref _p2AttackLocked, _p2RevertTimer,
                ref _p2MovementBlocked, isPlayerTwo: true);
        }

        // ──────────────────────────────────────────────
        //  Shared attack primitives
        // ──────────────────────────────────────────────

        private void PerformUnifiedAttack(
            BaseCharacter attacker,
            BaseCharacter target,
            Action attackAction,
            int damage,
            int hitDelayMs,
            ref bool attackFlag,
            Timer revertTimer,
            ref bool movementBlockFlag,
            bool isPlayerTwo)
        {
            if (attackFlag) return;
            attackFlag = true;
            if (isPlayerTwo) movementBlockFlag = true;
            if (revertTimer.Enabled) revertTimer.Stop();
            attackAction();
            revertTimer.Start();

            Timer hitTimer = new Timer { Interval = hitDelayMs };
            hitTimer.Tick += (s, ev) =>
            {
                if (!target.IsInvincible)
                {
                    int hitDistance = attacker.GetCurrentHitDistance();
                    if (hitDistance > 0)
                    {
                        Rectangle targetBox  = new Rectangle(target.Position, target.CurrentFrame?.Size ?? new Size(100, 120));
                        Rectangle attackBox  = BuildAttackBox(attacker, hitDistance);

                        if (attackBox.IntersectsWith(targetBox))
                        {
                            if (isPlayerTwo)
                            {
                                _matchState.DamagePlayer(damage);
                                _player.ApplyHitReaction(20, 300, 500);
                                ClampPosition(_player, -20);
                            }
                            else
                            {
                                _matchState.DamageOpponent(damage);
                                _opponent.ApplyHitReaction(20, 300, 500);
                                ClampPosition(_opponent, +20);
                            }
                        }
                    }
                }
                hitTimer.Dispose();
            };
            hitTimer.Start();
            _invalidate();
        }

        /// <summary>Applies a special hit with a time delay. Used for non-fireball specials.</summary>
        private void ApplySpecialHitDelayed(int damage, int knockback, int delayMs, bool fullScreen, bool isPlayerAttacking)
        {
            Timer t = new Timer { Interval = delayMs };
            t.Tick += (s, ev) =>
            {
                if (!_opponent.IsInvincible)
                {
                    bool shouldHit = fullScreen;
                    if (!shouldHit)
                    {
                        int hitDistance = _player.GetCurrentHitDistance();
                        if (hitDistance > 0)
                        {
                            int playerRight   = _player.Position.X + (_player.CurrentFrame?.Width ?? 100);
                            int opponentLeft  = _opponent.Position.X;
                            shouldHit = !_player.IsMirrored && (playerRight - opponentLeft) >= hitDistance;
                        }
                    }

                    if (shouldHit)
                    {
                        _matchState.DamageOpponent(damage);
                        _opponent.ApplyHitReaction(knockback, 300, 500);
                        ClampPosition(_opponent, +knockback);
                    }
                }
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }

        // ──────────────────────────────────────────────
        //  AI attack helpers (called from AIController)
        // ──────────────────────────────────────────────

        public void AiPerformAttack(Action attackAction, int damage, int capturedDist)
        {
            _p2MovementBlocked = true;
            attackAction?.Invoke();

            Timer aiHitTimer = new Timer { Interval = 200 };
            aiHitTimer.Tick += (s, ev) =>
            {
                aiHitTimer.Stop();
                aiHitTimer.Dispose();
                if (!_player.IsInvincible && capturedDist < 150)
                {
                    _matchState.DamagePlayer(damage);
                    _player.ApplyHitReaction(20, 300, 500);
                    ClampPosition(_player, -20);
                }
            };
            aiHitTimer.Start();

            Timer oppRevert = new Timer { Interval = 800 };
            oppRevert.Tick += (s, ev) =>
            {
                _opponent.ReturnToIdle();
                _p2MovementBlocked = false;
                oppRevert.Stop();
                oppRevert.Dispose();
                _invalidate();
            };
            oppRevert.Start();
        }

        public void AiPerformSpecial(bool fullScreen, int capturedDist)
        {
            Timer specialHit = new Timer { Interval = 400 };
            specialHit.Tick += (s, ev) =>
            {
                if (!_player.IsInvincible && (fullScreen || capturedDist < 150))
                {
                    _matchState.DamagePlayer(15);
                    _player.ApplyHitReaction(20, 300, 500);
                    ClampPosition(_player, -20);
                }
                specialHit.Stop();
                specialHit.Dispose();
            };
            specialHit.Start();

            Timer revert = new Timer { Interval = 800 };
            revert.Tick += (s, ev) =>
            {
                _opponent.ReturnToIdle();
                revert.Stop();
                revert.Dispose();
                _invalidate();
            };
            revert.Start();
        }

        // ──────────────────────────────────────────────
        //  Utilities
        // ──────────────────────────────────────────────

        private Rectangle BuildAttackBox(BaseCharacter attacker, int hitDistance)
        {
            if (attacker.IsMirrored)
                return new Rectangle(
                    attacker.Position.X - hitDistance,
                    attacker.Position.Y,
                    hitDistance,
                    attacker.CurrentFrame?.Height ?? 120);
            else
                return new Rectangle(
                    attacker.Position.X + (attacker.CurrentFrame?.Width ?? 100),
                    attacker.Position.Y,
                    hitDistance,
                    attacker.CurrentFrame?.Height ?? 120);
        }

        private void ClampPosition(BaseCharacter character, int delta)
        {
            int newX = character.Position.X + delta;
            int maxX = CanvasWidth - (character.CurrentFrame?.Width ?? 100);
            newX = Math.Max(0, Math.Min(newX, maxX));
            character.Position = new Point(newX, character.Position.Y);
        }

        public bool IsP1Moving(PlayerInputState p1) => p1.Left || p1.Right;

        public void Dispose()
        {
            _p1RevertTimer.Stop(); _p1RevertTimer.Dispose();
            _p2RevertTimer.Stop(); _p2RevertTimer.Dispose();
        }
    }
}
