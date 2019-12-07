namespace mtemu
{
    partial class TetrisForm
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
            if (disposing && (components != null)) {
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
            this.components = new System.ComponentModel.Container();
            this.FieldPictureBox = new System.Windows.Forms.PictureBox();
            this.TickTimer = new System.Windows.Forms.Timer(this.components);
            this.scoreLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FieldPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FieldPictureBox
            // 
            this.FieldPictureBox.BackColor = System.Drawing.Color.Black;
            this.FieldPictureBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.FieldPictureBox.Location = new System.Drawing.Point(0, 30);
            this.FieldPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.FieldPictureBox.Name = "FieldPictureBox";
            this.FieldPictureBox.Size = new System.Drawing.Size(321, 521);
            this.FieldPictureBox.TabIndex = 0;
            this.FieldPictureBox.TabStop = false;
            // 
            // TickTimer
            // 
            this.TickTimer.Interval = 250;
            this.TickTimer.Tick += new System.EventHandler(this.TickTimerTick_);
            // 
            // scoreLabel
            // 
            this.scoreLabel.BackColor = System.Drawing.Color.Black;
            this.scoreLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.scoreLabel.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.scoreLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.scoreLabel.Location = new System.Drawing.Point(0, 0);
            this.scoreLabel.Margin = new System.Windows.Forms.Padding(0);
            this.scoreLabel.Name = "scoreLabel";
            this.scoreLabel.Size = new System.Drawing.Size(321, 30);
            this.scoreLabel.TabIndex = 1;
            this.scoreLabel.Text = "0";
            this.scoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TetrisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 551);
            this.Controls.Add(this.scoreLabel);
            this.Controls.Add(this.FieldPictureBox);
            this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TetrisForm";
            this.Text = "Тетрис";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TetrisFormClosed_);
            this.Shown += new System.EventHandler(this.TetrisFormShown_);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormKeyDown_);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormKeyUp_);
            ((System.ComponentModel.ISupportInitialize)(this.FieldPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox FieldPictureBox;
        private System.Windows.Forms.Timer TickTimer;
        private System.Windows.Forms.Label scoreLabel;
    }
}