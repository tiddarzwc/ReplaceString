﻿using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using Terraria.ModLoader.IO;

namespace _ReplaceString_.Config
{
    public class ModDefinition : TagSerializable
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ModDefinition(string modName, string displayName)
        {
            Name = modName;
            DisplayName = displayName;
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound
            {
                { "name", Name },
                { "displayName", DisplayName }
            };
            return tag;
        }
    }
}
