using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace mtemu
{
    partial class MainForm
    {

        ////////////////////
        //       LED      //
        ////////////////////

        private void SetLeds_(int value)
        {
            for (int i = 0; i < leds_.Length; ++i) {
                if (Helpers.IsBitSet(value, i)) {
                    if (ledClicked_[i]) {
                        leds_[i].Image = Properties.Resources.red_led_on;
                    }
                    else {
                        leds_[i].Image = Properties.Resources.green_led_on;
                    }
                }
                else {
                    if (ledClicked_[i]) {
                        leds_[i].Image = Properties.Resources.red_led_off;
                    }
                    else {
                        leds_[i].Image = Properties.Resources.green_led_off;
                    }
                }
            }
        }

        private void LedsAnimation_()
        {
            Action<object> toggleLed = (object obj) => {
                int number = (int) obj;
                for (int i = 0; i < leds_.Length; ++i) {
                    if (Helpers.IsBitSet(number, i)) {
                        ledClicked_[i] = true;
                    }
                    else {
                        ledClicked_[i] = false;
                    }
                }
                SetLeds_(15);
            };
            TimerCallback callback = new TimerCallback(toggleLed);

            int count = 8;
            for (int i = 0; i < count; ++i) {
                new System.Threading.Timer(callback, 8, i * 600, -1);
                new System.Threading.Timer(callback, 4, i * 600 + 100, -1);
                new System.Threading.Timer(callback, 2, i * 600 + 200, -1);
                new System.Threading.Timer(callback, 1, i * 600 + 300, -1);
                new System.Threading.Timer(callback, 2, i * 600 + 400, -1);
                new System.Threading.Timer(callback, 4, i * 600 + 500, -1);
            }
            new System.Threading.Timer(callback, 8, count * 600, -1);
            // Double disable because bugs with slow computers
            new System.Threading.Timer(callback, 0, count * 600 + 100, -1);
            new System.Threading.Timer(callback, 0, (count + 1) * 600, -1);
        }

        private void UpdateEggsCounter_()
        {
            helpForm_.leftLabel.Text = $"{EasterEgg.FoundEggsCount()}/{EasterEgg.EggsCount()}";
            if (EasterEgg.EggsCount() - EasterEgg.FoundEggsCount() > 0 && !EasterEgg.IsNotified()) {
                EasterEgg.SetNotified();
                MessageBox.Show(
                    "Воу, ты нашел все пасхалки!\nНапиши в телеграме!\nCсылка будет в разделе помощи!",
                    "Поздравляю!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1
                );
                var linkBytes = Convert.FromBase64String("aHR0cHM6Ly90Lm1lL2pvaW5jaGF0L0F4VWdOaFJWSVBBQnJmTzY1em5HVXc=");
                helpForm_.linkLabel.Text = System.Text.Encoding.UTF8.GetString(linkBytes);
                helpForm_.linkLabel.Visible = true;
                helpForm_.codeLabel.Visible = true;
                helpForm_.codeText.Visible = true;
                helpForm_.codeButton.Visible = true;
                LedsAnimation_();
            }
        }

        ////////////////////
        //  EMULATOR OUT  //
        ////////////////////

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

        private void SetOut_(TextBox textBox, int value, bool asNew, int size = 4)
        {
            SetOut_(textBox, Helpers.IntToBinary(value, size), asNew);
        }

        private void UpdateCommmandList()
        {
            if (emulator_.CommandsCount() > 0) {
                ChangeCommand_(emulator_.GetNextIndex(), selectedColor_);
                SelectPrevCommand_(emulator_.GetPrevIndex());
                ChangeCall_(emulator_.GetCallIndex(), selectedColor_);
            }
        } 

        private void UpdateOutPanel(bool asNew = false)
        {

            SetFlag_(ovrText, "OVR=", emulator_.GetOvr());
            SetFlag_(c4Text, "C4=", emulator_.GetC4());
            SetFlag_(f3Text, "F3=", emulator_.GetF3());
            SetFlag_(zText, "Z=", emulator_.GetZ());
            SetFlag_(gText, "/G=", emulator_.GetG());
            SetFlag_(pText, "/P=", emulator_.GetP());

            SetOut_(fText, emulator_.GetF(), asNew);
            SetOut_(yText, emulator_.GetY(), asNew);
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
        }

        private void UpdateStack_(bool asNew = false)
        {
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
                        stackForm_.stackListView.EnsureVisible(i);
                    }
                }
            }
        }

        private void UpdateMemory_(bool asNew = false)
        {
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

        private void UpdateScheme_(bool asNew = false)
        {
            SetFlag_(schemeForm_.ovrText, "", emulator_.GetOvr());
            SetFlag_(schemeForm_.c4Text, "", emulator_.GetC4());
            SetFlag_(schemeForm_.f3Text, "", emulator_.GetF3());
            SetFlag_(schemeForm_.zText, "", emulator_.GetZ());
            SetFlag_(schemeForm_.gText, "", emulator_.GetG());
            SetFlag_(schemeForm_.pText, "", emulator_.GetP());

            SetOut_(schemeForm_.regQText, emulator_.GetPrevRegQ(), asNew);
            SetOut_(schemeForm_.regAText, emulator_.GetPrevRegA(), asNew);
            SetOut_(schemeForm_.regBText, emulator_.GetPrevRegB(), asNew);

            SetOut_(schemeForm_.rText, emulator_.GetR(), asNew);
            SetOut_(schemeForm_.sText, emulator_.GetS(), asNew);

            SetOut_(schemeForm_.fText, emulator_.GetF(), asNew);
            SetOut_(schemeForm_.yText, emulator_.GetY(), asNew);
            SetLeds_(emulator_.GetF());

            Command command = emulator_.ExecutedCommand();

            SetOut_(schemeForm_.aText, command.GetRawValue(WordType.A), asNew);
            SetOut_(schemeForm_.bText, command.GetRawValue(WordType.B), asNew);
            SetOut_(schemeForm_.dText, command.GetRawValue(WordType.D), asNew);

            SetFlag_(schemeForm_.c0Text, "", command.GetC0());

            SetOut_(schemeForm_.i02Text, command.GetI02(), asNew, 3);
            SetOut_(schemeForm_.i35Text, command.GetI35(), asNew, 3);
            SetOut_(schemeForm_.i68Text, command.GetI68(), asNew, 3);
        }

        private void UpdateOutput_(bool asNew = false)
        {
            UpdateCommmandList();
            UpdateOutPanel(asNew);
            UpdateStack_(asNew);
            UpdateMemory_(asNew);
            UpdateScheme_(asNew);
        }

        ////////////////////
        //     BUTTONS    //
        ////////////////////

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
            case Emulator.ResultCode.End:
                MessageBox.Show(
                    "Вы дошли до конца программы!",
                    "Это всё...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1
                );
                break;
            }

            UpdateOutput_();
        }

        public void ResetEmulator()
        {
            emulator_.Reset();
            UpdateOutput_();
        }

        public void ExecOneEmulator()
        {
            ResultCodeHandler_(emulator_.ExecOne());
        }

        public void ExecOneCallEmulator()
        {
            ResultCodeHandler_(emulator_.ExecOneCall());
        }

        public void ExecAllEmulator()
        {
            ResultCodeHandler_(emulator_.ExecAll());
        }

        private void ResetButtonClick_(object sender, EventArgs e)
        {
            ResetEmulator();
        }

        private void StepButtonClick_(object sender, EventArgs e)
        {
            ExecOneEmulator();
        }

        public void ExecOneCall()
        {
            ExecOneCallEmulator();
        }

        private void AutoButtonClick_(object sender, EventArgs e)
        {
            ExecAllEmulator();
        }
    }
}
