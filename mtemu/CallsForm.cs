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
    public partial class CallsForm : Form
    {
        MainForm mainForm_;
        bool moved_;

        public CallsForm(MainForm mainForm)
        {
            InitializeComponent();

            mainForm_ = mainForm;
            moved_ = false;

            Console.WriteLine(ClientRectangle.Height);
            Console.WriteLine(ClientRectangle.Width);
        }

        private void ProgramFormClosing_(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void RemoveButtonClick_(object sender, EventArgs e)
        {
            mainForm_.RemoveCall();
        }

        private void DownButtonClick_(object sender, EventArgs e)
        {
            mainForm_.MoveDownCall();
        }

        private void UpButtonClick_(object sender, EventArgs e)
        {
            mainForm_.MoveUpCall();
        }

        private void AddButtonClick_(object sender, EventArgs e)
        {
            mainForm_.AddCall();
        }

        private void SaveButtonClick_(object sender, EventArgs e)
        {
            mainForm_.SaveCall();
        }

        private bool DefaultKeyDown_(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Up) {
                mainForm_.ChangeCallByIndex(mainForm_.GetCallIndex() - 1);
                return true;
            }
            else if (e.Control && e.KeyCode == Keys.Down) {
                mainForm_.ChangeCallByIndex(mainForm_.GetCallIndex() + 1);
                return true;
            }
            else if (e.Control && e.KeyCode == Keys.Delete) {
                mainForm_.RemoveCall();
                return true;
            }
            else if (e.KeyCode == Keys.Enter) {
                if (e.Control) {
                    mainForm_.AddCall();
                }
                else {
                    mainForm_.SaveCall();
                }
                return true;
            }
            if (e.Control && e.KeyCode == Keys.R) {
                mainForm_.ResetEmulator();
                return true;
            }
            if (e.Control && e.KeyCode == Keys.T) {
                mainForm_.ExecOneEmulator();
                return true;
            }
            if (e.Control && e.KeyCode == Keys.Y) {
                mainForm_.ExecOneCallEmulator();
                return true;
            }
            if (e.Control && e.KeyCode == Keys.U) {
                mainForm_.ExecAllEmulator();
                return true;
            }
            return false;
        }

        private void AddressTextChanged_(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox) sender;

            // Clear from wrong chars
            int selPos = textBox.SelectionStart;
            int selLen = textBox.SelectionLength;
            textBox.Text = Helpers.ClearHex(textBox.Text, ref selPos);
            textBox.SelectionStart = selPos;
            textBox.SelectionLength = selLen;

            // Save value
            if (textBox.Text.Length == 3) {
                mainForm_.SetCallAddress(Helpers.HexToInt(textBox.Text));
                mainForm_.UpdateCallHandler();
            }
        }

        private void AddressTextKeyDown_(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox) sender;

            int selPos = textBox.SelectionStart;
            int selLen = textBox.SelectionLength;
            int value = Helpers.HexToInt(textBox.Text);

            if (DefaultKeyDown_(e)) {
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up) {
                if (value > 0) {
                    textBox.Text = $"{value - 1:X3}";
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down) {
                if (value < (1 << Call.ADDRESS_SIZE_BIT) - 1) {
                    textBox.Text = $"{value + 1:X3}";
                }
                e.Handled = true;
            }

            textBox.SelectionStart = selPos;
            textBox.SelectionLength = selLen;
        }

        private void CommentTextChanged_(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            mainForm_.SetCallComment(textBox.Text);
            mainForm_.UpdateCallHandler();
        }

        private void CallListSelectedIndexChanged_(object sender, EventArgs e)
        {
            if (callList.SelectedIndices.Count != 0) {
                mainForm_.ChangeCallByIndex(callList.SelectedIndices[0]);
            }
        }

        void ListViewColumnWidthChanging_(object list, ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = ((ListView) list).Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        private void DefaultKeyDown_(object sender, KeyEventArgs e)
        {
            if (DefaultKeyDown_(e)) {
                e.Handled = true;
            }
        }

        private void StepButtonClick_(object sender, EventArgs e)
        {
            mainForm_.ExecOneCall();
        }

        private void CallsFormMove_(object sender, EventArgs e)
        {
            moved_ = true;
        }

        private void CallsFormResizeEnd_(object sender, EventArgs e)
        {
            if (moved_) {
                mainForm_.OnCallsFormMoved();
                moved_ = false;
            }
        }
    }
}
