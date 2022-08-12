using System.Diagnostics;
using System.Net;
using System.Text;
using _ReplaceString_;
using NUnit.Framework.Internal;

namespace ut;

public class Tests
{
    private Process? _p;
    private Logger? _log;

    [SetUp]
    public void Setup()
    {
        var di = new DirectoryInfo($"{Path.GetTempPath()}/localizer");
        di.Delete(true);
        _p = new Process();
        _p.StartInfo.FileName = "localizer";
        _p.StartInfo.Arguments = ($"-f {Path.GetTempPath()}/localizer/file -d {Path.GetTempPath()}/localizer/db -D");
        Assert.That(_p.Start(), Is.EqualTo(true));
        _log = new Logger("network_ut", InternalTraceLevel.Info, Console.Out);
        Thread.Sleep(1000);
    }

    [TearDown]
    public void TearDown()
    {
        _p.Kill();
    }

    [Test]
    public void BehaveTest()
    {
        #region Upload One HJSON

        using (var client = new HttpClient())
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("TEST MOD"), "mod_name");
            content.Add(new StringContent("test.hjson"), "filename");
            content.Add(new StringContent("两面包夹芝士夹面包"), "display_name");
            content.Add(new StringContent("testauthor"), "author");
            content.Add(new StringContent("两面包夹芝士夹面包"), "description");
            content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("test test test")), "file",
                "test.hjson");
            var resp = client.PostAsync($"{Network.BackendAddress}/upload", content).Result;
            _log.Info($"upload status:{resp.StatusCode} content:{resp.Content.ReadAsStringAsync().Result}");
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        #endregion

        #region Check meta

        var meta = Network.GetFileInfo("test.hjson");
        Assert.That(meta.modName, Is.EqualTo("TEST MOD"));
        Assert.That(meta.fileName, Is.EqualTo("test.hjson"));
        Assert.That(meta.displayName, Is.EqualTo("两面包夹芝士夹面包"));
        Assert.That(meta.author, Is.EqualTo("testauthor"));
        Assert.That(meta.description, Is.EqualTo("两面包夹芝士夹面包"));
        _log.Info($"check meta ok");

        #endregion

        #region Check content

        Directory.CreateDirectory($"{Path.GetTempPath()}/localizer/download");
        Network.Download($"{Path.GetTempPath()}/localizer/download", "test.hjson");
        Assert.That(File.ReadAllText($"{Path.GetTempPath()}/localizer/download/test.hjson"),
            Is.EqualTo("test test test"));
        _log.Info($"check content ok");

        #endregion


        #region Check meta-list

        var metas = Network.GetModInfo("TEST MOD");
        Assert.That(metas.Length, Is.EqualTo(1));
        Assert.That(metas[0].modName, Is.EqualTo("TEST MOD"));
        Assert.That(metas[0].fileName, Is.EqualTo("test.hjson"));
        Assert.That(metas[0].displayName, Is.EqualTo("两面包夹芝士夹面包"));
        Assert.That(metas[0].author, Is.EqualTo("testauthor"));
        Assert.That(metas[0].description, Is.EqualTo("两面包夹芝士夹面包"));
        _log.Info($"check meta-list ok");

        #endregion

        #region Upload Another HJSON

        using (HttpClient client = new HttpClient())
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("TEST MOD"), "mod_name");
            content.Add(new StringContent("test.hjson1"), "filename");
            content.Add(new StringContent("两面包夹芝士夹面包1"), "display_name");
            content.Add(new StringContent("testauthor1"), "author");
            content.Add(new StringContent("两面包夹芝士夹面包1"), "description");
            content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("test test test1")), "file",
                "test.hjson");
            var resp = client.PostAsync($"{Network.BackendAddress}/upload", content).Result;
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _log.Info($"upload another status:{resp.StatusCode} content:{resp.Content.ReadAsStringAsync().Result}");
        }

        #endregion


        #region Check another meta

        meta = Network.GetFileInfo("test.hjson1");
        Assert.That(meta.modName, Is.EqualTo("TEST MOD"));
        Assert.That(meta.fileName, Is.EqualTo("test.hjson1"));
        Assert.That(meta.displayName, Is.EqualTo("两面包夹芝士夹面包1"));
        Assert.That(meta.author, Is.EqualTo("testauthor1"));
        Assert.That(meta.description, Is.EqualTo("两面包夹芝士夹面包1"));
        _log.Info($"check another meta ok");

        #endregion

        #region Check content

        Directory.CreateDirectory($"{Path.GetTempPath()}/localizer/download");
        Network.Download($"{Path.GetTempPath()}/localizer/download", "test.hjson1");
        Assert.That(File.ReadAllText($"{Path.GetTempPath()}/localizer/download/test.hjson1"),
            Is.EqualTo("test test test1"));
        _log.Info($"check another content ok");

        #endregion


        #region Check meta-list

        metas = Network.GetModInfo("TEST MOD");
        Assert.That(metas.Length, Is.EqualTo(2));
        Assert.That(metas[0].modName, Is.EqualTo("TEST MOD"));
        Assert.That(metas[0].fileName, Is.EqualTo("test.hjson"));
        Assert.That(metas[0].displayName, Is.EqualTo("两面包夹芝士夹面包"));
        Assert.That(metas[0].author, Is.EqualTo("testauthor"));
        Assert.That(metas[0].description, Is.EqualTo("两面包夹芝士夹面包"));


        Assert.That(metas[1].modName, Is.EqualTo("TEST MOD"));
        Assert.That(metas[1].fileName, Is.EqualTo("test.hjson1"));
        Assert.That(metas[1].displayName, Is.EqualTo("两面包夹芝士夹面包1"));
        Assert.That(metas[1].author, Is.EqualTo("testauthor1"));
        Assert.That(metas[1].description, Is.EqualTo("两面包夹芝士夹面包1"));
        _log.Info($"check another meta-list ok");

        #endregion
    }
}