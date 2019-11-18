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
        private string filenameLocal_;

        private string filename_ {
            get { return filenameLocal_; }
            set {
                filenameLocal_ = value;
                this.Text = "mtemu";
                if (value != null) {
                    this.Text += " - " + filenameLocal_;
                }
            }
        }
        private bool isProgramSaved_;
        private bool isCommandSaved_;
        private int prevSelected_;

        private Emulator emulator_;
        private Command currentCommand_;

        private Label[] textLabels_;
        private Label[] regLabels_;
        private TextBox[] textBoxes_;
        private Dictionary<WordType, ListView> listViewes_;
        private TextBox[] regTexts_;

        MemoryForm memoryForm_;
        StackForm stackForm_;

        private bool Reset_(string filename = null)
        {
            LoadCommand_(new Command(new string[] {
                "0000", "0000", "0000", "0010", "0001", "0111", "0000", "0000", "0000", "0000",
                //"1111", "1111", "1111", "0101", "1100", "1001", "1000", "1111", "1111", "1111",
            }));

            filename_ = filename;
            isProgramSaved_ = true;
            isCommandSaved_ = true;
            prevSelected_ = -1;
            saveButton.Enabled = false;
            removeButton.Enabled = false;
            upButton.Enabled = false;
            downButton.Enabled = false;

            emulator_ = new Emulator();
            commandList.Items.Clear();
            if (filename != null) {
                if (!emulator_.OpenFile(filename)) {
                    return false;
                }
                for (int i = 0; i < emulator_.Count(); ++i) {
                    commandList.Items.Add(emulator_[i].GetName(i));
                }
            }
            if (emulator_.Count() > 0) {
                saveButton.Enabled = true;
                removeButton.Enabled = true;
                upButton.Enabled = true;
                downButton.Enabled = true;
            }

            UpdateOutput_(true);
            return true;
        }

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

        private void UpdateCheckboxes_()
        {
            m0CheckBox.Checked = currentCommand_.GetFlag(FlagType.M0);
            m1CheckBox.Checked = currentCommand_.GetFlag(FlagType.M1);
        }

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

        private void EnableObject_(Control obj)
        {
            obj.Enabled = true;
            obj.BackColor = System.Drawing.SystemColors.Window;
            obj.ForeColor = System.Drawing.SystemColors.WindowText;
            obj.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void DisableObject_(Control obj)
        {
            obj.Enabled = false;
            obj.BackColor = System.Drawing.SystemColors.Control;
            obj.ForeColor = System.Drawing.SystemColors.GrayText;
            obj.Cursor = System.Windows.Forms.Cursors.No;
        }

        private void UpdateLists_()
        {
            switch (currentCommand_.GetCommandType()) {
            case CommandType.MtCommand:
                EnableObject_(flagPanel);
                EnableObject_(i02ListView);
                EnableObject_(i68ListView);
                EnableObject_(cc4Text);
                EnableObject_(cc7Text);
                EnableObject_(cc9Text);

                DisableObject_(ptListView);
                DisableObject_(deviceListView);
                DisableObject_(psListView);
                break;
            case CommandType.MemoryPointer:
                DisableObject_(flagPanel);
                DisableObject_(i02ListView);
                DisableObject_(i68ListView);
                DisableObject_(cc4Text);
                EnableObject_(cc7Text);
                DisableObject_(cc9Text);

                EnableObject_(ptListView);
                DisableObject_(deviceListView);
                DisableObject_(psListView);
                break;
            case CommandType.DevicePointer:
                DisableObject_(flagPanel);
                DisableObject_(i02ListView);
                DisableObject_(i68ListView);
                DisableObject_(cc4Text);
                EnableObject_(cc7Text);
                DisableObject_(cc9Text);

                EnableObject_(ptListView);
                EnableObject_(deviceListView);
                DisableObject_(psListView);
                break;
            case CommandType.LoadCommand:
                DisableObject_(flagPanel);
                DisableObject_(i02ListView);
                DisableObject_(i68ListView);
                DisableObject_(cc4Text);
                EnableObject_(cc7Text);
                DisableObject_(cc9Text);

                DisableObject_(ptListView);
                DisableObject_(deviceListView);
                EnableObject_(psListView);
                break;
            case CommandType.LoadSmallCommand:
                DisableObject_(flagPanel);
                DisableObject_(i02ListView);
                DisableObject_(i68ListView);
                DisableObject_(cc4Text);
                DisableObject_(cc7Text);
                DisableObject_(cc9Text);

                DisableObject_(ptListView);
                DisableObject_(deviceListView);
                EnableObject_(psListView);
                break;
            default:
                EnableObject_(flagPanel);
                EnableObject_(i02ListView);
                EnableObject_(i68ListView);
                EnableObject_(cc4Text);
                EnableObject_(cc7Text);
                EnableObject_(cc9Text);

                EnableObject_(ptListView);
                EnableObject_(deviceListView);
                EnableObject_(psListView);
                break;
            }
        }
        
        private void UpdateTexts_()
        {
            for (int i = 0; i < textBoxes_.Length; ++i) {
                if (textBoxes_[i].BackColor == System.Drawing.Color.Wheat) {
                    textBoxes_[i].BackColor = System.Drawing.SystemColors.Window;
                }
            }
        }

        private void UpdateCommandHandler_()
        {
            UpdateLabels_();
            UpdateLists_();
            UpdateCheckboxes_();
            UpdateTexts_();
        }

        private void LoadTexts_()
        {
            for (int i = 0; i < textBoxes_.Length; ++i) {
                textBoxes_[i].Text = Helpers.IntToBinary(currentCommand_[i], 4);
            }
        }
        private void LoadCommand_(Command command)
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
            listViewes_ = new Dictionary<WordType, ListView> {
                { WordType.CA, caListView },
                { WordType.I68, i68ListView },
                { WordType.I02, i02ListView },
                { WordType.I35, i35ListView },
                { WordType.PT, ptListView },
                { WordType.PS, psListView },
                { WordType.Device, deviceListView },
            };
            regTexts_ = new TextBox[] {
                r0Text,
                r1Text,
                r2Text,
                r3Text,
                r4Text,
                r5Text,
                r6Text,
                r7Text,
                r8Text,
                r9Text,
                r10Text,
                r11Text,
                r12Text,
                r13Text,
                r14Text,
                r15Text,
            };
            regLabels_ = new Label[] {
                r0Label,
                r1Label,
                r2Label,
                r3Label,
                r4Label,
                r5Label,
                r6Label,
                r7Label,
                r8Label,
                r9Label,
                r10Label,
                r11Label,
                r12Label,
                r13Label,
                r14Label,
                r15Label,
            };
            for (int i = 0; i < Emulator.GetRegSize(); ++i) {
                regLabels_[i].Text = $"R{i:d}";
            }

            memoryForm_ = new MemoryForm();
            for (int i = 0; i < Emulator.GetMemorySize(); ++i) {
                memoryForm_.memoryListView.Items.Add(new ListViewItem(new string[] {"", $"0x{i:X2}", "0000"}));
            }
            stackForm_ = new StackForm();
            for (int i = 0; i < Emulator.GetStackSize(); ++i) {
                stackForm_.stackListView.Items.Add(new ListViewItem(new string[] { "", $"0x{i:X}", "0000" }));
            }

            // Init lists with values
            foreach (KeyValuePair<WordType, ListView> listView in listViewes_) {
                listView.Value.Items.Clear();
                string[][] lists = Command.GetItems(listView.Key);
                foreach (string[] list in lists) {
                    listView.Value.Items.Add(new ListViewItem(list));
                }
            }

            Reset_();
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

                LoadCommand_(new Command(emulator_[listBox.SelectedIndex]));
                isCommandSaved_ = true;
            }
        }
        private void AddCommand_()
        {
            isProgramSaved_ = false;
            isCommandSaved_ = true;
            saveButton.Enabled = true;
            removeButton.Enabled = true;
            upButton.Enabled = true;
            downButton.Enabled = true;

            emulator_.AddCommand(new Command(currentCommand_));

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
                emulator_[number] = new Command(currentCommand_);
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
            emulator_.RemoveCommand(number);
            commandList.Items.RemoveAt(number);
            if (number >= commandList.Items.Count) {
                number = commandList.Items.Count - 1;
            }
            commandList.SelectedIndex = number;

            for (int i = 0; i < emulator_.Count(); ++i) {
                commandList.Items[i] = emulator_[i].GetName(i);
            }

            if (emulator_.Count() == 0) {
                saveButton.Enabled = false;
                removeButton.Enabled = false;
                upButton.Enabled = false;
                downButton.Enabled = false;
            }
        }

        private void ResetButtonClick_(object sender, EventArgs e)
        {
            emulator_.Reset();
            UpdateOutput_();
        }

        private void StepProgram_()
        {
            emulator_.ExecOne();
            UpdateOutput_();
        }

        private void StepButtonClick_(object sender, EventArgs e)
        {
            StepProgram_();
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

        private void SaveMenuItemClick_(object sender, EventArgs e)
        {
            SaveDialog_();
        }

        private void SaveAsMenuItemClick_(object sender, EventArgs e)
        {
            SaveDialog_(true);
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

        private void ExitMenuItemClick_(object sender, EventArgs e)
        {
            Close();
        }

        private void MainFormClosing_(object sender, FormClosingEventArgs e)
        {
            if (!BeforeCloseProgram_()) {
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

        private void DefaultListIndexChanged_(WordType type)
        {
            ListView listView = listViewes_[type];

            // If no selection
            if (listView.SelectedIndices.Count < 1) {
                return;
            }

            // Get text box for selected list
            int textIndex = Command.GetTextIndexByType(type);
            if (textIndex != -1) {
                TextBox textBox = textBoxes_[textIndex];

                // Copy value in textbox
                isCommandSaved_ = false;
                currentCommand_.SetValue(type, listView.SelectedIndices[0]);
                textBox.Text = Helpers.IntToBinary(currentCommand_[textIndex], 4);
                textBox.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void I35ListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(WordType.I35);
        }

        private void CaListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(WordType.CA);
        }

        private void I02ListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(WordType.I02);
        }

        private void I68ListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(WordType.I68);
        }

        private void PtListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(WordType.PT);
        }

        private void PsListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(WordType.PS);
        }

        private void DeviceListViewSelectedIndexChanged_(object sender, EventArgs e)
        {
            DefaultListIndexChanged_(WordType.Device);
        }

        private void DefaultCheckBoxChanged_(FlagType flagIndex, bool value)
        {
            // Get text box for checkbox
            int textIndex = Command.GetTextIndexByFlag(flagIndex);
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

        private void DefaultTextEnter_(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.TextLength;
        }

        private void StackFormMove_()
        {
            stackForm_.Top = Top;
            stackForm_.Left = Right;
            stackForm_.Height = Height / 2;
        }

        private void StackMenuItemClick_(object sender, EventArgs e)
        {
            stackForm_.Show(this);
            StackFormMove_();
            this.Focus();
        }

        private void MemoryFormMove_()
        {
            memoryForm_.Top = Top + stackForm_.Height;
            memoryForm_.Left = Right;
            memoryForm_.Height = Height / 2;
        }

        private void MemoryMenuItemClick_(object sender, EventArgs e)
        {
            memoryForm_.Show(this);
            MemoryFormMove_();
            this.Focus();
        }

        private void MoveSubForms_()
        {
            StackFormMove_();
            MemoryFormMove_();
        }

        private void MainFormMove_(object sender, EventArgs e)
        {
            MoveSubForms_();
        }

        private void UpButtonClick_(object sender, EventArgs e)
        {
            isProgramSaved_ = false;

            int index = commandList.SelectedIndex;
            if (index == 0) {
                return;
            }
            commandList.Items.Insert(index - 1, commandList.Items[index]);
            commandList.Items.RemoveAt(index + 1);
            emulator_.MoveCommandUp(index);
            commandList.SelectedIndex = index - 1;

            for (int i = 0; i < emulator_.Count(); ++i) {
                commandList.Items[i] = emulator_[i].GetName(i);
            }
        }

        private void DownButtonClick_(object sender, EventArgs e)
        {
            isProgramSaved_ = false;

            int index = commandList.SelectedIndex;
            if (index == commandList.Items.Count - 1) {
                return;
            }
            commandList.Items.Insert(index + 2, commandList.Items[index]);
            commandList.Items.RemoveAt(index);
            emulator_.MoveCommandDown(index);
            commandList.SelectedIndex = index + 1;

            for (int i = 0; i < emulator_.Count(); ++i) {
                commandList.Items[i] = emulator_[i].GetName(i);
            }
        }
    }
}
