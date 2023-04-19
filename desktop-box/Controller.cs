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

namespace desktop_box
{
    public class Controller
    {
        Bitmap bitmap;
        Form1 form;

        public Controller(Form1 form)
        {
            this.form = form;
        }

        public async void ShowDicrectory(String directory, Body body)
        {
            string[] pngFiles = Directory.GetFiles(directory, "*.png");
            foreach (string pngFile in pngFiles)
            {
                Bitmap pngImage = new Bitmap(pngFile);
                int timeDelay = (int)(body.delay == null ? 200 : body.delay);
                int width = (int)(body.width == null ? 200 : body.width);
                this.form.SetBits(this.ResizeBitMap(pngImage, width));
                await Task.Delay(timeDelay);
            }
            this.emptBitMap();
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


        public void Transparency(Bitmap bitmap, int op)

        {
            for (int i = 0; i < (bitmap.Width); i++)
            {
                for (int j = 0; j < (bitmap.Height); j++)
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
            if (op == 0)
            {
                emptBitMap();
                return;
            }
            for (int i = 0; i < (bitmap.Width); i++)
            {
                for (int j = 0; j < (bitmap.Height); j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    if (!(color.G == 0 && color.B == 0 && color.R == 0))
                    {
                        bitmap.SetPixel(i, j, Color.FromArgb((int)(op * 255), color));
                    }
                }
            }
            this.form.SetBits(bitmap);

        }


        public void emptBitMap()
        {
            this.form.SetBits(new Bitmap(1, 1));
        }

    }
}
