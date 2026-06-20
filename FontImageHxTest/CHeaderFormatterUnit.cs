using FontImageHx;
using System;
using System.Windows.Controls;

namespace FontImageHxTest
{
    [TestFixture]
    public class CHeaderFormatterUnit
    {
        [Test]
        public void BuildDefaultFileName_UsesTimestampFormat()
        {
            string fileName = CHeaderFormatter.BuildDefaultFileName(
                new DateTime(2026, 6, 20, 13, 45, 59));

            Assert.That(fileName, Is.EqualTo("font_20260620134559"));
        }

        [Test]
        public void BuildFontHeader_FormatsHeaderAndSelectedOrientation()
        {
            ImageProperty[] images =
            [
                new ImageProperty
                {
                    Character = 'A',
                    HexHorizontal = "0x80",
                    HexVertical = "0x40",
                    CharWidth = "1",
                    CharHeight = "1"
                }
            ];

            string header = CHeaderFormatter.BuildFontHeader(
                images,
                Orientation.Vertical,
                @"C:\temp\font.h");

            Assert.That(header, Does.Contain("// font.h"));
            Assert.That(header, Does.Contain("// Orientation: Vertical"));
            Assert.That(header, Does.Contain("0x40, // A 1x1"));
            Assert.That(header, Does.Contain("static uint8_t font[] = {"));
            Assert.That(header, Does.Contain("};"));
        }
    }
}
