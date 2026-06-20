using FontImageHx;

namespace FontImageHxTest
{
    [TestFixture]
    public class TextWizardTextUnit
    {
        [Test]
        public void Ascii_ContainsPrintableAsciiInExpectedRows()
        {
            Assert.That(
                TextWizardText.Ascii,
                Is.EqualTo(
                    " !\"#$%&'()*+,-./\n" +
                    "0123456789:;<=>?\n" +
                    "@ABCDEFGHIJKLMNO\n" +
                    "PQRSTUVWXYZ[\\]^_\n" +
                    "`abcdefghijklmno\n" +
                    "pqrstuvwxyz{|}~"));
        }
    }
}
