using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media;
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
            Update();
        }

        private void InputArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            if (OutputArea == null)
            {
                return;
            }

            string text
                = new TextRange(InputArea.Document.ContentStart,
                                InputArea.Document.ContentEnd).Text;
            viewModel.TextAreaString = text;

            if(text == "\r\n")
            {
                OutputArea.Source = new BitmapImage();
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

        private void PasteAscii_Click(object sender, RoutedEventArgs e)
        {
            // テキストを設定する
            const string ascii
            = " !\"#$%&'()*+,-./\n" +
              "0123456789:;<=>?\n" +
              "@ABCDEFGHIJKLMNO\n" +
              "PQRSTUVWXYZ[\\]^_\n" +
              "`abcdefghijklmno\n" +
              "pqrstuvwxyz{|}~";

            // FlowDocumentを作成する
            FlowDocument document = new(new Paragraph(new Run(ascii)));

            // RichTextBoxのDocumentプロパティにFlowDocumentを設定する
            InputArea.Document = document;
        }
    }
}
