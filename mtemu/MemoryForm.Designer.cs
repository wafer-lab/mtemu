namespace mtemu
{
    partial class MemoryForm
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
            this.memoryListView = new System.Windows.Forms.ListView();
            this.memoryFirstColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.memoryCodeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.memoryNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.memoryHexColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // memoryListView
            // 
            this.memoryListView.AutoArrange = false;
            this.memoryListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.memoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.memoryFirstColumn,
            this.memoryCodeColumn,
            this.memoryNameColumn,
            this.memoryHexColumn});
            this.memoryListView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.memoryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryListView.FullRowSelect = true;
            this.memoryListView.GridLines = true;
            this.memoryListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.memoryListView.Location = new System.Drawing.Point(0, 0);
            this.memoryListView.Margin = new System.Windows.Forms.Padding(0);
            this.memoryListView.MultiSelect = false;
            this.memoryListView.Name = "memoryListView";
            this.memoryListView.Size = new System.Drawing.Size(184, 291);
            this.memoryListView.TabIndex = 1;
            this.memoryListView.TabStop = false;
            this.memoryListView.UseCompatibleStateImageBehavior = false;
            this.memoryListView.View = System.Windows.Forms.View.Details;
            // 
            // memoryFirstColumn
            // 
            this.memoryFirstColumn.DisplayIndex = 2;
            this.memoryFirstColumn.Width = 0;
            // 
            // memoryCodeColumn
            // 
            this.memoryCodeColumn.DisplayIndex = 0;
            this.memoryCodeColumn.Text = "Адрес";
            this.memoryCodeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.memoryCodeColumn.Width = 48;
            // 
            // memoryNameColumn
            // 
            this.memoryNameColumn.DisplayIndex = 1;
            this.memoryNameColumn.Text = "Значение";
            this.memoryNameColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.memoryNameColumn.Width = 78;
            // 
            // memoryHexColumn
            // 
            this.memoryHexColumn.Text = "HEX";
            this.memoryHexColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.memoryHexColumn.Width = 40;
            // 
            // MemoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(184, 291);
            this.Controls.Add(this.memoryListView);
            this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MemoryForm";
            this.ShowInTaskbar = false;
            this.Text = "Память";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MemoryFormFormClosing_);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader memoryFirstColumn;
        private System.Windows.Forms.ColumnHeader memoryCodeColumn;
        private System.Windows.Forms.ColumnHeader memoryNameColumn;
        public System.Windows.Forms.ListView memoryListView;
        private System.Windows.Forms.ColumnHeader memoryHexColumn;
    }
}