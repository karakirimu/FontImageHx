using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FontBmpGen
{
    internal class imageinfo
    {
        public char? Character { get; init; }
        public string? Hex { get; init; }
    }

    internal class fontprofile
    {
        public int fontsize { get; init; }
        public int charwidth { get; init; }
        public int charheight { get; init; }
        public string? fontfamily { get; init; }
        public bool fontbold { get; init; }
        public bool fontitalic { get; init; }
        public bool fontunderline { get; init; }
        public int binarythreshold { get; init; }
        public string? text { get; init; }
        public List<imageinfo>? image {get; init;}
    }

    internal class MainViewModel : INotifyPropertyChanged
    {
        private int _fontSize = 12;
        private string _fontFamily = "Arial";
        private bool _fontBold;
        private bool _fontItalic;
        private bool _fontUnderline;
        private int _BinaryThreshold = 128;
        private string _textString = "";
        private int _charWidth = 16;
        private int _charHeight = 16;

        public ObservableCollection<ImageProperty> ConvertedImages { get; set; }
        public ObservableCollection<ComboBoxItem> FontFamilyList { get; init; }
        public MainWindowCommand Close { get; init; }
        public MainWindowCommand SaveBitmap { get; init; }
        public MainWindowCommand SaveCHex { get; init; }
        public MainWindowCommand OpenProfile { get; init; }
        public MainWindowCommand SaveProfile { get; init; }
        public MainViewModel()
        {
            Close = new MainWindowCommand(
                        (_) => { System.Windows.Application.Current.Shutdown(); });
            SaveBitmap = new MainWindowCommand((_) =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Bitmap Files (*.bmp)|*.bmp|All Files (*.*)|*.*",
                    DefaultExt = "bmp",
                    AddExtension = true,
                    FileName = $"font_{FontFamily}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    FontAdjustConfig config = new()
                    {
                        SingleCharWidth = CharWidth,
                        SingleCharHeight = CharHeight,
                        FontFamily = FontFamily,
                        FontSize = FontSize,
                        Bold = FontBold,
                        Italic = FontItalic,
                        Underline = FontUnderline
                    };

                    BitmapOperation.GetImage(TextAreaString, config, BinaryThreshold)
                        .Save(filePath, ImageFormat.Bmp);
                }
            });

            ConvertedImages = new ObservableCollection<ImageProperty>();

            SaveCHex = new MainWindowCommand((_) =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "C Header (*.h)|*.h|All Files (*.*)|*.*",
                    DefaultExt = "h",
                    AddExtension = true,
                    FileName = $"font_{FontFamily}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using StreamWriter sw = new(saveFileDialog.FileName);

                    sw.WriteLine($"//");
                    sw.WriteLine($"// {FontFamily}");
                    sw.WriteLine($"//");
                    sw.WriteLine($"// Width: {CharWidth} Height: {CharHeight}");
                    sw.WriteLine($"// FontSize: {FontSize} Bold: {FontBold} Italic: {FontItalic} Underline: {FontUnderline}");
                    sw.WriteLine("");
                    sw.WriteLine($"static uint8_t font_{FontFamily}[] = {{");

                    foreach (var im in ConvertedImages)
                    {
                        if (im != null)
                        {
                            sw.WriteLine($"    {im.Hex}, // {im.Character}");
                        }
                    }

                    sw.WriteLine($"}};\n");
                    sw.Close();
                }
            });

            OpenProfile = new MainWindowCommand((_) =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Profile (*.fbp)|*.fbp|All Files (*.*)|*.*",
                };

                if( openFileDialog.ShowDialog() == true)
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    fontprofile? profile = JsonSerializer.Deserialize<fontprofile>(json);
                    FontSize = profile.fontsize;
                    FontBold = profile.fontbold;
                    FontItalic = profile.fontitalic;
                    FontUnderline = profile.fontunderline;
                    FontFamily = profile.fontfamily;
                    CharWidth = profile.charwidth;
                    CharHeight = profile.charheight;
                    BinaryThreshold = profile.binarythreshold;
                    TextAreaString = profile.text;

                    ConvertedImages.Clear();

                    foreach(var imageinfo in profile.image)
                    {
                        ConvertedImages.Add(new ImageProperty()
                        {
                            View = BitmapOperation.GetImageFromString(imageinfo.Hex, profile.charwidth, profile.charheight),
                            Character = imageinfo.Character,
                            Hex = imageinfo.Hex
                        });
                    }

                }
            });

            SaveProfile = new MainWindowCommand((_) =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Profile (*.fbp)|*.fbp|All Files (*.*)|*.*",
                    DefaultExt = "fbp",
                    AddExtension = true,
                    FileName = $"profile_{FontFamily}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    List<imageinfo> images = new();

                    foreach (var im in ConvertedImages)
                    {
                        images.Add(new imageinfo()
                        {
                            Character = im.Character,
                            Hex = im.Hex,
                        });
                    }

                    fontprofile profile = new()
                    {
                        fontsize = _fontSize,
                        fontbold = _fontBold,
                        fontitalic = _fontItalic,
                        fontunderline = _fontUnderline,
                        fontfamily = _fontFamily,
                        charwidth = _charWidth,
                        charheight = _charHeight,
                        binarythreshold = _BinaryThreshold,
                        text = _textString,
                        image = images
                    };

                    string json = JsonSerializer.Serialize(profile);
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
            });
        }

        public int FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool FontBold
        {
            get => _fontBold;
            set
            {
                if (_fontBold != value)
                {
                    _fontBold = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool FontItalic
        {
            get => _fontItalic;
            set
            {
                if (_fontItalic != value)
                {
                    _fontItalic = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool FontUnderline
        {
            get => _fontUnderline;
            set
            {
                if (_fontUnderline != value)
                {
                    _fontUnderline = value;
                    OnPropertyChanged();
                }
            }
        }

        public int BinaryThreshold
        {
            get => _BinaryThreshold;
            set
            {
                if (_BinaryThreshold != value)
                {
                    _BinaryThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TextAreaString
        {
            get => _textString;
            set
            {
                _textString = value;
                //OnPropertyChanged();
            }
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
                if (_charHeight!= value)
                {
                    _charHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
