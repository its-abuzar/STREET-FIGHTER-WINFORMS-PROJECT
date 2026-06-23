using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Game_Project.Interfaces;

namespace Game_Project.Managers
{
    /// <summary>
    /// Single Responsibility: owns fireball creation, movement, collision, and rendering.
    /// Knows nothing about player input or health bars – it raises an event when it hits.
    /// Follows Open/Closed: new projectile types can subclass without changing this class.
    /// </summary>
    public class ProjectileManager : IRenderable, IDisposable
    {
        private GifSpeedController _controller;
        private Image  _currentFrame;
        private Point  _position;
        private bool   _active;
        private readonly Action _invalidate;

        /// <summary>Raised when the fireball strikes the opponent. Carries damage amount.</summary>
        public event Action<int> OnHitOpponent;

        public const int FireballDamage = 15;
        private const int Speed         = 12;
        private const int Size          = 80;

        public bool IsActive => _active;

        public ProjectileManager(Action invalidateCallback)
        {
            _invalidate = invalidateCallback;
        }

        public void Load()
        {
            string path = Path.Combine(Application.StartupPath, "Assets", "Characters", "Ryu", "FireBallFinal.gif");
            if (!File.Exists(path))
            {
                string dir = Path.Combine(Application.StartupPath, "Assets", "Characters", "Ryu");
                if (Directory.Exists(dir))
                {
                    foreach (string f in Directory.GetFiles(dir, "*.gif"))
                    {
                        if (Path.GetFileNameWithoutExtension(f).Equals("FireBallFinal", StringComparison.OrdinalIgnoreCase))
                        { path = f; break; }
                    }
                }
            }

            if (!File.Exists(path)) return;

            _controller = new GifSpeedController();
            _controller.TransparentColor = Color.Black;
            _controller.Load(path, 100, false);
            _controller.OnFrameChanged += frame =>
            {
                _currentFrame = frame;
                _invalidate();
            };
        }

        /// <summary>Spawns a fireball from the player's position.</summary>
        public void Spawn(Point playerPosition, Size playerSize)
        {
            if (_controller == null) return;
            _active = true;
            int startX = playerPosition.X + playerSize.Width - 30;
            int startY = playerPosition.Y + playerSize.Height / 2 - 50;
            _position = new Point(startX, startY);
            _controller.Play(null);
        }

        /// <summary>
        /// Advances the fireball each game-loop tick.
        /// Raises OnHitOpponent if it intersects the target rectangle.
        /// </summary>
        public void Update(Rectangle opponentRect, int canvasWidth)
        {
            if (!_active) return;

            _position.X += Speed;

            Rectangle fbRect = new Rectangle(_position, new Size(Size, Size));
            if (fbRect.IntersectsWith(opponentRect))
            {
                Deactivate();
                OnHitOpponent?.Invoke(FireballDamage);
                return;
            }

            if (_position.X > canvasWidth || _position.X + Size < 0)
                Deactivate();
        }

        private void Deactivate()
        {
            _active = false;
            _controller?.Stop();
        }

        public void Render(Graphics g, int canvasWidth, int canvasHeight)
        {
            if (_active && _currentFrame != null)
                g.DrawImage(_currentFrame, new Rectangle(_position, new Size(Size, Size)));
        }

        public void Dispose()
        {
            _controller?.Dispose();
        }
    }
}
