using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace _ReplaceString_.ConfigUI.C_Work;
#pragma warning disable CS0649
internal class WorkConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;
    [Label("Culture (for zip)")]
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

    [Label("Make")]
    [CustomModConfigItem(typeof(MakeElement))]
    public object make;

    [Label("Pack")]
    [CustomModConfigItem(typeof(PackElement))]
    public object pack;

    [Label("Update")]
    [CustomModConfigItem(typeof(UpdateElement))]
    public object update;
}
#pragma warning restore CS0649