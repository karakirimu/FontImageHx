using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace FontBmpGen
{
    internal enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    internal class GridBitmap
    {
        public GridBitmap() { }

        public static ToggleButton[][] CreateToggleButtonMap(ImageProperty item)
        {
            var isBlack = (Color pixelColor) =>
            {
                return pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0;
            };

            Bitmap bitmap = item.ViewSource;
            ToggleButton[][] resultMap = new ToggleButton[item.CharHeight][];
            for (int y = 0; y < item.CharHeight; y++)
            {
                resultMap[y] = new ToggleButton[item.CharWidth];
                for (int x = 0; x < item.CharWidth; x++)
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

        public static void Move(ToggleButton[][] buttonMap, MoveDirection direction)
        {
            if (buttonMap == null)
            {
                return;
            }

            switch (direction)
            {
                case MoveDirection.Up:
                    for (int y = 1; y < buttonMap.Length; y++)
                    {
                        int ty = y - 1;
                        int ny = y;

                        for (int x = 0; x < buttonMap[0].Length; x++)
                        {
                            buttonMap[ty][x].IsChecked
                                = buttonMap[ny][x].IsChecked;

                            if (buttonMap.Length - 1 == ny)
                            {
                                buttonMap[ny][x].IsChecked = false;
                            }
                        }
                    }
                    break;
                case MoveDirection.Down:
                    for (int y = 1; y < buttonMap.Length; y++)
                    {
                        int ty = buttonMap.Length - y;
                        int ny = buttonMap.Length - 1 - y;

                        for (int x = 0; x < buttonMap[0].Length; x++)
                        {
                            buttonMap[ty][x].IsChecked
                                = buttonMap[ny][x].IsChecked;

                            if (0 == ny)
                            {
                                buttonMap[ny][x].IsChecked = false;
                            }
                        }
                    }
                    break;
                case MoveDirection.Left:
                    for (int y = 0; y < buttonMap.Length; y++)
                    {
                        for (int x = 1; x < buttonMap[0].Length; x++)
                        {
                            int tx = x - 1;
                            int nx = x;

                            buttonMap[y][tx].IsChecked = buttonMap[y][nx].IsChecked;

                            if (buttonMap[0].Length - 1 == nx)
                            {
                                buttonMap[y][nx].IsChecked = false;
                            }
                        }
                    }
                    break;
                case MoveDirection.Right:
                    for (int y = 0; y < buttonMap.Length; y++)
                    {
                        for (int x = 1; x < buttonMap[0].Length; x++)
                        {
                            int tx = buttonMap[0].Length - x;
                            int nx = buttonMap[0].Length - 1 - x;

                            buttonMap[y][tx].IsChecked
                            = buttonMap[y][nx].IsChecked;

                            if (0 == nx)
                            {
                                buttonMap[y][nx].IsChecked = false;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
