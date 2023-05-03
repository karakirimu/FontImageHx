using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace FontBmpGen
{
    class BitmapEditViewModel : INotifyPropertyChanged
    {
        private int _charWidth = 0;
        private int _charHeight = 0;
        private Bitmap _Bitmap;
        private ToggleButton[][] _toggleButtonMap;

        public WindowCommand ClickUp { get; init; }
        public WindowCommand ClickLeft { get; init; }
        public WindowCommand ClickDown{ get; init; }
        public WindowCommand ClickRight { get; init; }
        public WindowCommand ClickApply{ get; init; }

        private ToggleButton[][] CreateToggleButtonMap(int width, int height)
        {
            var isBlack = (Color pixelColor) =>
            {
                return pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0;
            };

            Bitmap bitmap = _Bitmap;
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

        public BitmapEditViewModel(string hex, int width, int height)
        {
            _charWidth = width;
            _charHeight = height;
            _Bitmap = BitmapOperation.FromSequential(hex, width, height);
            _toggleButtonMap = CreateToggleButtonMap(width, height);

            ClickUp = new WindowCommand((_) =>
            {
                for (int y = 1; y < ToggleButtomMap.Length; y++)
                {
                    for (int x = 0; x < ToggleButtomMap[0].Length; x++)
                    {
                        ToggleButtomMap[y - 1][x].IsChecked = ToggleButtomMap[y][x].IsChecked;

                        if (y == ToggleButtomMap.Length - 1)
                        {
                            ToggleButtomMap[y][x].IsChecked = false;
                        }
                    }
                }
            });

            ClickLeft = new WindowCommand((_) =>
            {
                for (int y = 0; y < ToggleButtomMap.Length; y++)
                {
                    for (int x = 1; x < ToggleButtomMap[0].Length; x++)
                    {
                        ToggleButtomMap[y][x - 1].IsChecked = ToggleButtomMap[y][x].IsChecked;

                        if(x == ToggleButtomMap[0].Length - 1)
                        {
                            ToggleButtomMap[y][x].IsChecked = false;
                        }
                    }
                }
            });

            ClickDown = new WindowCommand((_) =>
            {
                for (int y = 1; y < ToggleButtomMap.Length; y++)
                {
                    for (int x = 0; x < ToggleButtomMap[0].Length; x++)
                    {
                        ToggleButtomMap[ToggleButtomMap.Length - y][x].IsChecked
                        = ToggleButtomMap[ToggleButtomMap.Length - y - 1][x].IsChecked;

                        if(ToggleButtomMap.Length - y == 1)
                        {
                            ToggleButtomMap[0][x].IsChecked = false;
                        }
                    }
                }
            });

            ClickRight = new WindowCommand((_) =>
            {
                for (int y = 0; y < ToggleButtomMap.Length; y++)
                {
                    for (int x = 1; x < ToggleButtomMap[0].Length; x++)
                    {
                        ToggleButtomMap[y][ToggleButtomMap[0].Length - x].IsChecked
                        = ToggleButtomMap[y][ToggleButtomMap[0].Length - x - 1].IsChecked;

                        if (ToggleButtomMap[0].Length - x == 1)
                        {
                            ToggleButtomMap[y][0].IsChecked = false;
                        }
                    }
                }
            });

            ClickApply = new WindowCommand((_) =>
            {


            });
        }

        public int CharWidth
        {
            get => _charWidth;
            set
            {
                if (_charWidth != value)
                {
                    _charWidth = value;
                    OnPropertyChanged();
                }
            }
        }
        public int CharHeight
        {
            get => _charHeight;
            set
            {
                if (_charHeight != value)
                {
                    _charHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public ToggleButton[][] ToggleButtomMap
        {
            get => _toggleButtonMap;
            set
            {
                if (_toggleButtonMap != value)
                {
                    _toggleButtonMap = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
