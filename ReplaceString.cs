global using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using _ReplaceString_.Command;
using _ReplaceString_.Config;
using DebugCommands.Flow;
using DebugCommands.Flow.DataFlows;
using Hjson;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Core;

namespace _ReplaceString_
{
    public static class Extension
    {
        public static IEnumerable<FieldInfo> GetFields_Deep(this Type type, BindingFlags flags)
        {
            Type t = type;
            IEnumerable<FieldInfo> fields = t.GetFields(flags);
            while (t.BaseType != null)
            {
                t = t.BaseType;
                fields = fields.Concat(t.GetFields(flags));
            }
            return fields;
        }
        public static IEnumerable<PropertyInfo> GetProperties_Deep(this Type type, BindingFlags flags)
        {
            Type t = type;
            IEnumerable<PropertyInfo> properties = t.GetProperties(flags);
            while (t.BaseType != null)
            {
                t = t.BaseType;
                properties = properties.Concat(t.GetProperties(flags));
            }
            return properties;
        }
        public static IEnumerable<MethodInfo> GetMethods_Deep(this Type type, BindingFlags flags)
        {
            Type t = type;
            IEnumerable<MethodInfo> properties = t.GetMethods(flags);
            while (t.BaseType != null)
            {
                t = t.BaseType;
                properties = properties.Concat(t.GetMethods(flags));
            }
            return properties;
        }
        public static object GetValue(this object obj, string name)
        {
            if (obj is Type type)
            {
                var fields = type.GetFields_Deep(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    if (field.Name == name)
                    {
                        return field.GetValue(obj);
                    }
                }
                var properties = type.GetProperties_Deep(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var property in properties)
                {
                    if (property.Name == name)
                    {
                        return property.GetValue(obj);
                    }
                }
            }
            else
            {
                var fields = obj.GetType().GetFields_Deep(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    if (field.Name == name)
                    {
                        return field.GetValue(obj);
                    }
                }
                var properties = obj.GetType().GetProperties_Deep(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var property in properties)
                {
                    if (property.Name == name)
                    {
                        return property.GetValue(obj);
                    }
                }
            }
            throw new ArgumentException("Name not found");
        }
        public static T GetValue<T>(this object obj, string name) => (T)obj.GetValue(name);
        public static object GetValue<T>(string name)
        {
            var fields = typeof(T).GetFields_Deep(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.Name == name)
                {
                    return field.GetValue(null);
                }
            }
            var properties = typeof(T).GetProperties_Deep(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                if (property.Name == name)
                {
                    return property.GetValue(null);
                }
            }
            throw new ArgumentException("Name not found");
        }
        public static ValueType GetValue<Type, ValueType>(string name) => (ValueType)GetValue<Type>(name);

        public static void SetValue(this object obj, string name, object value)
        {
            var fields = obj.GetType().GetFields_Deep(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.Name == name)
                {
                    field.SetValue(obj, value);
                }
            }
            var properties = obj.GetType().GetProperties_Deep(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                if (property.Name == name)
                {
                    property.SetValue(obj, value);
                }
            }
            throw new ArgumentException("Name not found");
        }
        public static void SetValue<T>(string name, object value)
        {
            var fields = typeof(T).GetFields_Deep(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.Name == name)
                {
                    field.SetValue(null, value);
                }
            }
            var properties = typeof(T).GetProperties_Deep(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                if (property.Name == name)
                {
                    property.SetValue(null, value);
                }
            }
            throw new ArgumentException("Name not found");
        }

        public static object Invoke(this object obj, string name, params object[] objects)
        {
            var methods = obj.GetType().GetMethods_Deep(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                if (method.Name == name)
                {
                    return method.Invoke(obj, objects);
                }
            }
            throw new ArgumentException("Name not found");
        }
        public static T Invoke<T>(this object obj, string name, params object[] objects) => (T)obj.Invoke(name, objects);
        public static object Invoke<T>(string name, params object[] objects)
        {
            var methods = typeof(T).GetMethods_Deep(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var method in methods)
            {
                if (method.Name == name)
                {
                    return method.Invoke(null, objects);
                }
            }
            throw new ArgumentException("Name not found");
        }
        public static ValueType Invoke<Type, ValueType>(string name, params object[] objects) => (ValueType)Invoke<Type>(name, objects);
        public static int FindEnd(in string str, int start)
        {
            int count = 0;
            do
            {
                if (str[start] == '{')
                {
                    ++count;
                }
                else if (str[start] == '}')
                {
                    --count;
                }
                if (count == 0)
                {
                    return start;
                }
            } while (++start < str.Length);
            return -1;
        }
    }
    public class ReplaceString : Mod
    {
        public static Mod Command;
        public List<Hook> hooks;
        public List<ILHook> ilHooks;
        public ModCatcher catcher;
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
            bool hasReplace = false;
            hooks.Add(new Hook(typeof(Mod).GetMethod("Autoload", BindingFlags.Instance | BindingFlags.NonPublic), (Autoload orig, Mod mod) =>
            {
                orig(mod);
                if (hasReplace)
                {
                    try
                    {
                        import.PostModLoad();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Post Load Error : {ex}");
                    }
                }
            }));

            ReplaceStringConfig config = new ReplaceStringConfig();
            try
            {
                string path = $"{Main.SavePath}/ModConfigs/_ReplaceString__ReplaceStringConfig.json";
                string json = File.Exists(path) ? File.ReadAllText(path) : "{}";
                JsonConvert.PopulateObject(json, config);
            }
            catch (Exception)
            {
                ;
            }
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
                    string fileName = $"{Main.SavePath}/Mods/ReplaceString/{name}_{Language.ActiveCulture.Name}.hjson";
                    if (!(config?.AutoloadModList.Any(d => d.Name == name) ?? false) || !File.Exists(fileName))
                    {
                        return;
                    }
                    using var file = new FileStream(fileName, FileMode.Open);
                    try
                    {
                        import = new Import(HjsonValue.Load(file));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Hjson Load Error : {ex}");
                    }
                    try
                    {
                        using (modFile.Open())
                        {
                            import.PreModLoad(name, asm, modFile.GetModAssembly());
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Pre Load Error : {ex}");
                    }
                    hasReplace = true;
                });
            }));
        }
        public override void Load()
        {
            Command = ModLoader.GetMod("DebugCommands");

            MainFlow.Instance.Append(new ExportCommand() / new FindModFlow() * new ExportConfig() / new ExporAction());
            MainFlow.Instance.Append(new ImportCommand() / new FindModFlow() / new ImportAction());
            MainFlow.Instance.Append(new MakeCommand() / new FindHjson() * new TargetCulture() / new MakeAction());
            MainFlow.Instance.Append(new PackCommand() / new FindDirectory() / new PackAction());
        }

        public override void Unload()
        {
            Command = null;
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
}