using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Game_Project
{
    public partial class OptionsScreen : Form
    {
        private KeyBindings bindings;
        private DataGridView dgv;
        private string configPath;

        public OptionsScreen()
        {
            InitializeComponent();
            configPath = Path.Combine(Application.StartupPath, "Assets", "Config", "keybindings.txt");   // now a .txt file
            LoadBindings();
            CreateUI();
        }

        private void LoadBindings()
        {
            bindings = new KeyBindings();
            if (File.Exists(configPath))
                bindings.Load(configPath);
        }

        private void SaveBindings()
        {
            bindings.Save(configPath);
        }

        private void CreateUI()
        {
            this.Text = "Options – Key Bindings";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 350,
                ReadOnly = false,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.Columns.Add("Action", "Action");
            dgv.Columns.Add("Key", "Current Key");
            dgv.Columns.Add("Player", "Player");

            AddRow("Move Left", bindings.P1_Left.ToString(), "Player 1");
            AddRow("Move Right", bindings.P1_Right.ToString(), "Player 1");
            AddRow("Move Up", bindings.P1_Up.ToString(), "Player 1");
            AddRow("Move Down", bindings.P1_Down.ToString(), "Player 1");
            AddRow("Light Punch", bindings.P1_LightPunch.ToString(), "Player 1");
            AddRow("Heavy Punch", bindings.P1_HeavyPunch.ToString(), "Player 1");
            AddRow("Light Kick", bindings.P1_LightKick.ToString(), "Player 1");
            AddRow("Heavy Kick", bindings.P1_HeavyKick.ToString(), "Player 1");
            AddRow("Special", bindings.P1_Special.ToString(), "Player 1");

            AddRow("Move Left", bindings.P2_Left.ToString(), "Player 2");
            AddRow("Move Right", bindings.P2_Right.ToString(), "Player 2");
            AddRow("Move Up", bindings.P2_Up.ToString(), "Player 2");
            AddRow("Move Down", bindings.P2_Down.ToString(), "Player 2");
            AddRow("Light Punch", bindings.P2_LightPunch.ToString(), "Player 2");
            AddRow("Heavy Punch", bindings.P2_HeavyPunch.ToString(), "Player 2");
            AddRow("Light Kick", bindings.P2_LightKick.ToString(), "Player 2");
            AddRow("Heavy Kick", bindings.P2_HeavyKick.ToString(), "Player 2");
            AddRow("Special", bindings.P2_Special.ToString(), "Player 2");

            dgv.CellClick += Dgv_CellClick;

            Button btnSave = new Button { Text = "Save", Location = new Point(200, 400), Size = new Size(100, 40) };
            btnSave.Click += (s, e) => { SaveBindings(); MessageBox.Show("Bindings saved!", "Options", MessageBoxButtons.OK, MessageBoxIcon.Information); this.Close(); };

            Button btnCancel = new Button { Text = "Cancel", Location = new Point(320, 400), Size = new Size(100, 40) };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(dgv);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddRow(string action, string key, string player)
        {
            dgv.Rows.Add(action, key, player);
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 1) return;
            string action = dgv.Rows[e.RowIndex].Cells[0].Value.ToString();
            string player = dgv.Rows[e.RowIndex].Cells[2].Value.ToString();

            KeyPressDialog kpd = new KeyPressDialog();
            kpd.ShowDialog();
            if (kpd.PressedKey != Keys.None)
            {
                UpdateBinding(player, action, kpd.PressedKey);
                dgv.Rows[e.RowIndex].Cells[1].Value = kpd.PressedKey.ToString();
            }
        }

        private void UpdateBinding(string player, string action, Keys newKey)
        {
            if (player == "Player 1")
            {
                if (action == "Move Left") bindings.P1_Left = newKey;
                else if (action == "Move Right") bindings.P1_Right = newKey;
                else if (action == "Move Up") bindings.P1_Up = newKey;
                else if (action == "Move Down") bindings.P1_Down = newKey;
                else if (action == "Light Punch") bindings.P1_LightPunch = newKey;
                else if (action == "Heavy Punch") bindings.P1_HeavyPunch = newKey;
                else if (action == "Light Kick") bindings.P1_LightKick = newKey;
                else if (action == "Heavy Kick") bindings.P1_HeavyKick = newKey;
                else if (action == "Special") bindings.P1_Special = newKey;
            }
            else
            {
                if (action == "Move Left") bindings.P2_Left = newKey;
                else if (action == "Move Right") bindings.P2_Right = newKey;
                else if (action == "Move Up") bindings.P2_Up = newKey;
                else if (action == "Move Down") bindings.P2_Down = newKey;
                else if (action == "Light Punch") bindings.P2_LightPunch = newKey;
                else if (action == "Heavy Punch") bindings.P2_HeavyPunch = newKey;
                else if (action == "Light Kick") bindings.P2_LightKick = newKey;
                else if (action == "Heavy Kick") bindings.P2_HeavyKick = newKey;
                else if (action == "Special") bindings.P2_Special = newKey;
            }
        }
    }

    public class KeyPressDialog : Form
    {
        public Keys PressedKey { get; private set; } = Keys.None;

        public KeyPressDialog()
        {
            this.Text = "Press a key...";
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            Label lbl = new Label { Text = "Press any key to assign.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            this.Controls.Add(lbl);
            this.KeyDown += (s, e) => { PressedKey = e.KeyCode; this.Close(); };
        }
    }
}