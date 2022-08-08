//using System;
//using System.Linq;
//using System.Reflection;
//using _ReplaceString_.ConfigUI.B_Export;
//using _ReplaceString_.ConfigUI.C_Work;
//using Microsoft.Xna.Framework;
//using Mono.Cecil.Cil;
//using MonoMod.Cil;
//using MonoMod.RuntimeDetour;
//using Terraria.Audio;
//using Terraria.ModLoader.Config;

//namespace _ReplaceString_.ConfigUI
//{
//    internal class UIHooks : ILoadable
//    {
//        public ILHook hook;
//        public void Load(Mod mod)
//        {
//            var type = typeof(ModConfig).Assembly.DefinedTypes.First(t => t.Name == "UIModConfig");
//            var info = type.GetMethod("SaveConfig", BindingFlags.NonPublic | BindingFlags.Instance);
//            hook = new ILHook(info, il =>
//            {
//                var cursor = new ILCursor(il);
//                var info = type.GetField("pendingConfig", BindingFlags.NonPublic | BindingFlags.Instance);
//                if (!cursor.TryGotoNext(MoveType.After, ins => ins.MatchLdfld(info)))
//                {
//                    throw new Exception("Config not found");
//                }
//                cursor.Emit(OpCodes.Ldarg_0);
//                cursor.Emit(OpCodes.Ldfld, info);
//                cursor.EmitDelegate((ModConfig modConfig) =>
//                {
//                    if (modConfig is WorkConfig)
//                    {
//                        modConfig.OnChanged();
//                    }
//                });
//            });
//        }

//        public void Unload()
//        {
//            hook.Dispose();
//        }
//    }
//}
