﻿namespace mtemu
{
    partial class HelpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.leftLabel = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.codeLabel = new System.Windows.Forms.Label();
            this.codeText = new System.Windows.Forms.TextBox();
            this.codeButton = new System.Windows.Forms.Button();
            this.hackallcodeLabel = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(4);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(4);
            this.label1.Size = new System.Drawing.Size(558, 44);
            this.label1.TabIndex = 0;
            this.label1.Text = "Чтобы узнать, какое сочетание клавиш работает также, как нажатие кнопки, просто н" +
    "аведите на кнопку!";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(13, 134);
            this.label2.Margin = new System.Windows.Forms.Padding(4);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(4);
            this.label2.Size = new System.Drawing.Size(558, 44);
            this.label2.TabIndex = 1;
            this.label2.Text = "Можно \"приклеить\" окна стека, памяти к главному окну, просто разместив их рядом с" +
    " правой стороны, а память программы с левой!";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(13, 65);
            this.label3.Margin = new System.Windows.Forms.Padding(4);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(4);
            this.label3.Size = new System.Drawing.Size(558, 61);
            this.label3.TabIndex = 2;
            this.label3.Text = "Чтобы в режиме редактирования команды выбрать предыдущую или следующую команду, м" +
    "ожно воспользоваться соответствующими сочетаниями клавиш Ctrl + Вверх и Ctrl + В" +
    "низ!";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(13, 186);
            this.label5.Margin = new System.Windows.Forms.Padding(4);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(4);
            this.label5.Size = new System.Drawing.Size(558, 44);
            this.label5.TabIndex = 4;
            this.label5.Text = "Тут есть пасхалки, чтобы было нескучно учится!\r\nПодсказка: Раз, два, три! Ёлочка," +
    " гори!";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // leftLabel
            // 
            this.leftLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftLabel.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.leftLabel.Location = new System.Drawing.Point(495, 186);
            this.leftLabel.Margin = new System.Windows.Forms.Padding(4);
            this.leftLabel.Name = "leftLabel";
            this.leftLabel.Padding = new System.Windows.Forms.Padding(4);
            this.leftLabel.Size = new System.Drawing.Size(76, 44);
            this.leftLabel.TabIndex = 5;
            this.leftLabel.Text = "99/99";
            this.leftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel
            // 
            this.linkLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkLabel.Location = new System.Drawing.Point(13, 238);
            this.linkLabel.Margin = new System.Windows.Forms.Padding(4);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Padding = new System.Windows.Forms.Padding(4);
            this.linkLabel.Size = new System.Drawing.Size(558, 27);
            this.linkLabel.TabIndex = 7;
            this.linkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel.Visible = false;
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClicked);
            // 
            // codeLabel
            // 
            this.codeLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.codeLabel.Location = new System.Drawing.Point(13, 273);
            this.codeLabel.Margin = new System.Windows.Forms.Padding(4);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Padding = new System.Windows.Forms.Padding(4);
            this.codeLabel.Size = new System.Drawing.Size(558, 27);
            this.codeLabel.TabIndex = 13;
            this.codeLabel.Text = "Посторонним вход воспрещен!";
            this.codeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.codeLabel.Visible = false;
            // 
            // codeText
            // 
            this.codeText.Location = new System.Drawing.Point(245, 275);
            this.codeText.Margin = new System.Windows.Forms.Padding(0);
            this.codeText.Name = "codeText";
            this.codeText.Size = new System.Drawing.Size(250, 23);
            this.codeText.TabIndex = 15;
            this.codeText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.codeText.Visible = false;
            this.codeText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CodeTextKeyDown_);
            // 
            // codeButton
            // 
            this.codeButton.Location = new System.Drawing.Point(495, 274);
            this.codeButton.Margin = new System.Windows.Forms.Padding(0);
            this.codeButton.Name = "codeButton";
            this.codeButton.Size = new System.Drawing.Size(75, 25);
            this.codeButton.TabIndex = 14;
            this.codeButton.Text = "Ввод";
            this.codeButton.UseVisualStyleBackColor = true;
            this.codeButton.Visible = false;
            this.codeButton.Click += new System.EventHandler(this.CodeButtonClick_);
            // 
            // hackallcodeLabel
            // 
            this.hackallcodeLabel.Location = new System.Drawing.Point(200, 331);
            this.hackallcodeLabel.Margin = new System.Windows.Forms.Padding(4);
            this.hackallcodeLabel.Name = "hackallcodeLabel";
            this.hackallcodeLabel.Size = new System.Drawing.Size(107, 17);
            this.hackallcodeLabel.TabIndex = 16;
            this.hackallcodeLabel.TabStop = true;
            this.hackallcodeLabel.Text = "@hackallcode";
            this.hackallcodeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.hackallcodeLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.hackallcodeLinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(304, 331);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(80, 17);
            this.linkLabel1.TabIndex = 17;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "@nan0_mal";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.nan0_malLinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(523, 331);
            this.label4.Margin = new System.Windows.Forms.Padding(4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "1.0.3";
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.hackallcodeLabel);
            this.Controls.Add(this.codeText);
            this.Controls.Add(this.codeButton);
            this.Controls.Add(this.codeLabel);
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.leftLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "HelpForm";
            this.Text = "Помощь";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label leftLabel;
        public System.Windows.Forms.LinkLabel linkLabel;
        public System.Windows.Forms.TextBox codeText;
        public System.Windows.Forms.Button codeButton;
        public System.Windows.Forms.Label codeLabel;
        public System.Windows.Forms.LinkLabel hackallcodeLabel;
        public System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label4;
    }
}
