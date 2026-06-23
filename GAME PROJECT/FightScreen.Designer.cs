namespace Game_Project
{
    partial class FightScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbStage = new System.Windows.Forms.PictureBox();
            this.pbPlayer = new System.Windows.Forms.PictureBox();
            this.pbOpponent = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbStage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOpponent)).BeginInit();
            this.SuspendLayout();
            // 
            // pbStage
            // 
            this.pbStage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbStage.Location = new System.Drawing.Point(0, 0);
            this.pbStage.Name = "pbStage";
            this.pbStage.Size = new System.Drawing.Size(800, 450);
            this.pbStage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbStage.TabIndex = 0;
            this.pbStage.TabStop = false;
            // 
            // pbPlayer
            // 
            this.pbPlayer.Location = new System.Drawing.Point(36, 128);
            this.pbPlayer.Name = "pbPlayer";
            this.pbPlayer.Size = new System.Drawing.Size(179, 284);
            this.pbPlayer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPlayer.TabIndex = 1;
            this.pbPlayer.TabStop = false;
            // 
            // pbOpponent
            // 
            this.pbOpponent.Location = new System.Drawing.Point(528, 128);
            this.pbOpponent.Name = "pbOpponent";
            this.pbOpponent.Size = new System.Drawing.Size(195, 284);
            this.pbOpponent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbOpponent.TabIndex = 2;
            this.pbOpponent.TabStop = false;
            // 
            // FightScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pbOpponent);
            this.Controls.Add(this.pbPlayer);
            this.Controls.Add(this.pbStage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FightScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FightScreen";
            ((System.ComponentModel.ISupportInitialize)(this.pbStage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOpponent)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbStage;
        private System.Windows.Forms.PictureBox pbPlayer;
        private System.Windows.Forms.PictureBox pbOpponent;
    }
}