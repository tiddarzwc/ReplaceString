using System.Text.RegularExpressions;

namespace _ReplaceString_.Data
{
    internal class Leaf : TreeNode
    {
        public const string EmptyString = "__EMPTY__";
        public string value;
        public Leaf(string name) : base(name) { }
        public Leaf(string name, string value) : base(name)
        {
            this.value = value == EmptyString ? string.Empty : value;
        }
        public string GetRealValue()
        {
            string val = value;
            string className = $"{name.Split('.')[^1]}";
            if (string.IsNullOrEmpty(val))
            {
                val = EmptyString;
            }
            else if (name == value)
            {
                val = className;
            }

            return val.Trim(' ', '\t', '\n');
        }
        public override string ToString()
        {
            string className = $"{name.Split('.')[^1]}";
            string val = value;
            if (string.IsNullOrEmpty(val))
            {
                val = EmptyString;
            }
            else if (name == value)
            {
                val = className;
            }
            else if (Regex.IsMatch(val, "[\\n\\r]"))
            {
                val = "\n\t'''\n\t" + Regex.Replace(val, "[\\n\\r]", "\n\t") + "\n\t'''";
            }
            else if (Regex.IsMatch(val.Trim(), "^(?:{|}|\\[|\\]|:|,|\"|')"))
            {
                val = '\"' + val.Replace("\n", "\\n").Replace("\"", "\\\"") + "\"";
            }

            return $"{className} : {val}";
        }
        public string GetValue(string space)
        {
            string className = $"{name.Split('.')[^1]}";
            string val = value;
            if (string.IsNullOrEmpty(val))
            {
                val = EmptyString;
            }
            else if (name == value)
            {
                val = className;
            }
            else if (Regex.IsMatch(val, "[\\n\\r]"))
            {
                val = "\n" + space + "\t" + Regex.Replace(val, "[\\n\\r]", "\n" + space + "\t");
            }

            return val;
        }
    }
}
