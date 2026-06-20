using FontImageHx;

namespace FontImageHxTest
{
    [TestFixture]
    public class ProfileSerializerUnit
    {
        [Test]
        public void SerializeLatestProfile_WritesVersion110AndImageData()
        {
            ImageProperty[] images =
            [
                new ImageProperty
                {
                    Character = 'A',
                    HexHorizontal = "0x80",
                    HexVertical = "0x80",
                    CharWidth = "1",
                    CharHeight = "1"
                }
            ];

            string json = ProfileSerializer.SerializeLatestProfile(images);

            Assert.That(json, Does.Contain("\"Version\":\"1.10\""));
            Assert.That(json, Does.Contain("\"Character\":\"A\""));
            Assert.That(json, Does.Contain("\"HexHorizontal\":\"0x80\""));
        }

        [Test]
        public void DeserializeLatestProfile_ReadsCurrentProfile()
        {
            string json =
                """
                {"Version":"1.10","Image":[{"Character":"B","HexHorizontal":"0x40","HexVertical":"0x40","FontSize":"12","CharWidth":"1","CharHeight":"1","FontFamily":"Arial","FontBold":false,"FontItalic":false,"FontUnderline":false,"BinaryThreshold":128,"NewLine":false,"Locked":false}]}
                """;

            Profile profile = ProfileSerializer.DeserializeLatestProfile(json);

            Assert.That(profile.Image, Has.Count.EqualTo(1));
            Assert.That(profile.Image[0].Character, Is.EqualTo('B'));
            Assert.That(profile.Image[0].HexHorizontal, Is.EqualTo("0x40"));
        }
    }
}
