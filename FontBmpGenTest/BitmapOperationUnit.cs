using FontBmpGen;

namespace FontBmpGenTest
{
    [TestFixture]
    public class BitmapOperationUnit : BitmapOperation
    {

        [SetUp]
        public void Setup()
        {
        }

        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 },0x01)]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 1, 0 },0x02)]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0 },0x04)]
        [TestCase(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 },0x08)]
        [TestCase(new byte[] { 0, 0, 0, 1, 0, 0, 0, 0 },0x10)]
        [TestCase(new byte[] { 0, 0, 1, 0, 0, 0, 0, 0 },0x20)]
        [TestCase(new byte[] { 0, 1, 0, 0, 0, 0, 0, 0 },0x40)]
        [TestCase(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 },0x80)]
        public void ToBit8(byte[] test, byte expected)
        {
            byte[][] target = new byte[][]{test};
            var converted = ToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, new byte[] { 0x00, 0x01 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 }, new byte[] { 0x00, 0x02 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, new byte[] { 0x00, 0x04 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 }, new byte[] { 0x00, 0x08 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 }, new byte[] { 0x00, 0x10 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }, new byte[] { 0x00, 0x20 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x00, 0x40 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x00, 0x80 })]
        [TestCase(new byte[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x01, 0x00 })]
        [TestCase(new byte[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x02, 0x00 })]
        [TestCase(new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x04, 0x00 })]
        [TestCase(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x08, 0x00 })]
        public void ToBit12(byte[] test, byte[] expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected[0]));
            Assert.That(converted[0][1], Is.EqualTo(expected[1]));
        }

        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, new byte[] { 0x00, 0x01 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 }, new byte[] { 0x00, 0x02 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, new byte[] { 0x00, 0x04 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 }, new byte[] { 0x00, 0x08 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 }, new byte[] { 0x00, 0x10 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }, new byte[] { 0x00, 0x20 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x00, 0x40 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x00, 0x80 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x01, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x02, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x04, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x08, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x10, 0x00 })]
        [TestCase(new byte[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x20, 0x00 })]
        [TestCase(new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x40, 0x00 })]
        [TestCase(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x80, 0x00 })]
        public void ToBit16(byte[] test, byte[] expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected[0]));
            Assert.That(converted[0][1], Is.EqualTo(expected[1]));
        }
    }
}