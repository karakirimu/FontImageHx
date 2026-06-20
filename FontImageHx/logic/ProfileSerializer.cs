using System.Collections.Generic;
using System.Text.Json;

namespace FontImageHx
{
    internal static class ProfileSerializer
    {
        public static Profile DeserializeLatestProfile(string json)
        {
            return VersionConvert.GetLatestProfile(json);
        }

        public static string SerializeLatestProfile(IReadOnlyList<ImageProperty> images)
        {
            Profile profile = new(images);
            return JsonSerializer.Serialize(profile);
        }
    }
}
