using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace ReplaceString.Config
{
    public class ReplaceStringConfig : ModConfig
    {
        [Label("Autoload Mods")]
        [CustomModConfigItem(typeof(ModDefinitionListElement))]
        public List<ModDefinition> AutoloadModList = new List<ModDefinition>();
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static ReplaceStringConfig Config { get; private set; }

        public override void OnLoaded()
        {
            Config = this;
        }
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
