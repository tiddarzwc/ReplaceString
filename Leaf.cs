using System.Text.RegularExpressions;
using _ReplaceString_.Command;
using _ReplaceString_.Translator.UI;

namespace _ReplaceString_
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
            //else
            //{
            //    val = Regex.Replace(
            //       Regex.Replace(Language.GetTextValue(val), "{\\$(.*)}", new MatchEvaluator(match => Language.GetTextValue(match.Groups[1].Value))),
            //       "\\${(.*)}", new MatchEvaluator(match => Language.GetTextValue(match.Groups[1].Value)));
            //}

            return val.Trim(' ', '\t', '\n');
        }
        public override string ToString()
        {
            string className = $"{name.Split('.')[^1]}";
            string val = value;
            //val = Regex.Replace(
            //   Regex.Replace(Language.GetTextValue(val), "{\\$(.*)}", new MatchEvaluator(match => Language.GetTextValue(match.Groups[1].Value))),
            //   "\\${(.*)}", new MatchEvaluator(match => Language.GetTextValue(match.Groups[1].Value)));
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
            else if (Regex.IsMatch(val.Trim(), "^(?:{|}|\\[|\\]|:|,|\\\"|')"))
            {
                val = '\"' + val.Replace("\n", "\\n").Replace("\"", "\\\"") + "\"";
            }
            return $"{className}{ExportConfig.Space}:{ExportConfig.Space}{val}";
        }
        public string GetValue(string space, bool ignoreEmpty)
        {
            string className = $"{name.Split('.')[^1]}";
            string val = value;
            if (string.IsNullOrEmpty(val))
            {
                val = ignoreEmpty ? val : EmptyString;
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
        public override bool Equals(TreeNode other)
        {
            return other is Leaf leaf && name == other.name && value == leaf.value;
        }
        public override int GetHashCode()
        {
            return name.GetHashCode() + value.GetHashCode();
        }
    }
}
