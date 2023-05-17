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

    public class BitmapOperation
    {
        public BitmapOperation() { }

        public static BitmapImage ConvertImage(Bitmap source)
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

        public static Bitmap GetCharacterImage(ImageProperty property)
        {
            ImageProperty def = new();
            FontAdjustConfig config = new()
            {
                SingleCharWidth = int.TryParse(property.CharWidth, out int charwidth) ? charwidth : int.Parse(def.CharWidth),
                SingleCharHeight = int.TryParse(property.CharHeight, out int charheight) ? charheight : int.Parse(def.CharHeight),
                FontFamily = property.FontFamily,
                FontSize = int.TryParse(property.FontSize, out int fontsize) ? fontsize : int.Parse(def.FontSize),
                Bold = property.FontBold,
                Italic = property.FontItalic,
                Underline = property.FontUnderline
            };

            return DrawCharacter(property.Character, config);
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

        public static string ToSequential(byte[][] bitmap)
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

        protected static byte[][] ToShrinkedBitmap(string sequential, int width, int height)
        {
            string[] hexValues = sequential.Split(',');
            int w = width / 8 + ((width % 8 > 0) ? 1 : 0);
            int h = height;
            byte[][] result = new byte[h][];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x == 0)
                    {
                        result[y] = new byte[w];
                    }

                    result[y][x] = Convert.ToByte(hexValues[y*w + x], 16);
                }
            }

            return result;
        }


        //public static BitmapImage GetImageFromString(string hex, int charwidth, int charheight)
        //{
        //    return ConvertImage(FromSequential(hex, charwidth, charheight));
        //}

        public static Bitmap FromSequential(string hex, int charwidth, int charheight)
        {
            byte[][] image = ToShrinkedBitmap(hex, charwidth, charheight);
            byte[][] bytedata = BitToByte(image, charwidth);

            Bitmap bitmap = new(charwidth, charheight);

            for (int y = 0; y < charheight; y++)
            {
                for (int x = 0; x < charwidth; x++)
                {
                    bitmap.SetPixel(x, y, bytedata[y][x] != 0? Color.White : Color.Black);
                }
            }

            return bitmap;
        }

        public static List<ImageProperty> CreateImageList(string text)
        {
            ImageProperty prop = new();

            FontAdjustConfig config = new()
            {
                SingleCharWidth = int.Parse(prop.CharWidth),
                SingleCharHeight = int.Parse(prop.CharHeight),
                FontFamily = prop.FontFamily,
                FontSize = int.Parse(prop.FontSize),
                Bold = prop.FontBold,
                Italic = prop.FontItalic,
                Underline = prop.FontUnderline
            };
            return CreateImageList(text,config);
        }

        public static List<ImageProperty> CreateImageList(string text, FontAdjustConfig config)
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
                    if(result.Count > 0)
                    {
                        offsetY += int.Parse(result[^1].CharHeight);
                    }
                    offsetX = 0;
                    continue;
                }
                ImageProperty prop
                    = CreateCharacterProperty(c, config, (offsetX == 0 && offsetY > 0));

                result.Add(prop);
                offsetX += int.Parse(prop.CharWidth);
            }

            return result;
        }

        public static ImageProperty CreateCharacterProperty(char c, FontAdjustConfig config, bool newline)
        {
            ImageProperty prop = new()
            {
                CharWidth = config.SingleCharWidth.ToString(),
                CharHeight = config.SingleCharHeight.ToString(),
                FontFamily = config.FontFamily,
                FontSize = config.FontSize.ToString(),
                FontBold = config.Bold,
                FontItalic = config.Italic,
                FontUnderline = config.Underline
            };

            Bitmap bm = BinarizeOtsu(DrawCharacter(c, config), prop.BinaryThreshold);

            prop.ViewSource = bm;
            prop.Character = c;
            prop.Hex = ToSequential(BinaryImageToHexBytes(bm));
            prop.NewLine = newline;
            return prop;
        }

        public static ImageProperty UpdateCharacterProperty(char c, ImageProperty prop)
        {
            ImageProperty def = new();
            FontAdjustConfig config = new()
            {
                SingleCharWidth = int.TryParse(prop.CharWidth, out int charwidth) ? charwidth : int.Parse(def.CharWidth),
                SingleCharHeight = int.TryParse(prop.CharHeight, out int charheight) ? charheight: int.Parse(def.CharHeight),
                FontFamily = prop.FontFamily,
                FontSize = int.TryParse(prop.FontSize, out int fontsize) ? fontsize : int.Parse(def.FontSize),
                Bold = prop.FontBold,
                Italic = prop.FontItalic,
                Underline = prop.FontUnderline
            };

            Bitmap bm = BinarizeOtsu(DrawCharacter(c, config), prop.BinaryThreshold);
            prop.ViewSource = bm;
            prop.Character = c;
            prop.Hex = ToSequential(BinaryImageToHexBytes(bm));

            return prop;
        }

        /// <summary>
        /// Combine single character image
        /// </summary>
        /// <param name="characters">bitmapped character</param>
        /// <returns></returns>
        public static Bitmap CombineImage(IReadOnlyList<ImageProperty> characters)
        {
            int width = 0;
            int height = characters[0].ViewSource.Height;

            int tmpw = 0;
            foreach (var c in characters)
            {
                if (c.NewLine)
                {
                    height += c.ViewSource.Height;
                    width = tmpw > width ? tmpw : width;
                    tmpw = 0;
                    continue;
                }
                tmpw += c.ViewSource.Width;
            }

            width = tmpw > width ? tmpw : width;

            var result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(result);
            int offsetX = 0;
            int offsetY = 0;
            g.Clear(Color.Black);
            foreach (var c in characters)
            {
                if (c.NewLine)
                {
                    offsetY += c.ViewSource.Height;
                    offsetX = 0;
                }

                g.DrawImage(
                    c.ViewSource,
                    new Rectangle(offsetX, offsetY, c.ViewSource.Width, c.ViewSource.Height));
                offsetX += c.ViewSource.Width;
            }

            return result;
        }

        /// <summary>
        /// Simple binarization
        /// </summary>
        /// <param name="bitmap">Bitmap image</param>
        /// <param name="threshold">binarization threshold value</param>
        /// <returns>Binarized bitmap</returns>
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

        /// <summary>
        /// Basic Otsu binarization
        /// </summary>
        /// <param name="bitmap">Colored bitmap</param>
        /// <param name="threshold">Initial value</param>
        /// <returns>Binarized bitmap</returns>
        public static Bitmap BinarizeOtsu(Bitmap bitmap, int threshold)
        {
            var variance = (double[] data) => {
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
            };

            static int[] BinarizeToIntArray(Bitmap bitmap, int threshold)
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
            };

            static double[] BitmapSequential(Bitmap bitmap)
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

            int pixels_whole = bitmap.Width * bitmap.Height;

            int pixels_1 = 0;
            int[] thresholded_im = BinarizeToIntArray(bitmap, threshold);
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

            double var1 = val_pixels1.Length > 0 ? variance(val_pixels1) : 0;
            double var0 = val_pixels0.Length > 0 ? variance(val_pixels0) : 0;

            int criteria = (int)(weight0 * var0 + weight1 * var1);

            return Binarize(bitmap, criteria);
        }

        /// <summary>
        /// Convert a Bitmap image to a 2-dimensional byte array.
        /// </summary>
        /// <param name="bitmap">Bitmap image</param>
        /// <returns>2-dimensional byte array</returns>
        public static byte[][] BinaryImageToHexBytes(Bitmap bitmap)
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

            return ByteToBit(result);
        }

        /// <summary>
        /// Convert 1 byte binarized columns to 8 byte buffer.
        /// </summary>
        /// <param name="bitmap">binarized shlinked bitmap</param>
        /// <param name="width">configured width</param>
        /// <returns></returns>
        protected static byte[][] BitToByte(byte[][] bitmap, int width)
        {
            int w = width;
            int h = bitmap.Length;

            byte[][] result = new byte[h][];

            for (int y = 0; y < h; y++)
            {
                result[y] = new byte[w];
                for (int x = 0; x < w; x++)
                {
                    int start = x / 8;
                    int bit = 7 - (x % 8);

                    result[y][x] = (byte)((bitmap[y][start] >> bit) & 1);
                }
            }

            return result;
        }

        /// <summary>
        /// Shrinks a column of a binarized 8-byte buffer to 1 byte.
        /// If there is a remainder in multiples of 8, align to the left.
        /// </summary>
        /// <param name="bitmap">binarized bitmap</param>
        /// <returns></returns>
        public static byte[][] ByteToBit(byte[][] bitmap)
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
