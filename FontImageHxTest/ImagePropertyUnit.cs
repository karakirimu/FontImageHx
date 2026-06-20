using FontImageHx;
using System;
using System.Drawing;

namespace FontImageHxTest
{
    [TestFixture]
    public class ImagePropertyUnit
    {
        [Test]
        public void ViewSource_ReplacingBitmapDisposesPreviousBitmap()
        {
            ImageProperty image = new();
            using Bitmap firstBitmap = new(1, 1);
            image.ViewSource = firstBitmap;

            Bitmap secondBitmap = new(1, 1);
            secondBitmap.SetPixel(0, 0, Color.Black);

            image.ViewSource = secondBitmap;

            Assert.Throws<ArgumentException>(() => firstBitmap.GetPixel(0, 0));
            Assert.That(image.ViewSource.GetPixel(0, 0).R, Is.EqualTo(Color.Black.R));

            image.Dispose();
        }

        [Test]
        public void Dispose_CanBeCalledTwice()
        {
            ImageProperty image = new();

            image.Dispose();

            Assert.DoesNotThrow(() => image.Dispose());
        }
    }
}
