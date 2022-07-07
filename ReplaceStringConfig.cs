using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;

namespace ReplaceString
{
    public class ReplaceStringConfig : ModConfig
    {
        [JsonDefaultListValue("[]")]
        public List<string> AutoloadModList = new List<string>();
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static ReplaceStringConfig Config { get; private set; }

        public override void OnLoaded()
        {
            Config = this;
        }
        public override void OnChanged()
        {
            //var modList = ModLoader.Mods.Select(mod => mod.Name);
            AutoloadModList = new HashSet<string>(AutoloadModList).ToList();
        }
    }
}
