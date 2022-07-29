﻿using System.IO;
using System.Linq;
using _ReplaceString_.Data;
using _ReplaceString_.Package;
using Hjson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace _ReplaceString_.ConfigUI.C_Work;

internal class MakeElement : ConfigElement
{
    public UIText selected = null;
    public bool fliterHooked = false;
    public override void OnInitialize()
    {
        DrawLabel = false;
        OnUpdate += delegate
        {
            if (!fliterHooked)
            {
                UIFocusInputTextFieldReplaced.TryRepalce(Parent.Parent.Parent.Parent.Parent);
                fliterHooked = true;
                UIFocusInputTextFieldReplaced.instance.hintText = "Fliter Files";
            }
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
        #region Make
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
                if (File.Exists(path))
                {
                    Make.SetupFolds(path);
                }
                else
                {
                    ResetChildren();
                }
            }
        };
        Append(panel);


        UIText text = new UIText("Make")
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
        #endregion

        #region Zip
        UIPanel panel2 = new UIPanel()
        {
            Left = new StyleDimension(50, 0),
            Width = new StyleDimension(50, 0),
            MarginLeft = 8,
            MarginTop = 4,
            HAlign = 0,
            Height = new StyleDimension(26, 0)
        };
        panel2.OnMouseOver += (evt, listeningElement) =>
        {
            if (selected != null)
            {
                panel2.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
            }
        };
        panel2.OnMouseOut += (evt, listeningElement) =>
        {
            panel2.BackgroundColor = new Color(44, 57, 105, 178);
        };
        panel2.OnClick += (evt, listeningElement) =>
        {
            if (selected != null)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                var path = $"{Main.SavePath}/Mods/ReplaceString/{selected.Text}";
                if (File.Exists(path))
                {
                    Zip.ZipHjson(path);
                }
                else
                {
                    ResetChildren();
                }
            }
        };
        Append(panel2);

        UIText text2 = new UIText("Zip")
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            TextColor = Color.Gray
        };
        text2.OnUpdate += delegate
        {
            text2.TextColor = selected != null ? Color.White : Color.Gray;
        };
        panel2.Append(text2);
        panel2.Activate();
        #endregion

        UIPanel panel3 = new UIPanel()
        {
            Width = new StyleDimension(50, 0),
            MarginLeft = 8 + 50 + 50,
            MarginTop = 4,
            HAlign = 0,
            Height = new StyleDimension(26, 0)
        };
        panel3.OnMouseOver += (evt, listeningElement) =>
        {
            panel3.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
        };
        panel3.OnMouseOut += (evt, listeningElement) =>
        {
            panel3.BackgroundColor = new Color(44, 57, 105, 178);
        };
        panel3.OnClick += (evt, listeningElement) =>
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            selected = null;
            ResetChildren();
        };
        Append(panel3);

        UIText text3 = new UIText("刷新")
        {
            HAlign = 0.5f,
            VAlign = 0.5f
        };
        panel3.Append(text3);
        panel3.Activate();


        int margin = 30 + 6;
        foreach (var file in Directory.GetFiles($"{Main.SavePath}/Mods/ReplaceString")
            .Where(p => Path.GetExtension(p) == ".hjson")
            .Where(p => Path.GetFileName(p).ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()))
            .Select(p => Path.GetFileName(p)))
        {
            var ui = new UIPanel()
            {
                Height = new StyleDimension(40, 0),
                MarginTop = margin,
                Width = new StyleDimension(-16, 1),
                MarginLeft = 8,
                BackgroundColor = new Color(44 - 23 - 10, 57 - 23 - 5, 105 - 23, 178),
                BorderColor = Color.Transparent
            };
            var fileName = new UIText(file)
            {
                HAlign = 0.5f
            };
            ui.OnMouseOver += (evt, listeningElement) =>
            {
                ui.BorderColor = Color.Gold;
                if (fileName.TextColor == Color.White)
                {
                    fileName.TextColor = Color.Yellow;
                }
            };
            ui.OnMouseOut += (evt, listeningElement) =>
            {
                ui.BorderColor = Color.Transparent;
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
        if (Elements.Count == 3)
        {
            var ui = new UIPanel()
            {
                Height = new StyleDimension(40, 0),
                MarginTop = margin,
                Width = new StyleDimension(-16, 1),
                MarginLeft = 8,
                BackgroundColor = new Color(44 - 23 - 10, 57 - 23 - 5, 105 - 23, 178),
                BorderColor = Color.Transparent
            };
            var uiText = new UIText("（暂无内容）")
            {
                HAlign = 0.5f,
                TextColor = Color.Gray
            };
            margin += 40;
            ui.Append(uiText);
            ui.Activate();
            Append(ui);
        }
        Height.Pixels = 40 + (Elements.Count - 3) * 40;
    }

    public override void Recalculate()
    {
        if (Parent != null)
        {
            Parent.Height.Pixels = Height.Pixels;
        }

        base.Recalculate();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        CalculatedStyle dimensions = GetDimensions();
        DrawPanel2(spriteBatch,
            new Vector2(dimensions.X, dimensions.Y),
            TextureAssets.SettingsPanel.Value,
            (float)(dimensions.Width + 1f),
            dimensions.Height,
            new Color(44, 57, 105, 178)
            );
    }
}
