using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using _ReplaceString_.Command;
using _ReplaceString_.Translator.UI;
using Hjson;

using Terraria.Localization;

namespace _ReplaceString_
{
    internal class TreeNode : IEquatable<TreeNode>
    {
        public string name;
        public List<TreeNode> children = new List<TreeNode>();
        public Dictionary<string, TreeNode> lookup = new Dictionary<string, TreeNode>();
        public TreeNode this[string name]
        {
            get
            {
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
        public StringBuilder BuildHjson(int depth)
        {
            StringBuilder sb = new StringBuilder();
            string tab = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                tab += ExportConfig.SpaceChar;
            }
            if (this is not Leaf)
            {
                sb.AppendLine(tab + name + ExportConfig.Space + ':' + ExportConfig.Space + '{');
                foreach (var child in children)
                {
                    if (!ExportConfig.Tight)
                    {
                        sb.AppendLine(child.BuildHjson(depth + 1).ToString());
                    }
                    else
                    {
                        sb.Append(child.BuildHjson(depth + 1));
                    }
                }
                sb.AppendLine(tab + '}');
            }
            else
            {
                sb.AppendLine(tab + ToString().Replace("\n", '\n' + tab));
            }
            return sb;
        }
        //public static TreeNode ReadHjson(StreamReader reader, string name = null)
        //{
        //    if (name is null)
        //    {
        //        name = reader.ReadLine().Trim('\t', ' ');
        //        name = name[..name.IndexOf(':')].Trim('\t', ' ');
        //    }
        //    TreeNode node = new TreeNode(name);
        //    while (!reader.EndOfStream)
        //    {
        //        string text = reader.ReadLine();
        //        string trim = text.Trim('\t', ' ');
        //        if (trim == "}")
        //        {
        //            break;
        //        }
        //        else if (trim == string.Empty)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            if (Regex.IsMatch(text, "(?<!:.*): *{(?![a-zA-Z]+)"))
        //            {
        //                node += ReadHjson(reader, trim[..trim.IndexOf(':')].Trim('\t', ' '));
        //            }
        //            else
        //            {
        //                node += Leaf.Read(text, reader);
        //            }
        //        }
        //    }
        //    return node;
        //}
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
        public virtual bool Equals(TreeNode other)
        {
            return name == other.name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TreeNode);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
