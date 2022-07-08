using System.Collections.Generic;
using System.Linq;
using DebugCommands.Flow.ActionFlows;
using DebugCommands.Flow.CommandFlows;
using DebugCommands.Flow.DataFlows;
using ReplaceString.Config;
using Terraria;
using System.Reflection;
using Terraria.ModLoader.Config;
using System.Threading;
using System;

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
            var names = list.Select(d => d.Name);
            if (names.Contains(mod.Name))
            {
                Main.NewText("Same modname exists");
            }
            else
            {
                list.Add(new ModDefinition(mod.Name, mod.DisplayName));
                Main.NewText("Success");
                typeof(ConfigManager).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static)
                    .Invoke(null, ModContent.GetInstance<ReplaceStringConfig>());
            }
        }
    }
    internal class FindAddedMod : DataFlow
    {
        public override string HelpInfo => "(ModName)";
        public override IEnumerable<string> GetAutoComplete()
        {
            return ModContent.GetInstance<ReplaceStringConfig>().AutoloadModList.Select(d => d.Name);
        }
        public override bool TryFlow(in string input)
        {
            string name = GetInput(input);
            if (ModContent.GetInstance<ReplaceStringConfig>().AutoloadModList.Any(d => d.Name == name))
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
            if (list.Any(d => d.Name == mod))
            {
                list.RemoveAll(d => d.Name == mod);
                Main.NewText("Success");
                typeof(ConfigManager).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static)
                    .Invoke(null, ModContent.GetInstance<ReplaceStringConfig>());
            }
            else
            {
                Main.NewText("modname don't exists");
            }
        }
    }

}
