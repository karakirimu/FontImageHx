using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;

namespace FontImageHx
{
    interface IProfile<T>
    {
        public string Version { get; init; }

        public IReadOnlyList<T> Image { get; init; }
    }

    /// <summary>
    /// Latest Profile
    /// </summary>
    internal class Profile : IProfile<ImageProperty>
    {
        public Profile(IReadOnlyList<ImageProperty> image)
        {
            Version = "1.10";
            Image = image;
        }

        public string Version { get; init; }

        public IReadOnlyList<ImageProperty> Image { get; init; }
    }

    internal class Profile_1_00 : IProfile<ImageProperty_1_00>
    {
        public Profile_1_00(IReadOnlyList<ImageProperty_1_00> image)
        {
            Version = "1.00";
            Image = image;
        }

        public string Version { get; init; }
        public IReadOnlyList<ImageProperty_1_00> Image { get; init; }
    }

    public class ImageProperty_1_00
    {
        public ImageProperty_1_00()
        {
            FontSize = "12";
            FontFamily = SystemFonts.DefaultFont.FontFamily.Name;
            FontBold = false;
            FontItalic = false;
            FontUnderline = false;
            NewLine = false;
            CharWidth = "16";
            CharHeight = "16";
            Hex = string.Empty;
            BinaryThreshold = 128;
            Locked = false;
        }

        public char Character { get; set; }
        public string Hex { get; set; }
        public string FontSize { get; set; }
        public string CharWidth { get; set; }
        public string CharHeight { get; set; }
        public string FontFamily { get; set; }
        public bool FontBold { get; set; }
        public bool FontItalic { get; set; }
        public bool FontUnderline { get; set; }
        public int BinaryThreshold { get; set; }
        public bool NewLine { get; set; }
        public bool Locked { get; set; }
    }

    internal class VersionConvert
    {
        public static Profile GetLatestProfile(string json)
        {
            Profile result = new Profile(new List<ImageProperty>());

            Profile? profile
                = JsonSerializer.Deserialize<Profile>(json);

            if (profile == null)
            {
                return result;
            }

            if(profile.Version == "1.00")
            {
                Profile_1_00? profile1_00
                    = JsonSerializer.Deserialize<Profile_1_00>(json);

                if(profile1_00 == null) 
                {
                    return result;
                }

                List<ImageProperty> converted = new();

                foreach (var im in profile1_00.Image)
                {
                    var r = new ImageProperty();
                    r.Character = im.Character;
                    r.FontSize = im.FontSize;
                    r.CharWidth = im.CharWidth;
                    r.CharHeight = im.CharHeight;
                    r.FontFamily = im.FontFamily;
                    r.FontBold = im.FontBold;
                    r.FontItalic = im.FontItalic;
                    r.FontUnderline = im.FontUnderline;
                    r.BinaryThreshold = im.BinaryThreshold;
                    r.NewLine = im.NewLine;
                    r.Locked = im.Locked;
                    r.HexHorizontal = im.Hex;

                    int width = int.Parse(im.CharWidth);
                    int height = int.Parse(im.CharHeight);
                    var array = BitmapOperation.ToShrinkedBitmapHorizontal(
                        im.Hex, width, height);
                    var uncompressed
                        = BitmapOperation.BitToByteHorizontal(array, width);
                    r.HexVertical = BitmapOperation.ToSequential(
                        BitmapOperation.ByteToBitVertical(uncompressed));

                    converted.Add(r);
                }

                result = new Profile(converted);
            }
            else
            {
                result = profile;
            }

            return result;
        }
    }
}
