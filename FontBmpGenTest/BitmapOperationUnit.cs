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

        [TestCase(new byte[] { 0, 1 }, 0x40)]
        [TestCase(new byte[] { 1, 0 }, 0x80)]
        public void ToBit2(byte[] test, byte expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0, 0, 1 }, 0x20)]
        [TestCase(new byte[] { 0, 1, 0 }, 0x40)]
        [TestCase(new byte[] { 1, 0, 0 }, 0x80)]
        public void ToBit3(byte[] test, byte expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0, 0, 0, 1 }, 0x10)]
        [TestCase(new byte[] { 0, 0, 1, 0 }, 0x20)]
        [TestCase(new byte[] { 0, 1, 0, 0 }, 0x40)]
        [TestCase(new byte[] { 1, 0, 0, 0 }, 0x80)]
        public void ToBit4(byte[] test, byte expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0, 0, 0, 0, 1 }, 0x08)]
        [TestCase(new byte[] { 0, 0, 0, 1, 0 }, 0x10)]
        [TestCase(new byte[] { 0, 0, 1, 0, 0 }, 0x20)]
        [TestCase(new byte[] { 0, 1, 0, 0, 0 }, 0x40)]
        [TestCase(new byte[] { 1, 0, 0, 0, 0 }, 0x80)]
        public void ToBit5(byte[] test, byte expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0, 0, 0, 0, 0, 1 }, 0x04)]
        [TestCase(new byte[] { 0, 0, 0, 0, 1, 0 }, 0x08)]
        [TestCase(new byte[] { 0, 0, 0, 1, 0, 0 }, 0x10)]
        [TestCase(new byte[] { 0, 0, 1, 0, 0, 0 }, 0x20)]
        [TestCase(new byte[] { 0, 1, 0, 0, 0, 0 }, 0x40)]
        [TestCase(new byte[] { 1, 0, 0, 0, 0, 0 }, 0x80)]
        public void ToBit6(byte[] test, byte expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 1 }, 0x02)]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 1, 0 }, 0x04)]
        [TestCase(new byte[] { 0, 0, 0, 0, 1, 0, 0 }, 0x08)]
        [TestCase(new byte[] { 0, 0, 0, 1, 0, 0, 0 }, 0x10)]
        [TestCase(new byte[] { 0, 0, 1, 0, 0, 0, 0 }, 0x20)]
        [TestCase(new byte[] { 0, 1, 0, 0, 0, 0, 0 }, 0x40)]
        [TestCase(new byte[] { 1, 0, 0, 0, 0, 0, 0 }, 0x80)]
        public void ToBit7(byte[] test, byte expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }, 0x01)]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 1, 0 }, 0x02)]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0 }, 0x04)]
        [TestCase(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 }, 0x08)]
        [TestCase(new byte[] { 0, 0, 0, 1, 0, 0, 0, 0 }, 0x10)]
        [TestCase(new byte[] { 0, 0, 1, 0, 0, 0, 0, 0 }, 0x20)]
        [TestCase(new byte[] { 0, 1, 0, 0, 0, 0, 0, 0 }, 0x40)]
        [TestCase(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, 0x80)]
        public void ToBit8(byte[] test, byte expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected));
        }

        // Left stuff
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, new byte[] { 0x00, 0x10 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 }, new byte[] { 0x00, 0x20 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, new byte[] { 0x00, 0x40 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 }, new byte[] { 0x00, 0x80 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 }, new byte[] { 0x01, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }, new byte[] { 0x02, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x04, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x08, 0x00 })]
        [TestCase(new byte[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x10, 0x00 })]
        [TestCase(new byte[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x20, 0x00 })]
        [TestCase(new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x40, 0x00 })]
        [TestCase(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new byte[] { 0x80, 0x00 })]
        public void ToBit12(byte[] test, byte[] expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = ByteToBit(target);
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
            var converted = ByteToBit(target);
            Assert.That(converted[0][0], Is.EqualTo(expected[0]));
            Assert.That(converted[0][1], Is.EqualTo(expected[1]));
        }

        [TestCase(new byte[] { 0x00, 0x01 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 })]
        [TestCase(new byte[] { 0x00, 0x02 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 })]
        [TestCase(new byte[] { 0x00, 0x04 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 })]
        [TestCase(new byte[] { 0x00, 0x08 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 })]
        [TestCase(new byte[] { 0x00, 0x10 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x00, 0x20 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x00, 0x40 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x00, 0x80 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x01, 0x00 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x02, 0x00 }, new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x04, 0x00 }, new byte[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x08, 0x00 }, new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x10, 0x00 }, new byte[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x20, 0x00 }, new byte[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x40, 0x00 }, new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x80, 0x00 }, new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        public void ToByte16(byte[] test, byte[] expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = BitToByte(target, expected.Length);
            Assert.Multiple(() =>
            {
                Assert.That(converted[0][0], Is.EqualTo(expected[0]));
                Assert.That(converted[0][1], Is.EqualTo(expected[1]));
                Assert.That(converted[0][2], Is.EqualTo(expected[2]));
                Assert.That(converted[0][3], Is.EqualTo(expected[3]));
                Assert.That(converted[0][4], Is.EqualTo(expected[4]));
                Assert.That(converted[0][5], Is.EqualTo(expected[5]));
                Assert.That(converted[0][6], Is.EqualTo(expected[6]));
                Assert.That(converted[0][7], Is.EqualTo(expected[7]));
                Assert.That(converted[0][8], Is.EqualTo(expected[8]));
                Assert.That(converted[0][9], Is.EqualTo(expected[9]));
                Assert.That(converted[0][10], Is.EqualTo(expected[10]));
                Assert.That(converted[0][11], Is.EqualTo(expected[11]));
                Assert.That(converted[0][12], Is.EqualTo(expected[12]));
                Assert.That(converted[0][13], Is.EqualTo(expected[13]));
                Assert.That(converted[0][14], Is.EqualTo(expected[14]));
                Assert.That(converted[0][15], Is.EqualTo(expected[15]));
            });
        }

        [TestCase(new byte[] { 0x00, 0x10 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 })]
        [TestCase(new byte[] { 0x00, 0x20 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 })]
        [TestCase(new byte[] { 0x00, 0x40 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 })]
        [TestCase(new byte[] { 0x00, 0x80 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 })]
        [TestCase(new byte[] { 0x01, 0x00 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x02, 0x00 }, new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x04, 0x00 }, new byte[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x08, 0x00 }, new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x10, 0x00 }, new byte[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x20, 0x00 }, new byte[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x40, 0x00 }, new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x80, 0x00 }, new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        public void ToByte12(byte[] test, byte[] expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = BitToByte(target, expected.Length);
            Assert.Multiple(() =>
            {
                Assert.That(converted[0][0], Is.EqualTo(expected[0]));
                Assert.That(converted[0][1], Is.EqualTo(expected[1]));
                Assert.That(converted[0][2], Is.EqualTo(expected[2]));
                Assert.That(converted[0][3], Is.EqualTo(expected[3]));
                Assert.That(converted[0][4], Is.EqualTo(expected[4]));
                Assert.That(converted[0][5], Is.EqualTo(expected[5]));
                Assert.That(converted[0][6], Is.EqualTo(expected[6]));
                Assert.That(converted[0][7], Is.EqualTo(expected[7]));
                Assert.That(converted[0][8], Is.EqualTo(expected[8]));
                Assert.That(converted[0][9], Is.EqualTo(expected[9]));
                Assert.That(converted[0][10], Is.EqualTo(expected[10]));
                Assert.That(converted[0][11], Is.EqualTo(expected[11]));
            });
        }

        [TestCase(new byte[] { 0x01 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 })]
        [TestCase(new byte[] { 0x02 }, new byte[] { 0, 0, 0, 0, 0, 0, 1, 0 })]
        [TestCase(new byte[] { 0x04 }, new byte[] { 0, 0, 0, 0, 0, 1, 0, 0 })]
        [TestCase(new byte[] { 0x08 }, new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 })]
        [TestCase(new byte[] { 0x10 }, new byte[] { 0, 0, 0, 1, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x20 }, new byte[] { 0, 0, 1, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x40 }, new byte[] { 0, 1, 0, 0, 0, 0, 0, 0 })]
        [TestCase(new byte[] { 0x80 }, new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 })]
        public void ToByte8(byte[] test, byte[] expected)
        {
            byte[][] target = new byte[][] { test };
            var converted = BitToByte(target, expected.Length);
            Assert.Multiple(() =>
            {
                Assert.That(converted[0][0], Is.EqualTo(expected[0]));
                Assert.That(converted[0][1], Is.EqualTo(expected[1]));
                Assert.That(converted[0][2], Is.EqualTo(expected[2]));
                Assert.That(converted[0][3], Is.EqualTo(expected[3]));
                Assert.That(converted[0][4], Is.EqualTo(expected[4]));
                Assert.That(converted[0][5], Is.EqualTo(expected[5]));
                Assert.That(converted[0][6], Is.EqualTo(expected[6]));
                Assert.That(converted[0][7], Is.EqualTo(expected[7]));
            });
        }
    }
}