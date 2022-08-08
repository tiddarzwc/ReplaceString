using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using _ReplaceString_.ConfigUI.ModUI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace _ReplaceString_.ConfigUI.A_Load
{
    [Label("LoadConfig")]
    public class LoadConfig : ModConfig
    {
        [Label("Auto Update")]
        [DefaultValue(true)]
        public bool autoUpdate = true;

        [Label("Autoload Mods")]
        [CustomModConfigItem(typeof(ModDefinitionListElement))]
        public List<ModDefinition> AutoloadModList { get; set; } = new List<ModDefinition>();
        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
}
