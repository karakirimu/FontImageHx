using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Controls.Primitives;

namespace FontBmpGen
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _textString = "";
        private ImageProperty _selectedImage;
        private ToggleButton[][] _toggleButtonMap;
        private int _fontSize = 12;
        private string _fontFamily = "";
        private string _hexView = "";
        private bool _newLine;
        private bool _fontBold;
        private bool _fontItalic;
        private bool _fontUnderline;
        private int _charWidth = 16;
        private int _charHeight = 16;
        private int _selectedIndex = -1;

        public WindowCommand ClickUp { get; init; }
        public WindowCommand ClickLeft { get; init; }
        public WindowCommand ClickDown { get; init; }
        public WindowCommand ClickRight { get; init; }
        public WindowCommand ClickApply { get; init; }



        public ObservableCollection<ImageProperty> ConvertedImages { get; set; }
        public WindowCommand Close { get; init; }
        public WindowCommand SaveBitmap { get; init; }
        public WindowCommand SaveCHex { get; init; }
        public WindowCommand OpenProfile { get; init; }
        public WindowCommand SaveProfile { get; init; }
        public WindowCommand PasteAscii { get; init; }
        public WindowCommand ImageUpdate { get; init; }
        public MainViewModel()
        {
            Close = new WindowCommand(
                        (_) => { System.Windows.Application.Current.Shutdown(); });
            SaveBitmap = new WindowCommand((_) =>
            {
                if (ConvertedImages == null)
                {
                    return;
                }

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Bitmap Files (*.bmp)|*.bmp|All Files (*.*)|*.*",
                    DefaultExt = "bmp",
                    AddExtension = true,
                    FileName = $"fontbitmap"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    BitmapOperation.CombineImage(ConvertedImages)
                    .Save(saveFileDialog.FileName, ImageFormat.Bmp);
                }
            });

            ConvertedImages = new ObservableCollection<ImageProperty>();

            SaveCHex = new WindowCommand((_) =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "C Header (*.h)|*.h|All Files (*.*)|*.*",
                    DefaultExt = "h",
                    AddExtension = true,
                    FileName = $"font_{DateTime.Now:yyyyMMddHHmmss}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using StreamWriter sw = new(saveFileDialog.FileName);
                    sw.WriteLine("//");
                    sw.WriteLine($"// {System.IO.Path.GetFileName(saveFileDialog.FileName)}");
                    sw.WriteLine("//");
                    sw.WriteLine(string.Empty);
                    sw.WriteLine($"static uint8_t font[] = {{");

                    foreach (var im in ConvertedImages)
                    {
                        if (im != null)
                        {
                            sw.WriteLine($"    {im.Hex}, // {im.Character} {im.CharWidth}x{im.CharHeight}");
                        }
                    }

                    sw.WriteLine($"}};");
                    sw.Close();
                }
            });

            OpenProfile = new WindowCommand((_) =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Profile (*.fbp)|*.fbp|All Files (*.*)|*.*",
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    List<ImageProperty>? profile
                        = JsonSerializer.Deserialize<List<ImageProperty>>(json);

                    ConvertedImages.Clear();
                    if (profile == null)
                    {
                        return;
                    }

                    foreach (var imageinfo in profile)
                    {
                        imageinfo.ViewSource = BitmapOperation.FromSequential(
                            imageinfo.Hex, imageinfo.CharWidth, imageinfo.CharHeight);

                        if (imageinfo.NewLine)
                        {
                            _textString += "\n";
                        }

                        _textString += imageinfo.Character;

                        ConvertedImages.Add(imageinfo);
                    }

                    OnPropertyChanged(nameof(TextAreaString));
                }
            });

            SaveProfile = new WindowCommand((_) =>
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Profile (*.fbp)|*.fbp|All Files (*.*)|*.*",
                    DefaultExt = "fbp",
                    AddExtension = true,
                    FileName = $"profile_font"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string json = JsonSerializer.Serialize(ConvertedImages);
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
            });

            PasteAscii = new WindowCommand((_) =>
            {
                const string ascii
                    = " !\"#$%&'()*+,-./\n" +
                      "0123456789:;<=>?\n" +
                      "@ABCDEFGHIJKLMNO\n" +
                      "PQRSTUVWXYZ[\\]^_\n" +
                      "`abcdefghijklmno\n" +
                      "pqrstuvwxyz{|}~";

                TextAreaString = ascii;
            });

            ClickUp = new WindowCommand((_) =>
            {
                GridBitmap.Move(ToggleButtonMap, MoveDirection.Up);
            });

            ClickLeft = new WindowCommand((_) =>
            {
                GridBitmap.Move(ToggleButtonMap, MoveDirection.Left);
            });

            ClickDown = new WindowCommand((_) =>
            {
                GridBitmap.Move(ToggleButtonMap, MoveDirection.Down);
            });

            ClickRight = new WindowCommand((_) =>
            {
                GridBitmap.Move(ToggleButtonMap, MoveDirection.Right);
            });

            ImageUpdate = new WindowCommand((_) => 
            {
                if (ConvertedImages.Contains(LastSelectedImage))
                {
                    byte[][] bitmapbyte = GridBitmap.ToggleToByte(ToggleButtonMap);
                    string hexdata = BitmapOperation.ToSequential(
                        BitmapOperation.ByteToBit(bitmapbyte));
                    int index = ConvertedImages.IndexOf(LastSelectedImage);

                    //ImageProperty wrap = new()
                    //{
                    //    ViewSource = BitmapOperation.FromSequential(
                    //        hexdata,
                    //        SelectedImage.CharWidth,
                    //        SelectedImage.CharHeight),
                    //    Character = SelectedImage.Character,
                    //    IsSelected = SelectedImage.IsSelected,
                    //    FontSize = SelectedImage.FontSize,
                    //    FontFamily = SelectedImage.FontFamily,
                    //    FontBold = SelectedImage.FontBold,
                    //    FontItalic = SelectedImage.FontItalic,
                    //    FontUnderline = SelectedImage.FontUnderline,
                    //    NewLine = SelectedImage.NewLine,
                    //    CharWidth = SelectedImage.CharWidth,
                    //    CharHeight = SelectedImage.CharHeight,
                    //    Hex = hexdata,
                    //    BinaryThreshold = SelectedImage.BinaryThreshold
                    //};

                    ImageProperty w = ConvertedImages[index].ShallowCopy();
                    w.ViewSource = BitmapOperation.FromSequential(
                            hexdata,
                            LastSelectedImage.CharWidth,
                            LastSelectedImage.CharHeight);
                    w.Hex = hexdata;

                    ConvertedImages[index] = w;
                    LastSelectedImage = w;
                    //OnPropertyChanged(nameof(ImageUpdate));
                }

            });
        }

        public ToggleButton[][] ToggleButtonMap
        {
            get => _toggleButtonMap;
            set
            {
                if (_toggleButtonMap != value)
                {
                    _toggleButtonMap = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateImageProperty(Action<ImageProperty> updateAction)
        {
            if (LastSelectedIndex < 0)
            {
                return;
            }

            var i = ConvertedImages[LastSelectedIndex].ShallowCopy();
            updateAction(i);
            i.ViewSource = BitmapOperation.GetCharacterImage(i);
            ConvertedImages[LastSelectedIndex] = i;
            _selectedImage = i;
            OnPropertyChanged(nameof(LastSelectedImage));
        }

        public string EditFontFamily
        {
            get => _fontFamily;
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    UpdateImageProperty((i) => { i.FontFamily = value; });
                }
            }
        }

        public int EditFontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    UpdateImageProperty((i) => { i.FontSize = value; });
                }
            }
        }

        public bool EditNewLine
        {
            get => _newLine;
            set
            {
                if (_newLine != value)
                {
                    _newLine = value;
                    UpdateImageProperty((i) => { i.NewLine = value; });
                }
            }
        }

        public bool EditFontBold
        {
            get => _fontBold;
            set
            {
                if (_fontBold != value)
                {
                    _fontBold = value;
                    UpdateImageProperty((i) => { i.FontBold = value; });
                }
            }
        }

        public bool EditFontItalic
        {
            get => _fontItalic;
            set
            {
                if (_fontItalic != value)
                {
                    _fontItalic = value;
                    UpdateImageProperty((i) => { i.FontItalic = value; });
                }
            }
        }

        public bool EditFontUnderline
        {
            get => _fontUnderline;
            set
            {
                if (_fontUnderline != value)
                {
                    _fontUnderline = value;
                    UpdateImageProperty((i) => { i.FontUnderline = value; });
                }
            }
        }

        public int EditCharWidth
        {
            get => _charWidth;
            set
            {
                if (_charWidth != value)
                {
                    _charWidth = value;
                    UpdateImageProperty((i) => { i.CharWidth = value; });
                }
            }
        }
        public int EditCharHeight
        {
            get => _charHeight;
            set
            {
                if (_charHeight != value)
                {
                    _charHeight = value;
                    UpdateImageProperty((i) => { i.CharHeight = value; });
                }
            }
        }

        public string EditHexView
        {
            get => _hexView;
            set
            {
                if (_hexView != value)
                {
                    _hexView = value;
                    UpdateImageProperty((i) => { i.Hex = value; });
                }
            }
        }

        public string TextAreaString
        {
            get => _textString;
            set
            {
                _textString = value;
                OnPropertyChanged();
            }
        }

        public ImageProperty LastSelectedImage
        {
            get => _selectedImage;
            set
            {
                if(value == null)
                {
                    return;
                }

                if (_selectedImage != value)
                {
                    _selectedImage = value;
                    _fontFamily = value.FontFamily;
                    _fontSize = value.FontSize;
                    _fontBold = value.FontBold;
                    _fontItalic = value.FontItalic;
                    _fontUnderline = value.FontUnderline;
                    _charWidth = value.CharWidth;
                    _charHeight = value.CharHeight;
                    _hexView = value.Hex;
                    _newLine = value.NewLine;
                    OnPropertyChanged(nameof(EditFontFamily));
                    OnPropertyChanged(nameof(EditFontSize));
                    OnPropertyChanged(nameof(EditFontBold));
                    OnPropertyChanged(nameof(EditFontItalic));
                    OnPropertyChanged(nameof(EditFontUnderline));
                    OnPropertyChanged(nameof(EditCharWidth));
                    OnPropertyChanged(nameof(EditCharHeight));
                    OnPropertyChanged(nameof(EditHexView));
                    OnPropertyChanged(nameof(EditNewLine));
                    OnPropertyChanged();
                    return;
                }
            }
        }

        public int LastSelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if(value < 0)
                {
                    return;
                }
                _selectedIndex = value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
