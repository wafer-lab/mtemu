namespace mtemu
{
    partial class ExtenderSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtenderSettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.devicesComboBox = new System.Windows.Forms.ComboBox();
            this.selectDeviceButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.updateListButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Устройство:";
            // 
            // devicesComboBox
            // 
            this.devicesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.devicesComboBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.devicesComboBox.FormattingEnabled = true;
            this.devicesComboBox.Location = new System.Drawing.Point(15, 29);
            this.devicesComboBox.Name = "devicesComboBox";
            this.devicesComboBox.Size = new System.Drawing.Size(307, 22);
            this.devicesComboBox.TabIndex = 1;
            // 
            // selectDeviceButton
            // 
            this.selectDeviceButton.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.selectDeviceButton.Location = new System.Drawing.Point(166, 58);
            this.selectDeviceButton.Name = "selectDeviceButton";
            this.selectDeviceButton.Size = new System.Drawing.Size(75, 23);
            this.selectDeviceButton.TabIndex = 2;
            this.selectDeviceButton.Text = "Выбрать";
            this.selectDeviceButton.UseVisualStyleBackColor = true;
            this.selectDeviceButton.Click += new System.EventHandler(this.selectDeviceButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cancelButton.Location = new System.Drawing.Point(247, 58);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // updateListButton
            // 
            this.updateListButton.Location = new System.Drawing.Point(15, 58);
            this.updateListButton.Name = "updateListButton";
            this.updateListButton.Size = new System.Drawing.Size(75, 23);
            this.updateListButton.TabIndex = 4;
            this.updateListButton.Text = "Обновить";
            this.updateListButton.UseVisualStyleBackColor = true;
            this.updateListButton.Click += new System.EventHandler(this.updateListButton_Click);
            // 
            // ExtenderSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(334, 93);
            this.Controls.Add(this.updateListButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.selectDeviceButton);
            this.Controls.Add(this.devicesComboBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExtenderSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройка внешнего устройства";
            this.Load += new System.EventHandler(this.ExtenderSettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox devicesComboBox;
        private System.Windows.Forms.Button selectDeviceButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button updateListButton;
    }
}