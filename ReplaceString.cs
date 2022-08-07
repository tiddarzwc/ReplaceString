global using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using _ReplaceString_.ConfigUI.A_Load;
using _ReplaceString_.ConfigUI.ModUI;
using _ReplaceString_.Data;
using _ReplaceString_.Package;
using Hjson;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Core;

namespace _ReplaceString_;
public class ReplaceString : Mod
{
    public static string BasePath => "ReplaceString";
    public List<Hook> hooks;
    public List<ILHook> ilHooks;
    public ModCatcher catcher;
    public Dictionary<string, ImportState> importStates = new Dictionary<string, ImportState>();
    public Dictionary<string, (string name, string description)> importInfo = new Dictionary<string, (string name, string description)>();
    public static readonly string[] blackList =
    {
        "ModLoader",
        "_ReplaceString_"
    };

    private static ReplaceString instance;
    public static ReplaceString Instance => instance;
    public static ModCatcher Catcher => instance.catcher;
    public delegate void Autoload(Mod mod);
    public ReplaceString()
    {
        instance = this;
        MonoModHooks.RequestNativeAccess();
        catcher = new ModCatcher();
        hooks = new List<Hook>();
        ilHooks = new List<ILHook>();
        Import import = null;
        string logPath = $"{ReplaceString.BasePath}/log.txt";
        #region ConfigLoad
        LoadConfig config = new LoadConfig();
        try
        {
            string path = $"{Main.SavePath}/ModConfigs/_ReplaceString__LoadConfig.json";
            string json = File.Exists(path) ? File.ReadAllText(path) : "{}";
            JsonConvert.PopulateObject(json, config);
            DefaultTranslation.Load();
        }
        catch (Exception ex)
        {
            importStates["ReplaceString"] = ImportState.ConfigLoadFail;
            string json = "{}";
            JsonConvert.PopulateObject(json, config);
            File.AppendAllText(logPath, $"Config Load Fail, Config was reset \n Exception : {ex}\n");
            File.WriteAllText($"{Main.SavePath}/ModConfigs/_ReplaceString__ReplaceStringConfig.json", "{}");
        }
        #endregion

        #region PreModLoad
        ilHooks.Add(new ILHook(typeof(AssemblyManager).GetMethod("Instantiate", BindingFlags.Static | BindingFlags.NonPublic), il =>
        {
            var cursor = new ILCursor(il);
            var type = typeof(AssemblyManager).GetNestedType("ModLoadContext", BindingFlags.NonPublic);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, type.GetField("assembly"));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Callvirt, type.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance).GetMethod);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, type.GetField("modFile"));
            cursor.EmitDelegate((Assembly asm, string name, TmodFile modFile) =>
            {
                if (blackList.Contains(name) || !config.AutoloadModList.Any(d => d.Name == name))
                {
                    return;
                }
                importStates[name] = ImportState.Success;
                string locFile = DefaultTranslation.Get(modFile.Name, "loc");
                string hjsonFile = DefaultTranslation.Get(modFile.Name, "hjson");
                if (hjsonFile is null && locFile is null)
                {
                    importStates[name] = ImportState.HjsonNotExist;
                    return;
                }

                try
                {
                    if (locFile != null)
                    {
                        import = new Import(Zip.UnZipHjson(locFile, out var info));
                        importInfo[modFile.Name] = info;
                    }
                    else
                    {
                        using var file = new FileStream(hjsonFile, FileMode.Open);
                        import = new Import(HjsonValue.Load(file));
                    }
                }
                catch (Exception ex)
                {
                    importStates[name] = ImportState.FileLoadFail;
                    File.AppendAllText(logPath, $"Hjson Load Fail\nException : {ex}\n");
                }
                if (importStates[name] == ImportState.Success)
                {
                    try
                    {
                        using (modFile.Open())
                        {
                            import.PreModLoad(name, asm, modFile.GetModAssembly());
                        }
                    }
                    catch (Exception ex)
                    {
                        importStates[name] = ImportState.PreModFail;
                        File.AppendAllText(logPath, $"PreModLoad Fail\nException : {ex}\n");
                    }
                }
            });
        }));
        #endregion

        #region PostModLoad
        hooks.Add(new Hook(typeof(Mod).GetMethod("Autoload", BindingFlags.Instance | BindingFlags.NonPublic), (Autoload orig, Mod mod) =>
        {
            orig(mod);
            if (importStates.ContainsKey(mod.Name) && importStates[mod.Name] == ImportState.Success)
            {
                try
                {
                    import.PostModLoad();
                }
                catch (Exception ex)
                {
                    importStates[mod.Name] = ImportState.PostModFail;
                    File.AppendAllText(logPath, $"PostModLoad Fail\nException : {ex}\n");
                }
            }
        }));
        #endregion
    }

    public override void Unload()
    {
        foreach (var hook in hooks)
        {
            hook.Dispose();
        }
        foreach (var hook in ilHooks)
        {
            hook.Dispose();
        }
        hooks = null;
        catcher.Unload();
    }

}