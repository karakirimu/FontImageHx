using FontImageHx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FontImageHx
{
    /// <summary>
    /// TextWizard.xaml の相互作用ロジック
    /// </summary>
    public partial class TextWizard : Window
    {
        public ObservableCollection<ImageProperty> Result { get; private set; }

        public TextWizard()
        {
            InitializeComponent();
            TextWizardViewModel viewModel = new();
            viewModel.PropertyChanged += OnPropertyChanged;
            DataContext = viewModel;

            Result = new ObservableCollection<ImageProperty>();
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not TextWizardViewModel)
            {
                return;
            }

            var vm = (TextWizardViewModel)sender;

            if (OutputArea == null)
            {
                return;
            }

            if (vm.TextAreaString == "\r\n" || vm.TextAreaString == "")
            {
                DisposeResultImages();
                OutputArea.Source = new BitmapImage();
                Result = new ObservableCollection<ImageProperty>();
                return;
            }

            List<ImageProperty> p
                = BitmapOperation.CreateImageList(vm.TextAreaString, new FontAdjustConfig()
                {
                    FontFamily = vm.EditFontFamily,
                    FontSize = vm.EditFontSize,
                    Bold = vm.EditFontBold,
                    Italic = vm.EditFontItalic,
                    Underline = vm.EditFontUnderline,
                    SingleCharWidth = vm.EditCharWidth,
                    SingleCharHeight = vm.EditCharHeight
                });

            DisposeResultImages();
            using (var combinedBitmap = BitmapOperation.CombineImage(p))
            {
                OutputArea.Source = BitmapOperation.ConvertImage(combinedBitmap);
            }
            Result = new ObservableCollection<ImageProperty>(p);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            if (DialogResult != true)
            {
                DisposeResultImages();
            }

            base.OnClosed(e);
        }

        private void DisposeResultImages()
        {
            foreach (ImageProperty image in Result)
            {
                image.Dispose();
            }
        }
    }
}
