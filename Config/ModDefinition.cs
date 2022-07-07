using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace ReplaceString.Config
{
    public class ModDefinition : TagSerializable
    {
        [DefaultValue("None")]
        public string modName;
        [JsonIgnore]
        public Mod mod;
        public bool IsModLoaded => mod != null;
        public string Name => modName;
        public string DisplayName => mod?.DisplayName ?? "Unloaded Mod";
        public ModDefinition(string modName)
        {
            this.modName = modName;
            mod = ModLoader.Mods.FirstOrDefault(mod => mod.Name == modName, null);
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound
            {
                { "name", modName }
            };
            return tag;
        }
    }
}
