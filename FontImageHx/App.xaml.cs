using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FontImageHx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IApplicationService, ApplicationService>();
            services.AddSingleton<IMainDialogService, MainDialogService>();
            services.AddSingleton<IMainFileService, OpenSave>();
            services.AddTransient<MainViewModel>();
            services.AddTransient(static serviceProvider =>
                new MainWindow(serviceProvider.GetRequiredService<MainViewModel>()));
        }
    }
}
