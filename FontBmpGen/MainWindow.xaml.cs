using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FontBmpGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            viewModel.PropertyChanged += OnPropertyChanged;
            DataContext = viewModel;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutputArea == null)
            {
                return;
            }

            string text = viewModel.TextAreaString;

            if (text == "\r\n" || text == "")
            {
                OutputArea.Source = new BitmapImage();
                viewModel.ConvertedImages.Clear();
                return;
            }

            FontAdjustConfig config = new()
            {
                SingleCharWidth = viewModel.CharWidth,
                SingleCharHeight = viewModel.CharHeight,
                FontFamily = viewModel.FontFamily,
                FontSize = viewModel.FontSize,
                Bold = viewModel.FontBold,
                Italic = viewModel.FontItalic,
                Underline = viewModel.FontUnderline
            };

            OutputArea.Source = BitmapOperation.DrawTextInSpecifiedSize(
                text, config, viewModel.BinaryThreshold);

            viewModel.ConvertedImages.Clear();

            List<ImageProperty> p
                = BitmapOperation.GetImageList(text, config, viewModel.BinaryThreshold);

            foreach (ImageProperty item in p)
            {
                viewModel.ConvertedImages.Add(item);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.ConvertedImages.Contains(viewModel.SelectedImage))
            {
                int index 
                    = viewModel.ConvertedImages.IndexOf(viewModel.SelectedImage);

                BitmapEditWindow window = new(
                    viewModel.SelectedImage.Hex,
                    viewModel.CharWidth,
                    viewModel.CharHeight);

                if (window.ShowDialog() == true)
                {
                    Trace.WriteLine(window.Result);

                    ImageProperty property = new ImageProperty()
                    {
                        View = BitmapOperation.GetImageFromString(
                            window.Result, viewModel.CharWidth, viewModel.CharHeight),
                        Hex = window.Result,
                        Character = viewModel.SelectedImage.Character
                    };

                    viewModel.ConvertedImages[index] = property;
                }
            }
        }
    }
}
