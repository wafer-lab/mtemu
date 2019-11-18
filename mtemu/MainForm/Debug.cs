using System;
using System.Collections.Generic;
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
                textBox.BackColor = System.Drawing.Color.LightGreen;
            }
            else {
                textBox.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void SetOut_(bool asNew, TextBox textBox, string value)
        {
            string oldValue = textBox.Text;
            textBox.Text = value;
            if (asNew || textBox.Text == oldValue) {
                textBox.BackColor = System.Drawing.SystemColors.Control;
            }
            else {
                textBox.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void SetOut_(bool asNew, TextBox textBox, int value, int minLen = 4)
        {
            SetOut_(asNew, textBox, Helpers.IntToBinary(value, minLen));
        }

        private void UpdateOutput_(bool asNew = false)
        {
            SetFlag_(ovrText, "OVR=", emulator_.GetOvr());
            SetFlag_(c4Text, "C4=", emulator_.GetC4());
            SetFlag_(f3Text, "F3=", emulator_.GetF3());
            SetFlag_(zText, "Z=", emulator_.GetZ());
            SetFlag_(gText, "/G=", emulator_.GetG());
            SetFlag_(pText, "/P=", emulator_.GetP());

            SetOut_(asNew, fText, emulator_.GetF());
            SetOut_(asNew, spText, $"0x{emulator_.GetSP():X1}");
            SetOut_(asNew, mpText, $"0x{emulator_.GetMP():X2}");
            SetOut_(asNew, pcText, $"0x{emulator_.GetPC():X3}");

            SetOut_(asNew, rqText, emulator_.GetRegQ());
            for (int i = 0; i < Emulator.GetRegSize(); ++i) {
                SetOut_(asNew, regTexts_[i], emulator_.GetRegValue(i));
            }

            if (emulator_.Count() > 0) {
                commandList.SelectedIndex = emulator_.GetPrevPC();
            }

            for (int i = 0; i < Emulator.GetStackSize(); ++i) {
                ListViewItem.ListViewSubItem item = stackForm_.stackListView.Items[i].SubItems[2];
                string newText = Helpers.IntToBinary(emulator_.GetStackValue(i), 4);
                if (item.Text != newText) {
                    item.Text = newText;
                }
            }

            for (int i = 0; i < Emulator.GetMemorySize(); ++i) {
                ListViewItem.ListViewSubItem item = memoryForm_.memoryListView.Items[i].SubItems[2];
                string newText = Helpers.IntToBinary(emulator_.GetMemValue(i), 8, 4);
                if (item.Text != newText) {
                    item.Text = newText;
                }
            }
        }

        private void ResetButtonClick_(object sender, EventArgs e)
        {
            emulator_.Reset();
            UpdateOutput_();
        }

        private void StepButtonClick_(object sender, EventArgs e)
        {
            emulator_.ExecOne();
            UpdateOutput_();
        }

        private void AutoButtonClick_(object sender, EventArgs e)
        {
            if (!emulator_.ExecAll()) {
                MessageBox.Show(
                    "Не удалось определить, где заканчивается программа!",
                    "Залупа!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1
                );
            }
            UpdateOutput_();
        }
    }
}
