using System;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using System.Text.Json;
using System.Text.Json.Serialization;
using _ReplaceString_.ConfigUI.Work;
using _ReplaceString_.Data;

namespace _ReplaceString_.Package
{
    internal static class Make
    {
        public static MakeConfig config;
        public static string SetupFolds(TreeNode root, MakeConfig config)
        {
            Make.config = config;
            string path = $"{Main.SavePath}/Mods/ReplaceString/{config.ModName}-{ModLoader.GetMod(root.name).Version.ToString().Replace(".", "")}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream file;
            StreamWriter writer;
            StringBuilder sb = new StringBuilder();

            using (file = new FileStream($"{path}/Config.json", FileMode.Create))
            {
                JsonSerializer.Serialize(file, config, new JsonSerializerOptions() { WriteIndented = true });
            }

            var item = root["ItemName"];
            var tooltip = root["ItemTooltip"];
            using (file = new FileStream($"{path}/Item.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in item.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tName :");
                        sb.AppendLine("\t{");
                        sb.AppendLine($"\t\tOrigin : {leaf.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\t\tCurrent : {leaf.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine("\t}");

                        sb.AppendLine($"\tTooltip :");
                        sb.AppendLine("\t{");
                        var tips = tooltip[leaf.name.Replace("ItemName", "ItemTooltip")] as Leaf;
                        sb.AppendLine($"\t\tOrigin : {tips.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\t\tCurrent : {tips.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine("\t}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var proj = root["ProjectileName"];
            using (file = new FileStream($"{path}/Projectile.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in proj.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var damage = root["DamageClassName"];
            using (file = new FileStream($"{path}/DamageClass.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in damage.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var info = root["InfoDisplayName"];
            using (file = new FileStream($"{path}/InfoDisplay.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in info.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var biome = root["BiomeName"];
            using (file = new FileStream($"{path}/Biome.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in biome.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var buff = root["BuffName"];
            var description = root["BuffDescription"];
            using (file = new FileStream($"{path}/Buff.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in buff.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tName :");
                        sb.AppendLine("\t{");
                        sb.AppendLine($"\t\tOrigin : {leaf.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\t\tCurrent : {leaf.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine("\t}");

                        sb.AppendLine($"\tDescription :");
                        sb.AppendLine("\t{");
                        var tips = description[leaf.name.Replace("BuffName", "BuffDescription")] as Leaf;
                        sb.AppendLine($"\t\tOrigin : {tips.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\t\tCurrent : {tips.GetValue("\t\t", config.IgnoreEmpty)}");
                        sb.AppendLine("\t}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var npc = root["NPCName"];
            using (file = new FileStream($"{path}/NPC.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in npc.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }


            var prefix = root["Prefix"];
            using (file = new FileStream($"{path}/Prefix.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in prefix.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var containers = root["Containers"];
            using (file = new FileStream($"{path}/Containers.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in containers.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name.Split('.')[^1]} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var tile = root["tileEntries"];
            using (file = new FileStream($"{path}/TileEntries.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in tile.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var wall = root["wallEntries"];
            using (file = new FileStream($"{path}/WallEntries.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in wall.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            var pathNode = root["Path"];
            using (file = new FileStream($"{path}/Path.txt", FileMode.Create))
            {
                using (writer = new StreamWriter(file))
                {
                    foreach (var leaf in pathNode.children.Select(c => c as Leaf))
                    {
                        sb.Clear();
                        sb.AppendLine($"{leaf.name} :");
                        sb.AppendLine("{");
                        sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                        sb.AppendLine("}");
                        writer.WriteLine(sb);
                    }
                }
            }

            if (config.LdstrFold)
            {
                SetupLdstrFolds(root["Ldstr"], path);
            }

            return path;
        }
        private static void SetupLdstrFolds(TreeNode head, string path)
        {
            if (head.children.FirstOrDefault() is Leaf)
            {
                using var file = new FileStream($"{path}/{head.name}.txt", FileMode.Create);
                using var writer = new StreamWriter(file);
                StringBuilder sb = new StringBuilder();
                foreach (var leaf in head.children.Select(c => c as Leaf))
                {
                    sb.AppendLine($"{leaf.name} :");
                    sb.AppendLine("{");
                    sb.AppendLine($"\tOrigin : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                    sb.AppendLine($"\tCurrent : {leaf.GetValue("\t", config.IgnoreEmpty)}");
                    sb.AppendLine("}");
                    writer.WriteLine(sb);
                    sb.Clear();
                }
            }
            else
            {
                string newPath = $"{path}/{head.name}";
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                foreach (var child in head.children)
                {
                    SetupLdstrFolds(child, newPath);
                }
            }
        }

    }
}
