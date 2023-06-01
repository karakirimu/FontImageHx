using FontImageHx;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace FontImageHx
{
    public class ImageProperty : INotifyPropertyChanged
    {
        public ImageProperty()
        {
            FontSize = "12";
            FontFamily = SystemFonts.DefaultFont.FontFamily.Name;
            FontBold = false;
            FontItalic = false;
            FontUnderline = false;
            NewLine = false;
            CharWidth = "16";
            CharHeight = "16";
            HexHorizontal = string.Empty;
            HexVertical = string.Empty;
            BinaryThreshold = 128;
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
        public string HexHorizontal { get; set; }
        public string HexVertical { get; set; }
        public string FontSize { get; set; }
        public string CharWidth { get; set; }
        public string CharHeight { get; set; }
        public string FontFamily { get; set; }
        public bool FontBold { get; set; }
        public bool FontItalic { get; set; }
        public bool FontUnderline { get; set; }
        public int BinaryThreshold { get; set; }
        public bool NewLine { get; set; }
        private bool _locked;
        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageProperty ShallowCopy()
        {
            return (ImageProperty)MemberwiseClone();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
