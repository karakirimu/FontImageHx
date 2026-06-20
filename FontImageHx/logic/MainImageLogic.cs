using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FontImageHx
{
    internal static class MainImageLogic
    {
        public static void RestoreViewSource(ImageProperty image)
        {
            if (int.TryParse(image.CharWidth, out int width)
                && int.TryParse(image.CharHeight, out int height))
            {
                image.ViewSource = BitmapOperation.FromSequential(
                    image.HexHorizontal, width, height);
            }
            else
            {
                image.ViewSource = new Bitmap(1, 1);
            }
        }

        public static string BuildTextAreaString(IEnumerable<ImageProperty> images)
        {
            StringBuilder builder = new();

            foreach (ImageProperty image in images)
            {
                if (image.NewLine)
                {
                    builder.Append('\n');
                }

                builder.Append(image.Character);
            }

            return builder.ToString();
        }

        public static ImageProperty CreateUpdatedImage(ImageProperty source, byte[][] bitmap)
        {
            ImageProperty updated = source.CloneValueProperties();
            string hexHorizontal = BitmapOperation.ToSequential(
                BitmapOperation.ByteToBitHorizontal(bitmap));
            string hexVertical = BitmapOperation.ToSequential(
                BitmapOperation.ByteToBitVertical(bitmap));

            if (int.TryParse(source.CharWidth, out int width)
                && int.TryParse(source.CharHeight, out int height))
            {
                updated.ViewSource = BitmapOperation.FromSequential(
                    hexHorizontal, width, height);
            }

            updated.HexHorizontal = hexHorizontal;
            updated.HexVertical = hexVertical;
            return updated;
        }

        public static ImageProperty UpdateCharacterImage(
            ImageProperty source,
            Action<ImageProperty> updateAction)
        {
            ImageProperty updated = source.CloneValueProperties();
            updateAction(updated);
            updated.ViewSource = BitmapOperation.GetCharacterImage(updated);
            return updated;
        }
    }
}
