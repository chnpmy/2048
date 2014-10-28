using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2048
{
    public partial class MainForm : Form
    {
        Label[] lblArray = new Label[16];
        int[] numberArray = new int[16];
        private int score = 0;
        //private int[] dx = {0, 0, 0};

        public const int Merged = 1;
        public const int BeMoved = 2;
        public const int Notmove = 3;

        public MainForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        Random random = new Random();
        private void MainForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                lblArray[i] = new Label();
                tableLayoutPanel.Controls.Add(lblArray[i], i % 4, i / 4);
                lblArray[i].Dock = DockStyle.Fill;
                numberArray[i] = 0;
                lblArray[i].Font = new Font("隶书", 30, FontStyle.Bold);
                lblArray[i].TextAlign = ContentAlignment.MiddleCenter;
                lblArray[i].BorderStyle = BorderStyle.FixedSingle;
            }
            
            int r = random.Next(16);
            SetArrayAndLabel(r, Gen2Or4());
            if (r == 0)
            {
                SetArrayAndLabel(random.Next(15) + 1, Gen2Or4());
            }
            else
            {
                SetArrayAndLabel(random.Next(r), Gen2Or4());
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            bool move = false;
            int[] oldArray = new int[16];
            for (int i = 0; i < 16; i++) oldArray[i] = numberArray[i];
            if (e.KeyCode == Keys.Up)
            {
                Up();
            }
            else if (e.KeyCode == Keys.Down)
            {
                Down();
            }
            else if (e.KeyCode == Keys.Left)
            {
                Left();
            }
            else if (e.KeyCode == Keys.Right)
            {
                Right();
            }
            else if (e.KeyCode == Keys.A)
            {
                for(int i = 0; i < 100;i++)
                AI();
                return ;
            }
            else if (e.KeyCode == Keys.R)
            {
                System.Diagnostics.Process.Start("2048");
                Application.Exit();
            }
            int[] newArray = new int[16];
            for (int i = 0; i < 16; i++) newArray[i] = numberArray[i];
            for (int i = 0; i < 16; i++)
            {
                if (oldArray[i] != newArray[i])
                    move = true;
            }
            if (move)
                New2Or4();
            lblScore.Text = score.ToString();
            if (death())
            {
                MessageBox.Show("Game Over!");
            }
        }

        private int Gen2Or4()
        {
            int i = random.Next(100);
            if (i >= 10)
                return 2;
            else
                return 4;
        }

        private void New2Or4()
        {

            //TODO 简单的新生成一个方块，需要改进
            Thread.Sleep(200);
            Application.DoEvents();
            int haveValue = 0;
            for (int i = 0; i < 16; i++)
            {
                if (numberArray[i] != 0)
                { 
                    haveValue++;
                }
            }
            int noValue = 16 - haveValue;
            if (noValue == 0)
                return;
            int r = random.Next(noValue);
            int k = 0;
            for (int j = 0; j < 16; j++)
            {
                if (numberArray[j] == 0)
                {
                    if (k == r)
                    {
                        SetArrayAndLabel(j, Gen2Or4());
                        break;
                    }
                    k++;
                }
            }
        }

        private void SetArrayAndLabel(int index, int val)
        {
            numberArray[index] = val;
            if (numberArray[index] == 2)
                lblArray[index].BackColor = Color.AliceBlue;
            else if (numberArray[index] == 4)
                lblArray[index].BackColor = Color.Aquamarine;
            else if (numberArray[index] == 8)
                lblArray[index].BackColor = Color.Aqua;
            else if (numberArray[index] == 16)
                lblArray[index].BackColor = Color.LightSkyBlue;
            else if (numberArray[index] == 32)
                lblArray[index].BackColor = Color.LightSeaGreen;
            else if (numberArray[index] == 64)
                lblArray[index].BackColor = Color.MediumSpringGreen;
            else if (numberArray[index] == 128)
                lblArray[index].BackColor = Color.Chartreuse;
            else if (numberArray[index] == 256)
                lblArray[index].BackColor = Color.ForestGreen;
            else if (numberArray[index] == 512)
                lblArray[index].BackColor = Color.Green;
            else if (numberArray[index] == 1024)
                lblArray[index].BackColor = Color.Goldenrod;
            else if (numberArray[index] >= 2048)
                lblArray[index].BackColor = Color.Red;
            else
                lblArray[index].BackColor = Color.GhostWhite;
            if (val != 0)
                lblArray[index].Text = val.ToString();
            else
                lblArray[index].Text = "";
        }

        private int MoveAToB(int a, int b)
        {
            if (a == b)
            {
                return Notmove;
            }
            if (numberArray[a] == numberArray[b] && numberArray[a] != 0)
            {
                SetArrayAndLabel(b, 2 * numberArray[b]);
                SetArrayAndLabel(a, 0);
                return Merged;
            }
            else if (numberArray[b] == 0 || numberArray[a] != 0)
            {
                SetArrayAndLabel(b, numberArray[a]);
                SetArrayAndLabel(a, 0);
                return BeMoved;
            }
            else
            {
                return Notmove;
            }
        }

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        private int postonum(int x, int y)
        {
            return x * 4 + y;
        }
        private bool legalpos(int x, int y)
        {
            if (x < 0 || x > 3) return false;
            if (y < 0 || y > 3) return false;
            return true;
        }
        private void moveline(int k, int l)
        {
            bool flag;
            int x, y, u, v;
            if (k == 0 || k == 2)
            {
                while (true)
                {
                    flag = false;
                    for (x = 0; x < 4; x++)
                        for (y = 0; y < 4; y++)
                        {
                            u = x + dx[k];
                            v = y + dy[k];
                            if (!legalpos(u, v)) continue;
                            if (l == 0)
                            {
                                if (numberArray[postonum(u, v)] == 0 && numberArray[postonum(x, y)] != 0)
                                {
                                    MoveAToB(postonum(x, y), postonum(u, v));
                                    flag = true;
                                }
                            }
                            else
                            {
                                if (numberArray[postonum(u, v)] == numberArray[postonum(x, y)])
                                {
                                    score = score + numberArray[postonum(u, v)] * 2;
                                    MoveAToB(postonum(x, y), postonum(u, v));
                                    flag = true;
                                }
                            }
                        }
                    if (!flag) break;
                    if (l == 1) break;
                }
            }
            else
            {
                while (true)
                {
                    flag = false;
                    for (x = 3; x >= 0; x--)
                        for (y = 3; y >= 0; y--)
                        {
                            u = x + dx[k];
                            v = y + dy[k];
                            if (!legalpos(u, v)) continue;
                            if (l == 0)
                            {
                                if (numberArray[postonum(u, v)] == 0 && numberArray[postonum(x, y)] != 0)
                                {
                                    MoveAToB(postonum(x, y), postonum(u, v));
                                    flag = true;
                                }
                            }
                            else
                            {
                                if (numberArray[postonum(u, v)] == numberArray[postonum(x, y)])
                                {
                                    score = score + numberArray[postonum(u, v)] * 2;
                                    MoveAToB(postonum(x, y), postonum(u, v));
                                    flag = true;
                                }
                            }
                        }
                    if (!flag) break;
                    if (l == 1) break;
                }
            }
        }
        private void Up()
        {
            moveline(0, 0);
            moveline(0, 1);
            moveline(0, 0);
        }
        private void Down()
        {
            moveline(1, 0);
            moveline(1, 1);
            moveline(1, 0);
        }
        private void Left()
        {
            moveline(2, 0);
            moveline(2, 1);
            moveline(2, 0);
        }
        private void Right()
        {
            moveline(3, 0);
            moveline(3, 1);
            moveline(3, 0);
        }
        private bool death()
        {
            int x, y, i;
            int[] oldArray = new int[16];
            for (i = 0; i < 16; i++)
            {
                if (numberArray[i] == 0) return false;
                oldArray[i] = numberArray[i];
            }
            moveline(0, 0);
            moveline(1, 0);
            moveline(2, 0);
            moveline(3, 0);
            moveline(0, 1);
            moveline(1, 1);
            moveline(2, 1);
            moveline(3, 1);
            for (i = 0; i < 16; i++)
                if (oldArray[i] != numberArray[i]) break;
            if (i < 16)
            {
                for (i = 0; i < 16; i++) SetArrayAndLabel(i, oldArray[i]);
                return false;
            }
            return true;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private int countblank()
        {
            int i, j, s = 0;
            for (i = 0; i < 16; i++)
                if (numberArray[i] == 0) ++s;

            return s*512 + numberArray[0]*8 + numberArray[4] + numberArray[1];

        }
        private int select()
        {
            int[] old = new int[16];
            int oldscore = score;
            int maxs = 0, cont = 0, s;
            for (int i = 0; i < 16; i++) old[i] = numberArray[i];
            for (int k = 0; k < 4; k++)
            {
                moveline(k, 0);
                moveline(k, 1);
                moveline(k, 0);
                s = countblank() + (score - oldscore);
                int j;
                for(j = 0; j < 16; j++)
                    if (old[j] != numberArray[j]) break;
                if (j == 16) s = -1;
                if (s > maxs)
                {
                    maxs = s;
                    cont = k;
                }
                score = oldscore;
                for (int i = 0; i < 16; i++) SetArrayAndLabel(i, old[i]);
            }
            return cont;
        }

        private void AI()
        {
            bool move = false;
            int[] oldArray = new int[16];
            for (int i = 0; i < 16; i++) oldArray[i] = numberArray[i];
            int cont = select();
            if (cont == 0)
            {
                Up();
            }
            else if (cont == 1)
            {
                Down();
            }
            else if (cont == 2)
            {
                Left();
            }
            else if (cont == 3)
            {
                Right();
            }
            int[] newArray = new int[16];
            for (int i = 0; i < 16; i++) newArray[i] = numberArray[i];
            for (int i = 0; i < 16; i++)
            {
                if (oldArray[i] != newArray[i])
                    move = true;
            }
            if (move)
                New2Or4();
            Application.DoEvents();
            lblScore.Text = score.ToString();
            if (death())
            {
                MessageBox.Show("Game Over!");
            }
        }

    }
}
