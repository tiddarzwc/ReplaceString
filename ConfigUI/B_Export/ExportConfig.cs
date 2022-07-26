using System;
using System.Text;
using Terraria.ModLoader.Config;
using System.Linq;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.UI;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using _ReplaceString_.ConfigUI.Export;

namespace _ReplaceString_.ConfigUI.B_Export;

#pragma warning disable CS0649
internal class ExportConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static string SpaceChar;
    public static string Space;
    public static bool Tight;
    [Label("缩进空格数，0为制表符")]
    [JsonDefaultValue("4")]
    [Range(0, 10)]
    public int spaceCount = 0;

    [Label("文件是否紧密")]
    [JsonDefaultValue("false")]
    public bool tight = false;

    [Label("选择已加载Mod导出")]
    [CustomModConfigItem(typeof(ExportElement))]
    public object export;
    public override void OnChanged()
    {
        if (spaceCount == 0)
        {
            SpaceChar = "\t";
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < spaceCount; i++)
            {
                sb.Append(' ');
            }
            SpaceChar = sb.ToString();
        }

        if (tight)
        {
            Space = "";
        }
        else
        {
            Space = " ";
        }
        Tight = tight;
    }
}
#pragma warning restore CS0649