using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using _ReplaceString_.ConfigUI.C_Work;
using _ReplaceString_.Data;
using Hjson;
using Terraria.Localization;

namespace _ReplaceString_.Package
{
    public static class Zip
    {
        public static void ZipHjson(string path, LocMetaData meta)
        {
            var locPath = Path.GetFileNameWithoutExtension(path.Replace("-packed", ""));
            locPath = $"{locPath[..locPath.IndexOf('-')]}-{meta.fileName}-{meta.culture}.loc";
            using var zip = new GZipStream(File.OpenWrite($"{ReplaceString.BasePath}/{locPath}"), CompressionLevel.Optimal);
            using var writer = new BinaryWriter(zip);
            writer.Write(JsonSerializer.Serialize(meta));
            var hjson = HjsonValue.Load(File.OpenRead(path));
            hjson.Save(zip, Stringify.Plain);
        }
        public static JsonValue UnZipHjson(string path, out LocMetaData info)
        {
            using var zip = new GZipStream(File.OpenRead(path), CompressionMode.Decompress);
            using var reader = new BinaryReader(zip);
            info = JsonSerializer.Deserialize<LocMetaData>(reader.ReadString());
            return HjsonValue.Load(zip);
        }

        public static LocMetaData GetMetaData(string path)
        {
            using var zip = new GZipStream(File.OpenRead(path), CompressionMode.Decompress);
            using var reader = new BinaryReader(zip);
            return JsonSerializer.Deserialize<LocMetaData>(reader.ReadString());
        }
    }
}
