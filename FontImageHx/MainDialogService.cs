using System.Collections.Generic;
using System.Windows;

namespace FontImageHx
{
    internal interface IMainDialogService
    {
        IReadOnlyList<ImageProperty>? ShowNewProfileDialog();

        void ShowAboutDialog();
    }

    internal class MainDialogService : IMainDialogService
    {
        public IReadOnlyList<ImageProperty>? ShowNewProfileDialog()
        {
            TextWizard textWizard = new()
            {
                Owner = Application.Current.MainWindow
            };
            if (textWizard.ShowDialog() == false)
            {
                return null;
            }

            return textWizard.Result;
        }

        public void ShowAboutDialog()
        {
            AboutWindow window = new()
            {
                Owner = Application.Current.MainWindow
            };
            window.Show();
        }
    }
}
