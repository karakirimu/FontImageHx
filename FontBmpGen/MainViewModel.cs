using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FontBmpGen
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _textString = "";
        private ImageProperty _selectedImage;
        private ToggleButton[][]? _toggleButtonMap;
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
        private bool? _allSelected = false;

        public WindowCommand ClickUp { get; init; }
        public WindowCommand ClickLeft { get; init; }
        public WindowCommand ClickDown { get; init; }
        public WindowCommand ClickRight { get; init; }

        public ObservableCollection<ImageProperty> ConvertedImages { get; set; }
        public WindowCommand Close { get; init; }
        public WindowCommand SaveBitmap { get; init; }
        public WindowCommand SaveCHex { get; init; }
        public WindowCommand NewProfile { get; init; }
        public WindowCommand OpenProfile { get; init; }
        public WindowCommand SaveProfile { get; init; }
        //public WindowCommand PasteAscii { get; init; }
        public WindowCommand ImageUpdate { get; init; }
        public MainViewModel()
        {
            ConvertedImages = new ObservableCollection<ImageProperty>();
            _selectedImage = new ImageProperty();

            Close = new WindowCommand((_) =>
            {
                Application.Current.Shutdown();
            });

            SaveBitmap = new WindowCommand((_) =>
            {
                OpenSave.ExportToBitmap(ConvertedImages);
            });

            SaveCHex = new WindowCommand((_) =>
            {
                OpenSave.ExportToCHeader(ConvertedImages);
            });

            NewProfile = new WindowCommand((_) =>
            {
                TextWizard textWizard = new()
                {
                    Owner = Application.Current.MainWindow
                };
                if (textWizard.ShowDialog() == false)
                {
                    return;
                }

                ConvertedImages.Clear();
                foreach(var im in textWizard.Result)
                {
                    im.PropertyChanged += OnImagePropertyChanged;
                    ConvertedImages.Add(im);
                }
            });

            OpenProfile = new WindowCommand((_) =>
            {
                var profile = OpenSave.OpenProfile();

                if(profile.Count == 0)
                {
                    return;
                }

                ConvertedImages.Clear();

                foreach (var im in profile)
                {
                    im.ViewSource = BitmapOperation.FromSequential(
                        im.Hex, im.CharWidth, im.CharHeight);

                    if (im.NewLine)
                    {
                        _textString += "\n";
                    }

                    _textString += im.Character;

                    im.PropertyChanged += OnImagePropertyChanged;
                    ConvertedImages.Add(im);
                }

                OnPropertyChanged(nameof(TextAreaString));
            });

            SaveProfile = new WindowCommand((_) =>
            {
                OpenSave.SaveProfile(ConvertedImages);
            });

            //PasteAscii = new WindowCommand((_) =>
            //{
            //    const string ascii
            //        = " !\"#$%&'()*+,-./\n" +
            //          "0123456789:;<=>?\n" +
            //          "@ABCDEFGHIJKLMNO\n" +
            //          "PQRSTUVWXYZ[\\]^_\n" +
            //          "`abcdefghijklmno\n" +
            //          "pqrstuvwxyz{|}~";

            //    TextAreaString = ascii;
            //});

            ClickUp = new WindowCommand((_) =>
            {
                BitmapCanvas.Move(ToggleButtonMap, MoveDirection.Up);
            });

            ClickLeft = new WindowCommand((_) =>
            {
                BitmapCanvas.Move(ToggleButtonMap, MoveDirection.Left);
            });

            ClickDown = new WindowCommand((_) =>
            {
                BitmapCanvas.Move(ToggleButtonMap, MoveDirection.Down);
            });

            ClickRight = new WindowCommand((_) =>
            {
                BitmapCanvas.Move(ToggleButtonMap, MoveDirection.Right);
            });

            ImageUpdate = new WindowCommand((_) => 
            {
                if(LastSelectedIndex < 0)
                {
                    return;
                }

                byte[][] bitmapbyte = BitmapCanvas.ToggleToByte(ToggleButtonMap);
                string hexdata = BitmapOperation.ToSequential(
                    BitmapOperation.ByteToBit(bitmapbyte));
                ImageProperty w = ConvertedImages[LastSelectedIndex].ShallowCopy();
                w.ViewSource = BitmapOperation.FromSequential(
                        hexdata,
                        LastSelectedImage.CharWidth,
                        LastSelectedImage.CharHeight);
                w.Hex = hexdata;

                ConvertedImages[LastSelectedIndex] = w;
                LastSelectedImage = w;
            });
        }

        private void OnImagePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            int counter = 0;
            foreach (var i in ConvertedImages)
            {
                counter += i.IsSelected ? 1 : -1;
            }

            if(counter == ConvertedImages.Count)
            {
                AllSelected = true;
                return;
            }

            if(counter == -ConvertedImages.Count)
            {
                AllSelected = false;
                return;
            }

            AllSelected = null;
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

        public bool? AllSelected
        {
            get => _allSelected;
            set
            {
                if (_allSelected != value)
                {
                    _allSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
