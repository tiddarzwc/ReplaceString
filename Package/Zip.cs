using System.IO;
using System.IO.Compression;
using _ReplaceString_.ConfigUI.C_Work;
using Hjson;

namespace _ReplaceString_.Package
{
    public static class Zip
    {
        public static void ZipHjson(string path)
        {
            using var zip = new GZipStream(File.OpenWrite(Path.ChangeExtension(path.Replace("-packed", ""), ".loc")), CompressionLevel.Optimal);
            using var writer = new BinaryWriter(zip);
            var config = ModContent.GetInstance<WorkConfig>();
            writer.Write(config.author);
            writer.Write(config.description);
            var hjson = HjsonValue.Load(File.OpenRead(path));
            hjson.Save(zip, Stringify.Plain);
        }
        public static JsonValue UnZipHjson(string path, out (string name, string description) info)
        {
            using var zip = new GZipStream(File.OpenRead(path), CompressionMode.Decompress);
            using var reader = new BinaryReader(zip);
            info = (reader.ReadString(), reader.ReadString());
            return HjsonValue.Load(zip);
        }
    }
}
