using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Game_Project
{
    public partial class CharacterSelectScreen : Form
    {
        protected List<Rectangle> characterRects = new List<Rectangle>();
        protected int currentIndex = 0;
        protected Pen selectionPen = new Pen(Color.Yellow, 6);
        protected SoundPlayer player = new SoundPlayer();
        protected Timer delayTimer;
        protected string selectedCharacterName;

        public static KeyBindings CurrentBindings { get; private set; }

        static CharacterSelectScreen()
        {
            string configPath = Path.Combine(Application.StartupPath, "Assets", "Config", "keybindings.txt");
            CurrentBindings = new KeyBindings();
            if (File.Exists(configPath))
                CurrentBindings.Load(configPath);
        }

        protected virtual void OnCharacterSelected()
        {
            player.Stop();
            delayTimer.Stop();
            MainWindow mainForm = this.ParentForm as MainWindow;
            if (mainForm != null)
            {
                Vs_Screen vsScreen = new Vs_Screen(selectedCharacterName);
                mainForm.LoadFormInPanel(vsScreen);
            }
            else
            {
                Vs_Screen vsScreen = new Vs_Screen(selectedCharacterName);
                vsScreen.Show();
                this.Close();
            }
        }

        public CharacterSelectScreen()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.BackgroundImage = Properties.Resources.select_screen;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            player.SoundLocation = Path.Combine(Application.StartupPath, "Assets", "Audio", "Ryu-SFA2-Gold-Theme.wav");
            player.PlayLooping();

            characterRects.Add(new Rectangle(247, 296, 90, 71)); // Ryu
            characterRects.Add(new Rectangle(337, 296, 90, 71)); // E. Honda
            characterRects.Add(new Rectangle(427, 296, 90, 71)); // Blanka
            characterRects.Add(new Rectangle(517, 296, 90, 71)); // Guile
            characterRects.Add(new Rectangle(247, 367, 90, 71)); // Ken
            characterRects.Add(new Rectangle(337, 367, 90, 71)); // Chun Li
            characterRects.Add(new Rectangle(427, 367, 90, 71)); // Zangief
            characterRects.Add(new Rectangle(517, 367, 90, 71)); // Dhalsim

            this.DoubleBuffered = true;
            this.Paint += CharacterSelectScreen_Paint;
            this.KeyDown += CharacterSelectScreen_KeyDown;

            delayTimer = new Timer();
            delayTimer.Interval = 1500;
            delayTimer.Tick += DelayTimer_Tick;
        }

        protected virtual void CharacterSelectScreen_Paint(object sender, PaintEventArgs e)
        {
            Rectangle selectedRect = characterRects[currentIndex];
            e.Graphics.DrawRectangle(selectionPen, selectedRect);
        }

        protected virtual void CharacterSelectScreen_KeyDown(object sender, KeyEventArgs e)
        {
            int columns = 4;
            if (e.KeyCode == CurrentBindings.P1_Left)
            {
                if (currentIndex % columns > 0) currentIndex--;
                this.Invalidate();
            }
            else if (e.KeyCode == CurrentBindings.P1_Right)
            {
                if (currentIndex % columns < columns - 1) currentIndex++;
                this.Invalidate();
            }
            else if (e.KeyCode == CurrentBindings.P1_Up)
            {
                if (currentIndex - columns >= 0) currentIndex -= columns;
                this.Invalidate();
            }
            else if (e.KeyCode == CurrentBindings.P1_Down)
            {
                if (currentIndex + columns < characterRects.Count) currentIndex += columns;
                this.Invalidate();
            }
            else if (e.KeyCode == CurrentBindings.P1_LightPunch || e.KeyCode == Keys.Enter)
            {
                SelectCharacter(currentIndex);
            }
        }

        protected virtual void SelectCharacter(int index)
        {
            if (delayTimer.Enabled) return;
            selectedCharacterName = GetCharacterName(index);
            delayTimer.Start();
            this.KeyDown -= CharacterSelectScreen_KeyDown;
        }

        protected virtual string GetCharacterName(int index)
        {
            string[] names = {
                "Ryu", "E. Honda", "Blanka", "Guile",
                "Ken", "Chun-Li", "Zangief", "Dhalsim"
            };
            return names[index];
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == CurrentBindings.P1_Left ||
                keyData == CurrentBindings.P1_Right ||
                keyData == CurrentBindings.P1_Up ||
                keyData == CurrentBindings.P1_Down ||
                keyData == CurrentBindings.P1_LightPunch ||
                keyData == Keys.Enter)
            {
                CharacterSelectScreen_KeyDown(this, new KeyEventArgs(keyData));
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            OnCharacterSelected();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            delayTimer?.Stop();
            delayTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}