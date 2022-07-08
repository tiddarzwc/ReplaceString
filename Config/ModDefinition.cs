using System.Linq;
using System.Text.Json.Serialization;
using Terraria.ModLoader.IO;

namespace ReplaceString.Config
{
    public class ModDefinition : TagSerializable
    {
        [JsonIgnore]
        public Mod mod;
        [JsonIgnore]
        public bool IsModLoaded => mod != null;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ModDefinition(string modName, string displayName)
        {
            Name = modName;
            DisplayName = displayName;
            mod = ModLoader.Mods.FirstOrDefault(mod => mod.Name == modName, null);
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound
            {
                { "name", Name },
                { "displayName", DisplayName }
            };
            return tag;
        }
    }
}
