using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _ReplaceString_.Translator
{
    internal static class Update
    {
        public static StringBuilder CacheInfo = new StringBuilder(10000);
        public static void UpdateTree(TreeNode oldRoot, TreeNode newRoot, TreeNode translation)
        {
            Dictionary<string, bool> old;
            if (oldRoot.children.FirstOrDefault() is not Leaf)
            {
                old = new Dictionary<string, bool>(
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
                foreach(var key in old.Where(pair => !pair.Value).Select(pair => pair.Key))
                {
                    CacheInfo.AppendLine($"Missing Node : {key}");
                }
            }
            else
            {
                var oldLeafs = oldRoot.children.Cast<Leaf>().ToList();
                old = new Dictionary<string, bool>(oldLeafs.Select(c => new KeyValuePair<string, bool>(c.value, false)));
                
                foreach (Leaf child in newRoot.children.Cast<Leaf>())
                {
                    if (old.ContainsKey(child.value))
                    {
                        old[child.value] = true;
                        string temp = (translation[oldLeafs.Where(pair => pair.value == child.value).First().name] as Leaf).value;
                        oldLeafs.RemoveAt(oldLeafs.FindIndex(pair => pair.value == child.value));
                        child.value = temp;
                    }
                    else
                    {
                        CacheInfo.AppendLine($"New Entry : ({child.name}, {child.value})");
                    }
                }

                foreach (var value in old.Where(pair => !pair.Value).Select(pair => pair.Key))
                {
                    var name = oldLeafs.First(l => l.value == value).name;
                    CacheInfo.AppendLine($"Missing Entry : ({name} : {value}) - ({translation[name]})");
                    oldLeafs.RemoveAt(oldLeafs.FindIndex(l => l.value == value));
                }
            }
        }

    }
}
