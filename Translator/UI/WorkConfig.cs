using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace _ReplaceString_.Translator.UI
{
    internal class WorkConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Make")]
        [CustomModConfigItem(typeof(MakeElement))]
        public byte make;
    }
}
