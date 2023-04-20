﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using desktop_box.entity;

namespace desktop_box
{
    public class Controller
    {
        public Bitmap bitmap { get; set; }
        Form1 form;
        int delayShow = 200;
        double transparency = 1;
        int? width;

        public Controller(Form1 form)
        {
            this.form = form;
        }

        public async void ShowPath(string path, Body body)
        {
            this.delayShow = body.delay ?? this.delayShow;
            this.width = body.width ?? this.width;
            this.transparency = body.transparency ?? this.transparency;
            if (File.Exists(path)) // 如果是文件
            {
                Bitmap image = new Bitmap(path);
                this.form.SetBits(image);
            }
            else if (Directory.Exists(path)) // 如果是文件夹
            {
                string[] pngFiles = Directory.GetFiles(path, "*.png");
                foreach (string pngFile in pngFiles)
                {
                    Bitmap pngImage = new Bitmap(pngFile);
                    this.form.SetBits(pngImage);
                    await Task.Delay(this.delayShow);
                }
                this.emptBitMap();
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
            this.form.SetBits(this.bitmap);

        }
        public void emptBitMap()
        {
            this.form.SetBits(new Bitmap(1, 1));
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
            else if (this.transparency == 1)
            {

            }
            else
            {
                for (int i = 0; i < (bitmap.Width); i++)
                {
                    for (int j = 0; j < (bitmap.Height); j++)
                    {
                        Color color = bitmap.GetPixel(i, j);
                        if (!(color.G == 0 && color.B == 0 && color.R == 0))
                        {
                            bitmap.SetPixel(i, j, Color.FromArgb((int)(this.transparency * 255), color));
                        }
                    }
                }
            }
            this.bitmap = bitmap;
            return bitmap;
        }
    }
}
