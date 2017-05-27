using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RMaskToScript_Fu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.AllowDrop = true;
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string format = textBox1.Text.Replace("{X}", "{0}").Replace("{Y}", "{1}").Replace("{Width}", "{2}").Replace("{Height}", "{3}") + "\r\n";

            foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                RectList list = MaskToRect(filePath);
                foreach (Rect rect in list)
                {
                    textBox2.Text += string.Format(format, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }

        private RectList MaskToRect(string filePath)
        {
            Bitmap bmp = new Bitmap(filePath);
            RectList list = new RectList();

            toolStripProgressBar1.Maximum = bmp.Size.Height;
            toolStripProgressBar1.Value = 0;

            for (int y = 0; y < bmp.Size.Height; y++)
            {
                for (int x = 0; x < bmp.Size.Width; x++)
                {
                    if (list.Contains(x, y))
                        continue;

                    Color c = bmp.GetPixel(x, y);
                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        Color cx;
                        Color cy;
                        int x_max;
                        int y_max = bmp.Size.Height;
                        int y2;

                        for (x_max = x; x_max < bmp.Size.Width - 1; x_max++)
                        {
                            for (y2 = y; y2 < bmp.Size.Height - 1; y2++)
                            {
                                cy = bmp.GetPixel(x_max, y2 + 1);
                                if (cy.R != 0 || cy.G != 0 || cy.B != 0)
                                    break;
                            }
                            if (y2 < y_max)
                                y_max = y2;

                            cx = bmp.GetPixel(x_max + 1, y);
                            if (cx.R != 0 || cx.G != 0 || cx.B != 0)
                                break;
                        }

                        list.Add(new Rect(x, y, x_max, y_max));
                    }
                }

                toolStripProgressBar1.Value++;
            }

            return list;
        }

        public class Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int X
            {
                get
                {
                    return Left;
                }
            }
            public int Y
            {
                get
                {
                    return Top;
                }
            }
            public int Width
            {
                get
                {
                    int result = Right - Left + 1;
                    return result;
                }
            }
            public int Height
            {
                get
                {
                    int result = Bottom - Top + 1;
                    return result;
                }
            }
            public Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom= bottom;
            }
            public bool Contains(int x, int y)
            {
                bool result = Left <= x && x <= Right && Top <= y && y <= Bottom;
                return result;
            }
        }

        public class RectList : List<Rect>
        {
            public bool Contains(int x, int y)
            {
                bool result = false;
                foreach (Rect rect in this)
                {
                    result = rect.Contains(x, y);
                    if (result)
                        break;
                }
                return result;
            }
        }
    }
}
