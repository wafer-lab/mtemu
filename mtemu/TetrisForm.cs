using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace mtemu
{
    public partial class TetrisForm : Form
    {
        private int score_ = 0;
        private int tick_ = 0;
        private const int width_ = 15, height_ = 25, k_ = 20;
        private int[,] shape_ = new int[2, 4];
        private int[,] field_ = new int[width_, height_];
        private Bitmap bitfield_ = new Bitmap(k_ * (width_ + 1) + 1, k_ * (height_ + 3) + 1);

        public TetrisForm()
        {
            InitializeComponent();
            SetShape_();
        }

        private void TetrisFormShown_(object sender, EventArgs e)
        {
            TickTimer.Enabled = true;
        }

        private void TetrisFormClosed_(object sender, FormClosedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            TickTimer.Enabled = false;
            string end = "очков";
            if (score_ % 10 == 1 && score_ != 11) {
                end = "очко";
            }
            else if (2 <= score_ % 10 && score_ % 10 <= 4) {
                end = "очка";
            }
            MessageBox.Show(
                $"Вы набрали {score_} {end}!",
                "Конец игры!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
            );
        }

        private void SetScore_(int value)
        {
            score_ = value;
            scoreLabel.Text = score_.ToString();
        }

        private void IncTickScore_()
        {
            ++tick_;
            if (tick_ % 3 == 0) {
                SetScore_(score_ + 1);
                tick_ = 0;
            }
        }

        private void IncRowScore_()
        {
            SetScore_(score_ + width_ * 100);
        }

        private void FillField_()
        {
            var gr = Graphics.FromImage(bitfield_);
            gr.Clear(Color.Black);
            gr.DrawRectangle(Pens.Red, k_, k_, (width_ - 1) * k_, (height_ - 1) * k_);
            for (int i = 0; i < width_; i++) {
                for (int j = 0; j < height_; j++) {
                    gr.FillRectangle(Brushes.Green, i * k_, j * k_, (k_ - 1) * field_[i, j], (k_ - 1) * field_[i, j]);
                }
            }
            for (int i = 0; i < 4; i++) {
                gr.FillRectangle(Brushes.Red, shape_[1, i] * k_, shape_[0, i] * k_, k_ - 1, k_ - 1);
            }
            FieldPictureBox.Image = bitfield_;
        }

        private void TickTimerTick_(object sender, EventArgs e)
        {
            IncTickScore_();

            if (field_[8, 4] == 1) {
                Close();
                return;
            }
            foreach (int i in (
                from i in Enumerable.Range(0, field_.GetLength(1))
                where (Enumerable.Range(0, field_.GetLength(0)).Select(j => field_[j, i]).Sum() >= width_ - 1)
                select i).ToArray().Take(1)
            ) {
                IncRowScore_();

                for (int k = i; k > 1; k--) {
                    for (int l = 1; l < width_; l++) {
                        field_[l, k] = field_[l, k - 1];
                    }
                }
            }
            Move(0, 1);
        }

        private void FormKeyDown_(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
            case Keys.A:
            case Keys.Left:
                Move(-1, 0);
                break;
            case Keys.D:
            case Keys.Right:
                Move(1, 0);
                break;
            case Keys.W:
            case Keys.Up:
                var shapeT = new int[2, 4];
                Array.Copy(shape_, shapeT, shape_.Length);
                int maxx = Enumerable.Range(0, 4).Select(j => shape_[1, j]).ToArray().Max();
                int maxy = Enumerable.Range(0, 4).Select(j => shape_[0, j]).ToArray().Max();
                for (int i = 0; i < 4; i++) {
                    int temp = shape_[0, i];
                    shape_[0, i] = maxy - (maxx - shape_[1, i]) - 1;
                    shape_[1, i] = maxx - (3 - (maxy - temp)) + 1;
                }
                if (FindMistake_())
                    Array.Copy(shapeT, shape_, shape_.Length);
                break;
            case Keys.S:
            case Keys.Down:
                TickTimer.Interval = 50;
                break;
            }
        }

        private void SetShape_()
        {
            Random x = new Random(DateTime.Now.Millisecond);
            switch (x.Next(7)) {
            case 0:
                shape_ = new int[,] { { 2, 3, 4, 5 }, { 8, 8, 8, 8 } };
                break;
            case 1:
                shape_ = new int[,] { { 2, 3, 2, 3 }, { 8, 8, 9, 9 } };
                break;
            case 2:
                shape_ = new int[,] { { 2, 3, 4, 4 }, { 8, 8, 8, 9 } };
                break;
            case 3:
                shape_ = new int[,] { { 2, 3, 4, 4 }, { 8, 8, 8, 7 } };
                break;
            case 4:
                shape_ = new int[,] { { 3, 3, 4, 4 }, { 7, 8, 8, 9 } };
                break;
            case 5:
                shape_ = new int[,] { { 3, 3, 4, 4 }, { 9, 8, 8, 7 } };
                break;
            case 6:
                shape_ = new int[,] { { 3, 4, 4, 4 }, { 8, 7, 8, 9 } };
                break;
            }
        }
        private void FormKeyUp_(object sender, KeyEventArgs e) => TickTimer.Interval = 250;

        private new void Move(int x, int y, bool cancel = false)
        {
            for (int i = 0; i < 4; i++) {
                shape_[1, i] += x;
                shape_[0, i] += y;
            }
            if (!cancel && FindMistake_()) {
                Move(-x, -y, true);
                if (y != 0) {
                    for (int i = 0; i < 4; i++) {
                        field_[shape_[1, i], shape_[0, i]]++;
                    }
                    SetShape_();
                }
            }
            FillField_();
        }

        private bool FindMistake_()
        {
            for (int i = 0; i < 4; i++) {
                if (shape_[1, i] >= width_ || shape_[0, i] >= height_ || shape_[1, i] <= 0 || shape_[0, i] <= 0 || field_[shape_[1, i], shape_[0, i]] == 1) {
                    return true;
                }
            }
            return false;
        }
    }
}
