using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace FontBmpGen
{
    public class TextWizardViewModel : INotifyPropertyChanged
    {
        public TextWizardViewModel()
        {
            editFontFamily = SystemFonts.DefaultFont.FontFamily.Name;
            editFontSize = 12;
            editCharWidth = 16;
            editCharHeight = 16;
            textAreaString = string.Empty;

            PasteAscii = new WindowCommand((_) =>
            {
                const string ascii
                    = " !\"#$%&'()*+,-./\n" +
                      "0123456789:;<=>?\n" +
                      "@ABCDEFGHIJKLMNO\n" +
                      "PQRSTUVWXYZ[\\]^_\n" +
                      "`abcdefghijklmno\n" +
                      "pqrstuvwxyz{|}~";

                TextAreaString = ascii;
            });
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

        private string editFontFamily;
        public string EditFontFamily { get => editFontFamily; set => SetProperty(ref editFontFamily, value); }

        private int editFontSize;
        public int EditFontSize { get => editFontSize; set => SetProperty(ref editFontSize, value); }

        private bool editFontBold;
        public bool EditFontBold { get => editFontBold; set => SetProperty(ref editFontBold, value); }

        private bool editFontItalic;
        public bool EditFontItalic { get => editFontItalic; set => SetProperty(ref editFontItalic, value); }

        private bool editFontUnderline;
        public bool EditFontUnderline { get => editFontUnderline; set => SetProperty(ref editFontUnderline, value); }

        private int editCharWidth;
        public int EditCharWidth { get => editCharWidth; set => SetProperty(ref editCharWidth, value); }

        private int editCharHeight;
        public int EditCharHeight { get => editCharHeight; set => SetProperty(ref editCharHeight, value); }

        private string textAreaString;
        public string TextAreaString { get => textAreaString; set => SetProperty(ref textAreaString, value); }

        public WindowCommand PasteAscii { get; init; }
    }
}
