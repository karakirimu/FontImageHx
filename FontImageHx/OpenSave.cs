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
                Profile profile
                    = VersionConvert.GetLatestProfile(json);

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
                Profile profile = new(images);
                string json = JsonSerializer.Serialize(profile);
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
                FileName = $"font_{DateTime.Now:yyyyMMddHHmmss}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using StreamWriter sw = new(saveFileDialog.FileName);
                sw.WriteLine("//");
                sw.WriteLine($"// {Path.GetFileName(saveFileDialog.FileName)}");
                sw.WriteLine($"// Orientation: {orientation}");
                sw.WriteLine("//");
                sw.WriteLine(string.Empty);
                sw.WriteLine($"static uint8_t font[] = {{");

                foreach (var im in images)
                {
                    if (im != null)
                    {
                        string hex = orientation == Orientation.Horizontal ? im.HexHorizontal : im.HexVertical;
                        sw.WriteLine($"    {hex}, // {im.Character} {im.CharWidth}x{im.CharHeight}");
                    }
                }

                sw.WriteLine($"}};");
                sw.Close();
            }
        }
    }
}
