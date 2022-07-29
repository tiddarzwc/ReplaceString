using System;
using System.Linq;
using System.Reflection;
using _ReplaceString_.ConfigUI.B_Export;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader.Config;

namespace _ReplaceString_.ConfigUI
{
    internal class UIHooks : ILoadable
    {
        public ILHook hook;
        public void Load(Mod mod)
        {
            var type = typeof(ModConfig).Assembly.DefinedTypes.First(t => t.Name == "UIModConfig");
            var info = type.GetMethod("SaveConfig", BindingFlags.NonPublic | BindingFlags.Instance);
            hook = new ILHook(info, il =>
            {
                var cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(MoveType.After, ins => ins.MatchCall(typeof(ConfigManager).GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Static))))
                {
                    throw new Exception("Config not found");
                }
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldfld, type.GetField("modConfig", BindingFlags.NonPublic | BindingFlags.Instance));
                cursor.EmitDelegate((ModConfig modConfig) =>
                {
                    if (modConfig is ExportConfig)
                    {
                        modConfig.OnChanged();
                    }
                });
            });
        }

        public void Unload()
        {
            hook.Dispose();
        }
    }
}
