using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace mtemu
{
    partial class MainForm
    {
        ////////////////////
        //   LIST VIEWS   //
        ////////////////////

        private void UpdateLists_()
        {
            foreach (KeyValuePair<WordType, ListView> listView in listViewes_) {
                ListViewItemCollection items = listView.Value.Items;
                for (int i = 0; i < items.Count; ++i) {
                    if (i == currentCommand_.GetSelIndex(listView.Key)) {
                        if (items[i].BackColor != selectedColor_) {
                            items[i].BackColor = selectedColor_;
                        }
                    }
                    else {
                        if (items[i].BackColor == selectedColor_) {
                            items[i].BackColor = enabledColor_;
                        }
                    }
                }
            }
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
                currentCommand_.SetValue(type, listView.SelectedIndices[0]);
                textBox.Text = Helpers.IntToBinary(currentCommand_[textIndex], 4);
                textBox.BackColor = selectedColor_;
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

        ////////////////////
        //   CHECKBOXES   //
        ////////////////////

        private void UpdateCheckboxes_()
        {
            m0CheckBox.Checked = currentCommand_.GetFlag(FlagType.M0);
            m1CheckBox.Checked = currentCommand_.GetFlag(FlagType.M1);
            offsetCheckBox.Checked = currentCommand_.isOffset;
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
                textBox.BackColor = selectedColor_;
            }
        }

        private void M0CheckBoxCheckedChanged_(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox) sender;
            if (checkBox.Focused) {
                DefaultCheckBoxChanged_(FlagType.M0, checkBox.Checked);
            }
        }

        private void M1CheckBoxCheckedChanged_(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox) sender;
            if (checkBox.Focused) {
                DefaultCheckBoxChanged_(FlagType.M1, checkBox.Checked);
            }
        }

        private void OffsetCheckBoxCheckedChanged_(object sender, EventArgs e)
        {
            currentCommand_.isOffset = offsetCheckBox.Checked;
            UpdateCommandHandler_();
        }
    }
}
