using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _ReplaceString_.Translator
{
    internal class MakeConfig
    {
        public MakeConfig(string modName, int targetCultureID, bool ldstrFold, bool ignoreEmpty)
        {
            ModName = modName;
            TargetCultureID = targetCultureID;
            LdstrFold = ldstrFold;
            IgnoreEmpty = ignoreEmpty;
        }

        public string ModName { get; set; }
        public int TargetCultureID { get; set; }
        public bool LdstrFold { get; set; }
        public bool IgnoreEmpty { get; set; }
    }
}
