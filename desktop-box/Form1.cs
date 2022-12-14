using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace desktop_box
{
    public partial class Form1 : Form
    {
        private bool bFormDragging;
        private Point oPointClicked;
        Bitmap bitmap;

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




        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Resource1));
            Bitmap eyes = (Bitmap) resources.GetObject("eyes");
            bitmap = ResizeBitMap(eyes, 200);
            SetBits(bitmap);
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

        public void MoveWindows(int beforeWidth, int beforeHeight, int afterWidth, int afterHeigth, double duration)
        {
            for (int i = 0; i < duration*1000 / 10; i++)
            {
                Location = new Point((int)(beforeWidth + (i + 1) * ((afterWidth - beforeWidth) / (duration * 1000 / 10))), (int)(beforeHeight + (i + 1) * ((afterHeigth - beforeHeight) / (duration * 1000 / 10))));
                Thread.Sleep(10);
            }
        }

        public void MoveWindows(int width, int height)
        {
            Location = new Point(width, height);
        }

        public void emptBitMap()
        {
            SetBits(new Bitmap(1, 1));
        }

        public void Transparency(Bitmap bitmap, int op)

        {
            for(int i = 0; i < (bitmap.Width); i++)
            {
                for(int j=0; j < (bitmap.Height ); j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    if (!(color.G == 0 && color.B == 0 && color.R == 0))
                    {
                        bitmap.SetPixel(i, j, Color.FromArgb(op, color));
                    }
                }
            }
            
        }
        public void Transparency(double op)
        {
            if (op==0)
            {
                emptBitMap();
                return;
            }
            for(int i = 0; i < (bitmap.Width); i++)
            {
                for(int j=0; j < (bitmap.Height ); j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    if (!(color.G == 0 && color.B == 0 && color.R == 0))
                    {
                        bitmap.SetPixel(i, j, Color.FromArgb((int)(op * 255), color));
                    }
                }
            }
            SetBits(bitmap);
            
        }


        public Bitmap ResizeBitMap(Bitmap bitmap, int width)
        {
            return new Bitmap(bitmap, width, width * bitmap.Height / bitmap.Width);
        }


        public void SetBits(Bitmap bitmap)
        {

            if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
                MessageBox.Show("Error Bitmap");

            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

            try
            {
                Win32.Point topLoc = new Win32.Point(Left, Top);
                Win32.Size bitMapSize = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                Win32.Point srcLoc = new Win32.Point(0, 0);

                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBits = Win32.SelectObject(memDc, hBitmap);

                blendFunc.BlendOp = Win32.AC_SRC_OVER;
                blendFunc.SourceConstantAlpha = 255;
                blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
                blendFunc.BlendFlags = 0;

                Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBits);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.ReleaseDC(IntPtr.Zero, screenDC);
                Win32.DeleteDC(memDc);
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void swapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveWindows(10, 900, 1780, 5, 1);
        }
    }
}
   

