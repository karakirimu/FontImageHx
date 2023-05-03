using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace FontBmpGen
{
    /// <summary>
    /// BitmapEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BitmapEditWindow : Window
    {
        private readonly BitmapEditViewModel viewModel;
        public string Result { get; private set; }
        public BitmapEditWindow(char character, string hex, int width, int height)
        {
            InitializeComponent();
            viewModel = new(character, hex, width, height);
                    
            CreateCanvasGrid(width, height);
            DataContext = viewModel;

            Result = string.Empty;
        }

        //private ToggleButton CreateToggle(bool value)
        //{
        //    ToggleButton toggle = new()
        //    {
        //        Style = (Style)FindResource("ToggleButtonStyle"),
        //        IsChecked = value
        //    };
        //    return toggle;
        //}

        private static TextBlock CreateIndex(int index)
        {
            TextBlock textBlock = new()
            {
                Text = $"{index}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            return textBlock;
        }

        private static void SetPos(UIElement element, int x, int y)
        {
            Grid.SetRow(element, y);
            Grid.SetColumn(element, x);
        }

        private void SetGridIndexNum(int x, int y)
        {           
            TextBlock textBlock = CreateIndex(x > 0 ? x : y);
            BitmapGrid.Children.Add(textBlock);
            SetPos(textBlock, x, y);
        }

        //private void CreateCanvasGrid(string hex, int width, int height)
        //{
        //    var isBlack = (Color pixelColor) =>
        //    {
        //        return pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0;
        //    };

        //    Bitmap bitmap = BitmapOperation.FromSequential(hex, width, height);
        //    for (int y = 0; y <= height; y++)
        //    {
        //        BitmapGrid.RowDefinitions.Add(
        //            new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        //        for (int x = 0; x <= width; x++)
        //        {
        //            if(y == 0)
        //            {
        //                BitmapGrid.ColumnDefinitions.Add(
        //                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        //                if(x > 0)
        //                {
        //                    SetGridIndexNum(x, y);
        //                }
        //                continue;
        //            }

        //            if(x == 0)
        //            {
        //                SetGridIndexNum(x, y);
        //                continue;
        //            }
        //            Color color = bitmap.GetPixel(x-1, y-1);
        //            ToggleButton toggle = CreateToggle(!isBlack(color));
        //            BitmapGrid.Children.Add(toggle);
        //            SetPos(toggle, x, y);
        //        }
        //    }
        //}

        private void CreateCanvasGrid(int width, int height)
        {
            Style resource = (Style)FindResource("ToggleButtonStyle");
            for (int y = 0; y <= height; y++)
            {
                BitmapGrid.RowDefinitions.Add(
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                for (int x = 0; x <= width; x++)
                {
                    if (y == 0)
                    {
                        BitmapGrid.ColumnDefinitions.Add(
                            new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        if (x > 0)
                        {
                            SetGridIndexNum(x, y);
                        }
                        continue;
                    }

                    if (x == 0)
                    {
                        SetGridIndexNum(x, y);
                        continue;
                    }

                    ToggleButton toggle = viewModel.ToggleButtomMap[y - 1][x - 1];
                    toggle.Style = resource;
                    BitmapGrid.Children.Add(toggle);
                    SetPos(toggle, x, y);
                }
            }
        }

        private byte[][] ToggleToByte(ToggleButton[][] buttons)
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

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            byte[][] data = 
                BitmapOperation.ByteToBit(ToggleToByte(viewModel.ToggleButtomMap));

            Result = BitmapOperation.ToSequential(data);
            DialogResult = true;
            Close();
        }
    }
}
