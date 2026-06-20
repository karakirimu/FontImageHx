using FontImageHx;
using System.Drawing;

namespace FontImageHxTest
{
    [TestFixture]
    public class MainImageLogicUnit
    {
        [Test]
        public void BuildTextAreaString_InsertsNewLineBeforeMarkedCharacter()
        {
            ImageProperty[] images =
            [
                new ImageProperty { Character = 'A', NewLine = false },
                new ImageProperty { Character = 'B', NewLine = true },
                new ImageProperty { Character = 'C', NewLine = false }
            ];

            string text = MainImageLogic.BuildTextAreaString(images);

            Assert.That(text, Is.EqualTo("A\nBC"));
        }

        [Test]
        public void RestoreViewSource_UsesFallbackBitmapWhenSizeIsInvalid()
        {
            ImageProperty image = new()
            {
                CharWidth = "x",
                CharHeight = "y",
                HexHorizontal = "0x80"
            };

            MainImageLogic.RestoreViewSource(image);

            Assert.That(image.ViewSource.Width, Is.EqualTo(1));
            Assert.That(image.ViewSource.Height, Is.EqualTo(1));
        }

        [Test]
        public void RestoreViewSource_RecreatesBitmapFromHorizontalHex()
        {
            ImageProperty image = new()
            {
                CharWidth = "2",
                CharHeight = "1",
                HexHorizontal = "0x80"
            };

            MainImageLogic.RestoreViewSource(image);

            Assert.That(image.ViewSource.GetPixel(0, 0).R, Is.EqualTo(Color.White.R));
            Assert.That(image.ViewSource.GetPixel(1, 0).R, Is.EqualTo(Color.Black.R));
        }

        [Test]
        public void CreateUpdatedImage_RebuildsHexAndBitmapWithoutMutatingSource()
        {
            ImageProperty source = new()
            {
                CharWidth = "2",
                CharHeight = "2",
                HexHorizontal = "0x00,0x00",
                HexVertical = "0x00,0x00"
            };
            byte[][] bitmap =
            [
                [1, 0],
                [0, 1]
            ];

            ImageProperty updated = MainImageLogic.CreateUpdatedImage(source, bitmap);

            Assert.Multiple(() =>
            {
                Assert.That(source.HexHorizontal, Is.EqualTo("0x00,0x00"));
                Assert.That(updated.HexHorizontal, Is.EqualTo("0x80,0x40"));
                Assert.That(updated.HexVertical, Is.EqualTo("0x80,0x40"));
                Assert.That(updated.ViewSource.GetPixel(0, 0).R, Is.EqualTo(Color.White.R));
                Assert.That(updated.ViewSource.GetPixel(1, 0).R, Is.EqualTo(Color.Black.R));
                Assert.That(updated.ViewSource.GetPixel(0, 1).R, Is.EqualTo(Color.Black.R));
                Assert.That(updated.ViewSource.GetPixel(1, 1).R, Is.EqualTo(Color.White.R));
            });
        }

        [Test]
        public void UpdateCharacterImage_ReturnsUpdatedCloneWithRegeneratedBitmap()
        {
            ImageProperty source = new()
            {
                Character = 'A',
                FontFamily = "Arial",
                FontSize = "12",
                CharWidth = "8",
                CharHeight = "8",
                HexHorizontal = "old"
            };

            ImageProperty updated = MainImageLogic.UpdateCharacterImage(
                source,
                static image =>
                {
                    image.Character = 'B';
                    image.FontBold = true;
                });

            Assert.Multiple(() =>
            {
                Assert.That(updated, Is.Not.SameAs(source));
                Assert.That(source.Character, Is.EqualTo('A'));
                Assert.That(source.FontBold, Is.False);
                Assert.That(updated.Character, Is.EqualTo('B'));
                Assert.That(updated.FontBold, Is.True);
                Assert.That(updated.ViewSource.Width, Is.EqualTo(8));
                Assert.That(updated.ViewSource.Height, Is.EqualTo(8));
            });
        }

        [Test]
        public void CreateUpdatedImage_DoesNotShareBitmapOwnershipWithSource()
        {
            ImageProperty source = new()
            {
                CharWidth = "2",
                CharHeight = "1",
                HexHorizontal = "0x80",
                HexVertical = "0x80"
            };
            MainImageLogic.RestoreViewSource(source);
            byte[][] bitmap =
            [
                [0, 1]
            ];

            ImageProperty updated = MainImageLogic.CreateUpdatedImage(source, bitmap);
            source.Dispose();

            Assert.That(updated.ViewSource.GetPixel(1, 0).R, Is.EqualTo(Color.White.R));
        }
    }
}
