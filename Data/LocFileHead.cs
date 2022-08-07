using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _ReplaceString_.Data
{
    public struct LocFileHead
    {
        public string author;
        public string description;
        public string version;

        public LocFileHead(string author, string description, string version)
        {
            this.author = author;
            this.description = description;
            this.version = version;
        }

        public override string ToString()
        {
            return $"{author} - {version}\n{description}";
        }
    }
}
