using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Hjson;

using Mono.Cecil.Cil;

using MonoMod.RuntimeDetour;

using Terraria.Localization;

namespace _ReplaceString_
{
    internal class Import
    {
        public Mod mod;
        public TreeNode head;
        public Export export;
        public static readonly Dictionary<Mod, List<ILHook>> hookList = new Dictionary<Mod, List<ILHook>>();
        public Import(JsonValue json)
        {
            head = TreeNode.ReadHjson(json);
        }
        public void Load()
        {
            if (!ModLoader.TryGetMod(head.name, out mod))
            {
                throw new Exception("Mod Not Found");
            }
            MonoModHooks.RequestNativeAccess();
            if (hookList.ContainsKey(mod))
            {
                foreach (var hook in hookList[mod])
                {
                    hook.Dispose();
                }
                hookList[mod].Clear();
            }
            else
            {
                hookList.Add(mod, new List<ILHook>());
            }
            export = new Export(mod);
            AddTranslation(Language.ActiveCulture);
            AddMapEntry(Language.ActiveCulture);
            AddLdstrAndPath();
            LocalizationLoader.RefreshModLanguage(Language.ActiveCulture);
        }
        public void PreModLoad()
        {
            if (!ModLoader.TryGetMod(head.name, out mod))
            {
                throw new Exception("Mod Not Found");
            }
            MonoModHooks.RequestNativeAccess();
            if (hookList.ContainsKey(mod))
            {
                foreach (var hook in hookList[mod])
                {
                    hook.Undo();
                }
                hookList[mod].Clear();
            }
            else
            {
                hookList.Add(mod, new List<ILHook>());
            }
            export = new Export(mod);
            AddMapEntry(Language.ActiveCulture);
            AddLdstrAndPath();
        }
        public void PostModLoad()
        {
            AddTranslation(Language.ActiveCulture);
        }
        public void AddTranslation(GameCulture culture)
        {
            foreach (var t in ModContent.GetContent<IModType>().Where(m => m.Mod == mod))
            {
                if (t is ModItem modItem)
                {
                    modItem.DisplayName.AddTranslation(culture, (head["ItemName"][t.Name] as Leaf).value);
                    modItem.Tooltip.AddTranslation(culture, (head["ItemTooltip"][t.Name] as Leaf).value);
                }
                else if (t is ModProjectile modProj)
                {
                    modProj.DisplayName.AddTranslation(culture, (head["ProjectileName"][modProj.Name] as Leaf).value);
                }
                else if (t is DamageClass damage)
                {
                    damage.ClassName.AddTranslation(culture, (head["DamageClassName"][damage.Name] as Leaf).value);
                }
                else if (t is InfoDisplay info)
                {
                    info.InfoName.AddTranslation(culture, (head["InfoDisplayName"][info.Name] as Leaf).value);
                }
                else if (t is ModBiome modBiome)
                {
                    modBiome.DisplayName.AddTranslation(culture, (head["BiomeName"][modBiome.Name] as Leaf).value);
                }
                else if (t is ModBuff modBuff)
                {
                    modBuff.DisplayName.AddTranslation(culture, (head["BuffName"][modBuff.Name] as Leaf).value);
                    modBuff.Description.AddTranslation(culture, (head["BuffDescription"][modBuff.Name] as Leaf).value);
                }
                else if (t is ModNPC modNPC)
                {
                    modNPC.DisplayName.AddTranslation(culture, (head["NPCName"][modNPC.Name] as Leaf).value);
                }
                else if (t is ModPrefix modPrefix)
                {
                    modPrefix.DisplayName.AddTranslation(culture, (head["Prefix"][modPrefix.Name] as Leaf).value);
                }
                else if (t is ModTile modTile)
                {
                    modTile.ContainerName.AddTranslation(culture, (head["Containers"][modTile.Name] as Leaf).value);
                }
            }
        }
        public void AddMapEntry(GameCulture culture)
        {
            var mapEntry = new TreeNode("MapEntry");
            string[] names = new string[] { "tileEntries", "wallEntries" };
            foreach (var name in names)
            {
                var node = head[name];
                var tileEntries = typeof(ModLoader).Assembly.DefinedTypes.First(t => t?.Name == "MapLoader").AsType().GetValue(name);
                var it = tileEntries.Invoke("GetEnumerator");
                while ((bool)it.Invoke("MoveNext"))
                {
                    object current = it.GetValue("Current");
                    ushort type = (ushort)current.GetValue("Key");
                    object entries = current.GetValue("Value");
                    ModTile tile = TileLoader.GetTile(type);
                    if (tile?.Mod != mod)
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
                                trans.AddTranslation(culture, (head[tile.Name] as Leaf).value);
                            }
                            else
                            {
                                trans.AddTranslation(culture, (head[$"{tile.Name}_{count}"] as Leaf).value);
                            }
                        }
                    }
                }
                mapEntry += node;
            }
        }

        public Dictionary<string, MethodBase> methods = new Dictionary<string, MethodBase>();
        public Dictionary<string, MethodBase> methodsPath = new Dictionary<string, MethodBase>();
        public static ILHookConfig config = new ILHookConfig()
        {
            ManualApply = false
        };
        public void AddLdstrAndPath()
        {
            var ldstr = head["Ldstr"];
            var path = head["Path"];
            foreach (var type in mod.Code.DefinedTypes)
            {
                foreach (var method in type.DeclaredMethods)
                {
                    methods.Add($"{type.FullName}.{method.Name.Replace("get_", "").Replace(".", "")}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                    methodsPath.Add($"{type.FullName.Replace('.', '_')}_{method.Name.Replace("get_", "").Replace(".", "")}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                }
                foreach (var method in type.DeclaredConstructors)
                {
                    methods.Add($"{type.FullName}.{method.Name.Replace("get_", "").Replace(".", "")}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                    methodsPath.Add($"{type.FullName.Replace('.', '_')}_{method.Name.Replace("get_", "").Replace(".", "")}".Replace("<", "").Replace(">", "").Replace("`", "_"), method);
                }
            }
            var oldstr = export.head["Ldstr"];
            var oldpath = export.head["Path"];
            for (int i = 0; i < ldstr.children.Count; i++)
            {
                AddLdstr(ldstr.children[i], oldstr.children[i], string.Empty);
            }
            AddPath(path, oldpath);
        }

        private void AddLdstr(TreeNode current, TreeNode old, string fullname)
        {
            if (current.children.First() is Leaf)
            {
                //Replace部分
                //var newStrings = current.children.Select(t => t as Leaf).GetEnumerator();
                //var oldStrings = old.children.Select(t => t as Leaf).GetEnumerator();
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
                    hookList[mod].Add(new ILHook(method, il =>
                    {
                        foreach (var ins in il.Instrs)
                        {
                            if (ins.OpCode == OpCodes.Ldstr && ins.Operand.ToString() == replace.Peek().oldString)
                            {
                                ins.Operand = replace.Dequeue().newString;
                            }
                        }
                    }, ref config));
                }
            }
            else
            {
                foreach (var pair in current.children.Join(old.children, n => n.name, n => n.name, (n1, n2) => (n1, n2)))
                {
                    if (string.IsNullOrEmpty(fullname))
                    {
                        AddLdstr(pair.n1, pair.n2, $"{current.name}");
                    }
                    else
                    {
                        AddLdstr(pair.n1, pair.n2, $"{fullname}.{current.name}");
                    }
                }
            }
        }

        private void AddPath(TreeNode current, TreeNode old)
        {
            Queue<(string oldString, string newString)> replace = new Queue<(string oldString, string newString)>();
            Queue<MethodBase> replacedMethod = new Queue<MethodBase>();
            var it = old.children.Where(c => c is Leaf).GetEnumerator();
            MethodBase method = null;
            foreach (Leaf child in current.children.Where(c => c is Leaf))
            {
                if (!it.MoveNext())
                {
                    throw new Exception("Invalid path add");
                }
                var leaf = it.Current as Leaf;
                if (child.GetRealValue() != leaf.GetRealValue())
                {
                    for (int i = child.name.Length - 1; i >= 0; --i)
                    {
                        if (child.name[i] == '_')
                        {
                            var t = methodsPath[child.name[0..i]];
                            if (method != t)
                            {
                                replacedMethod.Enqueue(t);
                            }

                            replace.Enqueue((leaf.value, child.value));
                            break;
                        }
                    }
                }
            }
            while (replacedMethod.Count > 0)
            {
                method = replacedMethod.Dequeue();
                hookList[mod].Add(new ILHook(method, il =>
                {
                    foreach (var ins in il.Instrs)
                    {
                        if (ins.OpCode == OpCodes.Ldstr && ins.Operand.ToString() == replace.Peek().oldString)
                        {
                            ins.Operand = replace.Dequeue().newString;
                        }
                    }
                }, ref config));
            }
        }

    }
}
