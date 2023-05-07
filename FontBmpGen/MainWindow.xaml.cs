using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace FontBmpGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            viewModel.PropertyChanged += OnPropertyChanged;
            DataContext = viewModel;
        }

        private void CreateEditControl(MainViewModel vm)
        {
            TextBlock CreateIndex(int index)
            {
                TextBlock textBlock = new()
                {
                    Text = $"{index}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                return textBlock;
            }

            void SetPos(UIElement element, int x, int y)
            {
                Grid.SetRow(element, y);
                Grid.SetColumn(element, x);
            }

            void SetGridIndexNum(int x, int y)
            {
                TextBlock textBlock = CreateIndex(x > 0 ? x : y);
                BitmapGrid.Children.Add(textBlock);
                SetPos(textBlock, x, y);
            }

            BitmapGrid.RowDefinitions.Clear();
            BitmapGrid.ColumnDefinitions.Clear();
            BitmapGrid.Children.Clear();

            var toggle = GridBitmap.CreateToggleButtonMap(vm.LastSelectedImage);

            Style resource = (Style)FindResource("ToggleButtonStyle");
            for (int y = 0; y <= vm.LastSelectedImage.CharHeight; y++)
            {
                BitmapGrid.RowDefinitions.Add(
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                for (int x = 0; x <= vm.LastSelectedImage.CharWidth; x++)
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

                    toggle[y - 1][x - 1].Style = resource;
                    //toggle[y - 1][x - 1]
                    ToggleButton t = toggle[y - 1][x - 1];
                    BitmapGrid.Children.Add(t);
                    SetPos(t, x, y);
                }
            }

            vm.ToggleButtonMap = toggle;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(sender is not MainViewModel)
            {
                return;
            }

            var vm = (MainViewModel)sender;

            if (e.PropertyName.Contains("Edit"))
            {
                return;
            }

            if(e.PropertyName == "ToggleButtonMap")
            {
                return;
            }

            //if(e.PropertyName == "ImageUpdate")
            //{
            //    if (viewModel.ConvertedImages.Contains(viewModel.SelectedImage))
            //    {
            //        int index
            //            = viewModel.ConvertedImages.IndexOf(viewModel.SelectedImage);

                    
            //    }
            //    return;
            //}

            if(e.PropertyName == "LastSelectedImage")
            {
                CreateEditControl(vm);
                return;
            }

            if (OutputArea == null)
            {
                return;
            }        

            if (vm.TextAreaString == "\r\n" || vm.TextAreaString == "")
            {
                OutputArea.Source = new BitmapImage();
                vm.ConvertedImages.Clear();
                return;
            }

            vm.ConvertedImages.Clear();

            List<ImageProperty> p = BitmapOperation.CreateImageList(vm.TextAreaString);
            OutputArea.Source = BitmapOperation.ConvertImage(BitmapOperation.CombineImage(p));

            foreach (ImageProperty item in p)
            {
                vm.ConvertedImages.Add(item);
            }
        }

        private void CheckBox_SelectAll(object sender, RoutedEventArgs e)
        {

        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (viewModel.ConvertedImages.Contains(viewModel.SelectedImage))
        //    {
        //        int index 
        //            = viewModel.ConvertedImages.IndexOf(viewModel.SelectedImage);

        //        BitmapEditWindow window = new(
        //            viewModel.SelectedImage.Character,
        //            viewModel.SelectedImage.Hex,
        //            viewModel.SelectedImage.CharWidth,
        //            viewModel.SelectedImage.CharHeight);

        //        if (window.ShowDialog() == true)
        //        {
        //            Trace.WriteLine(window.Result);

        //            ImageProperty prop = viewModel.ConvertedImages[index];
        //            prop.ViewSource = BitmapOperation.FromSequential(
        //                    window.Result,
        //                    viewModel.SelectedImage.CharWidth,
        //                    viewModel.SelectedImage.CharHeight);
        //            prop.Hex = window.Result;

        //            viewModel.ConvertedImages[index] = prop;

        //            IEnumerable<ImageProperty> obsCollection = viewModel.ConvertedImages;
        //            var list = new List<ImageProperty>(obsCollection);
        //            OutputArea.Source = BitmapOperation.ConvertImage(BitmapOperation.CombineImage(list));
        //        }
        //    }
        //}
    }
}
