using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _ReplaceString_.ConfigUI.Work
{
    internal class MakeConfig
    {
        public MakeConfig(string modName, int targetCultureID, bool ignoreEmpty)
        {
            ModName = modName;
            TargetCultureID = targetCultureID;
            IgnoreEmpty = ignoreEmpty;
        }

        public string ModName { get; set; }
        public int TargetCultureID { get; set; }
        public bool IgnoreEmpty { get; set; }
    }
}
