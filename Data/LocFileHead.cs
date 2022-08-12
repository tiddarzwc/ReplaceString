using System.Globalization;
using System.Text.Json.Serialization;

namespace _ReplaceString_.Data
{
    public class LocMetaData
    {
        [JsonPropertyName("mod_name")] public string modName { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        [JsonPropertyName("filename")] public string fileName { get; set; }
        [JsonPropertyName("display_name")] public string displayName { get; set; }
        public string culture { get; set; }
        public Version version { get; set; }

        public LocMetaData()
        {
            modName = string.Empty;
            author = string.Empty;
            description = string.Empty;
            fileName = "Default";
            displayName = string.Empty;
            culture = CultureInfo.CurrentCulture.Name;
            version = new Version();
        }

        public override string ToString()
        {
            return $"{displayName} by {author}  for {modName} - {version}\n{description}";
        }
    }
}
