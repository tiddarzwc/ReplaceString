using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Terraria.Localization;
using Terraria.ModLoader.Core;

namespace _ReplaceString_
{


    internal class Export
    {
        public Mod mod;
        public HashSet<string> duplicate = new HashSet<string>();
        public TreeNode head;
        public Export() { }
        public Export(Mod mod)
        {
            this.mod = mod;
            head = new TreeNode(mod.Name);
            _ = head + new TreeNode("ItemName") + new TreeNode("ItemTooltip")
                    + new TreeNode("ProjectileName") + new TreeNode("DamageClassName")
                    + new TreeNode("InfoDisplayName") + new TreeNode("BiomeName")
                    + new TreeNode("BuffName") + new TreeNode("BuffDescription")
                    + new TreeNode("NPCName") + new TreeNode("Prefix")
                    + new TreeNode("Containers");

            foreach (var t in ModContent.GetContent<IModType>().Where(t => t.Mod == mod))
            {
                if (t is ModItem modItem)
                {
                    head["ItemName"] += modItem.DisplayName;
                    head["ItemTooltip"] += modItem.Tooltip;
                }
                else if (t is ModProjectile modProj)
                {
                    head["ProjectileName"] += modProj.DisplayName;
                }
                else if (t is DamageClass damage)
                {
                    head["DamageClassName"] += damage.ClassName;
                }
                else if (t is InfoDisplay info)
                {
                    head["InfoDisplayName"] += info.InfoName;
                }
                else if (t is ModBiome modBiome)
                {
                    head["BiomeName"] += modBiome.DisplayName;
                }
                else if (t is ModBuff modBuff)
                {
                    head["BuffName"] += modBuff.DisplayName;
                    head["BuffDescription"] += modBuff.Description;
                }
                else if (t is ModNPC modNPC)
                {
                    head["NPCName"] += modNPC.DisplayName;
                }
                else if (t is ModPrefix modPrefix)
                {
                    head["Prefix"] += modPrefix.DisplayName;
                }
                else if (t is ModTile modTile)
                {
                    head["Containers"] += modTile.ContainerName;
                }
            }
            GetMapEntry();
            BuildDuplicate();
            GetLdstr();
        }
        public static void GetLdstr(TreeNode root, string name, byte[] asmBytes, HashSet<string> duplicate)
        {
            var ldstr = new TreeNode("Ldstr");
            var path = new TreeNode("Path");
            var asm = AssemblyDefinition.ReadAssembly(new MemoryStream(asmBytes));
            var module = asm.MainModule;
            TreeNode current = ldstr;
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods.Where(m => !m.IsAbstract && m.HasBody))
                {
                    int pathCount = 0;
                    int count = 0;
                    foreach (var instr in method.Body.Instructions)
                    {
                        if (instr.OpCode == OpCodes.Ldstr)
                        {
                            switch (IsValid(instr, name, duplicate))
                            {
                                case CheckResult.True:
                                    string methodName = method.Name.Replace("get_", "").Replace(".", "").Replace("|", "");
                                    if (type.Methods.Count(m => m.Name == method.Name) > 1)
                                    {
                                        foreach (var p in method.Parameters)
                                        {
                                            methodName += '_' + p.ParameterType.Name.Replace("[", "").Replace(",", "").Replace("]", "s").Replace("&", "Ref");
                                        }
                                    }
                                    string[] key = $"{type.FullName}.{methodName}".Replace("<", "").Replace(">", "").Replace("`", "_").Split('.');
                                    if (key.Length > 0)
                                    {
                                        for (int i = 0; i < key.Length; i++)
                                        {
                                            current = current[key[i]];
                                        }
                                        current += new Leaf($"{count}", instr.Operand.ToString());
                                    }
                                    current = ldstr;
                                    break;
                                case CheckResult.False:
                                    break;
                                case CheckResult.Path:
                                    path += new Leaf($"{type.FullName.Replace('.', '_')}_{method.Name.Replace("get_", "").Replace(".", "")}_{pathCount++}".Replace("<", "").Replace(">", "").Replace("`", "_"), instr.Operand.ToString());
                                    break;
                                default:
                                    break;
                            }
                            ++count;
                        }
                    }
                }
            }
            root += ldstr;
            root += path;
        }
        public void GetLdstr()
        {
            GetLdstr(head, mod.Name, mod.GetValue<TmodFile>("File").GetModAssembly(), duplicate);
        }
        public void GetMapEntry()
        {
            var mapEntry = new TreeNode("MapEntry");
            string[] names = new string[] { "tileEntries", "wallEntries" };
            foreach (var name in names)
            {
                var node = new TreeNode(name);
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
                        if (trans != null && trans.GetTranslation(Language.ActiveCulture) != string.Empty)
                        {
                            if (count++ == 0)
                            {
                                node += new Leaf(tile.Name, trans.GetTranslation(Language.ActiveCulture));
                            }
                            else
                            {
                                node += new Leaf($"{tile.Name}_{count}", entry.GetValue<ModTranslation>("translation").GetTranslation(Language.ActiveCulture));
                            }
                        }
                    }
                }
                mapEntry += node;
            }
            head += mapEntry;
        }
        public void Hjson(Stream stream)
        {
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(head.BuildHjson(0));
        }
        public void BuildDuplicate()
        {
            BuildDuplicate(head);
        }
        public void BuildDuplicate(TreeNode node)
        {
            if (node is Leaf leaf)
            {
                if (duplicate.Contains(leaf.value))
                {
                    return;
                }
                duplicate.Add(leaf.value);
            }
            else
            {
                foreach (var child in node.children)
                {
                    BuildDuplicate(child);
                }
            }
        }
        public static CheckResult IsValid(Instruction ins, string modName, HashSet<string> duplicate)
        {
            string operand = ins.Operand.ToString();
            if (operand.Trim('\t', ' ') == string.Empty)
            {
                return CheckResult.False;
            }
            if (duplicate.Contains(operand))//ModTranslation
            {
                return CheckResult.False;
            }
            if (ins.Next != null)
            {
                var next = ins.Next;
                //Dictionary的[string key] {get}
                if (next.Operand?.ToString().Contains("get_Item") ?? false)
                {
                    return CheckResult.False;
                }
            }

            if (!operand.Contains('\n'))
            {
                int count = operand.Count(ch => ch == '/' || ch == '\\');
                if (count > 0 && (ModLoader.Mods.Any(mod => operand.StartsWith(modName)) || operand.StartsWith("Terraria")))
                {
                    return CheckResult.Path;
                }
                else if (count > 1)
                {
                    return CheckResult.Path;
                }
            }
            return CheckResult.True;
        }
    }

    public enum CheckResult
    {
        True,
        False,
        Path
    }
}
