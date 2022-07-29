using Terraria.ModLoader.Config;

namespace _ReplaceString_.ConfigUI.B_Export;

#pragma warning disable CS0649
internal class ExportConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Label("选择已加载Mod导出")]
    [CustomModConfigItem(typeof(ExportElement))]
    public object export;
}
#pragma warning restore CS0649