using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _ReplaceString_.Config;
using Hjson;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace _ReplaceString_.Translator.UI;

internal class PackElement : ConfigElement
{
    public UIText selected = null;
    public bool fliterHooked = false;
    public override void OnInitialize()
    {
        DrawLabel = false;
        OnUpdate += delegate
        {
            //if (!fliterHooked)
            //{
            //    UIFocusInputTextFieldReplaced.TryRepalce(Parent.Parent.Parent.Parent.Parent);
            //    fliterHooked = true;
            //    UIFocusInputTextFieldReplaced.instance.hintText = "Fliter File";
            //}
            UIFocusInputTextFieldReplaced.enable = true;
            if (UIFocusInputTextFieldReplaced.TextChanged)
            {
                ResetChildren();
            }
        };
        ResetChildren();
    }

    public void ResetChildren()
    {
        Elements.Clear();
        UIPanel panel = new UIPanel()
        {
            Width = new StyleDimension(50, 0),
            MarginLeft = 8,
            MarginTop = 4,
            HAlign = 0,
            Height = new StyleDimension(26, 0)
        };
        panel.OnMouseOver += (evt, listeningElement) =>
        {
            if (selected != null)
            {
                panel.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
            }
        };
        panel.OnMouseOut += (evt, listeningElement) =>
        {
            panel.BackgroundColor = new Color(44, 57, 105, 178);
        };
        panel.OnClick += (evt, listeningElement) =>
        {
            if (selected != null)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                var path = $"{Main.SavePath}/Mods/ReplaceString/{selected.Text}";
                if (Directory.Exists(path))
                {
                    var (treeNode, config) = Pack.Packup(path);
                    File.WriteAllText($"{Main.SavePath}/Mods/ReplaceString/{treeNode.name}-{GameCulture.FromCultureName((GameCulture.CultureName)config.TargetCultureID).Name}-packed.hjson", treeNode.BuildHjson(0).ToString());
                }
                else
                {
                    ResetChildren();
                }
            }
        };
        Append(panel);

        UIText text = new UIText("Pack")
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            TextColor = Color.Gray
        };
        text.OnUpdate += delegate
        {
            text.TextColor = selected != null ? Color.White : Color.Gray;
        };
        panel.Append(text);
        panel.Activate();


        UIPanel panel2 = new UIPanel()
        {
            Width = new StyleDimension(50, 0),
            MarginLeft = 8 + 50,
            MarginTop = 4,
            HAlign = 0,
            Height = new StyleDimension(26, 0)
        };
        panel2.OnMouseOver += (evt, listeningElement) =>
        {
            panel2.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
        };
        panel2.OnMouseOut += (evt, listeningElement) =>
        {
            panel2.BackgroundColor = new Color(44, 57, 105, 178);
        };
        panel2.OnClick += (evt, listeningElement) =>
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            selected = null;
            ResetChildren();
        };
        Append(panel2);

        UIText text2 = new UIText("刷新")
        {
            HAlign = 0.5f,
            VAlign = 0.5f
        };
        panel2.Append(text2);
        panel2.Activate();


        int margin = 30;
        foreach (var dir in Directory.GetDirectories($"{Main.SavePath}/Mods/ReplaceString")
            .Where(p => p.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()))
            .Select(p => Path.GetFileName(p)))
        {
            var ui = new UIPanel()
            {
                Height = new StyleDimension(40, 0),
                MarginTop = margin,
                Width = new StyleDimension(-16, 1),
                MarginLeft = 8
            };
            var fileName = new UIText(dir)
            {
                HAlign = 0.5f
            };
            ui.OnMouseOver += (evt, listeningElement) =>
            {
                ui.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
                if (fileName.TextColor == Color.White)
                {
                    fileName.TextColor = Color.Yellow;
                }
            };
            ui.OnMouseOut += (evt, listeningElement) =>
            {
                ui.BackgroundColor = new Color(44, 57, 105, 178);
                if (fileName.TextColor == Color.Yellow)
                {
                    fileName.TextColor = Color.White;
                }
            };
            ui.OnClick += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                if (selected == fileName)
                {
                    selected = null;
                    fileName.TextColor = Color.Yellow;
                }
                else if (selected == null)
                {
                    selected = fileName;
                    fileName.TextColor = Color.Red;
                }
                else
                {
                    selected.TextColor = Color.White;
                    selected = fileName;
                    fileName.TextColor = Color.Red;
                }
            };
            margin += 40;
            ui.Append(fileName);
            ui.Activate();
            Append(ui);
        }

    }

    public override void Recalculate()
    {
        Height.Pixels = 34 + (Elements.Count - 2) * 40;
        if (Parent != null)
        {
            Parent.Height.Pixels = Height.Pixels;
        }

        base.Recalculate();
    }

}
