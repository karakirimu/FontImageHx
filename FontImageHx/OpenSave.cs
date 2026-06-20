using FontImageHx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace FontImageHx
{
    internal class OpenSave
    {
        public OpenSave() { }

        public static (string, IReadOnlyList<ImageProperty>) OpenProfile()
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

        public static void SaveProfile(IReadOnlyList<ImageProperty> images)
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

        public static void ExportToBitmap(IReadOnlyList<ImageProperty> images)
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
                BitmapOperation.CombineImage(images)
                .Save(saveFileDialog.FileName, ImageFormat.Bmp);
            }
        }

        public static void ExportToCHeader(IReadOnlyList<ImageProperty> images, Orientation orientation)
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
