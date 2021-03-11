using System.Windows.Forms;

namespace mtemu
{
    public partial class SchemeForm : Form
    {
        public SchemeForm()
        {
            InitializeComponent();
        }

        private void SchemeFormClosing_(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) {
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}
