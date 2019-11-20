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
        private static Color enabledColor_ = SystemColors.Window;
        private static Color enabledTextColor_ = SystemColors.WindowText;
        private static Color disabledColor_ = SystemColors.Control;
        private static Color disabledTextColor_ = SystemColors.GrayText;
        private static Color changedColor_ = Color.Wheat;
        private static Color selectedColor_ = Color.LightBlue;
        private static Color nextSelectedColor_ = Color.LightGray;

        private string filenamePrivate_;
        private string filename_ {
            get { return filenamePrivate_; }
            set {
                filenamePrivate_ = value;
                this.Text = "mtemu";
                if (value != null) {
                    this.Text += " - " + filenamePrivate_;
                }
            }
        }
        
        private int selected_;
        private int nextSelected_;

        private bool isCommandSaved_;
        private bool isProgramSaved_;
        
        private Emulator emulator_;
        private Command currentCommand_;

        private Label[] textLabels_;
        private TextBox[] textBoxes_;
        private Label[] regLabels_;
        private TextBox[] regTexts_;
        private Dictionary<WordType, ListView> listViewes_;
        
        MemoryForm memoryForm_;
        StackForm stackForm_;

        public MainForm()
        {
            InitializeComponent();

            // Current command
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

            // Common register
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
            for (int i = 0; i < Emulator.GetRegSize(); ++i) {
                regLabels_[i].Text = $"R{i:d}";
            }

            // Info lists
            listViewes_ = new Dictionary<WordType, ListView> {
                { WordType.CA, caListView },
                { WordType.I68, i68ListView },
                { WordType.I02, i02ListView },
                { WordType.I35, i35ListView },
                { WordType.PT, ptListView },
                { WordType.PS, psListView },
                { WordType.Device, deviceListView },
            };
            foreach (KeyValuePair<WordType, ListView> listView in listViewes_) {
                listView.Value.Items.Clear();
                string[][] lists = Command.GetItems(listView.Key);
                foreach (string[] list in lists) {
                    listView.Value.Items.Add(new ListViewItem(list));
                }
            }

            // Memory debug form
            memoryForm_ = new MemoryForm();
            for (int i = 0; i < Emulator.GetMemorySize(); ++i) {
                memoryForm_.memoryListView.Items.Add(new ListViewItem(new string[] {"", $"0x{i:X2}", "0000"}));
            }

            // Stack debug form
            stackForm_ = new StackForm();
            for (int i = 0; i < Emulator.GetStackSize(); ++i) {
                stackForm_.stackListView.Items.Add(new ListViewItem(new string[] { "", $"0x{i:X}", "0000" }));
            }

            // Reset to initial values
            Reset_();
        }

        private bool Reset_(string filename = null)
        {
            LoadCommand_(Command.GetDefault());

            filename_ = filename;
            selected_ = -1;
            nextSelected_ = -1;
            isCommandSaved_ = true;
            isProgramSaved_ = true;

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
                    commandList.Items.Add(CommandToItems(emulator_.GetCommand(i)));
                }
            }

            UpdateOutput_(true);
            return true;
        }

        ////////////////////
        //    CONTROLS    //
        ////////////////////

        private void EnableObject_(Control obj)
        {
            if (!obj.Enabled) {
                obj.Enabled = true;
                obj.BackColor = enabledColor_;
                obj.ForeColor = enabledTextColor_;
                obj.Cursor = Cursors.Default;
            }
        }

        private void DisableObject_(Control obj)
        {
            if (obj.Enabled) {
                obj.Enabled = false;
                obj.BackColor = disabledColor_;
                obj.ForeColor = disabledTextColor_;
                obj.Cursor = Cursors.No;
            }
        }

        private void DisableItems_(Control[] objs)
        {
            if (objs.Contains(flagPanel)) {
                DisableObject_(flagPanel);
            }
            else {
                EnableObject_(flagPanel);
            }
            foreach (TextBox textBox in textBoxes_) {
                if (objs.Contains(textBox)) {
                    DisableObject_(textBox);
                }
                else {
                    EnableObject_(textBox);
                }
            }
            foreach (KeyValuePair<WordType, ListView> listView in listViewes_) {
                if (objs.Contains(listView.Value)) {
                    DisableObject_(listView.Value);
                }
                else {
                    EnableObject_(listView.Value);
                }
            }
        }

        private void UpdateItemsAvailability_()
        {
            switch (currentCommand_.GetCommandType()) {
            case CommandType.MtCommand:
                DisableItems_(new Control[] {
                    ptListView,
                    deviceListView,
                    psListView,
                });
                break;
            case CommandType.MemoryPointer:
                DisableItems_(new Control[] {
                    flagPanel,
                    i02ListView,
                    i68ListView,
                    cc4Text,
                    cc9Text,

                    deviceListView,
                    psListView,
                });
                break;
            case CommandType.DevicePointer:
                DisableItems_(new Control[] {
                    flagPanel,
                    i02ListView,
                    i68ListView,
                    cc4Text,
                    cc9Text,

                    psListView,
                });
                break;
            case CommandType.LoadCommand:
                DisableItems_(new Control[] {
                    flagPanel,
                    i02ListView,
                    i68ListView,
                    cc4Text,
                    cc9Text,

                    ptListView,
                    deviceListView,
                });
                break;
            case CommandType.LoadSmallCommand:
                DisableItems_(new Control[] {
                    flagPanel,
                    i02ListView,
                    i68ListView,
                    cc4Text,
                    cc7Text,
                    cc9Text,

                    ptListView,
                    deviceListView,
                });
                break;
            case CommandType.Offset:
                DisableItems_(new Control[] {
                    flagPanel,
                    cc3Text,
                    cc4Text,
                    cc5Text,
                    cc6Text,
                    cc7Text,
                    cc8Text,
                    cc9Text,
                    caListView,
                    i68ListView,
                    i02ListView,
                    i35ListView,
                    ptListView,
                    psListView,
                    deviceListView,
                });
                break;
            }
        }

        private void UpdateCommandHandler_()
        {
            isCommandSaved_ = false;

            UpdateLabels_();
            UpdateTexts_();
            UpdateLists_();
            UpdateCheckboxes_();

            UpdateItemsAvailability_();
        }

        ////////////////////
        //      FORM      //
        ////////////////////

        private void StackFormMove_()
        {
            stackForm_.Top = Top;
            stackForm_.Left = Right; //Left + ClientRectangle.Width;
            stackForm_.Height = Height / 2;
        }

        private void MemoryFormMove_()
        {
            memoryForm_.Top = Top + stackForm_.Height;
            memoryForm_.Left = Right; //Left + ClientRectangle.Width;
            memoryForm_.Height = Height / 2;
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

        private void MainFormClosing_(object sender, FormClosingEventArgs e)
        {
            if (!BeforeCloseProgram_()) {
                e.Cancel = true;
            }
        }
    }
}
