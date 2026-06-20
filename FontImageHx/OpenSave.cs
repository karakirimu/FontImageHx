using FontImageHx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;

namespace FontImageHx
{
    internal interface IMainFileService
    {
        (string, IReadOnlyList<ImageProperty>) OpenProfile();

        void SaveProfile(IReadOnlyList<ImageProperty> images);

        void ExportToBitmap(IReadOnlyList<ImageProperty> images);

        void ExportToCHeader(IReadOnlyList<ImageProperty> images, Orientation orientation);
    }

    internal class OpenSave : IMainFileService
    {
        public OpenSave() { }

        public (string, IReadOnlyList<ImageProperty>) OpenProfile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Profile (*.fip)|*.fip|All Files (*.*)|*.*",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                Profile profile = ProfileSerializer.DeserializeLatestProfile(json);

                if (profile.Image.Count > 0)
                {
                    return (Path.GetFileName(openFileDialog.FileName), profile.Image);
                }
            }

            return (string.Empty, new List<ImageProperty>());
        }

        public void SaveProfile(IReadOnlyList<ImageProperty> images)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Profile (*.fip)|*.fip|All Files (*.*)|*.*",
                DefaultExt = "fip",
                AddExtension = true,
                FileName = $"profile_font"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string json = ProfileSerializer.SerializeLatestProfile(images);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        public void ExportToBitmap(IReadOnlyList<ImageProperty> images)
        {
            if (images == null || images.Count == 0)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Bitmap Files (*.bmp)|*.bmp|All Files (*.*)|*.*",
                DefaultExt = "bmp",
                AddExtension = true,
                FileName = $"fontbitmap"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using var combinedBitmap = BitmapOperation.CombineImage(images);
                combinedBitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
            }
        }

        public void ExportToCHeader(IReadOnlyList<ImageProperty> images, Orientation orientation)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "C Header (*.h)|*.h|All Files (*.*)|*.*",
                DefaultExt = "h",
                AddExtension = true,
                FileName = CHeaderFormatter.BuildDefaultFileName(DateTime.Now)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string header = CHeaderFormatter.BuildFontHeader(
                    images,
                    orientation,
                    saveFileDialog.FileName);
                File.WriteAllText(saveFileDialog.FileName, header);
            }
        }
    }
}
