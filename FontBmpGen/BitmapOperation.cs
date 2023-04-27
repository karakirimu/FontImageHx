using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace FontBmpGen
{
    public class FontAdjustConfig
    {
        public FontAdjustConfig()
        {
            FontFamily = "Arial";
            FontSize = 12;
            SingleCharWidth = 12;
            SingleCharHeight = 16;
            Bold = false;
            Italic = false;
            Underline = false;
        }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }

        public int SingleCharWidth { get; set; }
        public int SingleCharHeight { get; set; }

        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
    }

    public class ImageProperty
    {
        public BitmapImage? View { get; set; }
        public char? Character { get; set; }
        public string? Hex { get; set; }
    }

    public class BitmapOperation
    {
        public BitmapOperation() { }

        protected static BitmapImage ConvertImage(Bitmap source)
        {
            // BitmapをImageSourceに変換する
            BitmapImage bitmapImage = new();
            using (MemoryStream stream = new())
            {
                source.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            if (bitmapImage == null)
            {
                return new BitmapImage();
            }

            // ImageにImageSourceを設定する
            return bitmapImage;
        }

        protected static Bitmap DrawCharacter(char c, FontAdjustConfig config)
        {
            var bmp = new Bitmap(
                config.SingleCharWidth,
                config.SingleCharHeight,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(bmp);

            FontStyle fontStyle = FontStyle.Regular;
            if (config.Bold) fontStyle |= FontStyle.Bold;
            if (config.Italic) fontStyle |= FontStyle.Italic;
            if (config.Underline) fontStyle |= FontStyle.Underline;

            // フォントファイルを読み込む
            Font font = new(config.FontFamily, config.FontSize, fontStyle, GraphicsUnit.Pixel);

            // テキストのサイズを測定
            var sizeF = g.MeasureString(c.ToString(), font);

            // テキストを描画する位置を計算
            var x = (bmp.Width - sizeF.Width) / 2f;
            var y = (bmp.Height - sizeF.Height) / 2f;

            // 描画設定
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            g.SmoothingMode = SmoothingMode.HighSpeed;

            // テキストを描画
            using var brush = new SolidBrush(Color.White);
            g.DrawString(c.ToString(), font, brush, x, y);

            return bmp;
        }

        public static BitmapImage DrawTextInSpecifiedSize(string text, FontAdjustConfig config, int binaryThreshold)
        {
            return ConvertImage(GetImage(text, config, binaryThreshold));
        }

        public static Bitmap GetImage(string text, FontAdjustConfig config, int binaryThreshold)
        {
            int width = 0;
            int height = config.SingleCharHeight;

            int tmpw = 0;
            foreach (char c in text)
            {
                if (c == '\r')
                {
                    continue;
                }
                if (c == '\n')
                {
                    height += config.SingleCharHeight;
                    width = tmpw > width ? tmpw : width;
                    tmpw = 0;
                    continue;
                }
                tmpw += config.SingleCharWidth;
            }

            width = tmpw > width ? tmpw : width;

            var result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(result))
            {
                int offsetX = 0;
                int offsetY = 0;
                g.Clear(Color.Black);
                foreach (char c in text)
                {
                    if (c == '\0')
                    {
                        break;
                    }
                    if (c == '\r')
                    {
                        continue;
                    }
                    if (c == '\n')
                    {
                        offsetY += config.SingleCharHeight;
                        offsetX = 0;
                        continue;
                    }

                    g.DrawImage(BinarizeOtsu(DrawCharacter(c, config), binaryThreshold),
                                    new Rectangle(offsetX, offsetY, config.SingleCharWidth, config.SingleCharHeight));
                    offsetX += config.SingleCharWidth;
                }
            }

            return result;
        }

        protected static string ToSequential(byte[][] bitmap)
        {
            int w = bitmap[0].Length;
            int h = bitmap.Length;
            string result = string.Empty;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    result += "0x" + bitmap[y][x].ToString("X2") + ",";
                }
            }

            return result.Remove(result.Length - 1);
        }

        public static BitmapImage GetImageFromString(string hex, int charwidth, int charheight)
        {
            return ConvertImage(FromSequential(hex, charwidth, charheight));
        }

        protected static Bitmap FromSequential(string hex, int charwidth, int charheight)
        {
            string[] data = hex.Split(',');
            byte[] imagebyte = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                string val = data[i].Replace("0x", "");
                imagebyte[i] = Convert.ToByte(val, 16);
            }

            Bitmap bitmap = new(charwidth, charheight);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int bx = x / 8;
                    int bi = 7 - (x % 8);

                    bool result = (((imagebyte[y * charwidth / 8 + bx] >> bi) & 1) > 0);
                    bitmap.SetPixel(x, y, result ? Color.White : Color.Black);
                }
            }

            return bitmap;
        }

        public static List<ImageProperty> GetImageList(string text, FontAdjustConfig config, int binaryThreshold)
        {
            var result = new List<ImageProperty>();
            int offsetX = 0;
            int offsetY = 0;

            foreach (char c in text)
            {
                if (c == '\0')
                {
                    break;
                }
                if (c == '\r')
                {
                    continue;
                }
                if (c == '\n')
                {
                    offsetY += config.SingleCharHeight;
                    offsetX = 0;
                    continue;
                }

                Bitmap bm = BinarizeOtsu(DrawCharacter(c, config), binaryThreshold);

                ImageProperty prop = new()
                {
                    View = ConvertImage(bm),
                    Character = c,
                    Hex = ToSequential(BinaryImageToHexBytes(bm))
                };
                result.Add(prop);
                offsetX += config.SingleCharWidth;
            }

            return result;
        }

        //public static Bitmap GetImage(string text, string fontFamily, int fontsize, int binaryThreshold)
        //{
        //    // 文字のサイズを取得
        //    FormattedText formattedText = new(
        //        text,
        //        CultureInfo.CurrentCulture,
        //        FlowDirection.LeftToRight,
        //        new Typeface(new System.Windows.Media.FontFamily(fontFamily),
        //                        FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
        //        fontsize,
        //        System.Windows.Media.Brushes.Black, 1.00);

        //    // フォントファイルを読み込む
        //    Font font = new(fontFamily, fontsize);

        //    // 文字を描画する
        //    Bitmap bmp = new(
        //        (int)(Math.Ceiling(formattedText.Width) * 1.5),
        //        (int)(Math.Ceiling(formattedText.Height) * 1.5));
        //    Graphics g = Graphics.FromImage(bmp);
        //    g.Clear(System.Drawing.Color.Black);
        //    g.DrawString(text, font, System.Drawing.Brushes.White, new PointF(0, 0));

        //    return Binarize(bmp, binaryThreshold);
        //}

        public static Bitmap Binarize(Bitmap bitmap, int threshold)
        {
            Bitmap binarized = new(bitmap.Width, bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    System.Drawing.Color c = bitmap.GetPixel(x, y);
                    int gray = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
                    int value = gray > threshold ? 255 : 0;
                    binarized.SetPixel(x, y, System.Drawing.Color.FromArgb(value, value, value));
                }
            }

            return binarized;
        }

        public static double Variance(double[] data)
        {
            double mean = data.Sum() / data.Length;
            double sumOfSquaredDifferences = 0;

            foreach (double value in data)
            {
                double difference = value - mean;
                double squaredDifference = difference * difference;
                sumOfSquaredDifferences += squaredDifference;
            }

            double variance = sumOfSquaredDifferences / data.Length;
            return variance;
        }

        public static Bitmap BinarizeOtsu(Bitmap bitmap, int threshold)
        {
            int pixels_whole = bitmap.Width * bitmap.Height;

            int pixels_1 = 0;
            int[] thresholded_im = BinarizeSequential(bitmap, threshold);
            pixels_1 = thresholded_im.Count((val) => val == 1);

            double weight1 = pixels_1 / pixels_whole;
            double weight0 = 1 - weight1;

            if (weight1 == 0.0 || weight0 == 0.0)
            {
                return bitmap;
            }

            double[] im = BitmapSequential(bitmap);
            double[] val_pixels1 = im.Where((val, idx) => thresholded_im[idx] == 1).ToArray();
            double[] val_pixels0 = im.Where((val, idx) => thresholded_im[idx] == 0).ToArray();

            double var1 = val_pixels1.Length > 0 ? Variance(val_pixels1) : 0;
            double var0 = val_pixels0.Length > 0 ? Variance(val_pixels0) : 0;

            int criteria = (int)(weight0 * var0 + weight1 * var1);

            return Binarize(bitmap, criteria);
        }

        protected static double[] BitmapSequential(Bitmap bitmap)
        {
            double[] result = new double[bitmap.Height * bitmap.Width];

            int i = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    System.Drawing.Color c = bitmap.GetPixel(x, y);
                    result[i] = c.R * 0.299 + c.G * 0.587 + c.B * 0.114;
                    i++;
                }
            }

            return result;
        }

        protected static int[] BinarizeSequential(Bitmap bitmap, int threshold)
        {
            int[] result = new int[bitmap.Height * bitmap.Width];

            int i = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    System.Drawing.Color c = bitmap.GetPixel(x, y);
                    int gray = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
                    result[i] = gray > threshold ? 1 : 0;
                    i++;
                }
            }

            return result;
        }

        protected static byte[][] BinaryImageToHexBytes(Bitmap bitmap)
        {
            byte[][] result = new byte[bitmap.Height][];

            for (int y = 0; y < bitmap.Height; y++)
            {
                result[y] = new byte[bitmap.Width];
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
                    {
                        result[y][x] = 0x00;
                    }
                    else
                    {
                        result[y][x] = 0x01;
                    }
                }
            }

            return ToBit(result);
        }

        protected static byte[][] ToBit(byte[][] bitmap)
        {
            int w = (bitmap[0].Length / 8) + Convert.ToInt32(bitmap[0].Length % 8 > 0);
            int h = bitmap.Length;

            byte[][] result = new byte[h][];

            for (int y = 0; y < h; y++)
            {
                result[y] = new byte[w];
                for (int x = 0; x < w; x++)
                {
                    int start = x * 8;
                    int lest = bitmap[0].Length - start;
                    int max = lest < 8 ? lest : 8;

                    for (int i = 0; i < max; i++)
                    {
                        result[y][x] |= (byte)(bitmap[y][start + i] << (7 - i));
                    }
                }
            }

            return result;
        }

    }
}
