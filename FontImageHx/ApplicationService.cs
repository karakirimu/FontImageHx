using System.Windows;

namespace FontImageHx
{
    internal interface IApplicationService
    {
        void Shutdown();
    }

    internal class ApplicationService : IApplicationService
    {
        public void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}
