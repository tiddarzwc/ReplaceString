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

namespace _ReplaceString_.Command
{
    internal class PackCommand : CommandFlow
    {
        public override string Name => "pack";
    }

    internal class FindDirectory : DataFlow
    {
        public override string HelpInfo => "(Directory name)";
        public override IEnumerable<string> GetAutoComplete()
        {
            return Directory.GetDirectories($"{Main.SavePath}/Mods/ReplaceString").Select(p => Path.GetFileName(p));
        }
        public override bool TryFlow(in string input)
        {
            var file = GetInput(input);
            file = Directory.GetDirectories($"{Main.SavePath}/Mods/ReplaceString").Select(p => Path.GetFileName(p)).FirstOrDefault(f => f == file, null);
            if (string.IsNullOrWhiteSpace(file))
            {
                return false;
            }
            obj = file;
            return true;
        }
    }

    internal class PackAction : ActionFlow
    {
        public override void TakeAction()
        {
            object[] args = MainFlow.Arguments.ToArray();
            var path = $"{Main.SavePath}/Mods/ReplaceString/{args[0]}";
            var (treeNode, config) = Pack.Packup(path);
            File.WriteAllText($"{Main.SavePath}/Mods/ReplaceString/{treeNode.name}_{GameCulture.FromCultureName((GameCulture.CultureName)config.TargetCultureID).Name}_packed.hjson", treeNode.BuildHjson(0).ToString());
        }
    }
}
