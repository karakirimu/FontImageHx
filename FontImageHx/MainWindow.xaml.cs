using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FontImageHx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            viewModel.PropertyChanged += OnPropertyChanged;
            DataContext = viewModel;
        }

        private void CreateEditControl(MainViewModel vm)
        {
            TextBlock CreateIndex(int index)
            {
                TextBlock textBlock = new()
                {
                    Text = $"{index}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                return textBlock;
            }

            void SetPos(UIElement element, int x, int y)
            {
                Grid.SetRow(element, y);
                Grid.SetColumn(element, x);
            }

            void SetGridIndexNum(int x, int y)
            {
                TextBlock textBlock = CreateIndex(x > 0 ? x : y);
                BitmapGrid.Children.Add(textBlock);
                SetPos(textBlock, x, y);
            }

            BitmapGrid.RowDefinitions.Clear();
            BitmapGrid.ColumnDefinitions.Clear();
            BitmapGrid.Children.Clear();

            byte[][]? bitmap = vm.EditBitmap;
            if (bitmap == null)
            {
                return;
            }

            int height = bitmap.Length;
            int width = bitmap[0].Length;

            Style resource = (Style)FindResource("ToggleButtonStyle");
            ToggleButton[][] toggle = BitmapCanvas.CreateToggleButtonMap(bitmap);
            for (int y = 0; y <= height; y++)
            {
                BitmapGrid.RowDefinitions.Add(
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                for (int x = 0; x <= width; x++)
                {
                    if (y == 0)
                    {
                        BitmapGrid.ColumnDefinitions.Add(
                            new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        if (x > 0)
                        {
                            SetGridIndexNum(x, y);
                        }
                        continue;
                    }

                    if (x == 0)
                    {
                        SetGridIndexNum(x, y);
                        continue;
                    }

                    int bitmapX = x - 1;
                    int bitmapY = y - 1;
                    toggle[bitmapY][bitmapX].Style = resource;
                    toggle[bitmapY][bitmapX].Checked += (_, _) => vm.UpdateEditBitmapCell(bitmapX, bitmapY, true);
                    toggle[bitmapY][bitmapX].Unchecked += (_, _) => vm.UpdateEditBitmapCell(bitmapX, bitmapY, false);
                    //toggle[y - 1][x - 1]
                    ToggleButton t = toggle[bitmapY][bitmapX];
                    BitmapGrid.Children.Add(t);
                    SetPos(t, x, y);
                }
            }
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not MainViewModel)
            {
                return;
            }

            var vm = (MainViewModel)sender;

            if (e.PropertyName == "LastSelectedImage" || e.PropertyName == "EditBitmap")
            {
                CreateEditControl(vm);
                return;
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column is DataGridBoundColumn column)
                {
                    var bindingPath = ((Binding)column.Binding).Path.Path;
                    if (bindingPath == "Character")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = (TextBox)e.EditingElement;
                        var vm = (MainViewModel)DataContext;

                        if (el.Text.Length > 0)
                        {
                            var item = BitmapOperation.UpdateCharacterProperty(
                                el.Text[0], vm.ConvertedImages[rowIndex].ShallowCopy());
                            vm.ConvertedImages[rowIndex] = item;
                            CreateEditControl(vm);
                            //var datagrid = (DataGrid)sender;
                        }
                    }
                }
            }
        }
    }
}
