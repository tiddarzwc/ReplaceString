using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Hjson;

using Terraria.Localization;

namespace _ReplaceString_.Data
{
    internal class TreeNode
    {
        public string name;
        public List<TreeNode> children = new List<TreeNode>();
        public Dictionary<string, TreeNode> lookup;
        public TreeNode this[string name]
        {
            get
            {
                lookup ??= new Dictionary<string, TreeNode>();
                if (!lookup.TryGetValue(name, out TreeNode node))
                {
                    node = Search(name);
                    lookup.Add(name, node);
                }
                return node;
            }
            set
            {

            }
        }
        public TreeNode(string name)
        {
            this.name = name;
        }
        public TreeNode Search(string name)
        {
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(this);
            TreeNode node;
            while (queue.Count > 0)
            {
                node = queue.Dequeue();
                foreach (var t in node.children)
                {
                    if (t.name == name)
                    {
                        return t;
                    }
                    queue.Enqueue(t);
                }
            }
            node = new TreeNode(name);
            children.Add(node);
            return node;
        }
        public static TreeNode operator +(TreeNode parent, TreeNode child)
        {
            if (child.name == string.Empty)
            {
                return parent;
            }
            parent.children.Add(child);
            return parent;
        }
        public static TreeNode operator +(TreeNode parent, ModTranslation trans)
        {
            string text = trans.GetTranslation(Language.ActiveCulture);
            parent.children.Add(new Leaf(trans.Key, text));
            return parent;
        }
        public override string ToString()
        {
            return name;
        }
        public static readonly Regex endNumber = new Regex(@"\d+$", RegexOptions.Compiled);
        public StringBuilder BuildHjson(int depth)
        {
            StringBuilder sb = new StringBuilder();
            string tab = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                tab += '\t';
            }
            if (this is not Leaf)
            {
                sb.AppendLine(tab + name + " : {");
                string value = null;
                foreach (var child in children.OrderBy(c =>
                {
                    var match = endNumber.Match(c.name);
                    if (match.Success)
                    {
                        value = match.Value;
                        return c.name[..match.Index];
                    }
                    value = string.Empty;
                    return c.name;
                }).ThenBy(_ => value))
                {
                    sb.Append(child.BuildHjson(depth + 1));
                }
                sb.AppendLine(tab + '}');
            }
            else
            {
                sb.AppendLine(tab + ToString().Replace("\n", '\n' + tab));
            }
            return sb;
        }
        public static TreeNode ReadHjson(JsonValue json)
        {
            var obj = json as JsonObject;
            Debug.Assert(obj.Keys.Count == 1);
            TreeNode head = new TreeNode(obj.Keys.First());
            foreach (var (key, value) in obj.Values.First() as JsonObject)
            {
                head += ReadHjson(key, value);
            }
            return head;
        }
        private static TreeNode ReadHjson(string key, JsonValue value)
        {
            if (value is JsonObject obj)
            {
                TreeNode node = new TreeNode(key);
                foreach (var (k, v) in obj)
                {
                    node += ReadHjson(k, v);
                }
                return node;
            }
            else
            {
                return new Leaf(key, value.JsonType switch
                {
                    JsonType.Boolean => value.Qb().ToString(),
                    JsonType.String => value.Qs().ToString(),
                    JsonType.Number => value.Qi().ToString(),
                    JsonType.Array => value.Qa().ToString(),
                    _ => throw new NotImplementedException()
                });
            }
        }
    }
}
