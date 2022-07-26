using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using _ReplaceString_.ConfigUI.Work;
using _ReplaceString_.Data;

namespace _ReplaceString_.Package
{
    internal static class Pack
    {
        public static MakeConfig config;
        public static (TreeNode root, MakeConfig config) Packup(string path)
        {
            FileStream file;
            StreamReader reader;
            using (file = new FileStream($"{path}/Config.json", FileMode.Open))
            {
                config = JsonSerializer.Deserialize<MakeConfig>(file);
            }

            TreeNode root = new TreeNode(config.ModName);
            string line;
            bool begin;
            var item = root["ItemName"];
            var tooltip = root["ItemTooltip"];
            string key;
            string value;
            using (file = new FileStream($"{path}/Item.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        item += new Leaf($"{config.ModName}.ItemName.{key}", value);

                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }
                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        tooltip += new Leaf($"{config.ModName}.ItemTooltip.{key}", value);
                    }
                }
            }

            var proj = root["ProjectileName"];
            using (file = new FileStream($"{path}/Projectile.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        proj += new Leaf($"{config.ModName}.ProjectileName.{key}", value);
                    }
                }
            }

            var damage = root["DamageClassName"];
            using (file = new FileStream($"{path}/DamageClass.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        damage += new Leaf($"{config.ModName}.DamageClassName.{key}", value);
                    }
                }
            }

            var info = root["InfoDisplayName"];
            using (file = new FileStream($"{path}/InfoDisplay.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        info += new Leaf($"{config.ModName}.InfoDisplayName.{key}", value);
                    }
                }
            }

            var biome = root["BiomeName"];
            using (file = new FileStream($"{path}/Biome.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        biome += new Leaf($"{config.ModName}.BiomeName.{key}", value);
                    }
                }
            }


            var buff = root["BuffName"];
            var description = root["BuffDescription"];
            using (file = new FileStream($"{path}/Buff.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        buff += new Leaf($"{config.ModName}.BuffName.{key}", value);

                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }
                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        description += new Leaf($"{config.ModName}.BuffDescription.{key}", value);
                    }
                }
            }

            var npc = root["NPCName"];
            using (file = new FileStream($"{path}/NPC.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        npc += new Leaf($"{config.ModName}.NPCName.{key}", value);
                    }
                }
            }

            var prefix = root["Prefix"];
            using (file = new FileStream($"{path}/Prefix.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        prefix += new Leaf($"{config.ModName}.Prefix.{key}", value);
                    }
                }
            }

            var containers = root["Containers"];
            using (file = new FileStream($"{path}/Containers.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        containers += new Leaf($"{config.ModName}.Containers.{key}", value);
                    }
                }
            }

            var map = root["MapEntry"];
            var tile = map["tileEntries"];
            using (file = new FileStream($"{path}/TileEntries.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        tile += new Leaf(key, value);
                    }
                }
            }

            var wall = map["wallEntries"];
            using (file = new FileStream($"{path}/WallEntries.txt", FileMode.Open))
            {
                using (reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                        {
                            continue;
                        }
                        begin = true;
                        key = line.Split(':')[0].Trim('\t', ' ');
                        while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                        value = line[(line.IndexOf(':') + 1)..].Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                            {
                                value += (begin ? "" : "\n") + line;
                                begin = false;
                            }
                        }
                        wall += new Leaf(key, value);
                    }
                }
            }
            if (config.LdstrFold)
            {
                PackupLdstr($"{path}/Ldstr", root["Ldstr"]);
            }

            //var pathNode = root["Path"];
            //using (file = new FileStream($"{path}/Path.txt", FileMode.Open))
            //{
            //    using (reader = new StreamReader(file))
            //    {
            //        while ((line = reader.ReadLine()) != null)
            //        {
            //            if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
            //            {
            //                continue;
            //            }
            //            begin = true;
            //            key = line.Split(':')[0].Trim('\t', ' ');
            //            while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

            //            value = line[(line.IndexOf(':') + 1)..].Trim();
            //            if (string.IsNullOrWhiteSpace(value))
            //            {
            //                while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
            //                {
            //                    value += (begin ? "" : "\n") + line;
            //                    begin = false;
            //                }
            //            }
            //            pathNode += new Leaf(key, value);
            //        }
            //    }
            //}


            return (root, config);
        }

        private static void PackupLdstr(string path, TreeNode node)
        {
            var dir = Directory.GetDirectories(path);
            var file = Directory.GetFiles(path);
            foreach (var d in dir)
            {
                PackupLdstr(d, node[d.Split('\\')[^1]]);
            }

            foreach (var f in file)
            {
                bool begin;
                var child = node[Path.GetFileNameWithoutExtension(f.Split('\\')[^1])];
                string key, line, value;
                using var reader = File.OpenText(f);
                while ((line = reader.ReadLine()) != null)
                {
                    if (!Regex.IsMatch(line, "[a-zA-Z0-9]"))
                    {
                        continue;
                    }
                    begin = true;
                    key = line.Split(':')[0].Trim('\t', ' ');
                    while (!(line = reader.ReadLine().TrimStart('\t', ' ')).StartsWith("Current :")) { }

                    value = line[(line.IndexOf(':') + 1)..].Trim();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        while ((line = reader.ReadLine().TrimStart('\t', ' ')) != "}")
                        {
                            value += (begin ? "" : "\n") + line;
                            begin = false;
                        }
                    }
                    child += new Leaf(key, value);
                }
            }
        }
    }
}
