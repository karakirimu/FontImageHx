using FontImageHx;
using System.Drawing;
using System.Windows.Controls;

namespace FontImageHxTest
{
    [TestFixture]
    public class BitmapOperationFlowUnit
    {
        [Test]
        public void ToSequential_JoinsRowsInOrder()
        {
            byte[][] bitmap =
                new byte[][]
                {
                    new byte[] { 0x12, 0x34 },
                    new byte[] { 0xAB, 0xCD }
                };

            string sequential = BitmapOperation.ToSequential(bitmap);

            Assert.That(sequential, Is.EqualTo("0x12,0x34,0xAB,0xCD"));
        }

        [Test]
        public void ToShrinkedBitmapHorizontal_RestoresRowLayout()
        {
            string sequential = "0x80,0x40,0x20,0x10";

            byte[][] result = BitmapOperation.ToShrinkedBitmapHorizontal(sequential, 8, 4);

            Assert.That(result, Has.Length.EqualTo(4));
            Assert.That(result[0], Is.EqualTo(new byte[] { 0x80 }));
            Assert.That(result[1], Is.EqualTo(new byte[] { 0x40 }));
            Assert.That(result[2], Is.EqualTo(new byte[] { 0x20 }));
            Assert.That(result[3], Is.EqualTo(new byte[] { 0x10 }));
        }

        [Test]
        public void FromSequential_RoundTripsHorizontalCompressedBitmap()
        {
            string sequential = "0xC0,0x20";

            using Bitmap bitmap = BitmapOperation.FromSequential(sequential, 3, 2);

            Assert.That(bitmap.GetPixel(0, 0).R, Is.EqualTo(Color.White.R));
            Assert.That(bitmap.GetPixel(1, 0).R, Is.EqualTo(Color.White.R));
            Assert.That(bitmap.GetPixel(2, 0).R, Is.EqualTo(Color.Black.R));
            Assert.That(bitmap.GetPixel(0, 1).R, Is.EqualTo(Color.Black.R));
            Assert.That(bitmap.GetPixel(1, 1).R, Is.EqualTo(Color.Black.R));
            Assert.That(bitmap.GetPixel(2, 1).R, Is.EqualTo(Color.White.R));
        }

        [Test]
        public void CreateImageList_MarksOnlyFirstCharacterAfterNewLine()
        {
            FontAdjustConfig config = new()
            {
                FontFamily = "Arial",
                FontSize = 12,
                SingleCharWidth = 8,
                SingleCharHeight = 8
            };

            var result = BitmapOperation.CreateImageList("A\nBC", config);

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].Character, Is.EqualTo('A'));
            Assert.That(result[0].NewLine, Is.False);
            Assert.That(result[1].Character, Is.EqualTo('B'));
            Assert.That(result[1].NewLine, Is.True);
            Assert.That(result[2].Character, Is.EqualTo('C'));
            Assert.That(result[2].NewLine, Is.False);
        }

        [Test]
        public void BinaryImageToHexBytes_HorizontalAndVerticalUseExpectedPacking()
        {
            using Bitmap bitmap = new(2, 2);
            bitmap.SetPixel(0, 0, Color.White);
            bitmap.SetPixel(1, 0, Color.Black);
            bitmap.SetPixel(0, 1, Color.Black);
            bitmap.SetPixel(1, 1, Color.White);

            byte[][] horizontal = BitmapOperation.BinaryImageToHexBytes(bitmap, Orientation.Horizontal);
            byte[][] vertical = BitmapOperation.BinaryImageToHexBytes(bitmap, Orientation.Vertical);

            Assert.That(horizontal, Has.Length.EqualTo(2));
            Assert.That(horizontal[0], Is.EqualTo(new byte[] { 0x80 }));
            Assert.That(horizontal[1], Is.EqualTo(new byte[] { 0x40 }));
            Assert.That(vertical, Has.Length.EqualTo(2));
            Assert.That(vertical[0], Is.EqualTo(new byte[] { 0x80 }));
            Assert.That(vertical[1], Is.EqualTo(new byte[] { 0x40 }));
        }
    }
}
