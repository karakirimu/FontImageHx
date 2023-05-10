using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace FontBmpGen
{
    public class ImageProperty : INotifyPropertyChanged
    {
        public ImageProperty()
        {
            IsSelected = false;
            FontSize = "12";
            FontFamily = SystemFonts.DefaultFont.FontFamily.Name;
            FontBold = false;
            FontItalic = false;
            FontUnderline = false;
            NewLine = false;
            CharWidth = "16";
            CharHeight = "16";
            Hex = string.Empty;
            BinaryThreshold = 128;
        }

        [JsonIgnore]
        private bool _isSelected;
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        [JsonIgnore]
        private Bitmap _bitmap = new(1, 1);
        [JsonIgnore]
        public BitmapImage? View { get; private set; }
        [JsonIgnore]
        public Bitmap ViewSource
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                View = BitmapOperation.ConvertImage(value);
            }
        }
        public char Character { get; set; }
        public string Hex { get; set; }
        public string FontSize { get; set; }
        public string CharWidth { get; set; }
        public string CharHeight { get; set; }
        public string FontFamily { get; set; }
        public bool FontBold { get; set; }
        public bool FontItalic { get; set; }
        public bool FontUnderline { get; set; }
        public int BinaryThreshold { get; set; }
        public bool NewLine { get; set; }
        public bool Locked { get; set; }

        public ImageProperty ShallowCopy()
        {
            return (ImageProperty)MemberwiseClone();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
