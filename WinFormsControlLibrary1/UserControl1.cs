using System.Drawing;

namespace WinFormsControlLibrary1
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1() { InitializeComponent(); }
        public int mode = -1;
        public (string, long)[] typesVolume = new (string, long)[0];
        public (string, int)[] typesQuantity = new (string, int)[0];

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e) { }

        private void UserControl1_Load(object sender, EventArgs e) { }

        private void UserControl1_Paint(object sender, PaintEventArgs e)
        {
            int typesToDisplay = typesVolume.Length;
            SolidBrush[] colorBrush = new SolidBrush[typesToDisplay];
            for (int i = 0; i < typesToDisplay; ++i) colorBrush[i] = new SolidBrush(ColorFromHSV(i * (255.0 / typesToDisplay), 1, 1));
            Color black = Color.FromArgb(255, 0, 0, 0);
            Pen blackPen = new Pen(black);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            SolidBrush drawBrush2 = new SolidBrush(Color.LightGray);
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            blackPen.Width = 1;
            Font font = new Font("Segoe UI", 5);

            switch (mode)
            {
                case 0:
                    // https://www.codeproject.com/Questions/70200/Draw-a-pie-chart-in-C-net-win-forms?fbclid=IwAR2T0j5R_JWhdzwozDQ8fidSRw3u5N2LEr8MSRSojkci1IOyw4i4O1X_n4I
                    long totalVolume = 0;
                    int totalQuantity = 0;
                    foreach (var elem in typesVolume) totalVolume += elem.Item2;
                    foreach (var elem in typesQuantity) totalQuantity += elem.Item2;
                    float[,] angles = new float[typesToDisplay, 2];
                    float offset;
                    for (int i = 0; i < typesToDisplay; ++i)
                    {
                        offset = 0;
                        angles[i, 0] = (float)typesQuantity[i].Item2 / totalQuantity * 360;
                        for (int j = 0; j < i; ++j) offset += angles[j, 0];
                        e.Graphics.FillPie(colorBrush[i], 5, 5, 150, 150, offset, angles[i, 0]);
                        e.Graphics.DrawPie(blackPen, 5, 5, 150, 150, offset, angles[i, 0]);
                        e.Graphics.FillRectangle(colorBrush[i], 160, 5 + i * 20, 30, 15);
                        e.Graphics.DrawRectangle(blackPen, 160, 5 + i * 20, 30, 15);
                        e.Graphics.DrawString(typesQuantity[i].Item1 + " - " + typesQuantity[i].Item2.ToString(), font, drawBrush, 230, 5 + i * 20, drawFormat);

                        offset = 0;
                        angles[i, 1] = (float)typesVolume[i].Item2 / totalVolume * 360;
                        for (int j = 0; j < i; ++j) offset += angles[j, 1];
                        e.Graphics.FillPie(colorBrush[i], 255, 5, 150, 150, offset, angles[i, 1]);
                        e.Graphics.DrawPie(blackPen, 255, 5, 150, 150, offset, angles[i, 1]);
                        e.Graphics.FillRectangle(colorBrush[i], 410, 5 + i * 20, 30, 15);
                        e.Graphics.DrawRectangle(blackPen, 410, 5 + i * 20, 30, 15);
                        e.Graphics.DrawString(typesVolume[i].Item1 + " - " + typesVolume[i].Item2.ToString(), font, drawBrush, 500, 5 + i * 20, drawFormat);
                    }
                    break;
                case 1:
                    int maxQuantity = typesQuantity.MaxBy(x => x.Item2).Item2;
                    long maxVolume = typesVolume.MaxBy(x => x.Item2).Item2;
                    float barQuantity = (float)(Math.Pow(10, Math.Floor(Math.Log10(maxQuantity))) * Math.Ceiling(maxQuantity / Math.Pow(10, Math.Floor(Math.Log10(maxQuantity)))));
                    float barVolume = (float)(Math.Pow(10, Math.Floor(Math.Log10(maxVolume))) * Math.Ceiling(maxVolume / Math.Pow(10, Math.Floor(Math.Log10(maxVolume)))));
                    float height;

                    e.Graphics.FillRectangle(drawBrush2, 50, 5, 200, 250);
                    e.Graphics.FillRectangle(drawBrush2, 300, 5, 200, 250);
                    for (int i = 0; i < 11; ++i)
                    {
                        e.Graphics.DrawLine(blackPen, 50, 25 + 23 * i, 250, 25 + 23 * i);
                        e.Graphics.DrawString(Math.Round(barQuantity * (1 - 0.1 * i)).ToString(), font, drawBrush, 40, 25 + 23 * i, drawFormat);
                        e.Graphics.DrawLine(blackPen, 300, 25 + 23 * i, 500, 25 + 23 * i);
                        e.Graphics.DrawString(Math.Round(barVolume * (1 - 0.1 * i)).ToString(), font, drawBrush, 295, 25 + 23 * i, drawFormat);

                    }
                    for (int i = 0; i < typesToDisplay; ++i)
                    {
                        height = ((float)typesQuantity[i].Item2 / barQuantity) * 230;
                        e.Graphics.DrawString(typesQuantity[i].Item1, font, drawBrush, 70 + 20 * i, 260, drawFormat);
                        e.Graphics.FillRectangle(colorBrush[i], 54 + i * 20, 255 - height, 12, height);
                        e.Graphics.DrawRectangle(blackPen, 54 + i * 20, 255 - height, 12, height);

                        height = ((float)typesVolume[i].Item2 / barVolume) * 230;
                        e.Graphics.DrawString(typesVolume[i].Item1, font, drawBrush, 320 + 20 * i, 260, drawFormat);
                        e.Graphics.FillRectangle(colorBrush[i], 304 + i * 20, 255 - height, 12, height);
                        e.Graphics.DrawRectangle(blackPen, 304 + i * 20, 255 - height, 12, height);
                    }
                    break;
                case 2:
                    int maxQuantity0 = typesQuantity.MaxBy(x => x.Item2).Item2;
                    long maxVolume0 = typesVolume.MaxBy(x => x.Item2).Item2;
                    int volumeBarCount = (int)Math.Floor(Math.Log10(maxVolume0)) + 2;
                    int quantityBarCount = (int)Math.Floor(Math.Log10(maxQuantity0)) + 2;
                    float barQuantity0 = (float)(Math.Pow(10, Math.Floor(Math.Log10(maxQuantity0))) * Math.Ceiling(maxQuantity0 / Math.Pow(10, Math.Floor(Math.Log10(maxQuantity0)))));
                    long barVolume0 = (long)Math.Pow(10, volumeBarCount - 1);

                    e.Graphics.FillRectangle(drawBrush2, 50, 5, 200, 250);
                    e.Graphics.FillRectangle(drawBrush2, 300, 5, 200, 250);
                    long bars = 0;
                    for (int i = 0; i < volumeBarCount; ++i)
                    {
                        e.Graphics.DrawLine(blackPen, 300, 255 - i * 230 / (volumeBarCount - 1), 500, 255 - i * 230 / (volumeBarCount - 1));
                        e.Graphics.DrawString(bars.ToString(), font, drawBrush, 295, 255 - i * 230 / (volumeBarCount - 1), drawFormat);
                        if (bars == 0) bars = 1;
                        bars *= 10;
                    }
                    bars = 0;
                    for (int i = 0; i < quantityBarCount; ++i)
                    {
                        e.Graphics.DrawLine(blackPen, 50, 255 - i * 230 / (quantityBarCount - 1), 250, 255 - i * 230 / (quantityBarCount - 1));
                        e.Graphics.DrawString(bars.ToString(), font, drawBrush, 45, 255 - i * 230 / (quantityBarCount - 1), drawFormat);
                        if (bars == 0) bars = 1;
                        bars *= 10;
                    }
                    for (int i = 0; i < typesToDisplay; ++i)
                    {
                        height = (float)(Math.Log10(typesQuantity[i].Item2) / Math.Log10(barQuantity0)) * 230;
                        e.Graphics.DrawString(typesQuantity[i].Item1, font, drawBrush, 70 + 20 * i, 260, drawFormat);
                        e.Graphics.FillRectangle(colorBrush[i], 54 + i * 20, 255 - height, 12, height);
                        e.Graphics.DrawRectangle(blackPen, 54 + i * 20, 255 - height, 12, height);

                        height = (float)(Math.Log10(typesVolume[i].Item2) / Math.Log10(barVolume0)) * 230;
                        e.Graphics.DrawString(typesVolume[i].Item1, font, drawBrush, 320 + 20 * i, 260, drawFormat);
                        e.Graphics.FillRectangle(colorBrush[i], 304 + i * 20, 255 - height, 12, height);
                        e.Graphics.DrawRectangle(blackPen, 304 + i * 20, 255 - height, 12, height);
                    }
                    break;
                default:
                    break;
            }
        }

        // https://stackoverflow.com/questions/359612/how-to-convert-rgb-color-to-hsv
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}