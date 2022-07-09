using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace _ReplaceString_.Config
{
    public class ReplaceStringConfig : ModConfig
    {
        [Label("Autoload Mods")]
        [CustomModConfigItem(typeof(ModDefinitionListElement))]
        public List<ModDefinition> AutoloadModList { get; set; } = new List<ModDefinition>();
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override void OnChanged()
        {
            //var modList = ModLoader.Mods.Select(mod => mod.Name);
            AutoloadModList = AutoloadModList.ToHashSet().ToList();
            if (!Main.gameMenu && Main.netMode != NetmodeID.Server)
            {
                Main.NewText("Please reload mod");
            }
        }
    }
}
