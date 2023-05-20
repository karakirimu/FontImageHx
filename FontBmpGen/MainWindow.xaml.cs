using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FontBmpGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
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

            ToggleButton[][]? toggle
                = BitmapCanvas.CreateToggleButtonMap(vm.LastSelectedImage);

            if(toggle == null)
            {
                return;
            }

            if (!(int.TryParse(vm.LastSelectedImage.CharWidth, out int width)
                    && int.TryParse(vm.LastSelectedImage.CharHeight, out int height)))
            {
                return;
            }

            Style resource = (Style)FindResource("ToggleButtonStyle");
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

                    toggle[y - 1][x - 1].Style = resource;
                    //toggle[y - 1][x - 1]
                    ToggleButton t = toggle[y - 1][x - 1];
                    BitmapGrid.Children.Add(t);
                    SetPos(t, x, y);
                }
            }

            vm.ToggleButtonMap = toggle;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not MainViewModel)
            {
                return;
            }

            var vm = (MainViewModel)sender;

            if (e.PropertyName == "LastSelectedImage")
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
