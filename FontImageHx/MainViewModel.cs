using FontImageHx;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FontImageHx
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _textString = "";
        private ImageProperty _selectedImage;
        private ToggleButton[][]? _toggleButtonMap;
        private string _fontSize = "";
        private string _fontFamily = "";
        private string _hexView = "";
        private bool _newLine;
        private bool _fontBold;
        private bool _fontItalic;
        private bool _fontUnderline;
        private string _charWidth = "";
        private string _charHeight = "";
        private int _selectedIndex = -1;
        private bool _editLocked = false;

        private bool _isEditing = false;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _windowTitle = "FontImageHx";
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool _rowselectedview;
        public bool RowSelectedView
        {
            get => _rowselectedview;
            set
            {
                if (_rowselectedview != value)
                {
                    _rowselectedview = value;
                    OnPropertyChanged();
                }
            }
        }

        public WindowCommand ClickUp { get; init; }
        public WindowCommand ClickLeft { get; init; }
        public WindowCommand ClickDown { get; init; }
        public WindowCommand ClickRight { get; init; }

        public ObservableCollection<ImageProperty> ConvertedImages { get; set; }
        public WindowCommand Close { get; init; }
        public WindowCommand About { get; init; }
        public WindowCommand SaveBitmap { get; init; }
        public WindowCommand SaveCHex { get; init; }
        public WindowCommand NewProfile { get; init; }
        public WindowCommand OpenProfile { get; init; }
        public WindowCommand SaveProfile { get; init; }
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
                foreach (var im in textWizard.Result)
                {
                    im.PropertyChanged += OnImagePropertyUpdated;
                    ConvertedImages.Add(im);
                }

                WindowTitle = "Untitled - FontImageHx";
                IsEditing = true;
            });

            OpenProfile = new WindowCommand((_) =>
            {
                var profile = OpenSave.OpenProfile();

                if (profile.Item2.Count == 0)
                {
                    return;
                }

                ConvertedImages.Clear();

                foreach (var im in profile.Item2)
                {
                    if (int.TryParse(im.CharWidth, out int width)
                        && int.TryParse(im.CharHeight, out int height))
                    {
                        im.ViewSource = BitmapOperation.FromSequential(
                            im.Hex, width, height);
                    }
                    else
                    {
                        im.ViewSource = new System.Drawing.Bitmap(1, 1);
                    }

                    if (im.NewLine)
                    {
                        _textString += "\n";
                    }

                    _textString += im.Character;
                    im.PropertyChanged += OnImagePropertyUpdated;
                    ConvertedImages.Add(im);
                }

                OnPropertyChanged(nameof(TextAreaString));

                WindowTitle = $"{profile.Item1} - FontImageHx";
                IsEditing = true;
            });

            SaveProfile = new WindowCommand((_) =>
            {
                OpenSave.SaveProfile(ConvertedImages);
            });

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
                if (LastSelectedIndex < 0 || ToggleButtonMap == null)
                {
                    return;
                }

                byte[][] bitmapbyte = BitmapCanvas.ToggleToByte(ToggleButtonMap);
                string hexdata = BitmapOperation.ToSequential(
                    BitmapOperation.ByteToBitHorizontal(bitmapbyte));
                ImageProperty w = ConvertedImages[LastSelectedIndex].ShallowCopy();

                if (int.TryParse(LastSelectedImage.CharWidth, out int width)
                    && int.TryParse(LastSelectedImage.CharHeight, out int height))
                {
                    w.ViewSource = BitmapOperation.FromSequential(
                            hexdata, width, height);
                }

                w.Hex = hexdata;

                ConvertedImages[LastSelectedIndex] = w;
                LastSelectedImage = w;
            });

            About = new WindowCommand((_) =>
            {
                AboutWindow window = new()
                {
                    Owner = Application.Current.MainWindow
                };
                window.Show();
            });
        }

        public ToggleButton[][]? ToggleButtonMap
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
        public string EditFontSize
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
        public string EditCharWidth
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
        public string EditCharHeight
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

        public bool IsLocked
        {
            get => _editLocked;
            set
            {
                if (_editLocked != value)
                {
                    _editLocked = value;
                    UpdateImageProperty((i) => { i.Locked = value; });
                }
            }
        }

        public ImageProperty LastSelectedImage
        {
            get => _selectedImage;
            set
            {
                if (value == null)
                {
                    return;
                }

                if (_selectedImage != value)
                {
                    RowSelectedView = true;
                    _selectedImage = value;
                    _fontFamily = value.FontFamily;
                    _fontSize = value.FontSize.ToString();
                    _fontBold = value.FontBold;
                    _fontItalic = value.FontItalic;
                    _fontUnderline = value.FontUnderline;
                    _charWidth = value.CharWidth.ToString();
                    _charHeight = value.CharHeight.ToString();
                    _hexView = value.Hex;
                    _newLine = value.NewLine;
                    _editLocked = value.Locked;
                    OnPropertyChanged(nameof(EditFontFamily));
                    OnPropertyChanged(nameof(EditFontSize));
                    OnPropertyChanged(nameof(EditFontBold));
                    OnPropertyChanged(nameof(EditFontItalic));
                    OnPropertyChanged(nameof(EditFontUnderline));
                    OnPropertyChanged(nameof(EditCharWidth));
                    OnPropertyChanged(nameof(EditCharHeight));
                    OnPropertyChanged(nameof(EditHexView));
                    OnPropertyChanged(nameof(EditNewLine));
                    OnPropertyChanged(nameof(IsLocked));
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
                if (value < 0)
                {
                    return;
                }
                _selectedIndex = value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnImagePropertyUpdated(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ImageProperty)
            {
                return;
            }

            var im = (ImageProperty)sender;
            LastSelectedImage = im.ShallowCopy();
        }
    }
}
