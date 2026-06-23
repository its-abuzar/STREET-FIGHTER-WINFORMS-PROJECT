using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game_Project
{
    public partial class VsCharacterSelectScreen : CharacterSelectScreen
    {
        private int player2Index = 0;
        private bool player1Selected = false;
        private string player1Name, player2Name;
        private Pen p2SelectionPen = new Pen(Color.LimeGreen, 6);

        public VsCharacterSelectScreen()
        {
            delayTimer.Stop();
        }

        protected override void CharacterSelectScreen_Paint(object sender, PaintEventArgs e)
        {
            if (!player1Selected)
            {
                Rectangle selectedRect = characterRects[currentIndex];
                e.Graphics.DrawRectangle(selectionPen, selectedRect);
            }
            else
            {
                Rectangle selectedRect = characterRects[player2Index];
                e.Graphics.DrawRectangle(p2SelectionPen, selectedRect);
            }
        }

        protected override void CharacterSelectScreen_KeyDown(object sender, KeyEventArgs e)
        {
            int columns = 4;

            if (!player1Selected)
            {
                // Player 1 uses P1 bindings
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
                    player1Name = GetCharacterName(currentIndex);
                    player1Selected = true;
                    this.Invalidate();
                }
            }
            else
            {
                // Player 2 uses P2 bindings
                if (e.KeyCode == CurrentBindings.P2_Left)
                {
                    if (player2Index % columns > 0) player2Index--;
                    this.Invalidate();
                }
                else if (e.KeyCode == CurrentBindings.P2_Right)
                {
                    if (player2Index % columns < columns - 1) player2Index++;
                    this.Invalidate();
                }
                else if (e.KeyCode == CurrentBindings.P2_Up)
                {
                    if (player2Index - columns >= 0) player2Index -= columns;
                    this.Invalidate();
                }
                else if (e.KeyCode == CurrentBindings.P2_Down)
                {
                    if (player2Index + columns < characterRects.Count) player2Index += columns;
                    this.Invalidate();
                }
                else if (e.KeyCode == CurrentBindings.P2_LightPunch || e.KeyCode == Keys.Enter)
                {
                    player2Name = GetCharacterName(player2Index);
                    player.Stop();
                    MainWindow mainForm = this.ParentForm as MainWindow;
                    if (mainForm != null)
                    {
                        Vs_Screen vsScreen = new Vs_Screen(player1Name, player2Name, true);
                        mainForm.LoadFormInPanel(vsScreen);
                    }
                    else
                    {
                        Vs_Screen vsScreen = new Vs_Screen(player1Name, player2Name, true);
                        vsScreen.Show();
                        this.Close();
                    }
                }
            }
        }

        // Override ProcessCmdKey to forward Player 2 keys as well
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == CurrentBindings.P1_Left ||
                keyData == CurrentBindings.P1_Right ||
                keyData == CurrentBindings.P1_Up ||
                keyData == CurrentBindings.P1_Down ||
                keyData == CurrentBindings.P1_LightPunch ||
                keyData == Keys.Enter)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
            if (player1Selected)
            {
                if (keyData == CurrentBindings.P2_Left ||
                    keyData == CurrentBindings.P2_Right ||
                    keyData == CurrentBindings.P2_Up ||
                    keyData == CurrentBindings.P2_Down ||
                    keyData == CurrentBindings.P2_LightPunch ||
                    keyData == Keys.Enter)
                {
                    CharacterSelectScreen_KeyDown(this, new KeyEventArgs(keyData));
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void SelectCharacter(int index) { }
        protected override void OnCharacterSelected() { }
    }
}