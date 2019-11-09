using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mtemu
{
    public partial class MainForm : Form
    {
        private bool isProgramSaved_ = true;
        private bool isCommandSaved_ = true;
        private int prevSelected_ = -1;

        private MtProgram program_ = new MtProgram();
        private MtCommand currentCommand_;

        private Label[] textLabels_;
        private TextBox[] textBoxes_;
        private ListView[] listViewes_;

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

        private void UpdateLists_()
        {
            switch (currentCommand_.GetCommandType()) {
            case CommandType.MtCommand:
                m0CheckBox.Enabled = true;
                i02ListView.Enabled = true;
                m1CheckBox.Enabled = true;
                i68ListView.Enabled = true;
                cc4Text.Enabled = true;
                cc9Text.Enabled = true;

                ptListView.Enabled = false;
                psListView.Enabled = false;
                break;
            case CommandType.MemoryPointer:
            case CommandType.DevicePointer:
                m0CheckBox.Enabled = false;
                i02ListView.Enabled = false;
                m1CheckBox.Enabled = false;
                i68ListView.Enabled = false;
                cc4Text.Enabled = false;
                cc9Text.Enabled = false;

                ptListView.Enabled = true;
                psListView.Enabled = false;
                break;
            case CommandType.LoadCommand:
                m0CheckBox.Enabled = false;
                i02ListView.Enabled = false;
                m1CheckBox.Enabled = false;
                i68ListView.Enabled = false;
                cc4Text.Enabled = false;
                cc9Text.Enabled = false;

                ptListView.Enabled = false;
                psListView.Enabled = true;
                break;
            default:
                m0CheckBox.Enabled = true;
                i02ListView.Enabled = true;
                m1CheckBox.Enabled = true;
                i68ListView.Enabled = true;
                cc4Text.Enabled = true;
                cc9Text.Enabled = true;

                ptListView.Enabled = true;
                psListView.Enabled = true;
                break;
            }
        }

        private void UpdateCheckboxes_() {
            m0CheckBox.Checked = currentCommand_.GetFlag(FlagType.M0);
            m1CheckBox.Checked = currentCommand_.GetFlag(FlagType.M1);
        }

        private void UpdateCommandHandler_()
        {
            UpdateLabels_();
            UpdateLists_();
            UpdateCheckboxes_();
        }

        private void LoadTexts_()
        {
            for (int i = 0; i < textBoxes_.Length; ++i) {
                textBoxes_[i].Text = Helpers.IntToBinary(currentCommand_[i], 4);
            }
        }
        private void LoadCommand_(MtCommand command)
        {
            currentCommand_ = command;
            LoadTexts_();
            UpdateCommandHandler_();
        }

        public MainForm()
        {
            InitializeComponent();
            textLabels_ = new Label[] {
                cc0TextLabel,
                cc1TextLabel,
                cc2TextLabel,
                cc3TextLabel,
                cc4TextLabel,
                cc5TextLabel,
                cc6TextLabel,
                cc7TextLabel,
            };
            textBoxes_ = new TextBox[] {
                cc0Text,
                cc1Text,
                cc2Text,
                cc3Text,
                cc4Text,
                cc5Text,
                cc6Text,
                cc7Text,
                cc8Text,
                cc9Text,
            };
            listViewes_ = new ListView[] {
                caListView,
                i68ListView,
                i02ListView,
                i35ListView,
                ptListView,
                psListView,
            };

            // Init lists with values
            for (int i = 0; i < listViewes_.Length; ++i) {
                listViewes_[i].Items.Clear();
                string[][] lists = MtCommand.GetList((ListType) i);
                foreach (string[] list in lists) {
                    listViewes_[i].Items.Add(new ListViewItem(list));
                }
            }

            LoadCommand_(new MtCommand(new string[] {
                "0000",
                "0000",
                "0000",
                "0010",
                "0001",
                "0111",
                "0000",
                "0000",
                "0000",
                "0000",
            }));
        }

        private void CommandListSelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox) sender;
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

                currentCommand_ = new MtCommand(program_[listBox.SelectedIndex]);
                UpdateCommandHandler_();
                isCommandSaved_ = true;
            }
        }
        private void AddCommand_()
        {
            isProgramSaved_ = false;
            isCommandSaved_ = true;
            saveButton.Enabled = true;
            removeButton.Enabled = true;

            program_.AddCommand(new MtCommand(currentCommand_));

            int number = commandList.Items.Count;
            commandList.Items.Add(currentCommand_.GetName(number));
            commandList.SelectedIndex = number;
        }

        private void SaveCommand_()
        {
            isProgramSaved_ = false;
            isCommandSaved_ = true;

            int number = commandList.SelectedIndex;
            if (number != -1) {
                program_[number] = new MtCommand(currentCommand_);
                commandList.Items[number] = currentCommand_.GetName(number);
            }
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
            isProgramSaved_ = false;

            int number = commandList.SelectedIndex;
            program_.RemoveCommand(number);
            commandList.Items.RemoveAt(number);
            if (number >= commandList.Items.Count) {
                number = commandList.Items.Count - 1;
            }
            commandList.SelectedIndex = number;

            for (int i = 0; i < program_.Count(); ++i) {
                commandList.Items[i] = program_[i].GetName(i);
            }

            if (program_.Count() == 0) {
                saveButton.Enabled = false;
                removeButton.Enabled = false;
            }
        }

        private void StopButtonClick_(object sender, EventArgs e)
        {
            MessageBox.Show(
                "It will be soon...",
                "Ooops!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
        }

        private void StepButtonClick_(object sender, EventArgs e)
        {
            MessageBox.Show(
                "It will be soon...",
                "Ooops!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
        }

        private void AutoButtonClick_(object sender, EventArgs e)
        {
            MessageBox.Show(
                "It will be soon...",
                "Ooops!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
        }

        private void NewMenuItemClick_(object sender, EventArgs e)
        {
            MessageBox.Show(
                "It will be soon...",
                "Ooops!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
        }

        private void OpenMenuItemClick_(object sender, EventArgs e)
        {
            MessageBox.Show(
                "It will be soon...",
                "Ooops!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
        }

        private void SaveMenuItemClick_(object sender, EventArgs e)
        {
            DialogResult saveRes = MessageBox.Show(
                "Здесь должно быть сохранение",
                "Сохранение",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
            if (saveRes == DialogResult.OK) {
                isProgramSaved_ = true;
            }
        }

        private void SaveAsMenuItemClick_(object sender, EventArgs e)
        {
            DialogResult saveRes = MessageBox.Show(
                "Здесь должно быть сохранение",
                "Сохранение",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
            if (saveRes == DialogResult.OK) {
                isProgramSaved_ = true;
            }
        }

        private void HelpMenuItemClick_(object sender, EventArgs e)
        {
            MessageBox.Show(
                "It will be soon...",
                "Ooops!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
        }

        private void ExitMenuItemClick_(object sender, EventArgs e)
        {
            Close();
        }

        private void MainFormClosing_(object sender, FormClosingEventArgs e)
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
                    DialogResult saveRes = MessageBox.Show(
                        "Здесь должно быть сохранение",
                        "Сохранение",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1
                    );
                    if (saveRes == DialogResult.OK) {
                        return;
                    }
                }
                if (res == DialogResult.No) {
                    return;
                }
                e.Cancel = true;
            }
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
                isCommandSaved_ = false;
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

        void ListViewColumnWidthChanging_(object list, ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = ((ListView) list).Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        private void DefaultListIndexChanged_(ListType listIndex)
        {
            ListView listView = listViewes_[(int) listIndex];

            // If no selection
            if (listView.SelectedIndices.Count < 1) {
                return;
            }

            // Get text box for selected list
            int textIndex = currentCommand_.GetTextIndexByList(listIndex);
            if (textIndex != -1) {
                TextBox textBox = textBoxes_[textIndex];

                // Copy value in textbox
                isCommandSaved_ = false;
                currentCommand_.SetBinary(listIndex, listView.SelectedIndices[0]);
                textBox.Text = Helpers.IntToBinary(currentCommand_[textIndex], 4);
                UpdateCommandHandler_();
            }
        }

        private void I35ListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(ListType.I35);
        }

        private void CaListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(ListType.CA);
        }

        private void I02ListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(ListType.I02);
        }

        private void I68ListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(ListType.I68);
        }

        private void PtListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(ListType.PT);
        }

        private void PsListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(ListType.PS);
        }

        private void DefaultCheckBoxChanged_(FlagType flagIndex, bool value)
        {
            // Get text box for checkbox
            int textIndex = currentCommand_.GetTextIndexByFlag(flagIndex);
            if (textIndex != -1) {
                TextBox textBox = textBoxes_[textIndex];

                // Copy value in textbox
                isCommandSaved_ = false;
                currentCommand_.SetFlag(flagIndex, value);
                textBox.Text = Helpers.IntToBinary(currentCommand_[textIndex], 4);
                UpdateCommandHandler_();
            }
        }

        private void M0CheckBoxCheckedChanged_(object sender, EventArgs e)
        {
            DefaultCheckBoxChanged_(FlagType.M0, ((CheckBox) sender).Checked);
        }

        private void M1CheckBoxCheckedChanged_(object sender, EventArgs e)
        {
            DefaultCheckBoxChanged_(FlagType.M1, ((CheckBox) sender).Checked);
        }
    }
}
