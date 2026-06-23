using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Game_Project
{
    public class GifSpeedController
    {
        private Image[] frames;
        private int currentFrame;
        private Timer timer;
        private BaseCharacter target;
        private int delayMs;

        public event Action<Image> OnFrameChanged;

        public Color TransparentColor { get; set; } = Color.Magenta;

        public void Load(string filePath, int speedPercent = 100, bool mirror = false)
        {
            if (frames != null)
            {
                foreach (var f in frames) f?.Dispose();
                frames = null;
            }

            using (var src = Image.FromFile(filePath))
            {
                var dimension = new FrameDimension(src.FrameDimensionsList[0]);
                int count = src.GetFrameCount(dimension);
                frames = new Image[count];
                for (int i = 0; i < count; i++)
                {
                    src.SelectActiveFrame(dimension, i);
                    var frame = new Bitmap(src);
                    if (mirror)
                        frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    frame.MakeTransparent(TransparentColor);
                    frames[i] = frame;
                }

                int originalDelay = GetGifDelay(src);
                delayMs = originalDelay * 100 / Math.Max(1, speedPercent);
                if (delayMs < 5) delayMs = 5;
            }
        }

        private int GetGifDelay(Image gif)
        {
            try
            {
                var item = gif.GetPropertyItem(0x5100);
                int delay = BitConverter.ToInt32(item.Value, 0) * 10;
                return delay > 0 ? delay : 100;
            }
            catch { return 100; }
        }

        public void Play(BaseCharacter character)
        {
            Stop();
            target = character;
            currentFrame = 0;
            if (frames != null && frames.Length > 0)
            {
                if (character != null) character.CurrentFrame = frames[0];
                OnFrameChanged?.Invoke(frames[0]);
            }

            timer = new Timer();
            timer.Interval = delayMs;
            timer.Tick += (s, e) =>
            {
                if (frames != null && frames.Length > 0)
                {
                    currentFrame = (currentFrame + 1) % frames.Length;
                    if (target != null) target.CurrentFrame = frames[currentFrame];
                    OnFrameChanged?.Invoke(frames[currentFrame]);
                }
                else
                {
                    Stop();
                }
            };
            timer.Start();
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        public void Dispose()
        {
            Stop();
            if (frames != null)
            {
                foreach (var f in frames) f?.Dispose();
                frames = null;
            }
        }
    }
}