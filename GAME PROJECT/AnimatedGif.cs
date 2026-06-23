using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Game_Project
{
    public class AnimatedGif
    {
        private Image[] frames;
        private int currentFrame;
        private Timer timer;
        public event Action<Image> FrameChanged;

        public void Load(string filePath, int speedPercent = 100)
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
                    frames[i] = new Bitmap(src);
                }

                int originalDelay = GetGifDelay(src);
                int delayMs = originalDelay * 100 / Math.Max(1, speedPercent);
                if (delayMs < 5) delayMs = 5;

                timer = new Timer();
                timer.Interval = delayMs;
                timer.Tick += (s, e) =>
                {
                    currentFrame = (currentFrame + 1) % frames.Length;
                    FrameChanged?.Invoke(frames[currentFrame]);
                };
                timer.Start();
            }
        }

        private int GetGifDelay(Image gif)
        {
            try
            {
                var item = gif.GetPropertyItem(0x5100);
                return BitConverter.ToInt32(item.Value, 0) * 10;
            }
            catch { return 100; }
        }

        public void Stop()
        {
            timer?.Stop();
            timer?.Dispose();
        }
    }
}