using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ㄹ
{
    public partial class Form1 : Form
    {
        private List<Label> _tans;
        private Random _random;
        private AutoResetEvent _wait;

        Thread move;
        public Form1()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            _tans = new List<Label>();
            _random = new Random();
            label2.Visible = false;
            _wait = new AutoResetEvent(true);
            this.MouseDown += WowMouseDown;
            label1.MouseDown += WowMouseDown;

            move = new Thread(new ThreadStart(moveThread));
            move.Start();
        }

        private Label CreateTanhaek()
        {
            Label tan = new Label()
            {
                Text = "탄핵"
            };
            tan.MouseDown += WowMouseDown;
            tan.Location = new Point(_random.Next(Width), 0);

            _tans.Add(tan);

            return tan;
        }

        private void GameOver()
        {
            _wait.Set();
            timer1.Stop();
            label2.Visible = true;
            MessageBox.Show("당신은 탄핵당햇습니다!");
            move.Abort();
        }

        private int _direction = 1;
        private void WowMouseDown(object sender, MouseEventArgs e)
        {
            _direction = -_direction;
        }

        private int nextX()
        {
            int x = _direction * 20 + label1.Location.X;
            if (x <= 0) { _direction = -_direction; return 0; }
            if (x >= Width - label1.Width) { _direction = -_direction; return Width - label1.Width; }
            return x;
        }

        private readonly object yeah = new object();
        private void moveThread()
        {
            while (true)
            {
                lock (yeah)
                {
                    _wait.Reset();
                    foreach (var tan in _tans)
                    {
                        if (tan.Bounds.IntersectsWith(label1.Bounds))
                        {
                            GameOver();
                        }

                        tan.Location = new Point(tan.Location.X, tan.Location.Y + 10);
                        if (tan.Location.Y > Height)
                        {
                            this.Controls.Remove(tan);
                            tan.Dispose();
                        }
                    }
                    _wait.Set();
                    label1.Location = new Point(nextX(), label1.Location.Y);
                    Thread.Sleep(50);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _wait.WaitOne();
            this.Controls.Add(CreateTanhaek());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            move.Abort();
        }
    }
}
