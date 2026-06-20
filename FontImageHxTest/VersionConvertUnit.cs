using FontImageHx;

namespace FontImageHxTest
{
    [TestFixture]
    public class VersionConvertUnit
    {
        [Test]
        public void GetLatestProfile_ConvertsVersion100HexToBothOrientations()
        {
            string json =
                "{\r\n" +
                "  \"Version\": \"1.00\",\r\n" +
                "  \"Image\": [\r\n" +
                "    {\r\n" +
                "      \"Character\": \"A\",\r\n" +
                "      \"Hex\": \"0x80,0x40\",\r\n" +
                "      \"FontSize\": \"12\",\r\n" +
                "      \"CharWidth\": \"1\",\r\n" +
                "      \"CharHeight\": \"2\",\r\n" +
                "      \"FontFamily\": \"Arial\",\r\n" +
                "      \"FontBold\": false,\r\n" +
                "      \"FontItalic\": false,\r\n" +
                "      \"FontUnderline\": false,\r\n" +
                "      \"BinaryThreshold\": 128,\r\n" +
                "      \"NewLine\": true,\r\n" +
                "      \"Locked\": true\r\n" +
                "    }\r\n" +
                "  ]\r\n" +
                "}";

            var profile = VersionConvert.GetLatestProfile(json);

            Assert.That(profile.Version, Is.EqualTo("1.10"));
            Assert.That(profile.Image, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(profile.Image[0].Character, Is.EqualTo('A'));
                Assert.That(profile.Image[0].HexHorizontal, Is.EqualTo("0x80,0x40"));
                Assert.That(profile.Image[0].HexVertical, Is.EqualTo("0x80"));
                Assert.That(profile.Image[0].NewLine, Is.True);
                Assert.That(profile.Image[0].Locked, Is.True);
            });
        }

        [Test]
        public void GetLatestProfile_ReturnsLatestProfileAsIs()
        {
            string json =
                "{\r\n" +
                "  \"Version\": \"1.10\",\r\n" +
                "  \"Image\": [\r\n" +
                "    {\r\n" +
                "      \"Character\": \"B\",\r\n" +
                "      \"HexHorizontal\": \"0x80\",\r\n" +
                "      \"HexVertical\": \"0x80\",\r\n" +
                "      \"FontSize\": \"12\",\r\n" +
                "      \"CharWidth\": \"1\",\r\n" +
                "      \"CharHeight\": \"1\",\r\n" +
                "      \"FontFamily\": \"Arial\",\r\n" +
                "      \"FontBold\": false,\r\n" +
                "      \"FontItalic\": false,\r\n" +
                "      \"FontUnderline\": false,\r\n" +
                "      \"BinaryThreshold\": 128,\r\n" +
                "      \"NewLine\": false,\r\n" +
                "      \"Locked\": false\r\n" +
                "    }\r\n" +
                "  ]\r\n" +
                "}";

            var profile = VersionConvert.GetLatestProfile(json);

            Assert.That(profile.Version, Is.EqualTo("1.10"));
            Assert.That(profile.Image, Has.Count.EqualTo(1));
            Assert.That(profile.Image[0].Character, Is.EqualTo('B'));
            Assert.That(profile.Image[0].HexHorizontal, Is.EqualTo("0x80"));
            Assert.That(profile.Image[0].HexVertical, Is.EqualTo("0x80"));
        }
    }
}
