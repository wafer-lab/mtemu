using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mtemu
{
    partial class MainForm
    {
        private bool BeforeCloseProgram_()
        {
            if (!isProgramSaved_) {
                DialogResult res = MessageBox.Show(
                    "Текущие изменения не сохранены. Хотите сохранить?",
                    "Вы уверены?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1
                );
                if (res == DialogResult.Yes) {
                    return SaveDialog_();
                }
                if (res == DialogResult.No) {
                    return true;
                }
                return false;
            }
            return true;
        }

        private void NewMenuItemClick_(object sender, EventArgs e)
        {
            if (BeforeCloseProgram_()) {
                Reset_();
            }
        }

        private bool OpenDialog_()
        {
            DialogResult openRes = openFileDialog.ShowDialog();
            if (openRes == DialogResult.OK) {
                if (openFileDialog.FileName != filename_) {
                    if (!Reset_(openFileDialog.FileName)) {
                        MessageBox.Show(
                            "Выбран файл некорректного формата!",
                            "Не удалось открыть файл!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1
                        );
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void OpenMenuItemClick_(object sender, EventArgs e)
        {
            if (BeforeCloseProgram_()) {
                OpenDialog_();
            }
        }

        private bool SaveDialog_(bool asNew = false)
        {
            if (filename_ == null || asNew) {
                saveFileDialog.FileName = filename_;
                DialogResult saveRes = saveFileDialog.ShowDialog();
                if (saveRes != DialogResult.OK) {
                    return false;
                }
                filename_ = saveFileDialog.FileName;
            }
            if (!emulator_.SaveFile(filename_)) {
                MessageBox.Show(
                    "Недостаточно прав для выполнения данного действия!",
                    "Не удалось сохранить файл!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1
                );
                return false;
            }
            isProgramSaved_ = true;
            return true;
        }

        private void SaveMenuItemClick_(object sender, EventArgs e)
        {
            SaveDialog_();
        }

        private void SaveAsMenuItemClick_(object sender, EventArgs e)
        {
            SaveDialog_(true);
        }

        private void ExitMenuItemClick_(object sender, EventArgs e)
        {
            Close();
        }

        private void StackMenuItemClick_(object sender, EventArgs e)
        {
            if (!stackForm_.Visible) {
                stackForm_.Show(this);
                StackFormMove_();
                this.Focus();
            }
        }

        private void MemoryMenuItemClick_(object sender, EventArgs e)
        {
            if (!memoryForm_.Visible) {
                memoryForm_.Show(this);
                MemoryFormMove_();
                this.Focus();
            }
        }

        private void SchemeMenuItemClick_(object sender, EventArgs e)
        {
            if (!schemeForm_.Visible) {
                schemeForm_.Show(this);
                SchemeFormMove_();
            }
        }

        private void ProgramMenuItemClick_(object sender, EventArgs e)
        {
            if (!callsForm_.Visible) {
                callsForm_.Show(this);
                CallsFormMove_();
            }
        }

        private void ExtenderSettingsMenuItemClick_(object sender, EventArgs e)
        {
            var deviceOpened = portExtender_.IsDeviceOpened();
            if (deviceOpened)
                portExtender_.CloseDevice();

            DialogResult dr = extenderSettingsForm_.ShowDialog(this);

            if (dr == DialogResult.OK) {
                PortExtender.DeviceInfo devInfo = extenderSettingsForm_.GetSelectedDeviceInfo();
                bool res = portExtender_.SelectDevice(devInfo);
                if (!res)
                    MessageBox.Show(
                        "Не удалось выбрать внешнее устройство",
                        "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                if (deviceOpened)
                    portExtender_.ReopenLastDevice();
            }
        }

        private void HelpMenuItemClick_(object sender, EventArgs e)
        {
            helpForm_.ShowDialog(this);
        }

        private void DebugMenuItemClick_(object sender, EventArgs e)
        {
            if (debugPanel.Visible) {
                debugPanel.Hide();
                Width -= debugPanel.Width;
                infoPanel.Left -= debugPanel.Width;
                memoryForm_.Left -= debugPanel.Width;
                stackForm_.Left -= debugPanel.Width;
                debugMenuItem.Text = debugMenuPrefix + " (показать)";
            }
            else {
                debugPanel.Show();
                Width += debugPanel.Width;
                infoPanel.Left += debugPanel.Width;
                memoryForm_.Left += debugPanel.Width;
                stackForm_.Left += debugPanel.Width;
                debugMenuItem.Text = debugMenuPrefix + " (скрыть)";
            }
        }

        private void InfoMenuItemClick_(object sender, EventArgs e)
        {
            if (infoPanel.Visible) {
                infoPanel.Hide();
                Width -= infoPanel.Width;
                infoPanel.Left -= infoPanel.Width;
                memoryForm_.Left -= infoPanel.Width;
                stackForm_.Left -= infoPanel.Width;
                infoMenuItem.Text = infoMenuPrefix + " (показать)";
            }
            else {
                infoPanel.Show();
                Width += infoPanel.Width;
                infoPanel.Left += infoPanel.Width;
                memoryForm_.Left += infoPanel.Width;
                stackForm_.Left += infoPanel.Width;
                infoMenuItem.Text = infoMenuPrefix + " (скрыть)";
            }
        }
    }
}
