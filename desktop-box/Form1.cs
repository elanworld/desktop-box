using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;
using System.Security.Policy;


namespace desktop_box
{
    public partial class Form1 : Form
    {
        private bool bFormDragging;
        private Point oPointClicked;
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public Form1()
        {
            InitializeComponent();

        }
        #region 重载


        protected override void OnHandleCreated(EventArgs e)
        {
            InitializeStyles();
            base.OnHandleCreated(e);
        }

        private void InitializeStyles()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        //创建一个分层窗口来支持 setBitmap
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cParms;
            }
        }

        #endregion


        //加载图片文件
        private async void Form1_Load(object sender, EventArgs e)
        {
            // 设置窗口样式，使其不出现在Alt+Tab切换列表中
            int WS_EX_TOOLWINDOW = 0x80;
            SetWindowLong(this.Handle, -20, GetWindowLong(this.Handle, -20) | WS_EX_TOOLWINDOW);
        }

        public void SetBits(Bitmap bitmap, byte alpha = 255)
        {
            Win32.SetBits(this.Handle, bitmap, this.Location.X, this.Location.Y, alpha);
        }


        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bFormDragging = true;
                oPointClicked = new Point(e.X, e.Y);
            }
        }

        private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bFormDragging = false;
            }
        }

        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (bFormDragging)
            {
                Point oMoveToPoint = default(Point);
                //以当前鼠标位置为基础，找出目标位置
                oMoveToPoint = PointToScreen(new Point(e.X, e.Y));
                oMoveToPoint.Offset(oPointClicked.X * -1, (oPointClicked.Y + SystemInformation.CaptionHeight + SystemInformation.BorderSize.Height) * -1 + 24);
                Location = oMoveToPoint;
            }
        }

public void MoveWindows(int beforeX, int beforeY, int afterX, int afterY, double duration)
    {
        DateTime startTime = DateTime.Now;
        TimeSpan elapsed;
        TimeSpan targetInterval = TimeSpan.FromMilliseconds(10);
        
        for (int i = 0; i < duration * 1000 / targetInterval.TotalMilliseconds; i++)
        {
            // Calculate progress ratio
            double progress = (i + 1) / (duration * 1000 / targetInterval.TotalMilliseconds);

            // Calculate new position
            int newX = (int)(beforeX + progress * (afterX - beforeX));
            int newY = (int)(beforeY + progress * (afterY - beforeY));

            // Move window
            Location = new Point(newX, newY);

            // Calculate elapsed time since startTime
            elapsed = DateTime.Now - startTime;

            // Calculate remaining time to sleep
            TimeSpan remainingTime = TimeSpan.FromMilliseconds(targetInterval.TotalMilliseconds * (i+1)) - elapsed;

            // If remaining time is positive, sleep for remaining time
            if (remainingTime > TimeSpan.Zero)
                Thread.Sleep(remainingTime);
        }
        Console.WriteLine($"swap time:{DateTime.Now - startTime}" );
    }


        public void MoveWindows(int width, int height)
        {
            Location = new Point(width, height);
        }



        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void swapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveWindows(10, 900, 1780, 5, 1);
        }

        /**
         * 在窗体上显示文字并删除文字(lanyer窗体未生效)
         */
        public void ShowText(string text, int timeout=5000)
        {
            // 设置窗体可见
           // this.Opacity = 0.5D;
            TextBox textBox = new TextBox();
            textBox.Text = text;
            textBox.Location = new Point(0, 0);
            textBox.ReadOnly = true;
            textBox.BorderStyle = BorderStyle.None;
            this.Controls.Add(textBox);


            // 创建一个计时器，一段时间后隐藏窗体
            Timer textTimer = new Timer();
            textTimer.Interval = timeout; // 5秒后隐藏窗体
            textTimer.Tick += (sender, e) =>
            {
                this.Opacity = 1D;
                this.Controls.Remove(textBox);
                textTimer.Stop(); // 停止计时器
                textTimer.Dispose(); // 释放计时器资源
            };
            textTimer.Start(); // 启动计时器
        }

        public void ShowFloatingText(string text, int timeout = 3000)
        {
            FloatingTextForm floatingTextForm = new FloatingTextForm(text, timeout);
            floatingTextForm.Location = this.Location;
            floatingTextForm.Show();
        }

    }
}


