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
    public partial class ExtenderSettingsForm : Form
    {
        public ExtenderSettingsForm()
        {
            InitializeComponent();
        }

        private void ResetDevicesList()
        {
            devicesComboBox.Items.Clear();
            devicesComboBox.Items.Add(" ------------- Не выбрано ------------- ");
        }

        private PortExtender.DeviceInfo selectedDeviceInfo_ = new PortExtender.DeviceInfo();
        private PortExtender.DeviceInfo[] devicesInfo_ = null;

        public PortExtender.DeviceInfo GetSelectedDeviceInfo()
        {
            if (devicesInfo_ == null)
                return new PortExtender.DeviceInfo();

            return selectedDeviceInfo_;
        }

        private void selectDeviceButton_Click(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex > 0)
                selectedDeviceInfo_ = devicesInfo_[devicesComboBox.SelectedIndex - 1];
            else
                selectedDeviceInfo_ = new PortExtender.DeviceInfo();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ExtenderSettingsForm_Load(object sender, EventArgs e)
        {
            ResetDevicesList();

            devicesInfo_ = PortExtender.GetAvailableDevices();
            if (devicesInfo_ != null)
            {
                var idx = 0;
                foreach (var devInfo in devicesInfo_)
                {
                    var tmp = devicesComboBox.Items.Add("MTPE-" + devInfo.serial + " (" + devInfo.com_id + ")");
                    if (devInfo.com_id == selectedDeviceInfo_.com_id)
                        idx = tmp;
                }

                devicesComboBox.SelectedIndex = idx;
            }
            else
                devicesComboBox.SelectedIndex = 0;
        }
    }
}
