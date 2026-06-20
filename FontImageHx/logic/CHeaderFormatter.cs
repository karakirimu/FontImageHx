using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace FontImageHx
{
    internal static class CHeaderFormatter
    {
        public static string BuildFontHeader(
            IReadOnlyList<ImageProperty> images,
            Orientation orientation,
            string fileName)
        {
            StringBuilder builder = new();

            builder.AppendLine("//");
            builder.AppendLine($"// {Path.GetFileName(fileName)}");
            builder.AppendLine($"// Orientation: {orientation}");
            builder.AppendLine("//");
            builder.AppendLine(string.Empty);
            builder.AppendLine("static uint8_t font[] = {");

            foreach (ImageProperty image in images)
            {
                string hex = orientation == Orientation.Horizontal
                    ? image.HexHorizontal
                    : image.HexVertical;
                builder.AppendLine(
                    $"    {hex}, // {image.Character} {image.CharWidth}x{image.CharHeight}");
            }

            builder.AppendLine("};");
            return builder.ToString();
        }

        public static string BuildDefaultFileName(DateTime now)
        {
            return $"font_{now:yyyyMMddHHmmss}";
        }
    }
}
