using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Game_Project
{
    public partial class Vs_Screen : Form
    {
        private string playerCharacter;
        private string opponentCharacter;
        private SoundPlayer player = new SoundPlayer();
        private Timer closeTimer;
        private bool vsModeFlag = false;
        private PrivateFontCollection pfc = new PrivateFontCollection();
        private Font nameFontRegular;

        // Constructor for single‑player (opponent random)
        public Vs_Screen(string CharacterName) : this(CharacterName, "", false)
        {
        }

        // Constructor for VS mode (both players defined)
        public Vs_Screen(string playerName, string opponentName, bool vsMode = false)
        {
            InitializeComponent();

            playerCharacter = playerName;
            opponentCharacter = opponentName;
            vsModeFlag = vsMode;

            if (string.IsNullOrEmpty(opponentCharacter))
                opponentCharacter = GetOpponentCharacter();

            // Load custom fonts from the Fonts folder
            string fontsPath = Path.Combine(Application.StartupPath, "Assets", "Fonts");
            if (Directory.Exists(fontsPath))
            {
                pfc.AddFontFile(Path.Combine(fontsPath, "Fighting Spirit 2.otf"));
                pfc.AddFontFile(Path.Combine(fontsPath, "Fighting Spirit 2 bold.otf"));
            }

            if (pfc.Families.Length > 0)
            {
                nameFontRegular = new Font(pfc.Families[0], 30, FontStyle.Regular);
            }
            // Fallback if custom fonts not loaded
            if (nameFontRegular == null)
                nameFontRegular = new Font("Arial", 14, FontStyle.Bold);

            player.SoundLocation = Path.Combine(Application.StartupPath, "Assets", "Audio", "Ryu-SF2-Arcade-Theme.wav");
            player.PlayLooping();

            pbBackground.Dock = DockStyle.Fill;
            pbBackground.SizeMode = PictureBoxSizeMode.StretchImage;

            pbPlayer.BackColor = Color.Transparent;
            pbPlayer.Parent = pbBackground;
            pbPlayer.SizeMode = PictureBoxSizeMode.Zoom;

            pbOpponent.BackColor = Color.Transparent;
            pbOpponent.Parent = pbBackground;
            pbOpponent.SizeMode = PictureBoxSizeMode.Zoom;

            LoadCharacterImage(pbPlayer, playerCharacter);
            LoadCharacterImage(pbOpponent, opponentCharacter);

            // Flip opponent image
            FlipImageHorizontally(pbOpponent);

            // ─── Add player name label (centered below portrait) ───
            Label playerNameLabel = new Label();
            playerNameLabel.Text = playerCharacter;
            playerNameLabel.Font = nameFontRegular;
            playerNameLabel.ForeColor = Color.Black;
            playerNameLabel.BackColor = Color.Transparent;
            playerNameLabel.AutoSize = true;
            int playerX = pbPlayer.Left + (pbPlayer.Width / 2) - (playerNameLabel.Width / 2);
            int playerY = pbPlayer.Bottom + 20;
            playerNameLabel.Location = new Point(playerX, playerY);
            pbBackground.Controls.Add(playerNameLabel);
            playerNameLabel.BringToFront();

            // ─── Add opponent name label (centered below portrait) ───
            Label opponentNameLabel = new Label();
            opponentNameLabel.Text = opponentCharacter;
            opponentNameLabel.Font = nameFontRegular;
            opponentNameLabel.ForeColor = Color.Black;
            opponentNameLabel.BackColor = Color.Transparent;
            opponentNameLabel.AutoSize = true;
            int oppX = pbOpponent.Left + (pbOpponent.Width / 2) - (opponentNameLabel.Width / 2);
            int oppY = pbOpponent.Top - 40;
            opponentNameLabel.Location = new Point(oppX, oppY);
            pbBackground.Controls.Add(opponentNameLabel);
            opponentNameLabel.BringToFront();

            closeTimer = new Timer();
            closeTimer.Interval = 3000;
            closeTimer.Tick += CloseTimer_Tick;
            closeTimer.Start();
        }

        // ─── Keep all your existing methods unchanged ───
        private void FlipImageHorizontally(PictureBox pb)
        {
            if (pb.Image == null) return;
            try
            {
                Bitmap original = new Bitmap(pb.Image);
                original.RotateFlip(RotateFlipType.RotateNoneFlipX);
                Image old = pb.Image;
                pb.Image = original;
                old.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error flipping image: {ex.Message}");
            }
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop();
            closeTimer.Dispose();
            player.Stop();
            player.Dispose();

            MainWindow mainForm = this.ParentForm as MainWindow;
            if (mainForm != null)
            {
                FightScreen fightScreen = new FightScreen(playerCharacter, opponentCharacter, vsModeFlag);
                mainForm.LoadFormInPanel(fightScreen);
            }
            else
            {
                FightScreen fightScreen = new FightScreen(playerCharacter, opponentCharacter, vsModeFlag);
                fightScreen.Show();
                this.Close();
            }
        }

        private string GetOpponentCharacter()
        {
            string[] allCharacters = {
                "Ryu", "E. Honda", "Blanka", "Guile",
                "Ken", "Chun-Li", "Zangief", "Dhalsim"
            };
            Random rand = new Random();
            string opponent;
            do
            {
                opponent = allCharacters[rand.Next(allCharacters.Length)];
            } while (opponent == playerCharacter);
            return opponent;
        }

        private void LoadCharacterImage(PictureBox pictureBox, string characterName)
        {
            string fileName = Path.Combine(Application.StartupPath, "Assets", "UI", "vsScreenCharacterImages", characterName + ".png");
            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Image not found: {fileName}");
                return;
            }
            try
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    Image img = Image.FromStream(fs);
                    pictureBox.Image = (Image)img.Clone();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (closeTimer != null)
            {
                closeTimer.Stop();
                closeTimer.Dispose();
            }
            player?.Stop();
            player?.Dispose();
            nameFontRegular?.Dispose();
            foreach (FontFamily family in pfc.Families)
                family.Dispose();
            pfc.Dispose();
            base.OnFormClosed(e);
        }
    }
}