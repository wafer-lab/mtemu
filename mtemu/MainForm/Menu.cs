﻿using System;
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
            DialogResult dr = extenderSettingsForm_.ShowDialog(this);

            if (dr == DialogResult.OK) {
                // TODO: Add react to result
                PortExtender.DeviceInfo devInfo = extenderSettingsForm_.GetSelectedDeviceInfo();
                bool res = portExtender_.SelectDevice(devInfo);
                if(!res)
                    MessageBox.Show("Не удалось выбрать внешнее устройство", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                // TODO: Add react to result
            }
        }

        private void HelpMenuItemClick_(object sender, EventArgs e)
        {
            helpForm_.ShowDialog(this);
        }
    }
}
