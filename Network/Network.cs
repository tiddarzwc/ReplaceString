using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using _ReplaceString_.Data;

namespace _ReplaceString_;

public static class Network
{
    public static IEnumerable<(string fileName, LocFileHead head)> GetInfo(string modName)
    {
        return new(string fileName, LocFileHead head)[] { ("测试", new LocFileHead("", "", "")) };
        return Array.Empty<(string fileName, LocFileHead head)>();
    }

    public static void Download(string fileName)
    {

    }


}
