﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using System.Reflection;
namespace ReplaceString;
public struct ModInfo
{
    public static ModInfo Default => new ModInfo(ModContent.Request<Texture2D>("ReplaceString/DeletedMod", AssetRequestMode.ImmediateLoad).Value, false, "Deleted Mod");
    public Texture2D icon;
    public bool enable;
    public string displayName;
    public ModInfo(Texture2D icon, bool enable, string displayName)
    {
        this.icon = icon;
        this.enable = enable;
        this.displayName = displayName;
    }
}
public class ModCatcher : IDisposable
{
    public Dictionary<string, ModInfo> modInfos = new Dictionary<string, ModInfo>();
    public ILHook hook;
    public ModCatcher()
    {
        var type = typeof(ModContent).Assembly.DefinedTypes.First(t => t.Name == "UIModItem");
        var method = type.GetMethod("OnInitialize", BindingFlags.Instance | BindingFlags.Public);
        hook = new ILHook(method, il =>
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, ins => ins.MatchNewobj<UIImage>()))
            {
                throw new Exception("Unknown Exception");
            }
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, type.GetField("_mod", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                .EmitDelegate((Texture2D icon, object mod) =>
            {
                modInfos[(string)mod.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance).GetValue(mod)] =
                    new ModInfo(icon,
                        (bool)mod.GetType().GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance).GetValue(mod),
                        (string)mod.GetType().GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance).GetValue(mod));
                return icon;
            });

        });
    }

    public void Unload()
    {
        hook.Dispose();
        hook = null;
    }

    public void Dispose()
    {
        hook.Dispose();
        hook = null;
    }
}