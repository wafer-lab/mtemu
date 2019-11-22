using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mtemu
{
    partial class MainForm
    {
        private void SetFlag_(TextBox textBox, string prefix, bool value)
        {
            textBox.Text = prefix + (value ? "1" : "0");
            if (value) {
                textBox.BackColor = Color.LightGreen;
            }
            else {
                textBox.BackColor = disabledColor_;
            }
        }

        private void SetOut_(TextBox textBox, string value, bool asNew)
        {
            string oldValue = textBox.Text;
            textBox.Text = value;
            if (asNew || textBox.Text == oldValue) {
                textBox.BackColor = disabledColor_;
            }
            else {
                textBox.BackColor = changedColor_;
            }
        }

        private void SetOut_(TextBox textBox, int value, bool asNew)
        {
            SetOut_(textBox, Helpers.IntToBinary(value, 4), asNew);
        }

        private void UpdateOutput_(bool asNew = false)
        {
            SetFlag_(ovrText, "OVR=", emulator_.GetOvr());
            SetFlag_(c4Text, "C4=", emulator_.GetC4());
            SetFlag_(f3Text, "F3=", emulator_.GetF3());
            SetFlag_(zText, "Z=", emulator_.GetZ());
            SetFlag_(gText, "/G=", emulator_.GetG());
            SetFlag_(pText, "/P=", emulator_.GetP());

            SetOut_(fText, emulator_.GetF(), asNew);
            SetOut_(spText, $"0x{emulator_.GetSP():X1}", asNew);
            SetOut_(mpText, $"0x{emulator_.GetMP():X2}", asNew);

            int pc = emulator_.GetPC();
            if (pc == -1) {
                pc = 0;
            }
            SetOut_(pcText, $"0x{pc:X3}", asNew);

            SetOut_(rqText, emulator_.GetRegQ(), asNew);
            for (int i = 0; i < Emulator.GetRegSize(); ++i) {
                SetOut_(regTexts_[i], emulator_.GetRegValue(i), asNew);
            }

            if (emulator_.Count() > 0) {
                ChangeCommand_(emulator_.GetNextIndex(), selectedColor_);
                SelectNextCommand_(emulator_.GetLastIndex());
            }

            for (int i = 0; i < Emulator.GetStackSize(); ++i) {
                ListViewItem item = stackForm_.stackListView.Items[i];
                ListViewItem.ListViewSubItem subitem = item.SubItems[2];
                string newText = $"0x{emulator_.GetStackValue(i):X3}";
                if (item.BackColor != enabledColor_) {
                    item.BackColor = enabledColor_;
                }
                if (subitem.Text != newText) {
                    subitem.Text = newText;
                    if (!asNew) {
                        item.BackColor = changedColor_;
                    }
                }
            }

            for (int i = 0; i < Emulator.GetMemorySize(); ++i) {
                ListViewItem item = memoryForm_.memoryListView.Items[i];
                string newText = Helpers.IntToBinary(emulator_.GetMemValue(i), 8, 4);
                string newText2 = $"0x{emulator_.GetMemValue(i):X2}";

                if (item.BackColor != enabledColor_) {
                    item.BackColor = enabledColor_;
                }

                if (item.SubItems[2].Text != newText) {
                    item.SubItems[2].Text = newText;
                    item.SubItems[3].Text = newText2;
                    if (!asNew) {
                        item.BackColor = changedColor_;
                    }
                }
            }
        }

        private void ResetButtonClick_(object sender, EventArgs e)
        {
            emulator_.Reset();
            UpdateOutput_();
        }

        private void ResultCodeHandler_(Emulator.ResultCode rc)
        {
            switch (rc) {
            case Emulator.ResultCode.Loop:
                MessageBox.Show(
                    "Не удалось определить, где заканчивается программа!",
                    "Залупа!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1
                );
                break;
            case Emulator.ResultCode.IncorrectCommand:
                MessageBox.Show(
                    "Невозможно исполнить команду!",
                    "Некорректная команда!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1
                );
                break;
            case Emulator.ResultCode.NoCommands:
                MessageBox.Show(
                    "Вы не добавили ни одной команды!",
                    "Нет комманд!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1
                );
                break;
            }

            UpdateOutput_();
        }

        private void StepButtonClick_(object sender, EventArgs e)
        {
            ResultCodeHandler_(emulator_.ExecOne());
        }

        private void AutoButtonClick_(object sender, EventArgs e)
        {
            ResultCodeHandler_(emulator_.ExecAll());
        }
    }
}
