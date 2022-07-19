using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace _ReplaceString_.Translator.UI;
#pragma warning disable CS0649
internal class WorkConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;
    [Label("Culture")]
    [DefaultValue("Chinese")]
    [OptionStrings(new string[]
    {
        "English",
        "German",
        "Italian",
        "French",
        "Spanish",
        "Russian",
        "Chinese",
        "Portuguese",
        "Polish",
        "Unknown"
    })]
    public string culture; 
     
    [Label("Ignore Space")]
    [DefaultValue(false)]
    public bool ignoreSpace;

    [Label("Make")]
    [CustomModConfigItem(typeof(MakeElement))]
    public object make;


    [Label("Pack")]
    [CustomModConfigItem(typeof(PackElement))]
    public object pack;
}
#pragma warning restore CS0649