using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PrjLangton
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics g;
        Random rand;
        Brush[] colors;
        Panel[] panels;
        Label[] labels;
        char[] rulestext;
        readonly List<byte> Rules = new List<byte>();
        byte[,] grid = new byte[256, 256];
        byte angle = 0;
        byte x = 128;
        byte y = 128;
        bool stop = true;
        int display = 100;

        private void Form1_Shown(object sender, EventArgs e)
        {
            g = CreateGraphics();
            rand = new Random();
            panels = new Panel[] {panel1, panel2, panel3, panel4, panel5, panel6, panel7, panel8, panel9, panel10, panel11, panel12,
            panel13, panel14, panel15, panel16, panel17, panel18, panel19, panel20, panel21, panel22, panel23, panel24};
            labels = new Label[] {lbl1, lbl2, lbl3, lbl4, lbl5, lbl6, lbl7, lbl8, lbl9, lbl10, lbl11, lbl12,
            lbl13, lbl14, lbl15, lbl16, lbl17, lbl18, lbl19, lbl20, lbl21, lbl22, lbl23, lbl24};
            colors = new Brush[] {Brushes.White, Brushes.Black, Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.Purple, Brushes.DarkTurquoise,
            Brushes.Magenta, Brushes.Orange, Brushes.Lime, Brushes.Maroon, Brushes.SteelBlue, Brushes.Chocolate, Brushes.Gold, Brushes.LimeGreen,
            Brushes.Aquamarine, Brushes.Teal, Brushes.Cyan, Brushes.Teal, Brushes.BlueViolet, Brushes.Violet, Brushes.DeepPink, Brushes.Crimson};
            rulestext = new char[] {'L', 'R', 'U', 'D', 'W', 'E'};
            Rules.Add(2);
            Rules.Add(6);
        }

        private void walk()
        {
            switch (angle)
            {
                case 0:
                    y--;
                    break;
                case 1:
                    x++;
                    y--;
                    break;
                case 2:
                    x++;
                    break;
                case 3:
                    x++;
                    y++;
                    break;
                case 4:
                    y++;
                    break;
                case 5:
                    x--;
                    y++;
                    break;
                case 6:
                    x--;
                    break;
                case 7:
                    x--;
                    y--;
                    break;
            }
            angle += Rules[grid[x, y]];
            angle &= 7;
            grid[x, y]++;
            grid[x, y] %= Convert.ToByte(Rules.Count);
            g.FillRectangle(colors[grid[x, y]], x * 2, y * 2, 2, 2);
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnReset.Enabled = true;
            stop = false;
            while (!stop)
                await Task.Run(() => walk());
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            stop = true;
            btnReset.Enabled = false;
            btnStart.Enabled = true;
            grid = new byte[256, 256];
            x = 128;
            y = 128;
            angle = 0;
            Invalidate();
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            int c = int.Parse(txtColors.Text);
            int m = int.Parse(txtMoves.Text);
            txtLoad.Text = "";
            for (int i = 0; i < c; i++)
                txtLoad.Text += rulestext[rand.Next(m)];
            btnLoad_Click(null, null);
            btnLoad_MouseClick(null, null);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            txtLoad.Text = txtLoad.Text.ToUpper();
            btnReset_Click(null, null);
        }

        private void btnLoad_MouseClick(object sender, MouseEventArgs e)
        {
            Rules.Clear();
            string ant = txtLoad.Text;
            for (int i = 0; i < ant.Length; i++)
            {
                switch (ant[i])
                {
                    case 'L':
                        Rules.Add(6);
                        labels[i].Text = "Left";
                        break;
                    case 'R':
                        Rules.Add(2);
                        labels[i].Text = "Right";
                        break;
                    case 'U':
                        Rules.Add(0);
                        labels[i].Text = "Up";
                        break;
                    case 'D':
                        Rules.Add(4);
                        labels[i].Text = "Down";
                        break;
                    case 'W':
                        Rules.Add(7);
                        labels[i].Text = "West";
                        break;
                    case 'E':
                        Rules.Add(1);
                        labels[i].Text = "East";
                        break;
                }
                panels[i].BackColor = ((SolidBrush) colors[i]).Color;
            }
            for (int i = ant.Length; i < 24; i++)
            {
                panels[i].BackColor = Color.FromKnownColor(KnownColor.ScrollBar);
                labels[i].Text = "";
            }
        }

        private async void btnAuto_Click(object sender, EventArgs e)
        {
            int len = int.Parse(txtAuto.Text);
            display = int.Parse(txtDisplay.Text);
            for (int i = 1; i < Math.Pow(2, len - 1); i++)
            {
                string ant = Convert.ToString(i, 2);
                while (ant.Length < len)
                    ant = "0" + ant;
                ant = ant.Replace('0', 'L');
                ant = ant.Replace('1', 'R');
                if (!stop)
                    btnReset_Click(null, null);
                txtLoad.Text = ant;
                btnLoad_MouseClick(null, null);
                btnStart_Click(null, null);
                await Task.Delay(display);
                btnReset_Click(null, null);
                await Task.Delay(1);
            }
        }
    }
}