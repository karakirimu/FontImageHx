using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Navigation;

namespace FontBmpGen
{
    public class AboutWindowViewModel : INotifyPropertyChanged
    {
        public AboutWindowViewModel()
        {
            System.Reflection.Assembly assembly
                = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi
                = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            _softwareVersion = $"FontImageHx v{fvi.ProductVersion}";
        }

        private string _softwareVersion;
        public string SoftwareVersion
        {
            get => _softwareVersion;
            set => SetProperty(ref _softwareVersion, value);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// About.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            var viewModel = new AboutWindowViewModel();
            DataContext = viewModel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("explorer", e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
