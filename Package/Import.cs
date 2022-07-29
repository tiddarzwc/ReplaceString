using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _ReplaceString_.Data;
using Hjson;

using Mono.Cecil.Cil;

using MonoMod.RuntimeDetour;

using Terraria.Localization;

namespace _ReplaceString_.Package
{
    internal class Import
    {
        public string name;
        public Assembly assembly;
        public TreeNode root;
        public TreeNode oldTree;
        public static readonly Dictionary<string, List<ILHook>> hookList = new Dictionary<string, List<ILHook>>();
        public Import(JsonValue json)
        {
            root = TreeNode.ReadHjson(json);
        }
        public void Load()
        {
            if (!ModLoader.TryGetMod(root.name, out var mod))
            {
                throw new Exception("Mod Not Found");
            }
            name = mod.Name;
            assembly = mod.Code;
            MonoModHooks.RequestNativeAccess();
            if (hookList.ContainsKey(name))
            {
                foreach (var hook in hookList[name])
                {
                    hook.Dispose();
                }
                hookList[name].Clear();
            }
            else
            {
                hookList.Add(name, new List<ILHook>());
            }
            oldTree = new Export(mod).head;
            AddTranslation(Language.ActiveCulture);
            AddMapEntry(Language.ActiveCulture);
            AddLdstrAndPath();
            LocalizationLoader.RefreshModLanguage(Language.ActiveCulture);
        }
        public void PreModLoad(string name, Assembly assembly, byte[] asmBytes)
        {
            this.name = name;
            this.assembly = assembly;
            if (hookList.ContainsKey(name))
            {
                foreach (var hook in hookList[name])
                {
                    hook.Undo();
                }
                hookList[name].Clear();
            }
            else
            {
                hookList.Add(name, new List<ILHook>());
            }
            oldTree = new TreeNode(name);
            Export.GetLdstr(oldTree, asmBytes);
            AddMapEntry(Language.ActiveCulture);
            AddLdstrAndPath();
        }
        public void PostModLoad()
        {
            AddTranslation(Language.ActiveCulture);
        }
        public static void TryAddTranslation(ModTranslation trans, TreeNode root, string name, GameCulture culture)
        {
            //用字典查找好一点……但是摸了
            var result = root.children.Find(t => t.name == name);
            if (result != null)
            {
                trans.AddTranslation(culture, (result as Leaf).value);
            }
        }
        public void AddTranslation(GameCulture culture)
        {
            foreach (var t in ModContent.GetContent<IModType>().Where(m => m.Mod.Name == name))
            {
                if (t is ModItem modItem)
                {
                    TryAddTranslation(modItem.DisplayName, root["ItemName"], t.Name, culture);
                    TryAddTranslation(modItem.Tooltip, root["ItemTooltip"], t.Name, culture);
                }
                else if (t is ModProjectile modProj)
                {
                    TryAddTranslation(modProj.DisplayName, root["ProjectileName"], t.Name, culture);
                }
                else if (t is DamageClass damage)
                {
                    TryAddTranslation(damage.ClassName, root["DamageClassName"], t.Name, culture);
                }
                else if (t is InfoDisplay info)
                {
                    TryAddTranslation(info.InfoName, root["InfoDisplayName"], t.Name, culture);
                }
                else if (t is ModBiome modBiome)
                {
                    TryAddTranslation(modBiome.DisplayName, root["BiomeName"], t.Name, culture);
                }
                else if (t is ModBuff modBuff)
                {
                    TryAddTranslation(modBuff.DisplayName, root["BuffName"], t.Name, culture);
                    TryAddTranslation(modBuff.Description, root["BuffDescription"], t.Name, culture);
                }
                else if (t is ModNPC modNPC)
                {
                    TryAddTranslation(modNPC.DisplayName, root["NPCName"], t.Name, culture);
                }
                else if (t is ModPrefix modPrefix)
                {
                    TryAddTranslation(modPrefix.DisplayName, root["Prefix"], t.Name, culture);
                }
                else if (t is ModTile modTile)
                {
                    TryAddTranslation(modTile.ContainerName, root["Containers"], t.Name, culture);
                }
            }
        }
        public void AddMapEntry(GameCulture culture)
        {
            var mapEntry = new TreeNode("MapEntry");
            string[] names = new string[] { "tileEntries", "wallEntries" };
            foreach (var name in names)
            {
                var node = root[name];
                var tileEntries = typeof(ModLoader).Assembly.DefinedTypes.First(t => t?.Name == "MapLoader").AsType().GetValue(name);
                var it = tileEntries.Invoke("GetEnumerator");
                while ((bool)it.Invoke("MoveNext"))
                {
                    object current = it.GetValue("Current");
                    ushort type = (ushort)current.GetValue("Key");
                    object entries = current.GetValue("Value");
                    ModTile tile = TileLoader.GetTile(type);
                    if (tile?.Mod?.Name != name)
                    {
                        continue;
                    }
                    var it2 = entries.Invoke("GetEnumerator");
                    int count = 0;
                    while ((bool)it2.Invoke("MoveNext"))
                    {
                        object entry = it2.GetValue("Current");
                        ModTranslation trans = entry.GetValue<ModTranslation>("translation");
                        if (trans != null)
                        {
                            if (count++ == 0)
                            {
                                TryAddTranslation(trans, root, tile.Name, culture);
                            }
                            else
                            {
                                TryAddTranslation(trans, root, $"{tile.Name}_{count}", culture);
                            }
                        }
                    }
                }
                mapEntry += node;
            }
        }

        public Dictionary<string, MethodBase> methods = new Dictionary<string, MethodBase>();
        //public Dictionary<string, MethodBase> methodsPath = new Dictionary<string, MethodBase>();
        public static ILHookConfig config = new ILHookConfig()
        {
            ManualApply = false
        };
        public void AddLdstrAndPath()
        {
            var ldstr = root["Ldstr"];
            //var path = root["Path"];
            foreach (var type in assembly.DefinedTypes)
            {
                foreach (var method in type.DeclaredMethods)
                {
                    string methodName = method.Name.Replace("get_", "").Replace(".", "").Replace("|", "");
                    if (type.DeclaredMethods.Count(m => m.Name == method.Name) > 1)
                    {
                        methodName = UUtils.GetSpecialMethodName(method);
                    }
                    methods.Add($"{type.FullName}.{methodName}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                    //methodsPath.Add($"{type.FullName.Replace('.', '_')}_{methodName}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                }
                foreach (var method in type.DeclaredConstructors)
                {
                    string methodName = method.Name.Replace("get_", "").Replace(".", "").Replace("|", "");
                    if (type.DeclaredConstructors.Count(m => m.Name == method.Name) > 1)
                    {
                        methodName = UUtils.GetSpecialMethodName(method);
                    }
                    methods.Add($"{type.FullName}.{methodName}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                    //methodsPath.Add($"{type.FullName.Replace('.', '_')}_{methodName}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                }
            }
            var oldstr = oldTree["Ldstr"];
            //var oldpath = oldTree["Path"];
            var it = ldstr.children.Join(oldstr.children, n => n.name, n => n.name, (n1, n2) => (n1, n2));
            foreach (var (n1, n2) in it)
            {
                AddLdstr(n1, n2, string.Empty);
            }
            //AddPath(path, oldpath);
        }

        private void AddLdstr(TreeNode current, TreeNode old, string fullname)
        {
            if (current.children.First() is Leaf)
            {
                //Replace部分
                var strs = from n in
                               from l in current.children select l as Leaf
                           join o in
                                from l in old.children select l as Leaf
                           on n.name equals o.name
                           select (n, o);

                Queue<(string oldString, string newString)> replace = new Queue<(string oldString, string newString)>();
                foreach (var (newString, oldString) in strs)
                {
                    if (newString.GetRealValue() != oldString.GetRealValue())
                    {
                        replace.Enqueue((oldString.value, newString.value));
                    }
                }
                if (replace.Count > 0)
                {
                    var method = methods[$"{fullname}.{current.name}"];
                    hookList[name].Add(new ILHook(method, il =>
                    {
                        foreach (var ins in il.Instrs)
                        {
                            if (ins.OpCode == OpCodes.Ldstr && ins.Operand.ToString() == replace.Peek().oldString)
                            {
                                ins.Operand = replace.Dequeue().newString;
                                if (replace.Count == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }, ref config));
                }
            }
            else
            {
                var it = current.children.Join(old.children, n => n.name, n => n.name, (n1, n2) => (n1, n2));
                foreach (var (n1, n2) in it)
                {
                    if (string.IsNullOrEmpty(fullname))
                    {
                        AddLdstr(n1, n2, $"{current.name}");
                    }
                    else
                    {
                        AddLdstr(n1, n2, $"{fullname}.{current.name}");
                    }
                }
            }
        }

        //private void AddPath(TreeNode current, TreeNode old)
        //{
        //    Queue<(string oldString, string newString)> replace = new Queue<(string oldString, string newString)>();
        //    Queue<MethodBase> replacedMethod = new Queue<MethodBase>();
        //    var it = old.children.Where(c => c is Leaf).GetEnumerator();
        //    MethodBase method = null;
        //    foreach (Leaf child in current.children.Where(c => c is Leaf))
        //    {
        //        if (!it.MoveNext())
        //        {
        //            throw new Exception("Invalid path add");
        //        }
        //        var leaf = it.Current as Leaf;
        //        if (child.GetRealValue() != leaf.GetRealValue())
        //        {
        //            for (int i = child.name.Length - 1; i >= 0; --i)
        //            {
        //                if (child.name[i] == '_')
        //                {
        //                    var t = methodsPath[child.name[0..i]];
        //                    if (method != t)
        //                    {
        //                        replacedMethod.Enqueue(t);
        //                    }

        //                    replace.Enqueue((leaf.value, child.value));
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    while (replacedMethod.Count > 0)
        //    {
        //        method = replacedMethod.Dequeue();
        //        hookList[name].Add(new ILHook(method, il =>
        //        {
        //            foreach (var ins in il.Instrs)
        //            {
        //                if (ins.OpCode == OpCodes.Ldstr && ins.Operand.ToString() == replace.Peek().oldString)
        //                {
        //                    ins.Operand = replace.Dequeue().newString;
        //                }
        //            }
        //        }, ref config));
        //    }
        //}

    }
}
