using System.Drawing;
using System.Security.Policy;

namespace WinFormsControlLibrary1
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1() { InitializeComponent(); }
        public int mode = -1;
        public static int padding = 5;
        public static int skip = 20;
        public static readonly string[] units = { "bytes", "KB", "MB", "GB", "TB", "PB" };
        public (string, long)[] typesVolume = new (string, long)[0];
        public (string, int)[] typesQuantity = new (string, int)[0];
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e) { }
        private void UserControl1_Load(object sender, EventArgs e) { }
        private void pictureBox1_Paint(object sender, PaintEventArgs e) { }

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
            blackPen.Width = 1;
            Font font = new Font("Segoe UI", 10);
            e.Graphics.DrawString("by quantity", font, drawBrush, 10, 300, drawFormat);
            e.Graphics.DrawString("by file size (bytes)", font, drawBrush, 290, 300, drawFormat);

            if (mode == 0)
            {
                long totalVolume = 0;
                int totalQuantity = 0;
                foreach (var elem in typesVolume) totalVolume += elem.Item2;
                foreach (var elem in typesQuantity) totalQuantity += elem.Item2;
                float[,] angles = new float[typesToDisplay, 2];
                float offset;
                long size;
                int un;
                for (int i = 0; i < typesToDisplay; ++i)
                {
                    offset = 0;
                    angles[i, 0] = (float)typesQuantity[i].Item2 / totalQuantity * 360;
                    for (int j = 0; j < i; ++j) offset += angles[j, 0];
                    e.Graphics.FillPie(colorBrush[i], padding, padding, 150, 150, offset, angles[i, 0]);
                    e.Graphics.DrawPie(blackPen, padding, padding, 150, 150, offset, angles[i, 0]);
                    e.Graphics.FillRectangle(colorBrush[i], 160, padding + i * skip, 30, 15);
                    e.Graphics.DrawRectangle(blackPen, 160, padding + i * skip, 30, 15);
                    e.Graphics.DrawString(typesQuantity[i].Item1 + " - " + typesQuantity[i].Item2, font, drawBrush, 195, padding + i * skip, drawFormat);

                    offset = 0;
                    angles[i, 1] = (float)typesVolume[i].Item2 / totalVolume * 360;
                    for (int j = 0; j < i; ++j) offset += angles[j, 1];
                    e.Graphics.FillPie(colorBrush[i], 285, padding, 150, 150, offset, angles[i, 1]);
                    e.Graphics.DrawPie(blackPen, 285, padding, 150, 150, offset, angles[i, 1]);
                    e.Graphics.FillRectangle(colorBrush[i], 440, padding + i * skip, 30, 15);
                    e.Graphics.DrawRectangle(blackPen, 440, padding + i * skip, 30, 15);
                    size = typesVolume[i].Item2;
                    un = 0;
                    while (size >= 1024)
                    {
                        size /= 1024;
                        ++un;
                    }
                    e.Graphics.DrawString(typesVolume[i].Item1 + " - " + size + units[un], font, drawBrush, 475, padding + i * skip, drawFormat);
                }
            }
            else
            {
                if (typesQuantity.Length == 0) return;
                int maxQuantity = typesQuantity.MaxBy(x => x.Item2).Item2;
                long maxVolume = typesVolume.MaxBy(x => x.Item2).Item2;
                float height;
                float barQuantity;
                long barVolume;
                e.Graphics.FillRectangle(drawBrush2, 50, padding, 200, 250);
                e.Graphics.FillRectangle(drawBrush2, 300, padding, 200, 250);
                if (mode == 1)
                {
                    barQuantity = (float)(Math.Pow(10, Math.Floor(Math.Log10(maxQuantity))) * Math.Ceiling(maxQuantity / Math.Pow(10, Math.Floor(Math.Log10(maxQuantity)))));
                    barVolume = (long)(Math.Pow(10, Math.Floor(Math.Log10(maxVolume))) * Math.Ceiling(maxVolume / Math.Pow(10, Math.Floor(Math.Log10(maxVolume)))));
                    for (int i = 0; i < 11; ++i)
                    {
                        e.Graphics.DrawLine(blackPen, 50, 25 + 23 * i, 250, 25 + 23 * i);
                        e.Graphics.DrawString(Math.Round(barQuantity * (1 - 0.1 * i)).ToString(), font, drawBrush, 10, 15 + 23 * i, drawFormat);
                        e.Graphics.DrawLine(blackPen, 300, 25 + 23 * i, 500, 25 + 23 * i);
                        e.Graphics.DrawString(Math.Round(barVolume * (1 - 0.1 * i)).ToString(), font, drawBrush, 260, 15 + 23 * i, drawFormat);
                    }
                }
                else
                {
                    int volumeBarCount = (int)Math.Floor(Math.Log10(maxVolume)) + 2;
                    int quantityBarCount = (int)Math.Floor(Math.Log10(maxQuantity)) + 2;
                    barQuantity = (float)(Math.Pow(10, Math.Floor(Math.Log10(maxQuantity))) * Math.Ceiling(maxQuantity / Math.Pow(10, Math.Floor(Math.Log10(maxQuantity)))));
                    barVolume = (long)Math.Pow(10, volumeBarCount - 1);
                    long bars = 0;
                    for (int i = 0; i < volumeBarCount; ++i)
                    {
                        e.Graphics.DrawLine(blackPen, 300, 255 - i * 230 / (volumeBarCount - 1), 500, 255 - i * 230 / (volumeBarCount - 1));
                        e.Graphics.DrawString(10 + "^" + bars.ToString(), font, drawBrush, 260, 245 - i * 230 / (volumeBarCount - 1), drawFormat);
                        ++bars;
                    }
                    bars = 0;
                    for (int i = 0; i < quantityBarCount; ++i)
                    {
                        e.Graphics.DrawLine(blackPen, 50, 255 - i * 230 / (quantityBarCount - 1), 250, 255 - i * 230 / (quantityBarCount - 1));
                        e.Graphics.DrawString(10 + "^" + bars.ToString(), font, drawBrush, 10, 245 - i * 230 / (quantityBarCount - 1), drawFormat);
                        ++bars;
                    }
                }
                int h = 260;
                for (int i = 0; i < typesToDisplay; ++i)
                {
                    if (i % 2 == 0) h = 260;
                    else h = 275;
                    if (mode == 1) height = ((float)typesQuantity[i].Item2 / barQuantity) * 230;
                    else height = (float)(Math.Log10(typesQuantity[i].Item2) / Math.Log10(barQuantity)) * 230;
                    e.Graphics.DrawString(typesQuantity[i].Item1, font, drawBrush, 54 + skip * i, h, drawFormat);
                    e.Graphics.FillRectangle(colorBrush[i], 54 + i * skip, 255 - height, 12, height);
                    e.Graphics.DrawRectangle(blackPen, 54 + i * skip, 255 - height, 12, height);

                    if (mode == 1) height = ((float)typesVolume[i].Item2 / barVolume) * 230;
                    else height = (float)(Math.Log10(typesVolume[i].Item2) / Math.Log10(barVolume)) * 230;
                    e.Graphics.DrawString(typesVolume[i].Item1, font, drawBrush, 304 + skip * i, h, drawFormat);
                    e.Graphics.FillRectangle(colorBrush[i], 304 + i * skip, 255 - height, 12, height);
                    e.Graphics.DrawRectangle(blackPen, 304 + i * skip, 255 - height, 12, height);
                }
            }
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            else if (hi == 1) return Color.FromArgb(255, q, v, p);
            else if (hi == 2) return Color.FromArgb(255, p, v, t);
            else if (hi == 3) return Color.FromArgb(255, p, q, v);
            else if (hi == 4) return Color.FromArgb(255, t, p, v);
            else return Color.FromArgb(255, v, p, q);
        }
    }
}