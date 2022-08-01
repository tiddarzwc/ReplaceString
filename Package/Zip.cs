using System.IO;
using System.IO.Compression;
using System.Linq;
using _ReplaceString_.ConfigUI.C_Work;
using Hjson;
using Terraria.Localization;

namespace _ReplaceString_.Package
{
    public static class Zip
    {
        public static void ZipHjson(string path)
        {
            var config = ModContent.GetInstance<WorkConfig>();
            var locPath = Path.GetFileNameWithoutExtension(path.Replace("-packed", ""));
            int endIndex = locPath.Length, startIndex = locPath.Length - 1;
            int temp = 2;
            for (; startIndex > 0; startIndex--)
            {
                if (locPath[startIndex] == '-' && --temp == 0)
                {
                    startIndex++;
                    break;
                }
            }
            locPath = locPath.Replace(locPath[startIndex..endIndex], GameCulture.FromCultureName(
                typeof(GameCulture.CultureName)
                .GetEnumValues()
                .Cast<GameCulture.CultureName>()
                .FirstOrDefault(c => c.ToString() == ModContent.GetInstance<WorkConfig>().culture)).Name);
            using var zip = new GZipStream(File.OpenWrite($"{ReplaceString.BasePath}/{locPath}--{config.author.RemoveChars(Path.GetInvalidFileNameChars())}.loc"), CompressionLevel.Optimal);
            using var writer = new BinaryWriter(zip);
            writer.Write(config.author ?? string.Empty);
            writer.Write(config.description ?? string.Empty);
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
