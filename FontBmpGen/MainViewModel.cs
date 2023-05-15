using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FontBmpGen
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
        private bool? _allSelected = false;

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
        private bool _checkselectedview;
        public bool CheckSelectedView
        {
            get => _checkselectedview;
            set
            {
                if (_checkselectedview != value)
                {
                    _checkselectedview = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedfontFamily = "";
        public string SelectedFontFamily
        {
            get => _selectedfontFamily;
            set
            {
                if(_selectedfontFamily != value)
                {
                    _selectedfontFamily = value;
                    UpdateCheckedRows((i) => { i.FontFamily = value; });
                    OnPropertyChanged();
                }
            }
        }
        private string _selectedfontSize = "";
        public string SelectedFontSize
        {
            get => _selectedfontSize;
            set
            {
                if (_selectedfontSize != value)
                {
                    _selectedfontSize = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool?   _selectednewLine;
        public bool? SelectedNewLine
        {
            get => _selectednewLine;
            set
            {
                if (_selectednewLine != value)
                {
                    _selectednewLine = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool?   _selectedfontBold;
        public bool? SelectedFontBold
        {
            get => _selectedfontBold;
            set
            {
                if (_selectedfontBold != value)
                {
                    _selectedfontBold = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool?   _selectedfontItalic;
        public bool? SelectedFontItalic
        {
            get => _selectedfontItalic;
            set
            {
                if( _selectedfontItalic != value)
                {
                    _selectedfontItalic = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool?   _selectedfontUnderline;
        public bool? SelectedFontUnderline
        {
            get => _selectedfontUnderline;
            set
            {
                if( _selectedfontUnderline != value)
                {
                    _selectedfontUnderline = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _selectedcharWidth = "";
        public string SelectedCharWidth
        {
            get => _selectedcharWidth;
            set
            {
                if (_selectedcharWidth != value)
                {
                    _selectedcharWidth = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _selectedcharHeight = "";
        public string SelectedCharHeight
        {
            get => _selectedcharHeight;
            set
            {
                if( _selectedcharHeight != value)
                {
                    _selectedcharHeight = value;
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
                    if(int.TryParse(im.CharWidth, out int width)
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

                    im.PropertyChanged += OnImagePropertyChanged;
                    ConvertedImages.Add(im);
                }

                OnPropertyChanged(nameof(TextAreaString));
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
                if(LastSelectedIndex < 0)
                {
                    return;
                }

                byte[][] bitmapbyte = BitmapCanvas.ToggleToByte(ToggleButtonMap);
                string hexdata = BitmapOperation.ToSequential(
                    BitmapOperation.ByteToBit(bitmapbyte));
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
        }

        private void OnImagePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {           
            int counter = ConvertedImages.Count(i => i.IsSelected);
            if(counter == ConvertedImages.Count)
            {
                AllSelected = true;
            }
            else
            {
                AllSelected = null;
            }

            if(counter == 0)
            {
                AllSelected = false;
                CheckSelectedView = false;
                RowSelectedView = false;
                return;
            }

            var groups = ConvertedImages.Where(i => i.IsSelected).GroupBy(i => new
            {
                i.FontFamily,
                i.FontSize,
                i.FontBold,
                i.FontItalic,
                i.FontUnderline,
                i.NewLine,
                i.CharWidth,
                i.CharHeight
            }).Distinct();

            bool fontfamily = groups.GroupBy(i => i.Key.FontFamily).Distinct().Count() > 1;
            bool fontsize = groups.GroupBy(i => i.Key.FontSize).Distinct().Count() > 1;
            bool bold = groups.GroupBy(i => i.Key.FontBold).Distinct().Count() > 1;
            bool italic = groups.GroupBy(i => i.Key.FontItalic).Distinct().Count() > 1;
            bool newline = groups.GroupBy(i => i.Key.NewLine).Distinct().Count() > 1;
            bool underline = groups.GroupBy(i => i.Key.FontUnderline).Distinct().Count() > 1;
            bool charwidth = groups.GroupBy(i => i.Key.CharWidth).Distinct().Count() > 1;
            bool charheight = groups.GroupBy(i => i.Key.CharHeight).Distinct().Count() > 1;

            if (AllSelected == true || AllSelected == null)
            {
                SelectedFontFamily = fontfamily ? string.Empty : ConvertedImages[0].FontFamily;
                SelectedFontSize = fontsize ? string.Empty : ConvertedImages[0].FontSize.ToString();
                SelectedFontItalic = italic ? false : ConvertedImages[0].FontItalic;
                SelectedFontBold = bold ? false : ConvertedImages[0].FontBold;
                SelectedFontUnderline = underline ? false : ConvertedImages[0].FontUnderline;
                SelectedNewLine = newline ? null : ConvertedImages[0].NewLine;
                SelectedCharWidth = charwidth ? string.Empty : ConvertedImages[0].CharWidth.ToString();
                SelectedCharHeight = charheight ? string.Empty : ConvertedImages[0].CharHeight.ToString();

                CheckSelectedView = true;
                RowSelectedView = false;
            }
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

        private void UpdateCheckedRows(Action<ImageProperty> updateAction)
        {
            if (!CheckSelectedView)
            {
                return;
            }

            for (int i = 0; i < ConvertedImages.Count; i++)
            {
                if (ConvertedImages[i].IsSelected)
                {
                    UpdateImageProperty(updateAction);
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
                    _fontSize = value.FontSize.ToString();
                    _fontBold = value.FontBold;
                    _fontItalic = value.FontItalic;
                    _fontUnderline = value.FontUnderline;
                    _charWidth = value.CharWidth.ToString();
                    _charHeight = value.CharHeight.ToString();
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
                    CheckSelectedView = false;
                    RowSelectedView = true;
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

                    if(value != null)
                    {
                        for (int i = 0; i < ConvertedImages.Count; i++)
                        {
                            ConvertedImages[i].IsSelected = value ?? true;
                        }
                    }

                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
