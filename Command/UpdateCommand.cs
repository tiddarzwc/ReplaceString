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
using Hjson;
using Terraria;

namespace _ReplaceString_.Command
{
    internal class UpdateCommand : CommandFlow
    {
        public override string Name => "update";
    }

    internal class UpdateAction : ActionFlow
    {
        public override void TakeAction()
        {
            var args = MainFlow.Instance.arguments.ToArray();
            var oldPath = $"{Main.SavePath}/Mods/ReplaceString/{args[0]}";
            var newPath = $"{Main.SavePath}/Mods/ReplaceString/{args[1]}";
            var translation = $"{Main.SavePath}/Mods/ReplaceString/{args[2]}";
            var oldTree = TreeNode.ReadHjson(HjsonValue.Load(File.Open(oldPath, FileMode.Open)));
            var newTree = TreeNode.ReadHjson(HjsonValue.Load(File.Open(newPath, FileMode.Open)));
            var transTree = TreeNode.ReadHjson(HjsonValue.Load(File.Open(translation, FileMode.Open)));
            Update.UpdateTree(oldTree, newTree, transTree);
            File.WriteAllText($"{Main.SavePath}/Mods/ReplaceString/{Path.GetFileName(args[1] as string)}-update.txt",
                newTree.BuildHjson(0).ToString());
            File.WriteAllText($"{Main.SavePath}/Mods/ReplaceString/update-info.txt", Update.CacheInfo.ToString());
            Update.CacheInfo.Clear();
        }
    }
}
