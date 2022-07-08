using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using DebugCommands.Flow.DataFlows;

namespace _ReplaceString_.Command
{
    internal class ExportConfig : DataFlow
    {
        public static bool tight;
        public static int spaceCount = 0;
        public static string Space => tight ? string.Empty : " ";
        public static string SpaceChar => spaceCount == 0 ? "\t" : MultiChar(' ', spaceCount);
        private static string MultiChar(char c, int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }
        public override IEnumerable<string> GetAutoComplete()
        {
            return new string[] { "-t", "-l" };
        }
        public override bool TryFlow(in string input)
        {
            if (input.Contains("-t"))
            {
                tight = true;
            }
            else
            {
                tight = false;
            }
            if (Regex.IsMatch(input, "-[1-9]"))
            {
                spaceCount = int.Parse(Regex.Matches(input, "-([1-9])")[1].Value);
            }
            useInput = false;
            return !DebugCommands.DebugCommands.disableUseDefault;
        }
    }
}
