using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading.Tasks;

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

            ToggleButton[][] toggle
                = BitmapCanvas.CreateToggleButtonMap(vm.LastSelectedImage);

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
            if(sender is not MainViewModel)
            {
                return;
            }

            var vm = (MainViewModel)sender;

            //if (e.PropertyName.Contains("Edit"))
            //{
            //    return;
            //}

            //if(e.PropertyName == "ToggleButtonMap")
            //{
            //    return;
            //}

            //if(e.PropertyName == "AllSelected")
            //{
            //}

            //if(e.PropertyName == "ImageUpdate")
            //{
            //    if (viewModel.ConvertedImages.Contains(viewModel.SelectedImage))
            //    {
            //        int index
            //            = viewModel.ConvertedImages.IndexOf(viewModel.SelectedImage);

                    
            //    }
            //    return;
            //}

            if(e.PropertyName == "LastSelectedImage")
            {
                CreateEditControl(vm);
                return;
            }

            //if (OutputArea == null)
            //{
            //    return;
            //}        

            //if (vm.TextAreaString == "\r\n" || vm.TextAreaString == "")
            //{
            //    //OutputArea.Source = new BitmapImage();
            //    vm.ConvertedImages.Clear();
            //    return;
            //}

            //vm.ConvertedImages.Clear();

            //List<ImageProperty> p = BitmapOperation.CreateImageList(vm.TextAreaString);
            ////OutputArea.Source = BitmapOperation.ConvertImage(BitmapOperation.CombineImage(p));

            //foreach (ImageProperty item in p)
            //{
            //    vm.ConvertedImages.Add(item);
            //}
        }

        //private void CheckBox_SelectAll(object sender, RoutedEventArgs e)
        //{
        //    if(sender is not CheckBox)
        //    {
        //        return;
        //    }

        //    CheckBox chkSelectAll = (CheckBox)sender;
        //    MainViewModel vm = (MainViewModel)DataContext;

        //    if (chkSelectAll.IsChecked == true)
        //    {
        //        foreach (var i in vm.ConvertedImages)
        //        {
        //            i.IsSelected = true;
        //        }
        //    }
        //    else
        //    {
        //        foreach(var i in vm.ConvertedImages)
        //        {
        //            i.IsSelected = false;
        //        }
        //    }
        //}

        //private void DataGrid_CurrentCellChanged(object sender, System.EventArgs e)
        //{
        //    if (sender is not DataGrid)
        //    {
        //        return;
        //    }

        //    var datagrid = (DataGrid)sender;
        //    var item = (ImageProperty)datagrid.CurrentItem;
        //    var vm = (MainViewModel)datagrid.DataContext;

        //    if(vm.LastSelectedIndex < 0)
        //    {
        //        return;
        //    }

        //    vm.ConvertedImages[vm.LastSelectedIndex] = item.ShallowCopy();
        //}

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

                        if(el.Text.Length > 0)
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
