using FontImageHx;
using System.Drawing;
using System.Windows.Controls.Primitives;

namespace FontImageHx
{
    internal enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    internal class BitmapCanvas
    {
        public BitmapCanvas() { }

        public static byte[][]? CreateBitmapState(ImageProperty item)
        {
            if (!(int.TryParse(item.CharWidth, out int width)
                && int.TryParse(item.CharHeight, out int height)))
            {
                return null;
            }

            var isBlack = (Color pixelColor) =>
            {
                return pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0;
            };

            Bitmap bitmap = item.ViewSource;
            byte[][] result = new byte[height][];
            for (int y = 0; y < height; y++)
            {
                result[y] = new byte[width];
                for (int x = 0; x < width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    result[y][x] = (byte)(isBlack(color) ? 0 : 1);
                }
            }

            return result;
        }

        public static ToggleButton[][] CreateToggleButtonMap(byte[][] bitmap)
        {
            ToggleButton[][] resultMap = new ToggleButton[bitmap.Length][];
            for (int y = 0; y < bitmap.Length; y++)
            {
                resultMap[y] = new ToggleButton[bitmap[y].Length];
                for (int x = 0; x < bitmap[y].Length; x++)
                {
                    resultMap[y][x] = new ToggleButton
                    {
                        IsChecked = bitmap[y][x] != 0
                    };
                }
            }

            return resultMap;
        }
    }
}
