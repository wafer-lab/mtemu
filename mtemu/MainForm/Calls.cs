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
        public void UpdateCallHandler()
        {
            isCallSaved_ = false;

            callForm_.addressText.Text = $"{currentCall_.GetAddress():X3}";
            callForm_.commentText.Text = currentCall_.GetComment();
        }

        private void LoadCall_(Call call)
        {
            currentCall_ = call;
            UpdateCallHandler();
            isCallSaved_ = true;
        }

        ////////////////////
        //   CALLS LIST   //
        ////////////////////

        private void SelectCall_(int index, Color selectedColor)
        {
            if (0 <= selectedCall_ && selectedCall_ < callForm_.callList.Items.Count) {
                callForm_.callList.Items[selectedCall_].BackColor = enabledColor_;
            }
            selectedCall_ = index;
            if (0 <= selectedCall_ && selectedCall_ < callForm_.callList.Items.Count) {
                callForm_.callList.Items[selectedCall_].BackColor = selectedColor;
            }
        }

        private void ChangeCall_(int newSelected, Color color)
        {
            if (newSelected < -1 || callForm_.callList.Items.Count <= newSelected) {
                return;
            }

            if (newSelected != selectedCall_) {
                if (!isCallSaved_) {
                    DialogResult saveRes = MessageBox.Show(
                        "Сохранить текущий вызов?",
                        "Сохранение",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1
                    );
                    if (saveRes == DialogResult.Cancel) {
                        return;
                    }
                    if (saveRes == DialogResult.Yes) {
                        SaveCall();
                    }

                    SelectCall_(newSelected, color);
                }
                SelectCall_(newSelected, color);

                if (newSelected == -1) {
                    LoadCall_(Call.GetDefault());

                    callForm_.removeButton.Enabled = false;
                    callForm_.saveButton.Enabled = false;
                    callForm_.upButton.Enabled = false;
                    callForm_.downButton.Enabled = false;
                }
                else {
                    LoadCall_(new Call(emulator_.GetCall(newSelected)));

                    callForm_.removeButton.Enabled = true;
                    callForm_.saveButton.Enabled = true;
                    if (newSelected == 0) {
                        callForm_.upButton.Enabled = false;
                    }
                    else {
                        callForm_.upButton.Enabled = true;
                    }
                    if (newSelected == callForm_.callList.Items.Count - 1) {
                        callForm_.downButton.Enabled = false;
                    }
                    else {
                        callForm_.downButton.Enabled = true;
                    }
                }
            }
        }


        private ListViewItem CallToItems(Call call)
        {
            return new ListViewItem(new string[] { "", $"0x{call.GetAddress():X3}", call.GetComment() });
        }

        public int GetCallIndex()
        {
            return selectedCall_;
        }

        public void ChangeCallByIndex(int index)
        {
            ChangeCall_(index, selectedColor_);
        }

        ////////////////////
        //    CONTROLS    //
        ////////////////////

        public void SetCallAddress(int address)
        {
            currentCall_.SetAddress(address);
        }

        public void SetCallComment(string comment)
        {
            currentCall_.SetComment(comment);
        }

        public void AddCall()
        {
            isProgramSaved_ = false;
            isCallSaved_ = true;

            emulator_.AddCall(new Call(currentCall_));
            callForm_.callList.Items.Add(CallToItems(emulator_.LastCall()));
            ChangeCall_(callForm_.callList.Items.Count - 1, selectedColor_);
        }

        public void SaveCall()
        {
            isProgramSaved_ = false;
            isCallSaved_ = true;

            int number = selectedCall_;
            if (number != -1) {
                emulator_.UpdateCall(number, new Call(currentCall_));
                for (int i = number; i < emulator_.CallsCount(); ++i) {
                    callForm_.callList.Items[i] = CallToItems(emulator_.GetCall(i));
                }
                SelectCall_(number, selectedColor_);
            }
        }

        public void RemoveCall()
        {
            if (0 <= selectedCall_ && selectedCall_ < callForm_.callList.Items.Count) {
                isProgramSaved_ = false;

                int number = selectedCall_;
                emulator_.RemoveCall(number);
                callForm_.callList.Items.RemoveAt(number);
                if (number >= callForm_.callList.Items.Count) {
                    number = callForm_.callList.Items.Count - 1;
                }
                ChangeCall_(number, selectedColor_);

                if (number != -1) {
                    for (int i = number; i < emulator_.CallsCount(); ++i) {
                        callForm_.callList.Items[i] = CallToItems(emulator_.GetCall(i));
                    }
                    SelectCall_(number, selectedColor_);
                }
            }
        }

        public void MoveUpCall()
        {
            int index = selectedCall_;
            if (index == 0) {
                return;
            }
            emulator_.MoveCallUp(index);
            ChangeCall_(index - 1, selectedColor_);

            for (int i = index - 1; i < emulator_.CallsCount(); ++i) {
                callForm_.callList.Items[i] = CallToItems(emulator_.GetCall(i));
            }
            SelectCall_(index - 1, selectedColor_);

            isProgramSaved_ = false;
        }

        public void MoveDownCall()
        {
            int index = selectedCall_;
            if (index == callForm_.callList.Items.Count - 1) {
                return;
            }
            emulator_.MoveCallDown(index);
            ChangeCall_(index + 1, selectedColor_);

            for (int i = index; i < emulator_.CallsCount(); ++i) {
                callForm_.callList.Items[i] = CallToItems(emulator_.GetCall(i));
            }
            SelectCall_(index + 1, selectedColor_);

            isProgramSaved_ = false;
        }

    }
}
