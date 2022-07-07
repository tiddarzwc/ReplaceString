using System.Collections.Generic;
using DebugCommands.Flow.ActionFlows;
using DebugCommands.Flow.CommandFlows;
using DebugCommands.Flow.DataFlows;
using Terraria;

namespace ReplaceString.Command
{
    internal class AddMod : CommandFlow
    {
        public override string Name => "rsadd";
    }
    internal class RemoveMod : CommandFlow
    {
        public override string Name => "rsremove";
    }
    internal class AddAction : ActionFlow
    {
        public override void TakeAction()
        {
            var mod = (Mod)((object[])ReplaceString.Command.Call("Arguement"))[0];
            var list = ModContent.GetInstance<ReplaceStringConfig>().AutoloadModList;
            if (list.Contains(mod.Name))
            {
                Main.NewText("Same modname exists");
            }
            else
            {
                list.Add(mod.Name);
                Main.NewText("Success");
            }
        }
    }
    internal class FindAddedMod : DataFlow
    {
        public override string HelpInfo => "(ModName)";
        public override IEnumerable<string> GetAutoComplete()
        {
            return ModContent.GetInstance<ReplaceStringConfig>().AutoloadModList;
        }
        public override bool TryFlow(in string input)
        {
            string name = GetInput(input);
            if (ModContent.GetInstance<ReplaceStringConfig>().AutoloadModList.Contains(name))
            {
                obj = name;
                return true;
            }
            return false;
        }
    }
    internal class RemoveAction : ActionFlow
    {
        public override void TakeAction()
        {
            var mod = (string)((object[])ReplaceString.Command.Call("Arguement"))[0];
            var list = ModContent.GetInstance<ReplaceStringConfig>().AutoloadModList;
            if (list.Contains(mod))
            {
                list.Remove(mod);
                Main.NewText("Success");
            }
            else
            {
                Main.NewText("modname don't exists");
            }
        }
    }

}
