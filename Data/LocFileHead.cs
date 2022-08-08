using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _ReplaceString_.Data
{
    public struct LocMetaData
    {
        [JsonInclude]
        public string author;
        [JsonInclude]
        public string description;
        [JsonInclude]
        public string fileName;
        [JsonInclude]
        public string displayName;
        [JsonInclude]
        public string culture;
        [JsonInclude]
        public Version version;

        public LocMetaData()
        {
            author = string.Empty;
            description = string.Empty;
            fileName = "Default";
            displayName = string.Empty;
            culture = CultureInfo.CurrentCulture.Name;
            version = new Version();
        }

        public override string ToString()
        {
            return $"{author} - {version}\n{description}";
        }
    }
}
