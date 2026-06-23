using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Game_Project
{
    public partial class MainMenu : Form
    {
        private SoundPlayer menuMusic;
        private bool keyPressed = false;
        private int selectedIndex = 0;
        private string[] menuItems = { "START GAME", "VS MODE", "OPTION", "QUIT TO DOS" };
        private Point[] arrowPositions;

        public static KeyBindings CurrentBindings { get; private set; }

        static MainMenu()
        {
            string configPath = Path.Combine(Application.StartupPath, "Assets", "Config", "keybindings.txt");
            CurrentBindings = new KeyBindings();
            if (File.Exists(configPath))
                CurrentBindings.Load(configPath);
        }

        public MainMenu()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.Focus();

            string imagePath = Path.Combine(Application.StartupPath, "Assets", "UI", "Main Menu.png");
            if (File.Exists(imagePath))
                this.BackgroundImage = Image.FromFile(imagePath);
            else
                this.BackColor = Color.Black;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            menuMusic = new SoundPlayer();
            menuMusic.SoundLocation = Path.Combine(Application.StartupPath, "Assets", "Audio", "MainMenuTheme.wav");
            try { menuMusic.PlayLooping(); } catch { }

            arrowPositions = new Point[]
            {
                new Point(230, 298), // START GAME
                new Point(230, 325), // VS MODE
                new Point(230, 357), // OPTION
                new Point(230, 381)  // QUIT TO DOS
            };

            this.Paint += MainMenu_Paint;
        }

        private void MainMenu_Paint(object sender, PaintEventArgs e)
        {
            if (selectedIndex >= 0 && selectedIndex < arrowPositions.Length)
            {
                Point pos = arrowPositions[selectedIndex];
                using (Font arrowFont = new Font("Arial", 24, FontStyle.Bold))
                {
                    e.Graphics.DrawString("→", arrowFont, Brushes.Yellow, pos.X, pos.Y);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyPressed) return base.ProcessCmdKey(ref msg, keyData);

            // Up / Down using bindings
            if (keyData == CurrentBindings.P1_Up)
            {
                selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                this.Invalidate();
                return true;
            }
            if (keyData == CurrentBindings.P1_Down)
            {
                selectedIndex = (selectedIndex + 1) % menuItems.Length;
                this.Invalidate();
                return true;
            }
            // Accept either the custom LightPunch key OR the Enter key
            if (keyData == CurrentBindings.P1_LightPunch || keyData == Keys.Enter)
            {
                keyPressed = true;
                ExecuteSelectedOption();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ExecuteSelectedOption()
        {
            switch (menuItems[selectedIndex])
            {
                case "START GAME":
                    menuMusic?.Stop();
                    menuMusic?.Dispose();
                    MainWindow mainForm = this.ParentForm as MainWindow;
                    if (mainForm != null)
                    {
                        CharacterSelectScreen charSelect = new CharacterSelectScreen();
                        mainForm.LoadFormInPanel(charSelect);
                    }
                    else
                    {
                        this.Close();
                        MainWindow mw = new MainWindow();
                        mw.LoadFormInPanel(new CharacterSelectScreen());
                        mw.Show();
                    }
                    break;
                case "VS MODE":
                    menuMusic?.Stop();
                    menuMusic?.Dispose();
                    MainWindow vsMain = this.ParentForm as MainWindow;
                    if (vsMain != null)
                    {
                        VsCharacterSelectScreen vsSelect = new VsCharacterSelectScreen();
                        vsMain.LoadFormInPanel(vsSelect);
                    }
                    else
                    {
                        this.Close();
                        MainWindow mw = new MainWindow();
                        mw.LoadFormInPanel(new VsCharacterSelectScreen());
                        mw.Show();
                    }
                    break;
                case "OPTION":
                    OptionsScreen options = new OptionsScreen();
                    options.ShowDialog();
                    // SoundPlayer.PlayLooping() stops when a modal dialog steals
                    // focus, so restart the music after the dialog is dismissed.
                    try
                    {
                        menuMusic?.Stop();
                        menuMusic = new SoundPlayer();
                        menuMusic.SoundLocation = Path.Combine(Application.StartupPath, "Assets", "Audio", "MainMenuTheme.wav");
                        menuMusic.PlayLooping();
                    }
                    catch { }
                    keyPressed = false;
                    break;
                case "QUIT TO DOS":
                    menuMusic?.Stop();
                    menuMusic?.Dispose();
                    Application.Exit();
                    break;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            menuMusic?.Stop();
            menuMusic?.Dispose();
            base.OnFormClosed(e);
        }
    }
}