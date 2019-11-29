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
    public partial class MemoryForm : Form
    {
        MainForm mainForm_;
        bool moved_;

        public MemoryForm(MainForm mainForm)
        {
            InitializeComponent();

            mainForm_ = mainForm;
            moved_ = false;
        }

        private void MemoryFormClosing_(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void MemoryFormMove_(object sender, EventArgs e)
        {
            moved_ = true;
        }

        private void MemoryFormResizeEnd_(object sender, EventArgs e)
        {
            if (moved_) {
                mainForm_.OnMemoryFormMoved();
                moved_ = false;
            }
        }
    }
}
