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
        public static string Get(string modName)    
        {
            if(instance.DefaultPath.TryGetValue(modName, out var path) && File.Exists(path))
            {
                return path;
            }
            var locs = Directory.GetFiles(ReplaceString.BasePath, "*.loc");
            var hjsons = Directory.GetFiles(ReplaceString.BasePath, "*.hjson");
            var res = locs.FirstOrDefault() ?? hjsons.FirstOrDefault();
            if(res != null)
            {
                instance.DefaultPath[modName] = res;
            }
            return res;
        }
        public static void Set(string modName, string fileName)
        {
            instance.DefaultPath[modName] = $"{ReplaceString.BasePath}/{fileName}";
            Save();
        }
    }
}
