using System;
using System.Text;
using System.Windows.Forms;

namespace mtemu
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        private void linkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel.Text);
        }

        private void CheckCode_()
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(codeText.Text);
            if (Convert.ToBase64String(plainTextBytes) == "cmFmIHBpZG9y") {
                TetrisForm tetrisForm_ = new TetrisForm();
                tetrisForm_.ShowDialog();
            }
            else {
                MessageBox.Show(
                    $"Неправильный код!",
                    "Ошибка!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1
                );
            }
        }

        private void CodeButtonClick_(object sender, EventArgs e)
        {
            CheckCode_();
        }

        private void CodeTextKeyDown_(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                CheckCode_();
                e.Handled = true;
            }
        }
    }
}
