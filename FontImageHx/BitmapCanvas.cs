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

        public static ToggleButton[][]? CreateToggleButtonMap(ImageProperty item)
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
            ToggleButton[][] resultMap = new ToggleButton[height][];
            for (int y = 0; y < height; y++)
            {
                resultMap[y] = new ToggleButton[width];
                for (int x = 0; x < width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    resultMap[y][x] = new ToggleButton
                    {
                        IsChecked = !isBlack(color)
                    };
                }
            }

            return resultMap;
        }

        public static byte[][] ToggleToByte(ToggleButton[][] buttons)
        {
            byte[][] result = new byte[buttons.Length][];
            for (int y = 0; y < buttons.Length; y++)
            {
                result[y] = new byte[buttons[0].Length];
                for (int x = 0; x < buttons[0].Length; x++)
                {
                    result[y][x] = (byte)((buttons[y][x].IsChecked ?? true) ? 1 : 0);
                }
            }

            return result;
        }

        public static void Move(ToggleButton[][]? buttonMap, MoveDirection direction)
        {
            if (buttonMap == null)
            {
                return;
            }

            byte[][] moved = ToggleToByte(buttonMap);
            BitmapMatrix.Move(moved, direction);

            for (int y = 0; y < buttonMap.Length; y++)
            {
                for (int x = 0; x < buttonMap[0].Length; x++)
                {
                    buttonMap[y][x].IsChecked = moved[y][x] != 0;
                }
            }
        }
    }
}
