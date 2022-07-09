using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _ReplaceString_.Translator;
using DebugCommands.Flow;
using DebugCommands.Flow.ActionFlows;
using DebugCommands.Flow.CommandFlows;
using DebugCommands.Flow.DataFlows;
using Terraria;
using Terraria.Localization;
using Hjson;

namespace _ReplaceString_.Command
{
    internal class MakeCommand : CommandFlow
    {
        public override string Name => "make";
    }

    internal class FindHjson : DataFlow
    {
        public override string HelpInfo => "(Hjson file name)";
        public override IEnumerable<string> GetAutoComplete()
        {
            return Directory.GetFiles($"{Main.SavePath}/Mods/ReplaceString").Where(p => Path.GetExtension(p) == ".hjson").Select(p => Path.GetFileName(p));
        }
        public override bool TryFlow(in string input)
        {
            var file = GetInput(input);
            file = Directory.GetFiles($"{Main.SavePath}/Mods/ReplaceString").Where(p => Path.GetExtension(p) == ".hjson").Select(p => Path.GetFileName(p)).FirstOrDefault(f => f == file, null);
            if(string.IsNullOrWhiteSpace(file))
            {
                return false;
            }
            obj = file;
            return true;
        }
    }

    internal class TargetCulture : DataFlow
    {
        public override string HelpInfo => "(culture name)";
        public override IEnumerable<string> GetAutoComplete()
        {
            return typeof(GameCulture.CultureName).GetEnumNames();
        }
        public override bool TryFlow(in string input)
        {
            var culture = GetInput(input);
            var id =  typeof(GameCulture.CultureName).GetEnumValues().Cast<GameCulture.CultureName>().FirstOrDefault(e => e.ToString() == culture);
            if(id == default)
            {
                return false;
            }
            
            obj = id;
            return true;
        }
    }
    internal class MakeAction : ActionFlow
    {
        public override void TakeAction()
        {
            object[] args = MainFlow.Arguments.ToArray();
            var path = $"{Main.SavePath}/Mods/ReplaceString/{args[0]}";
            var culture = (GameCulture.CultureName)args[1];
            using var file = new FileStream(path, FileMode.Open);
            Make.SetupFolds(TreeNode.ReadHjson(HjsonValue.Load(file)), new MakeConfig(Path.GetFileName(path).Split('-')[0], (int)culture, true, false));
        }
    }
}
