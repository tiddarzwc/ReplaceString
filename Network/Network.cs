using System.Net.Http.Json;
using _ReplaceString_.Data;

namespace _ReplaceString_;

public static class Network
{
    public const string BackendAddress = "http://localhost:8888";

    /// <summary>
    /// 根据fileName获得metaData
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static LocMetaData GetFileInfo(string fileName)
    {
        using var client = new HttpClient();
        return client.GetFromJsonAsync<LocMetaData>($"{BackendAddress}/meta?file={fileName}").Result;
    }

    /// <summary>
    /// 根据modName获得一个mod所有汉化包的metaData
    /// </summary>
    /// <param name="modName"></param>
    /// <returns></returns>
    public static LocMetaData[] GetModInfo(string modName)
    {
        using var client = new HttpClient();
        return client.GetFromJsonAsync<LocMetaData[]>($"{BackendAddress}/list?mod={modName}").Result;
    }

    /// <summary>
    /// 根据fileName下载汉化包
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="targetDir"></param>
    public static void Download(string targetDir, string fileName)
    {
        using var file = new FileStream(Path.Combine(targetDir, fileName), FileMode.Create);
        using var client = new HttpClient();
        client.GetStreamAsync($"{BackendAddress}/download?file={fileName}").Result.CopyTo(file);
    }
}