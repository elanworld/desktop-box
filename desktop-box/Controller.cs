using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using desktop_box.entity;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;

namespace desktop_box
{
    public class Controller
    {
        public Bitmap bitmap { get; set; } = new Bitmap(1,1);
        public Form1 form;
        int delayShow = 200;
        double transparency = 1;
        int? width;

        public Controller(Form1 form)
        {
            this.form = form;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Resource1));
            Bitmap eyes = (Bitmap)resources.GetObject("eyes");
            this.SetBits(eyes);
        }

        public void SetBits(Bitmap bitmap)
        {
            this.bitmap = this.SetProp(bitmap);
            this.form.SetBits(this.bitmap, (byte)(transparency * 255));
        }

        public async void ShowPath(string path, Body body)
        {
            this.delayShow = body.delay ?? this.delayShow;
            this.width = body.width ?? this.width;
            this.transparency = body.transparency ?? this.transparency;
            if (File.Exists(path)) // 如果是文件
            {
                Bitmap image = new Bitmap(path);
                this.SetBits(image);
            }
            else if (Directory.Exists(path)) // 如果是文件夹
            {
                string[] pngFiles = Directory.GetFiles(path, "*.png");
                foreach (string pngFile in pngFiles)
                {
                    Bitmap pngImage = new Bitmap(pngFile);
                    this.SetBits(pngImage);
                    await Task.Delay(this.delayShow);
                }
                this.emptBitMap();
            }
            else // 其他情况
            {
                this.form.ShowFloatingText(path, this.delayShow);
            }
        }

        public void setProp(Body body)
        {
            this.delayShow = body.delay ?? this.delayShow;
            this.width = body.width ?? this.width;
            this.transparency = body.transparency ?? this.transparency;
        }

        public void updateBitMap()
        {
            if (this.bitmap != null)
            {
                this.SetBits(this.bitmap);
            }
        }


        private List<Bitmap> ReadGifFrames(string filePath)
        {
            List<Bitmap> frames = new List<Bitmap>();

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                GifBitmapDecoder decoder = new GifBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                foreach (BitmapFrame frame in decoder.Frames)
                {
                    Bitmap bitmap = ConvertToBitmap(frame);
                    frames.Add(bitmap);
                }
            }
            return frames;
        }

        private Bitmap ConvertToBitmap(BitmapFrame bitmapFrame)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(bitmapFrame);
                enc.Save(outStream);
                return new Bitmap(outStream);
            }
        }


        public Bitmap ResizeBitMap(Bitmap bitmap, int width)
        {
            return new Bitmap(bitmap, width, width * bitmap.Height / bitmap.Width);
        }

        /**
         * 设置透明度
         */
        public void Transparency(double op)
        {
            this.transparency = op;
            this.SetBits(this.bitmap);

        }
        public void emptBitMap()
        {
            this.form.SetBits(new Bitmap(1, 1), 0);
        }

        public Bitmap SetProp(Bitmap bitmap)
        {
            if (this.width != null)
            {
                bitmap = this.ResizeBitMap(bitmap, (int)this.width);
            }
            if (this.transparency == 0)
            {
                bitmap = new Bitmap(1, 1);
            }
            return bitmap;
        }

        public static Bitmap TextToBitmap(string text, Font font)
        {
            // 创建一个临时的画布
            Bitmap bitmap = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(bitmap);

            // 测量文本的大小
            SizeF textSize = graphics.MeasureString(text, font);

            // 释放临时画布
            graphics.Dispose();
            bitmap.Dispose();

            // 根据文本大小创建新的位图
            bitmap = new Bitmap((int)textSize.Width, (int)textSize.Height);
            graphics = Graphics.FromImage(bitmap);

            // 绘制不透明背景
            graphics.Clear(Color.White); // 设置为不透明的白色背景

            // 绘制文本到位图
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; // 设置抗锯齿
            graphics.DrawString(text, font, Brushes.Black, new PointF(0, 0));

            // 释放资源
            graphics.Dispose();

            return bitmap;
        }

    }
}
