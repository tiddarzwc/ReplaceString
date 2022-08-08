using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using _ReplaceString_.Data;
using _ReplaceString_.Package;

namespace _ReplaceString_;

public static class Network
{
    /// <summary>
    /// 根据fileName获得metaData
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static LocMetaData GetFileInfo(string fileName)
    {
        return new LocMetaData();
    }

    /// <summary>
    /// 根据modName获得一个mod所有汉化包的metaData
    /// </summary>
    /// <param name="modName"></param>
    /// <returns></returns>
    public static IEnumerable<LocMetaData> GetModInfo(string modName)
    {
        return new LocMetaData[] { new LocMetaData() };
    }

    /// <summary>
    /// 根据fileName下载汉化包
    /// </summary>
    /// <param name="fileName"></param>
    public static void Download(string fileName)
    {
        var targetDirector = ReplaceString.BasePath;
    }

}
