using System;
using System.Windows.Forms;

namespace Game_Project
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadFormInPanel(new MainMenu());
        }

        internal void LoadFormInPanel(Form childForm)
        {
            panelContainer.Controls.Clear();

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelContainer.Controls.Add(childForm);
            panelContainer.Tag = childForm;

            childForm.Show();
            childForm.Focus();
        }
    }
}