using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mtemu
{
    public partial class MainForm : Form
    {
        private static string debugMenuPrefix = "Отладка";
        private static string infoMenuPrefix = "Справка";

        private static Color enabledColor_ = SystemColors.Window;
        private static Color enabledTextColor_ = SystemColors.WindowText;
        private static Color disabledColor_ = SystemColors.Control;
        private static Color disabledTextColor_ = SystemColors.GrayText;
        private static Color changedColor_ = Color.Wheat;
        private static Color selectedColor_ = Color.FromArgb(0, 190, 200, 234);
        private static Color nextSelectedColor_ = Color.FromArgb(0, 234, 234, 234);

        private static int moveDelta_ = 30;

        private int easterEggMask_;

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
        private int selectedCall_;

        private bool isCallSaved_;
        private bool isCommandSaved_;
        private bool isProgramSaved_;

        private Emulator emulator_;
        private Command currentCommand_;
        private Call currentCall_;
        private PortExtender portExtender_;

        private Label[] textLabels_;
        private TextBox[] textBoxes_;
        private Label[] regLabels_;
        private TextBox[] regTexts_;
        private Dictionary<WordType, ListView> listViewes_;
        private PictureBox[] leds_;
        private bool[] ledClicked_;

        bool stickCallsForm_;
        CallsForm callsForm_;

        bool stickMemoryForm_;
        MemoryForm memoryForm_;

        bool stickStackForm_;
        StackForm stackForm_;

        SchemeForm schemeForm_;
        ExtenderSettingsForm extenderSettingsForm_;
        HelpForm helpForm_;

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
                { WordType.DEVICE, deviceListView },
            };
            foreach (KeyValuePair<WordType, ListView> listView in listViewes_) {
                listView.Value.Items.Clear();
                string[][] lists = Command.GetItems(listView.Key);
                foreach (string[] list in lists) {
                    listView.Value.Items.Add(new ListViewItem(list));
                }
            }

            // Leds for fun
            easterEggMask_ = 0;
            leds_ = new PictureBox[] {
                led0,
                led1,
                led2,
                led3,
            };
            ledClicked_ = new bool[] { false, false, false, false };

            // Memory debug form
            stickMemoryForm_ = true;
            memoryForm_ = new MemoryForm(this);
            for (int i = 0; i < Emulator.GetMemorySize(); ++i) {
                memoryForm_.memoryListView.Items.Add(new ListViewItem(new string[] { "", $"0x{i:X2}", "0000 0000", "0x00" }));
            }

            // Stack debug form
            stickStackForm_ = true;
            stackForm_ = new StackForm(this);
            for (int i = 0; i < Emulator.GetStackSize(); ++i) {
                stackForm_.stackListView.Items.Add(new ListViewItem(new string[] { "", $"0x{i:X}", "0x000" }));
            }

            // Form with program editing
            stickCallsForm_ = false;
            callsForm_ = new CallsForm(this);

            // Form with scheme of ALU
            schemeForm_ = new SchemeForm();

            // Form with extenser device settings
            extenderSettingsForm_ = new ExtenderSettingsForm();

            portExtender_ = new PortExtender(DeviceRemovedHandler);

            // Form with help
            helpForm_ = new HelpForm();
            UpdateEggsCounter_();

            // Reset to initial values
            Reset_();
        }

        private bool Reset_(string filename = null, byte[] input = null)
        {
            LoadCommand_(Command.GetDefault());
            LoadCall_(Call.GetDefault());

            filename_ = filename;
            selected_ = -1;
            nextSelected_ = -1;
            selectedCall_ = -1;
            isCallSaved_ = true;
            isCommandSaved_ = true;
            isProgramSaved_ = true;

            saveButton.Enabled = false;
            removeButton.Enabled = false;
            upButton.Enabled = false;
            downButton.Enabled = false;

            emulator_ = new Emulator(portExtender_);
            commandList.Items.Clear();
            callsForm_.callList.Items.Clear();
            if (filename != null || input != null) {
                if (filename_ != null) {
                    if (!emulator_.OpenFile(filename)) {
                        return false;
                    }
                }
                else if (input != null) {
                    if (!emulator_.OpenRaw(input)) {
                        return false;
                    }
                    filename_ = null;
                }
                for (int i = 0; i < emulator_.CommandsCount(); ++i) {
                    commandList.Items.Add(CommandToItems(emulator_.GetCommand(i)));
                }
                for (int i = 0; i < emulator_.CallsCount(); ++i) {
                    callsForm_.callList.Items.Add(CallToItems(emulator_.GetCall(i)));
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
            switch (currentCommand_.GetCommandView()) {
            case ViewType.MT_COMMAND:
                DisableItems_(new Control[] {
                    ptListView,
                    deviceListView,
                    psListView,
                });
                break;
            case ViewType.MEMORY_POINTER:
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
            case ViewType.DEVICE_POINTER:
                DisableItems_(new Control[] {
                    flagPanel,
                    i02ListView,
                    i68ListView,
                    cc4Text,
                    cc9Text,

                    psListView,
                });
                break;
            case ViewType.LOAD_8BIT:
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
            case ViewType.LOAD_HIGH_4BIT:
                DisableItems_(new Control[] {
                    flagPanel,
                    i02ListView,
                    i68ListView,
                    cc4Text,
                    cc8Text,
                    cc9Text,

                    ptListView,
                    deviceListView,
                });
                break;
            case ViewType.LOAD_LOW_4BIT:
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
            case ViewType.OFFSET:
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
            if (stickStackForm_) {
                stackForm_.Top = Top;
                stackForm_.Left = Right; // Left + ClientRectangle.Width;
                stackForm_.Height = Height / 2;
            }
        }

        private void MemoryFormMove_()
        {
            if (stickMemoryForm_) {
                memoryForm_.Top = Top + stackForm_.Height;
                memoryForm_.Left = Right; // Left + ClientRectangle.Width;
                memoryForm_.Height = Height / 2;
            }
        }

        private void CallsFormMove_()
        {
            if (stickCallsForm_) {
                callsForm_.Top = Top;
                callsForm_.Left = Left - callsForm_.Width;
            }
        }

        private void SchemeFormMove_()
        {
            // TODO: Add a form position if necessary
        }

        public void OnStackFormMoved()
        {
            if (
                Top - moveDelta_ <= stackForm_.Top
                && stackForm_.Top <= Top + moveDelta_
                && Right - moveDelta_ <= stackForm_.Left 
                && stackForm_.Left <= Right + moveDelta_
            ) {
                stickStackForm_ = true;
            }
            else {
                stickStackForm_ = false;
            }

            MoveSubForms_();
        }

        public void OnMemoryFormMoved()
        {
            if (
                Top - moveDelta_ <= memoryForm_.Top - memoryForm_.Height
                && memoryForm_.Top - memoryForm_.Height <= Top + moveDelta_
                && Right - moveDelta_ <= memoryForm_.Left
                && memoryForm_.Left <= Right + moveDelta_
            ) {
                stickMemoryForm_ = true;
            }
            else {
                stickMemoryForm_ = false;
            }

            MoveSubForms_();
        }

        public void OnCallsFormMoved()
        {
            if (
                Top - moveDelta_ <= callsForm_.Top
                && callsForm_.Top <= Top + moveDelta_
                && Left - moveDelta_ <= callsForm_.Right
                && callsForm_.Right <= Left + moveDelta_
            ) {
                stickCallsForm_ = true;
            }
            else {
                stickCallsForm_ = false;
            }

            MoveSubForms_();
        }

        private void MoveSubForms_()
        {
            StackFormMove_();
            MemoryFormMove_();
            CallsFormMove_();
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

            portExtender_.CloseDevice();
        }

        private bool DefaultKeyDown_(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Up) {
                ChangeCommand_(selected_ - 1, selectedColor_);
                return true;
            }
            if (e.Control && e.KeyCode == Keys.Down) {
                ChangeCommand_(selected_ + 1, selectedColor_);
                return true;
            }
            if (e.Control && e.KeyCode == Keys.Left) {
                commandRadioButton.Checked = true;
                return true;
            }
            if (e.Control && e.KeyCode == Keys.Right) {
                offsetRadioButton.Checked = true;
                return true;
            }
            if (e.Control && e.KeyCode == Keys.Delete) {
                RemoveCommand_();
                return true;
            }
            if (e.KeyCode == Keys.Enter) {
                if (e.Control) {
                    AddCommand_();
                }
                else {
                    SaveCommand_();
                }
                return true;
            }
            if (e.Control && e.KeyCode == Keys.R) {
                ResetEmulator();
                return true;
            }
            if (e.Control && e.KeyCode == Keys.T) {
                ExecOneEmulator();
                return true;
            }
            if (e.Control && e.KeyCode == Keys.Y) {
                ExecOneCallEmulator();
                return true;
            }
            if (e.Control && e.KeyCode == Keys.U) {
                ExecAllEmulator();
                return true;
            }
            return false;
        }

        private void DefaultKeyDown_(object sender, KeyEventArgs e)
        {
            if (DefaultKeyDown_(e)) {
                e.Handled = true;
            }
        }

        private void DisableLeds_(object obj)
        {
            int number = (int) obj;
            ledClicked_[number] = false;
            SetLeds_(emulator_.GetF());
        }

        private void LedClick_(int number)
        {
            if (!ledClicked_[number]) {
                // Enable led
                ledClicked_[number] = true;
                SetLeds_(emulator_.GetF());

                // Disable led after 60 ms
                TimerCallback callback = new TimerCallback(DisableLeds_);
                System.Threading.Timer timer = new System.Threading.Timer(callback, number, 60,-1);

                // Remembre click
                easterEggMask_ <<= 2;
                easterEggMask_ += number;
                easterEggMask_ &= (1 << 8) - 1;
            }

            // Check last 4 clicks
            byte[] program = EasterEgg.GetData(easterEggMask_);
            if (program != null) {
                if (BeforeCloseProgram_()) {
                    Reset_(null, program);
                    UpdateEggsCounter_();
                    easterEggMask_ = 0;
                }
            }
        }

        private void Led3Click_(object sender, EventArgs e)
        {
            LedClick_(3);
        }

        private void Led2Click_(object sender, EventArgs e)
        {
            LedClick_(2);
        }

        private void Led1Click_(object sender, EventArgs e)
        {
            LedClick_(1);
        }

        private void Led0Click_(object sender, EventArgs e)
        {
            LedClick_(0);
        }
    }
}
