namespace mtemu
{
    partial class CallsForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "0x000",
            "1234567890123456789012345678901234567890123"}, -1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CallsForm));
            this.callList = new System.Windows.Forms.ListView();
            this.firstColumn = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.addrColumn = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.nameColumn = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.currentPanel = new System.Windows.Forms.Panel();
            this.commentLabel = new System.Windows.Forms.Label();
            this.hexLabel = new System.Windows.Forms.Label();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.commentText = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.addressTextLabel = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.addressText = new System.Windows.Forms.TextBox();
            this.listLabel = new System.Windows.Forms.Label();
            this.stepButton = new System.Windows.Forms.Button();
            this.formToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.currentPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // callList
            //
            this.callList.AutoArrange = false;
            this.callList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.callList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.firstColumn,
            this.addrColumn,
            this.nameColumn});
            this.callList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.callList.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.callList.FullRowSelect = true;
            this.callList.GridLines = true;
            this.callList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.callList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.callList.Location = new System.Drawing.Point(9, 37);
            this.callList.MultiSelect = false;
            this.callList.Name = "callList";
            this.callList.Size = new System.Drawing.Size(382, 434);
            this.callList.TabIndex = 1;
            this.callList.TabStop = false;
            this.callList.UseCompatibleStateImageBehavior = false;
            this.callList.View = System.Windows.Forms.View.Details;
            this.callList.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.ListViewColumnWidthChanging_);
            this.callList.SelectedIndexChanged += new System.EventHandler(this.CallListSelectedIndexChanged_);
            this.callList.Click += new System.EventHandler(this.CallListSelectedIndexChanged_);
            this.callList.Enter += new System.EventHandler(this.CallListSelectedIndexChanged_);
            this.callList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // firstColumn
            //
            this.firstColumn.Text = "";
            this.firstColumn.Width = 0;
            //
            // addrColumn
            //
            this.addrColumn.Text = "Адрес";
            this.addrColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.addrColumn.Width = 47;
            //
            // nameColumn
            //
            this.nameColumn.Text = "Коментарий";
            this.nameColumn.Width = 317;
            //
            // currentPanel
            //
            this.currentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.currentPanel.Controls.Add(this.commentLabel);
            this.currentPanel.Controls.Add(this.hexLabel);
            this.currentPanel.Controls.Add(this.downButton);
            this.currentPanel.Controls.Add(this.upButton);
            this.currentPanel.Controls.Add(this.commentText);
            this.currentPanel.Controls.Add(this.addButton);
            this.currentPanel.Controls.Add(this.addressTextLabel);
            this.currentPanel.Controls.Add(this.removeButton);
            this.currentPanel.Controls.Add(this.saveButton);
            this.currentPanel.Controls.Add(this.addressText);
            this.currentPanel.Location = new System.Drawing.Point(9, 477);
            this.currentPanel.Name = "currentPanel";
            this.currentPanel.Padding = new System.Windows.Forms.Padding(6);
            this.currentPanel.Size = new System.Drawing.Size(382, 98);
            this.currentPanel.TabIndex = 2;
            //
            // commentLabel
            //
            this.commentLabel.AutoSize = true;
            this.commentLabel.Font = new System.Drawing.Font("Consolas", 10F);
            this.commentLabel.Location = new System.Drawing.Point(178, 10);
            this.commentLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 2);
            this.commentLabel.Name = "commentLabel";
            this.commentLabel.Size = new System.Drawing.Size(96, 17);
            this.commentLabel.TabIndex = 14;
            this.commentLabel.Text = "Комментарий";
            this.commentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // hexLabel
            //
            this.hexLabel.AutoSize = true;
            this.hexLabel.Font = new System.Drawing.Font("Consolas", 10F);
            this.hexLabel.Location = new System.Drawing.Point(10, 34);
            this.hexLabel.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.hexLabel.Name = "hexLabel";
            this.hexLabel.Size = new System.Drawing.Size(24, 17);
            this.hexLabel.TabIndex = 13;
            this.hexLabel.Text = "0x";
            this.hexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // downButton
            //
            this.downButton.Enabled = false;
            this.downButton.Font = new System.Drawing.Font("Consolas", 10F);
            this.downButton.Location = new System.Drawing.Point(108, 62);
            this.downButton.Margin = new System.Windows.Forms.Padding(4);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(24, 24);
            this.downButton.TabIndex = 0;
            this.downButton.TabStop = false;
            this.downButton.Text = "▼";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.DownButtonClick_);
            this.downButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // upButton
            //
            this.upButton.Enabled = false;
            this.upButton.Font = new System.Drawing.Font("Consolas", 10F);
            this.upButton.Location = new System.Drawing.Point(140, 62);
            this.upButton.Margin = new System.Windows.Forms.Padding(4);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(24, 24);
            this.upButton.TabIndex = 0;
            this.upButton.TabStop = false;
            this.upButton.Text = "▲";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.UpButtonClick_);
            this.upButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // commentText
            //
            this.commentText.Font = new System.Drawing.Font("Consolas", 10F);
            this.commentText.Location = new System.Drawing.Point(74, 31);
            this.commentText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 4);
            this.commentText.MaxLength = 43;
            this.commentText.Name = "commentText";
            this.commentText.Size = new System.Drawing.Size(296, 23);
            this.commentText.TabIndex = 2;
            this.commentText.Text = "MOV r0, 0x33 // result in R0";
            this.commentText.WordWrap = false;
            this.commentText.TextChanged += new System.EventHandler(this.CommentTextChanged_);
            this.commentText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // addButton
            //
            this.addButton.Font = new System.Drawing.Font("Consolas", 10F);
            this.addButton.Location = new System.Drawing.Point(172, 62);
            this.addButton.Margin = new System.Windows.Forms.Padding(4);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(90, 24);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Добавить";
            this.formToolTip.SetToolTip(this.addButton, "Ctrl + Enter");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButtonClick_);
            this.addButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // addressTextLabel
            //
            this.addressTextLabel.AutoSize = true;
            this.addressTextLabel.Font = new System.Drawing.Font("Consolas", 10F);
            this.addressTextLabel.Location = new System.Drawing.Point(14, 10);
            this.addressTextLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 2);
            this.addressTextLabel.Name = "addressTextLabel";
            this.addressTextLabel.Size = new System.Drawing.Size(48, 17);
            this.addressTextLabel.TabIndex = 0;
            this.addressTextLabel.Text = "Адрес";
            this.addressTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // removeButton
            //
            this.removeButton.Enabled = false;
            this.removeButton.Font = new System.Drawing.Font("Consolas", 10F);
            this.removeButton.Location = new System.Drawing.Point(10, 62);
            this.removeButton.Margin = new System.Windows.Forms.Padding(4);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(90, 24);
            this.removeButton.TabIndex = 0;
            this.removeButton.TabStop = false;
            this.removeButton.Text = "Удалить";
            this.formToolTip.SetToolTip(this.removeButton, "Ctrl + Del");
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.RemoveButtonClick_);
            this.removeButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // saveButton
            //
            this.saveButton.Enabled = false;
            this.saveButton.Font = new System.Drawing.Font("Consolas", 10F);
            this.saveButton.Location = new System.Drawing.Point(280, 62);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(90, 24);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Сохранить";
            this.formToolTip.SetToolTip(this.saveButton, "Enter");
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonClick_);
            this.saveButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // addressText
            //
            this.addressText.Font = new System.Drawing.Font("Consolas", 10F);
            this.addressText.Location = new System.Drawing.Point(34, 31);
            this.addressText.Margin = new System.Windows.Forms.Padding(0, 2, 4, 4);
            this.addressText.MaxLength = 3;
            this.addressText.Name = "addressText";
            this.addressText.Size = new System.Drawing.Size(32, 23);
            this.addressText.TabIndex = 1;
            this.addressText.Text = "000";
            this.addressText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.addressText.WordWrap = false;
            this.addressText.TextChanged += new System.EventHandler(this.AddressTextChanged_);
            this.addressText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddressTextKeyDown_);
            //
            // listLabel
            //
            this.listLabel.AutoSize = true;
            this.listLabel.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold);
            this.listLabel.Location = new System.Drawing.Point(128, 9);
            this.listLabel.Margin = new System.Windows.Forms.Padding(3);
            this.listLabel.Name = "listLabel";
            this.listLabel.Size = new System.Drawing.Size(150, 22);
            this.listLabel.TabIndex = 3;
            this.listLabel.Text = "Список вызовов";
            //
            // stepButton
            //
            this.stepButton.Font = new System.Drawing.Font("Consolas", 9F);
            this.stepButton.Location = new System.Drawing.Point(329, 9);
            this.stepButton.Name = "stepButton";
            this.stepButton.Size = new System.Drawing.Size(62, 22);
            this.stepButton.TabIndex = 4;
            this.stepButton.TabStop = false;
            this.stepButton.Text = "Шаг";
            this.formToolTip.SetToolTip(this.stepButton, "Ctrl + Y");
            this.stepButton.UseVisualStyleBackColor = true;
            this.stepButton.Click += new System.EventHandler(this.StepButtonClick_);
            this.stepButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            //
            // CallsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(400, 584);
            this.Controls.Add(this.stepButton);
            this.Controls.Add(this.listLabel);
            this.Controls.Add(this.callList);
            this.Controls.Add(this.currentPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "CallsForm";
            this.Text = "Память программы";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgramFormClosing_);
            this.ResizeEnd += new System.EventHandler(this.CallsFormResizeEnd_);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DefaultKeyDown_);
            this.Move += new System.EventHandler(this.CallsFormMove_);
            this.currentPanel.ResumeLayout(false);
            this.currentPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ColumnHeader firstColumn;
        private System.Windows.Forms.ColumnHeader addrColumn;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.Panel currentPanel;
        private System.Windows.Forms.Label addressTextLabel;
        private System.Windows.Forms.Label listLabel;
        private System.Windows.Forms.Label hexLabel;
        private System.Windows.Forms.Label commentLabel;
        public System.Windows.Forms.ListView callList;
        public System.Windows.Forms.TextBox commentText;
        public System.Windows.Forms.TextBox addressText;
        public System.Windows.Forms.Button downButton;
        public System.Windows.Forms.Button upButton;
        public System.Windows.Forms.Button addButton;
        public System.Windows.Forms.Button removeButton;
        public System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button stepButton;
        private System.Windows.Forms.ToolTip formToolTip;
    }
}
