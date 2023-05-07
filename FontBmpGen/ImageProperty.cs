using System.Drawing;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace FontBmpGen
{
    public class ImageProperty
    {
        public ImageProperty()
        {
            IsSelected = false;
            FontSize = 12;
            FontFamily = SystemFonts.DefaultFont.FontFamily.Name;
            FontBold = false;
            FontItalic = false;
            FontUnderline = false;
            NewLine = false;
            CharWidth = 16;
            CharHeight = 16;
            Hex = string.Empty;
            BinaryThreshold = 128;
        }
        public ImageProperty ShallowCopy()
        {
            return (ImageProperty)MemberwiseClone();
        }

        [JsonIgnore]
        public bool IsSelected { get; set; }
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
        public int FontSize { get; set; }
        public int CharWidth { get; set; }
        public int CharHeight { get; set; }
        public string FontFamily { get; set; }
        public bool FontBold { get; set; }
        public bool FontItalic { get; set; }
        public bool FontUnderline { get; set; }
        public int BinaryThreshold { get; set; }
        public bool NewLine { get; set; }
        public bool Locked { get; set; }
    }
}
