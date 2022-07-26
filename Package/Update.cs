using System.Collections.Generic;
using System.Linq;
using System.Text;
using _ReplaceString_.Data;

namespace _ReplaceString_.Package
{
    internal static class Update
    {
        public static StringBuilder CacheInfo = new StringBuilder(10000);
        public static void UpdateTree(TreeNode oldRoot, TreeNode newRoot, TreeNode translation)
        {
            if (oldRoot.children.FirstOrDefault() is not Leaf)
            {
                Dictionary<string, bool> old = new Dictionary<string, bool>(
                    oldRoot.children.Select(c => new KeyValuePair<string, bool>(c.name, false))
                    );
                foreach (TreeNode child in newRoot.children)
                {
                    if (old.ContainsKey(child.name))
                    {
                        old[child.name] = true;
                        UpdateTree(oldRoot[child.name], child, translation[child.name]);
                    }
                    else
                    {
                        CacheInfo.AppendLine($"New Node : {child.name}");
                    }
                }
                foreach (var key in old.Where(pair => !pair.Value).Select(pair => pair.Key))
                {
                    CacheInfo.AppendLine($"Missing Node : {key}");
                }
            }
            else
            {
                var oldLeafs = oldRoot.children.Cast<Leaf>().ToDictionary(l => l.name, l => l.value);
                Dictionary<(string name, string value), bool> exist = new Dictionary<(string, string), bool>
                    (oldLeafs.Select(c => new KeyValuePair<(string, string), bool>((c.Key, c.Value), false)));
                (string name, string value) leaf;
                foreach (Leaf child in newRoot.children.Cast<Leaf>())
                {
                    if (exist.ContainsKey((child.name, child.value)))
                    {
                        exist[(child.name, child.value)] = true;
                        child.value = (translation[child.name] as Leaf).value;
                    }
                    else if((leaf = exist.Keys.FirstOrDefault(k => k.name == child.name, (null, null))).name != null)
                    {
                        exist[(child.name, child.value)] = true;
                        CacheInfo.AppendLine($"\tChanged Entry : {{");
                        CacheInfo.AppendLine($"\t\tOld : {leaf.ToString().Replace("\n", "\n\t\t")}");
                        CacheInfo.AppendLine($"\t\tNew : {child.ToString().Replace("\n", "\n\t\t")}");
                    }
                    else
                    {
                        CacheInfo.AppendLine($"\tNew Entry : ({child.name}, {child.value})".Replace("\n", "\n\t"));
                    }
                }

                foreach (var (key, value) in exist.Where(pair => !pair.Value).Select(pair => pair.Key))
                {
                    CacheInfo.AppendLine($"Missing Entry : ({key} : {value}) - ({translation[key]})");
                }
              
            }
        }

    }
}
