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
        private void UpdateLabels_()
        {
            for (int i = 0; i < textLabels_.Length; ++i) {
                string newLabel = currentCommand_.GetLabel(i);
                if (newLabel.Length <= 6) {
                    textLabels_[i].Font = new Font("Consolas", 10F);
                }
                else {
                    textLabels_[i].Font = new Font("Consolas", 10F - newLabel.Length + 6);
                }
                textLabels_[i].Text = newLabel;
            }
        }

        private void UpdateTexts_()
        {
            for (int i = 0; i < textBoxes_.Length; ++i) {
                textBoxes_[i].Text = Helpers.IntToBinary(currentCommand_[i], 4);
                textBoxes_[i].BackColor = enabledColor_;
            }
        }

        private void LoadCommand_(Command command)
        {
            currentCommand_ = command;
            UpdateCommandHandler_();
            isCommandSaved_ = true;
        }

        ////////////////////
        //    CONTROLS    //
        ////////////////////

        private void AddCommand_()
        {
            isProgramSaved_ = false;
            isCommandSaved_ = true;

            emulator_.AddCommand(new Command(currentCommand_));
            commandList.Items.Add(emulator_.LastCommand().GetName());
            commandList.SelectedIndex = commandList.Items.Count - 1;
        }

        private void SaveCommand_()
        {
            isProgramSaved_ = false;
            isCommandSaved_ = true;

            int number = commandList.SelectedIndex;
            if (number != -1) {
                emulator_.UpdateCommand(number, new Command(currentCommand_));

                for (int i = number; i < emulator_.Count(); ++i) {
                    commandList.Items[i] = emulator_.GetCommand(i).GetName();
                }
            }
        }

        private void RemoveCommand_()
        {
            isProgramSaved_ = false;

            int number = commandList.SelectedIndex;
            emulator_.RemoveCommand(number);
            commandList.Items.RemoveAt(number);
            if (number >= commandList.Items.Count) {
                number = commandList.Items.Count - 1;
            }
            commandList.SelectedIndex = number;

            for (int i = number; i < emulator_.Count(); ++i) {
                commandList.Items[i] = emulator_.GetCommand(i).GetName();
            }
        }

        private void MoveUpCommand_()
        {
            int index = commandList.SelectedIndex;
            if (index == 0) {
                return;
            }
            commandList.Items.Insert(index - 1, commandList.Items[index]);
            commandList.Items.RemoveAt(index + 1);
            emulator_.MoveCommandUp(index);
            commandList.SelectedIndex = index - 1;

            for (int i = index - 1; i < emulator_.Count(); ++i) {
                commandList.Items[i] = emulator_.GetCommand(i).GetName();
            }

            isProgramSaved_ = false;
        }

        private void MoveDownCommand_()
        {
            int index = commandList.SelectedIndex;
            if (index == commandList.Items.Count - 1) {
                return;
            }
            commandList.Items.Insert(index + 2, commandList.Items[index]);
            commandList.Items.RemoveAt(index);
            emulator_.MoveCommandDown(index);
            commandList.SelectedIndex = index + 1;

            for (int i = index; i < emulator_.Count(); ++i) {
                commandList.Items[i] = emulator_.GetCommand(i).GetName();
            }

            isProgramSaved_ = false;
        }

        private void AddButtonClick_(object sender, EventArgs e)
        {
            AddCommand_();
        }

        private void SaveButtonClick_(object sender, EventArgs e)
        {
            SaveCommand_();
        }

        private void RemoveButtonClick_(object sender, EventArgs e)
        {
            RemoveCommand_();
        }

        private void UpButtonClick_(object sender, EventArgs e)
        {
            MoveUpCommand_();
        }

        private void DownButtonClick_(object sender, EventArgs e)
        {
            MoveDownCommand_();
        }

        ////////////////////
        //  COMMAND LIST  //
        ////////////////////

        private void CommandListSelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox) sender;
            if (listBox.SelectedIndex == -1) {
                removeButton.Enabled = false;
                saveButton.Enabled = false;
                upButton.Enabled = false;
                downButton.Enabled = false;
            }
            else {
                removeButton.Enabled = true;
                saveButton.Enabled = true;
                if (listBox.SelectedIndex == 0) {
                    upButton.Enabled = false;
                }
                else {
                    upButton.Enabled = true;
                }
                if (listBox.SelectedIndex == listBox.Items.Count - 1) {
                    downButton.Enabled = false;
                }
                else {
                    downButton.Enabled = true;
                }
            }

            if (listBox.SelectedIndex != -1 && listBox.SelectedIndex != prevSelected_) {
                if (!isCommandSaved_) {
                    int newSelected = listBox.SelectedIndex;
                    listBox.SelectedIndex = prevSelected_;

                    DialogResult saveRes = MessageBox.Show(
                        "Сохранить текущую команду?",
                        "Сохранение",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    );
                    if (saveRes == DialogResult.Cancel) {
                        return;
                    }
                    if (saveRes == DialogResult.Yes) {
                        SaveCommand_();
                    }

                    prevSelected_ = newSelected;
                    listBox.SelectedIndex = newSelected;
                }
                prevSelected_ = listBox.SelectedIndex;

                LoadCommand_(new Command(emulator_.GetCommand(listBox.SelectedIndex)));
            }
        }

        ////////////////////
        //   TEXT BOXES   //
        ////////////////////

        private void DefaultTextEnter_(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.TextLength;
        }

        private bool DefaultTextChanged_(int textIndex)
        {
            TextBox textBox = textBoxes_[textIndex];

            // Clear from wrong chars
            int selPos = textBox.SelectionStart;
            int selLen = textBox.SelectionLength;
            textBox.Text = Helpers.ClearBinary(textBox.Text, ref selPos);
            textBox.SelectionStart = selPos;
            textBox.SelectionLength = selLen;

            // Save value
            if (textBox.Text.Length == 4) {
                currentCommand_[textIndex] = Helpers.BinaryToInt(textBox.Text);
                UpdateCommandHandler_();
            }
            return true;
        }

        private void Cc0TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(0);
        }

        private void Cc1TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(1);
        }

        private void Cc2TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(2);
        }

        private void Cc3TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(3);
        }

        private void Cc4TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(4);
        }

        private void Cc5TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(5);
        }

        private void Cc6TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(6);
        }

        private void Cc7TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(7);
        }

        private void Cc8TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(8);
        }

        private void Cc9TextChanged_(object sender, EventArgs e)
        {
            DefaultTextChanged_(9);
        }

        private bool DefaultTextKeyDown_(int index, KeyEventArgs e)
        {
            TextBox textBox = textBoxes_[index];

            int selPos = textBox.SelectionStart;
            int selLen = textBox.SelectionLength;

            int value = Helpers.BinaryToInt(textBox.Text);
            if (e.KeyCode == Keys.Up) {
                if (value > 0) {
                    textBox.Text = Helpers.IntToBinary(value - 1, 4);
                }
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Down) {
                if (value < 15) {
                    textBox.Text = Helpers.IntToBinary(value + 1, 4);
                }
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter) {
                if (e.Control) {
                    AddCommand_();
                }
                else {
                    SaveCommand_();
                }
                e.Handled = true;
            }

            textBox.SelectionStart = selPos;
            textBox.SelectionLength = selLen;
            return true;
        }

        private void Cc0TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(0, e);
        }

        private void Cc1TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(1, e);
        }

        private void Cc2TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(2, e);
        }

        private void Cc3TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(3, e);
        }

        private void Cc4TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(4, e);
        }

        private void Cc5TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(5, e);
        }

        private void Cc6TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(6, e);
        }

        private void Cc7TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(7, e);
        }

        private void Cc8TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(8, e);
        }

        private void Cc9TextKeyDown_(object sender, KeyEventArgs e)
        {
            DefaultTextKeyDown_(9, e);
        }
    }
}
