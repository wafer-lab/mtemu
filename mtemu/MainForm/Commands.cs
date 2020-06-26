using System;
using System.Drawing;
using System.Windows.Forms;

namespace mtemu
{
    partial class MainForm
    {
        private void UpdateLabels_()
        {
            for (int i = 0; i < textLabels_.Length; ++i) {
                string newLabel = currentCommand_.GetLabel(i);
                if (i == 0) {
                    newLabel += $" = 0x{currentCommand_.GetNextAddr():X3}";
                }
                if (i == 0 || newLabel.Length <= 5) {
                    textLabels_[i].Font = new Font("Consolas", 10F);
                }
                else if (newLabel.Length <= 7) {
                    textLabels_[i].Font = new Font("Consolas", 10F - newLabel.Length + 5);
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
        //  COMMAND LIST  //
        ////////////////////

        private void SelectCommand_(int index, Color selectedColor)
        {
            if (0 <= selected_ && selected_ < commandList.Items.Count) {
                commandList.Items[selected_].BackColor = enabledColor_;
            }
            selected_ = index;
            if (0 <= selected_ && selected_ < commandList.Items.Count) {
                commandList.Items[selected_].BackColor = selectedColor;
                commandList.EnsureVisible(selected_);
            }
        }

        private void ChangeCommand_(int newSelected, Color color, bool force = false)
        {
            if (newSelected < -1 || commandList.Items.Count <= newSelected) {
                return;
            }

            if (newSelected != selected_ || force) {
                if (!isCommandSaved_ && !force) {
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
                }
                SelectCommand_(newSelected, color);

                if (newSelected == -1) {
                    LoadCommand_(Command.GetDefault());

                    removeButton.Enabled = false;
                    saveButton.Enabled = false;
                    upButton.Enabled = false;
                    downButton.Enabled = false;
                }
                else {
                    LoadCommand_(new Command(emulator_.GetCommand(newSelected)));

                    removeButton.Enabled = true;
                    saveButton.Enabled = true;
                    if (newSelected == 0) {
                        upButton.Enabled = false;
                    }
                    else {
                        upButton.Enabled = true;
                    }
                    if (newSelected == commandList.Items.Count - 1) {
                        downButton.Enabled = false;
                    }
                    else {
                        downButton.Enabled = true;
                    }
                }
            }
        }

        private void SelectPrevCommand_(int index)
        {
            if (0 <= nextSelected_ && nextSelected_ < commandList.Items.Count) {
                if (commandList.Items[nextSelected_].BackColor == nextSelectedColor_) {
                    commandList.Items[nextSelected_].BackColor = enabledColor_;
                }
            }
            nextSelected_ = index;
            if (0 <= nextSelected_ && nextSelected_ < commandList.Items.Count) {
                commandList.Items[nextSelected_].BackColor = nextSelectedColor_;
            }
        }

        private ListViewItem CommandToItems(Command command)
        {
            string number = "";
            string jump = "";
            if (!command.isOffset) {
                number = $"0x{command.GetNumber():X3}";
                jump = command.GetJumpName();
            }
            ListViewItem lv = new ListViewItem(new string[] { "", number, command.GetName(), jump });
            if (command.isOffset) {
                lv.Font = new Font("Consolas", 10F, FontStyle.Italic | FontStyle.Underline);
            }
            return lv;
        }

        private void CommandListSelectedIndexChanged_(object sender, EventArgs e)
        {
            if (commandList.SelectedIndices.Count != 0) {
                ChangeCommand_(commandList.SelectedIndices[0], selectedColor_);
            }
        }

        ////////////////////
        //    CONTROLS    //
        ////////////////////

        private void IncorrectCommandDialog()
        {
            MessageBox.Show(
                "В одной из ячеек введено неправильное значение!",
                "Неправильная команда!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1
            );
        }

        private void AddCommand_()
        {
            isProgramSaved_ = false;
            isCommandSaved_ = true;

            int index = selected_ + 1;
            if (!emulator_.AddCommand(index, new Command(currentCommand_))) {
                IncorrectCommandDialog();
                return;
            }
            commandList.Items.Insert(index, CommandToItems(emulator_.GetCommand(index)));

            for (int i = index; i < emulator_.CommandsCount(); ++i) {
                commandList.Items[i] = CommandToItems(emulator_.GetCommand(i));
            }
            ChangeCommand_(index, selectedColor_);
        }

        private void SaveCommand_()
        {
            isProgramSaved_ = false;
            isCommandSaved_ = true;

            int number = selected_;
            if (number != -1) {
                if (!emulator_.UpdateCommand(number, new Command(currentCommand_))) {
                    IncorrectCommandDialog();
                    return;
                }

                for (int i = number; i < emulator_.CommandsCount(); ++i) {
                    commandList.Items[i] = CommandToItems(emulator_.GetCommand(i));
                }
                ChangeCommand_(number, selectedColor_, true);
            }
        }

        private void RemoveCommand_()
        {
            if (0 <= selected_ && selected_ < commandList.Items.Count) {
                isProgramSaved_ = false;

                int number = selected_;
                emulator_.RemoveCommand(number);
                commandList.Items.RemoveAt(number);
                if (number >= commandList.Items.Count) {
                    number = commandList.Items.Count - 1;
                }

                if (number != -1) {
                    for (int i = number; i < emulator_.CommandsCount(); ++i) {
                        commandList.Items[i] = CommandToItems(emulator_.GetCommand(i));
                    }
                }

                ChangeCommand_(number, selectedColor_, true);
            }
        }

        private void MoveUpCommand_()
        {
            int index = selected_;
            if (index == 0) {
                return;
            }
            emulator_.MoveCommandUp(index);

            for (int i = index - 1; i < emulator_.CommandsCount(); ++i) {
                commandList.Items[i] = CommandToItems(emulator_.GetCommand(i));
            }

            ChangeCommand_(index - 1, selectedColor_);

            isProgramSaved_ = false;
        }

        private void MoveDownCommand_()
        {
            int index = selected_;
            if (index == commandList.Items.Count - 1) {
                return;
            }
            emulator_.MoveCommandDown(index);

            for (int i = index; i < emulator_.CommandsCount(); ++i) {
                commandList.Items[i] = CommandToItems(emulator_.GetCommand(i));
            }

            ChangeCommand_(index + 1, selectedColor_);

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
        //   TEXT BOXES   //
        ////////////////////

        private void RadioButtonTabStopChanged(object sender, EventArgs e)
        {
            (sender as RadioButton).TabStop = false;
        }

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

            if (DefaultKeyDown_(e)) {
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up) {
                if (value > 0) {
                    textBox.Text = Helpers.IntToBinary(value - 1, 4);
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down) {
                if (value < 15) {
                    textBox.Text = Helpers.IntToBinary(value + 1, 4);
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
