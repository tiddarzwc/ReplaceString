using System.IO;
using _ReplaceString_.Translator;
using Terraria;
using Terraria.Localization;

namespace _ReplaceString_.Command
{
    internal class ExporAction : DebugCommands.Flow.ActionFlows.ActionFlow
    {
        public override void TakeAction()
        {
            var mod = (Mod)((object[])ReplaceString.Command.Call("Arguement"))[0];
            Export export = new Export(mod);
            if (!Directory.Exists($"{Main.SavePath}/Mods/ReplaceString"))
            {
                Directory.CreateDirectory($"{Main.SavePath}/Mods/ReplaceString");
            }
            using FileStream file = new FileStream($"{Main.SavePath}/Mods/ReplaceString/{mod.Name}_{Language.ActiveCulture.Name}.hjson", FileMode.Create);
            export.Hjson(file);
        }
    }
}
