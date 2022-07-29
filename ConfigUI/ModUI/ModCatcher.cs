using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace _ReplaceString_.ConfigUI.ModUI;
public struct ModInfo
{
    public static ModInfo Default => new ModInfo(ModContent.Request<Texture2D>("_ReplaceString_/DeletedMod", AssetRequestMode.ImmediateLoad).Value, false, "Deleted Mod");
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
                string name = (string)mod.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance).GetValue(mod);
                if (ReplaceString.blackList.Contains(name))
                {
                    return icon;
                }
                modInfos[name] = new ModInfo(icon,
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
        GC.SuppressFinalize(this);
    }
    private static object modsMenu;
    private static FieldInfo info;
    private static bool loading = false;
    public static event Action OnFinish;
    public static bool IsLoading()
    {
        if (ReplaceString.Catcher.modInfos.Count == 0 && !loading)
        {
            loading = true;
        }
        if (!loading)
        {
            return false;
        }

        if (modsMenu == null)
        {
            modsMenu = (UIElement)typeof(ModContent).Assembly.DefinedTypes.First(t => t.Name == "Interface")
                .GetField("modsMenu", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);
            modsMenu.GetType().GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(modsMenu, new CancellationTokenSource());
            info = modsMenu.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.Public);
            info.SetValue(modsMenu, true);
            modsMenu.GetType().GetMethod("Populate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(modsMenu, null);
            return false;
        }
        loading = (bool)info.GetValue(modsMenu);
        if (!loading)
        {
            OnFinish();
        }
        return loading;
    }
}
