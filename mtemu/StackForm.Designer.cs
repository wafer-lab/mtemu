namespace mtemu
{
    partial class StackForm
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
            this.stackListView = new System.Windows.Forms.ListView();
            this.stackFirstColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stackCodeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stackNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // stackListView
            // 
            this.stackListView.AutoArrange = false;
            this.stackListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stackListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.stackFirstColumn,
            this.stackCodeColumn,
            this.stackNameColumn});
            this.stackListView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.stackListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackListView.FullRowSelect = true;
            this.stackListView.GridLines = true;
            this.stackListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.stackListView.Location = new System.Drawing.Point(0, 0);
            this.stackListView.Margin = new System.Windows.Forms.Padding(0);
            this.stackListView.MultiSelect = false;
            this.stackListView.Name = "stackListView";
            this.stackListView.Size = new System.Drawing.Size(184, 291);
            this.stackListView.TabIndex = 1;
            this.stackListView.TabStop = false;
            this.stackListView.UseCompatibleStateImageBehavior = false;
            this.stackListView.View = System.Windows.Forms.View.Details;
            // 
            // stackFirstColumn
            // 
            this.stackFirstColumn.DisplayIndex = 2;
            this.stackFirstColumn.Width = 0;
            // 
            // stackCodeColumn
            // 
            this.stackCodeColumn.DisplayIndex = 0;
            this.stackCodeColumn.Text = "Адрес";
            this.stackCodeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.stackCodeColumn.Width = 48;
            // 
            // stackNameColumn
            // 
            this.stackNameColumn.DisplayIndex = 1;
            this.stackNameColumn.Text = "Значение";
            this.stackNameColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.stackNameColumn.Width = 118;
            // 
            // StackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(184, 291);
            this.Controls.Add(this.stackListView);
            this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "StackForm";
            this.ShowInTaskbar = false;
            this.Text = "Стек";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StackFormClosing_);
            this.ResizeEnd += new System.EventHandler(this.StackFormResizeEnd_);
            this.Move += new System.EventHandler(this.StackFormMove_);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader stackFirstColumn;
        private System.Windows.Forms.ColumnHeader stackCodeColumn;
        private System.Windows.Forms.ColumnHeader stackNameColumn;
        public System.Windows.Forms.ListView stackListView;
    }
}