using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Newtonsoft;
using Newtonsoft.Json;
using Terraria.ModLoader.Core;

namespace _ReplaceString_.Data
{
    internal class DefaultTranslation
    {
        private static string ConfigPath => $"{ReplaceString.BasePath}/Config.json";
        private Dictionary<string, string> DefaultAuthor { get; set; } = new Dictionary<string, string>();
        private static readonly DefaultTranslation instance = new DefaultTranslation();
        public static void Load()
        {
            if (!File.Exists(ConfigPath))
            {
                instance.DefaultAuthor = new Dictionary<string, string>();
                Save();
                return;
            }
            JsonConvert.PopulateObject(File.ReadAllText(ConfigPath), instance);
        }
        public static void Save()
        {
            if(!Directory.Exists(ReplaceString.BasePath))
            {
                Directory.CreateDirectory(ReplaceString.BasePath);
            }
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(instance));
        }
        public static string Get(TmodFile modFile)
        {
            string res = null;
            if(instance.DefaultAuthor.TryGetValue(modFile.Name, out var val) && Directory.GetFiles(ReplaceString.BasePath).Any(path =>
            {
                string file = Path.GetFileName(path);
                if( file.StartsWith(modFile.Name) && file.Contains(val))
                {
                    res = path;
                    return true;
                }
                return false;
            }))
            {
                return res;
            }
            return null;
        }
        public static void Add(string mod, string author)
        {
            instance.DefaultAuthor.Add(mod, author);
        }
    }
}
