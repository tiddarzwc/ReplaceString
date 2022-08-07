using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.Core;

namespace _ReplaceString_.Data
{
    internal class DefaultTranslation
    {
        private static string ConfigPath => $"{ReplaceString.BasePath}/Config.json";
        private static DefaultTranslation instance = null;
        public Dictionary<string, string> DefaultPath { get; set; } = new Dictionary<string, string>();
        public static void Load()
        {
            if (!File.Exists(ConfigPath))
            {
                instance = new DefaultTranslation();
                Save();
                return;
            }
          
            instance = JsonSerializer.Deserialize<DefaultTranslation>(File.ReadAllText(ConfigPath));
        }
        public static void Save()
        {
            if(!Directory.Exists(ReplaceString.BasePath))
            {
                Directory.CreateDirectory(ReplaceString.BasePath);
            }
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(instance));
        }
        public static string Get(string name, string ext)
        {
            string res = null;
            var files = Directory.GetFiles(ReplaceString.BasePath, $"*.{ext}");
            if(instance.DefaultPath.TryGetValue(name, out var val) && files.Any(path =>
            {
                res = path;
                return Path.GetFileName(path) == val;
            }))
            {
                return res;
            }
            return Directory.GetFiles(ReplaceString.BasePath, $"{name}*{ext}").FirstOrDefault();
        }
        public static void Set(string mod, string path)
        {
            instance.DefaultPath[mod] = path;
            Save();
        }
    }
}
