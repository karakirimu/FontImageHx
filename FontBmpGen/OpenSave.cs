using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;

namespace FontBmpGen
{
    internal class OpenSave
    {
        public OpenSave() { }

        public static (string, List<ImageProperty>) OpenProfile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Profile (*.fbp)|*.fbp|All Files (*.*)|*.*",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                List<ImageProperty>? profile
                    = JsonSerializer.Deserialize<List<ImageProperty>>(json);

                if(profile != null )
                {
                    return (Path.GetFileName(openFileDialog.FileName), profile);
                }
            }

            return (string.Empty , new List<ImageProperty>());
        }

        public static void SaveProfile(IReadOnlyList<ImageProperty> images)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Profile (*.fbp)|*.fbp|All Files (*.*)|*.*",
                DefaultExt = "fbp",
                AddExtension = true,
                FileName = $"profile_font"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string json = JsonSerializer.Serialize(images);
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

        public static void ExportToCHeader(IReadOnlyList<ImageProperty> images)
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
                sw.WriteLine("//");
                sw.WriteLine(string.Empty);
                sw.WriteLine($"static uint8_t font[] = {{");

                foreach (var im in images)
                {
                    if (im != null)
                    {
                        sw.WriteLine($"    {im.Hex}, // {im.Character} {im.CharWidth}x{im.CharHeight}");
                    }
                }

                sw.WriteLine($"}};");
                sw.Close();
            }
        }
    }
}
