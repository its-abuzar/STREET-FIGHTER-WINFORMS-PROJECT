using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Game_Project.Models;

namespace Game_Project.Managers
{
    /// <summary>
    /// Single Responsibility: draws all HUD elements onto a Graphics context.
    /// Knows nothing about game logic, input, characters, or timers.
    /// Implements IDisposable to release GDI font resources.
    /// </summary>
    public class HUDRenderer : IDisposable
    {
        private readonly PrivateFontCollection _pfc = new PrivateFontCollection();
        private Font _nameFont;
        private Font _timerFont;

        public HUDRenderer()
        {
            LoadFonts();
        }

        private void LoadFonts()
        {
            string fontsPath = Path.Combine(Application.StartupPath, "Assets", "Fonts");
            if (Directory.Exists(fontsPath))
            {
                string regular = Path.Combine(fontsPath, "Fighting Spirit 2.otf");
                string bold    = Path.Combine(fontsPath, "Fighting Spirit 2 bold.otf");
                if (File.Exists(regular)) _pfc.AddFontFile(regular);
                if (File.Exists(bold))    _pfc.AddFontFile(bold);
            }

            if (_pfc.Families.Length > 0)
            {
                _nameFont  = new Font(_pfc.Families[0], 14, FontStyle.Regular);
                _timerFont = new Font(_pfc.Families[0], 24, FontStyle.Regular);
            }

            if (_nameFont  == null) _nameFont  = new Font("Arial", 14, FontStyle.Bold);
            if (_timerFont == null) _timerFont = new Font("Arial", 24, FontStyle.Bold);
        }

        // ── Health bars ───────────────────────────────────────────────────

        /// <summary>Draws a health bar panel. Called from Panel.Paint event.</summary>
        public void DrawHealthBar(Graphics g, int health, int width, int height)
        {
            float percent   = Math.Max(0, health) / 100f;
            int   fillWidth = (int)(width * percent);

            g.Clear(Color.Red);
            if (fillWidth > 0)
                using (var brush = new SolidBrush(Color.Yellow))
                    g.FillRectangle(brush, 0, 0, fillWidth, height);

            using (var borderPen = new Pen(Color.Black, 2))
                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
            using (var innerPen = new Pen(Color.DarkGray, 1))
                g.DrawRectangle(innerPen, 2, 2, width - 5, height - 5);
        }

        // ── HUD overlay drawn on the canvas ──────────────────────────────

        public void RenderHUD(Graphics g, int canvasWidth, MatchState state,
                              string playerName, string opponentName)
        {
            DrawPlayerName(g, playerName);
            DrawOpponentName(g, opponentName, canvasWidth);
            DrawTimer(g, canvasWidth, state.TimeLeft);
            if (state.ShowResult)
                DrawResult(g, canvasWidth, state.ResultText, state.ResultSubtitle);
        }

        private void DrawPlayerName(Graphics g, string name)
        {
            DrawOutlinedText(g, name, _nameFont, Brushes.White, Brushes.Black, 10f, 45f, 1);
        }

        private void DrawOpponentName(Graphics g, string name, int canvasWidth)
        {
            SizeF size = g.MeasureString(name, _nameFont);
            float x    = canvasWidth - size.Width - 10;
            DrawOutlinedText(g, name, _nameFont, Brushes.White, Brushes.Black, x, 45f, 1);
        }

        private void DrawTimer(Graphics g, int canvasWidth, int timeLeft)
        {
            string text = $"{timeLeft / 60:00}:{timeLeft % 60:00}";
            SizeF  size = g.MeasureString(text, _timerFont);
            float  x    = (canvasWidth - size.Width) / 2f;
            DrawOutlinedText(g, text, _timerFont, Brushes.Yellow, Brushes.Black, x, 5f, 1);
        }

        private void DrawResult(Graphics g, int canvasWidth, string resultText, string subtitle)
        {
            Font resultFont = (_timerFont?.FontFamily != null)
                ? new Font(_timerFont.FontFamily, 64, FontStyle.Bold)
                : new Font("Arial", 64, FontStyle.Bold);

            SizeF textSize = g.MeasureString(resultText, resultFont);
            float x        = (canvasWidth - textSize.Width) / 2f;
            float y        = (g.ClipBounds.Height - textSize.Height) / 2f - 30f;

            DrawOutlinedText(g, resultText, resultFont, Brushes.Yellow, Brushes.Black, x, y, 2);

            if (!string.IsNullOrEmpty(subtitle))
            {
                using (Font subFont = new Font(resultFont.FontFamily, 28, FontStyle.Bold))
                {
                    SizeF subSize = g.MeasureString(subtitle, subFont);
                    float subX    = (canvasWidth - subSize.Width) / 2f;
                    float subY    = y + textSize.Height + 20f;
                    DrawOutlinedText(g, subtitle, subFont, Brushes.White, Brushes.Black, subX, subY, 2);
                }
            }

            resultFont.Dispose();
        }

        // ── Debug rendering ───────────────────────────────────────────────

        public void RenderDebug(Graphics g, BaseCharacter player, BaseCharacter opponent, bool attackLocked)
        {
            if (!attackLocked && !player.IsHitStunned && player.CurrentAnimation != null)
            {
                int hitDist = player.GetCurrentHitDistance();
                if (hitDist > 0)
                {
                    int startX = player.Position.X + (player.CurrentFrame?.Width ?? 100);
                    Rectangle hitZone = new Rectangle(startX, player.Position.Y, hitDist, player.CurrentFrame?.Height ?? 120);
                    using (var pen = new Pen(Color.Red, 3))
                        g.DrawRectangle(pen, hitZone);
                }
                if (opponent.CurrentFrame != null)
                {
                    Rectangle oppRect = new Rectangle(opponent.Position, opponent.CurrentFrame.Size);
                    using (var pen = new Pen(Color.Blue, 2))
                        g.DrawRectangle(pen, oppRect);
                }
            }
        }

        // ── Utility ───────────────────────────────────────────────────────

        private static void DrawOutlinedText(Graphics g, string text, Font font,
            Brush fill, Brush outline, float x, float y, int radius)
        {
            for (int dx = -radius; dx <= radius; dx++)
                for (int dy = -radius; dy <= radius; dy++)
                    if (dx != 0 || dy != 0)
                        g.DrawString(text, font, outline, x + dx, y + dy);
            g.DrawString(text, font, fill, x, y);
        }

        public void Dispose()
        {
            _nameFont?.Dispose();
            _timerFont?.Dispose();
            _pfc?.Dispose();
        }
    }
}
