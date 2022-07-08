using System.IO;
using _ReplaceString_;

using DebugCommands.Flow.ActionFlows;

using Hjson;

using Terraria;
using Terraria.Localization;

namespace _ReplaceString_.Command
{
    internal class ImportAction : ActionFlow
    {
        public override void TakeAction()
        {
            var mod = (Mod)((object[])ReplaceString.Command.Call("Arguement"))[0];
            if (!Directory.Exists($"{Main.SavePath}/Mods/ReplaceString"))
            {
                Directory.CreateDirectory($"{Main.SavePath}/Mods/ReplaceString");
            }
            if (!File.Exists($"{Main.SavePath}/Mods/ReplaceString/{mod.Name}_{Language.ActiveCulture.Name}.hjson"))
            {
                ModContent.GetInstance<ReplaceString>().Logger.Warn($"{mod.Name}_{Language.ActiveCulture.Name}.hjson not found, Please check mod name and language");
                return;
            }
            using FileStream file = new FileStream($"{Main.SavePath}/Mods/ReplaceString/{mod.Name}_{Language.ActiveCulture.Name}.hjson", FileMode.Open);
            new Import(HjsonValue.Load(file)).Load();
        }
    }
}
