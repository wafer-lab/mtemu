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
    public partial class StackForm : Form
    {
        MainForm mainForm_;
        bool moved_;

        public StackForm(MainForm mainForm)
        {
            InitializeComponent();

            mainForm_ = mainForm;
            moved_ = false;
        }

        private void StackFormClosing_(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void StackFormMove_(object sender, EventArgs e)
        {
            moved_ = true;
        }

        private void StackFormResizeEnd_(object sender, EventArgs e)
        {
            if (moved_) {
                mainForm_.OnStackFormMoved();
                moved_ = false;
            }
        }
    }
}
