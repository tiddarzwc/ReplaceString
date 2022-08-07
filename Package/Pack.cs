using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using _ReplaceString_.ConfigUI.C_Work;
using _ReplaceString_.Data;

namespace _ReplaceString_.Package
{
    internal class CacheReader : StreamReader
    {
        public CacheReader(Stream stream) : base(stream)
        {
        }

        public CacheReader(string path) : base(path)
        {
        }

        public CacheReader(Stream stream, bool detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks)
        {
        }

        public CacheReader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public CacheReader(string path, FileStreamOptions options) : base(path, options)
        {
        }

        public CacheReader(string path, bool detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks)
        {
        }

        public CacheReader(string path, Encoding encoding) : base(path, encoding)
        {
        }

        public CacheReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public CacheReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public CacheReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public CacheReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public CacheReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, FileStreamOptions options) : base(path, encoding, detectEncodingFromByteOrderMarks, options)
        {
        }

        public CacheReader(Stream stream, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = true, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        {
        }
        private string cache = null;
        private bool useCache = false;
        public override string ReadLine()
        {
            if (useCache)
            {
                useCache = false;
                return cache;
            }
            return cache = base.ReadLine();
        }
        public void RollBack() => useCache = true;
    }
    internal static class Pack
    {
        public static string modName;
        public static string ReadValue(CacheReader reader, string symbol, string startWith)
        {
            string line, value;
            bool firstRead = true;
            while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith(symbol)) { }

            value = line[(line.IndexOf(':') + 1)..].Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                while ((line = reader.ReadLine()).StartsWith(startWith))
                {
                    value += (firstRead ? "" : "\n") + line[startWith.Length..];
                    firstRead = false;
                }
                reader.RollBack();
            }
            return value;
        }
        public static Regex regex = new Regex("[a-zA-Z0-9]", RegexOptions.Compiled);
        public static bool ReadKey(CacheReader reader, out string key)
        {
            string line = reader.ReadLine();
            if (line == null || !regex.IsMatch(line))
            {
                key = null;
                return false;
            }
            key = line.Split(':')[0].Trim('\t', ' ');
            return true;
        }
        public static void ReadFile(string path, TreeNode root, params string[] args)
        {
            string value, oldValue;
            TreeNode[] nodes = args.Select(a => new TreeNode(a)).ToArray();
            using FileStream file = new FileStream(path, FileMode.Open);
            using CacheReader reader = new CacheReader(file);
            string startWith = "\t\t" + (args.Length > 1 ? "\t" : "");
            while (!reader.EndOfStream)
            {
                if (!ReadKey(reader, out string key))
                {
                    continue;
                }
                for (int i = 0; i < args.Length; i++)
                {
                    if (args.Length > 1)
                    {
                        while (!ReadKey(reader, out _)) { }
                    }
                    oldValue = ReadValue(reader, "Origin :", startWith);
                    value = ReadValue(reader, "Current :", startWith);
                    if (oldValue != value && !string.IsNullOrWhiteSpace(value))
                    {
                        nodes[i] += new Leaf($"{modName}.{args[i]}.{key}", value);
                    }
                }
            }
            foreach (var node in nodes)
            {
                root += node;
            }
        }
        public static TreeNode Packup(string path)
        {
            var fileName = Path.GetFileName(path);
            modName = fileName[..(fileName.IndexOf('-'))];
            TreeNode root = new TreeNode(modName);
            ReadFile($"{path}/Item.txt", root, "ItemName", "ItemTooltip");
            ReadFile($"{path}/Projectile.txt", root, "ProjectileName");
            ReadFile($"{path}/DamageClass.txt", root, "DamageClassName");
            ReadFile($"{path}/InfoDisplay.txt", root, "InfoDisplayName");
            ReadFile($"{path}/Biome.txt", root, "BiomeName");
            ReadFile($"{path}/Buff.txt", root, "BuffName", "BuffDescription");
            ReadFile($"{path}/NPC.txt", root, "NPCName");
            ReadFile($"{path}/Prefix.txt", root, "Prefix");
            ReadFile($"{path}/Containers.txt", root, "Containers");

            var map = root["MapEntry"];
            ReadFile($"{path}/TileEntries.txt", map, "tileEntries");
            ReadFile($"{path}/WallEntries.txt", map, "wallEntries");

            PackupLdstr($"{path}/Ldstr", root["Ldstr"]);

            CullEmpty(root);

            return root;
        }
        private static void PackupLdstr(string path, TreeNode node)
        {
            var dirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            foreach (var dir in dirs)
            {
                PackupLdstr(dir, node[dir.Split('\\')[^1]]);
            }

            foreach (var file in files)
            {
                var child = node[Path.GetFileNameWithoutExtension(file.Split('\\')[^1])];
                string value, oldValue;
                using var reader = new CacheReader(File.OpenRead(file));
                while (!reader.EndOfStream)
                {
                    if (!ReadKey(reader, out string key))
                    {
                        continue;
                    }
                    Debug.Assert(int.TryParse(key.Split(':')[0], out _));
                    oldValue = ReadValue(reader, "Origin :", "\t\t");
                    value = ReadValue(reader, "Current :", "\t\t");
                    if (oldValue != value && !string.IsNullOrWhiteSpace(value))
                    {
                        child += new Leaf(key, value);
                    }
                }
            }
        }
        public static bool CullEmpty(TreeNode root)
        {
            for (int i = 0; i < root.children.Count; i++)
            {
                if (CullEmpty(root.children[i]))
                {
                    root.children.RemoveAt(i--);
                }
            }
            if (root.children.Count == 0 && root is not Leaf)
            {
                return true;
            }
            return false;
        }
    }
}
