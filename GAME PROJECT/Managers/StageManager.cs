using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Game_Project.Interfaces;

namespace Game_Project.Managers
{
    /// <summary>
    /// Single Responsibility: loads a random animated stage GIF and exposes the
    /// current frame. Knows nothing about characters, health, or input.
    /// Implements IRenderable so the renderer can call it polymorphically.
    /// </summary>
    public class StageManager : IRenderable, IDisposable
    {
        private AnimatedGif _stageAnimation;
        private Image       _currentFrame;
        private bool        _loaded;
        private readonly Random _rand;
        private readonly Action _invalidate;

        public StageManager(Random rand, Action invalidateCallback)
        {
            _rand       = rand;
            _invalidate = invalidateCallback;
        }

        public void Load()
        {
            string stagesPath = Path.Combine(Application.StartupPath, "Assets", "Stages");
            if (!Directory.Exists(stagesPath)) return;

            string[] gifs = Directory.GetFiles(stagesPath, "*.gif");
            if (gifs.Length == 0) return;

            string chosen = gifs[_rand.Next(gifs.Length)];
            try
            {
                _stageAnimation = new AnimatedGif();
                _stageAnimation.Load(chosen);
                _stageAnimation.FrameChanged += frame =>
                {
                    _currentFrame = frame;
                    _invalidate();
                };
                _loaded = true;
            }
            catch { }
        }

        public void Render(Graphics g, int canvasWidth, int canvasHeight)
        {
            if (_loaded && _currentFrame != null)
                g.DrawImage(_currentFrame, 0, 0, canvasWidth, canvasHeight);
            else
                g.Clear(Color.DarkGray);
        }

        public void Dispose()
        {
            _stageAnimation?.Stop();
        }
    }
}
