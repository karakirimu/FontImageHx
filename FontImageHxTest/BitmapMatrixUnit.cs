using FontImageHx;

namespace FontImageHxTest
{
    [TestFixture]
    public class BitmapMatrixUnit
    {
        [TestCase("Up", new byte[] { 0, 1, 0, 0, 0, 0 })]
        [TestCase("Down", new byte[] { 0, 0, 0, 0, 0, 0 })]
        [TestCase("Left", new byte[] { 0, 0, 0, 1, 0, 0 })]
        [TestCase("Right", new byte[] { 0, 0, 0, 0, 0, 1 })]
        public void Move_ShiftsSinglePixelAndClearsEdge(string directionName, byte[] expected)
        {
            byte[][] bitmap =
                new byte[][]
                {
                    new byte[] { 0, 0, 0 },
                    new byte[] { 0, 1, 0 }
                };

            MoveDirection direction = Enum.Parse<MoveDirection>(directionName);
            BitmapMatrix.Move(bitmap, direction);

            Assert.That(bitmap[0], Is.EqualTo(expected[..3]));
            Assert.That(bitmap[1], Is.EqualTo(expected[3..]));
        }

        [Test]
        public void Move_NullInput_DoesNothing()
        {
            Assert.DoesNotThrow(() => BitmapMatrix.Move(null, MoveDirection.Up));
        }
    }
}
