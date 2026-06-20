using FontImageHx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace FontImageHx
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly IApplicationService _applicationService;
        private readonly IMainDialogService _dialogService;
        private readonly IMainFileService _fileService;
        private string _textString = "";
        private ImageProperty _selectedImage;
        private byte[][]? _editBitmap;
        private string _fontSize = "";
        private string _fontFamily = "";
        private string _horizontalHexView = "";
        private string _verticalHexView = "";
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
        public WindowCommand SaveCHexHorizontal { get; init; }
        public WindowCommand SaveCHexVertical { get; init; }
        public WindowCommand NewProfile { get; init; }
        public WindowCommand OpenProfile { get; init; }
        public WindowCommand SaveProfile { get; init; }
        public WindowCommand ImageUpdate { get; init; }
        public MainViewModel(
            IApplicationService applicationService,
            IMainDialogService dialogService,
            IMainFileService fileService)
        {
            _applicationService = applicationService;
            _dialogService = dialogService;
            _fileService = fileService;
            ConvertedImages = new ObservableCollection<ImageProperty>();
            _selectedImage = new ImageProperty();

            Close = new WindowCommand((_) =>
            {
                _applicationService.Shutdown();
            });

            SaveBitmap = new WindowCommand((_) =>
            {
                _fileService.ExportToBitmap(ConvertedImages);
            });

            SaveCHexHorizontal = new WindowCommand((_) =>
            {
                _fileService.ExportToCHeader(ConvertedImages, Orientation.Horizontal);
            });

            SaveCHexVertical = new WindowCommand((_) =>
            {
                _fileService.ExportToCHeader(ConvertedImages, Orientation.Vertical);
            });

            NewProfile = new WindowCommand((_) =>
            {
                IReadOnlyList<ImageProperty>? images = _dialogService.ShowNewProfileDialog();
                if (images == null)
                {
                    return;
                }

                ConvertedImages.Clear();
                foreach (var im in images)
                {
                    im.PropertyChanged += OnImagePropertyUpdated;
                    ConvertedImages.Add(im);
                }

                WindowTitle = "Untitled - FontImageHx";
                IsEditing = true;
            });

            OpenProfile = new WindowCommand((_) =>
            {
                var profile = _fileService.OpenProfile();

                if (profile.Item2.Count == 0)
                {
                    return;
                }

                ConvertedImages.Clear();

                foreach (var im in profile.Item2)
                {
                    MainImageLogic.RestoreViewSource(im);
                    im.PropertyChanged += OnImagePropertyUpdated;
                    ConvertedImages.Add(im);
                }

                _textString = MainImageLogic.BuildTextAreaString(profile.Item2);
                OnPropertyChanged(nameof(TextAreaString));

                WindowTitle = $"{profile.Item1} - FontImageHx";
                IsEditing = true;
            });

            SaveProfile = new WindowCommand((_) =>
            {
                _fileService.SaveProfile(ConvertedImages);
            });

            ClickUp = new WindowCommand((_) =>
            {
                MoveEditBitmap(MoveDirection.Up);
            });

            ClickLeft = new WindowCommand((_) =>
            {
                MoveEditBitmap(MoveDirection.Left);
            });

            ClickDown = new WindowCommand((_) =>
            {
                MoveEditBitmap(MoveDirection.Down);
            });

            ClickRight = new WindowCommand((_) =>
            {
                MoveEditBitmap(MoveDirection.Right);
            });

            ImageUpdate = new WindowCommand((_) =>
            {
                if (LastSelectedIndex < 0 || EditBitmap == null)
                {
                    return;
                }

                ImageProperty w = MainImageLogic.CreateUpdatedImage(
                    ConvertedImages[LastSelectedIndex],
                    EditBitmap);
                ConvertedImages[LastSelectedIndex] = w;
                LastSelectedImage = w;
            });

            About = new WindowCommand((_) =>
            {
                _dialogService.ShowAboutDialog();
            });
        }

        public byte[][]? EditBitmap
        {
            get => _editBitmap;
            private set
            {
                if (!ReferenceEquals(_editBitmap, value))
                {
                    _editBitmap = value;
                    OnPropertyChanged();
                }
            }
        }

        internal void UpdateEditBitmapCell(int x, int y, bool isChecked)
        {
            if (EditBitmap == null)
            {
                return;
            }

            if (y < 0 || y >= EditBitmap.Length)
            {
                return;
            }

            if (x < 0 || x >= EditBitmap[y].Length)
            {
                return;
            }

            EditBitmap[y][x] = (byte)(isChecked ? 1 : 0);
        }

        private void MoveEditBitmap(MoveDirection direction)
        {
            if (EditBitmap == null)
            {
                return;
            }

            BitmapMatrix.Move(EditBitmap, direction);
            OnPropertyChanged(nameof(EditBitmap));
        }

        private void RefreshEditBitmapFromImage(ImageProperty image)
        {
            EditBitmap = BitmapCanvas.CreateBitmapState(image);
        }

        private void UpdateImageProperty(Action<ImageProperty> updateAction)
        {
            if (LastSelectedIndex < 0)
            {
                return;
            }

            var i = MainImageLogic.UpdateCharacterImage(
                ConvertedImages[LastSelectedIndex],
                updateAction);
            ConvertedImages[LastSelectedIndex] = i;
            _selectedImage = i;
            RefreshEditBitmapFromImage(i);
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
        public string EditHorizontalHexView
        {
            get => _horizontalHexView;
            set
            {
                if (_horizontalHexView != value)
                {
                    _horizontalHexView = value;
                    UpdateImageProperty((i) => { i.HexHorizontal = value; });
                }
            }
        }

        public string EditVerticalHexView
        {
            get => _verticalHexView;
            set
            {
                if (_verticalHexView != value)
                {
                    _verticalHexView = value;
                    UpdateImageProperty((i) => { i.HexVertical = value; });
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
                    _horizontalHexView = value.HexHorizontal;
                    _verticalHexView = value.HexVertical;
                    _newLine = value.NewLine;
                    _editLocked = value.Locked;
                    RefreshEditBitmapFromImage(value);
                    OnPropertyChanged(nameof(EditFontFamily));
                    OnPropertyChanged(nameof(EditFontSize));
                    OnPropertyChanged(nameof(EditFontBold));
                    OnPropertyChanged(nameof(EditFontItalic));
                    OnPropertyChanged(nameof(EditFontUnderline));
                    OnPropertyChanged(nameof(EditCharWidth));
                    OnPropertyChanged(nameof(EditCharHeight));
                    OnPropertyChanged(nameof(EditHorizontalHexView));
                    OnPropertyChanged(nameof(EditVerticalHexView));
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
