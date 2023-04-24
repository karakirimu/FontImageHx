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

            //PopulateFontList();
            viewModel = new MainViewModel();
            viewModel.PropertyChanged += OnPropertyChanged;
            DataContext = viewModel;
            
        }

        //private void PopulateFontList()
        //{
        //    var CreateItem = (string text, FontFamily fontFamily, double fontSize) =>
        //    {
        //        ComboBoxItem item = new();
        //        TextBlock textBlock = new()
        //        {
        //            Text = text,
        //            FontFamily = fontFamily,
        //            FontSize = fontSize
        //        };

        //        item.Content = textBlock;

        //        return item;
        //    };

        //    // インストールされているすべてのフォントを取得する
        //    IEnumerable<FontFamily> fonts = Fonts.SystemFontFamilies;
        //    List<FontFamily> fontfamily = new(fonts);
        //    fontfamily.Sort((f1, f2) => f1.Source.CompareTo(f2.Source));

        //    foreach (FontFamily font in fontfamily)
        //    {
        //        FontCombo.Items.Add(CreateItem(font.Source, new FontFamily(font.Source), 14.0));
        //    }
        //}

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
            //string text = viewModel.TextAreaString;

            if(text == "\r\n")
            {
                OutputArea.Source = new BitmapImage();
                return;
            }

            //OutputArea.Source = BitmapOperation.UpdateImageArea(text,
            //    viewModel.FontFamily, viewModel.FontSize, viewModel.BinaryThreshold);
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
