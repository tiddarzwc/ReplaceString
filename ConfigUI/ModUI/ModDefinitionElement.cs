using System.IO;
using System.Linq;
using _ReplaceString_.ConfigUI.A_Load;
using _ReplaceString_.Package;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static _ReplaceString_.ConfigUI.Constant;

namespace _ReplaceString_.ConfigUI.ModUI;

public class ModDefinitionElement : UIElement
{
    public ModDefinition value;
    public UIText text;
    public UIPanel filePanel;
    public bool showLoadInfo;
    public static string HoverString { get; set; }
    public static Vector2 HoverPosition { get; set; }
    public Color DefaultColor
    {
        get
        {
            if (showLoadInfo && ReplaceString.Instance.importStates.TryGetValue(value.Name, out var state))
            {
                return state == ImportState.Success ? Color.Green : Color.Red;
            }
            return Color.White;
        }
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        var dimension = GetDimensions();
        Color panelColor = UICommon.DefaultUIBlue.MultiplyRGBA(new Color(180, 180, 180));
        ConfigElement.DrawPanel2(spriteBatch, new Vector2(dimension.X, dimension.Y + 1), TextureAssets.SettingsPanel.Value, dimension.Width, dimension.Height - 2, panelColor);
    }

    public ModDefinitionElement(ModDefinition value, bool showLoadInfo)
    {
        this.value = value;
        this.showLoadInfo = showLoadInfo;
        if (!ReplaceString.Catcher.modInfos.TryGetValue(value.Name, out var info))
        {
            info = ModInfo.Default with { displayName = value.DisplayName };
        }
        var icon = new UIImage(info.icon)
        {
            MarginLeft = ICON_SPACE - ICON_SIZE / 2f,
            MarginTop = ICON_SPACE - ICON_SIZE / 2f,
            ImageScale = 0.5f
        };

        Append(icon);

        var modText = $"{info.displayName} ({value.Name})";
        if (modText.Length > 50)
        {
            modText = modText[..50] + "...";
        }

        text = new UIText(modText)
        {
            Left = new StyleDimension(MOD_HEIGHT, 0),
            TextColor = DefaultColor,
            Top = new StyleDimension(4, 0),
            Height = new StyleDimension(40, 0),
            Width = new StyleDimension(-70, 1),
            TextOriginY = 0.5f,
            TextOriginX = 0,
        };
        Append(text);

        text.OnMouseOver += (evt, listeningElement) =>
        {
            if (text.TextColor == DefaultColor)
            {
                text.TextColor = Color.Yellow;
            }
        };
        text.OnMouseOut += (evt, listeningElement) =>
        {
            if (text.TextColor == Color.Yellow)
            {
                text.TextColor = DefaultColor;
            }
        };

        if (!showLoadInfo)
        {
            return;
        }

        text.OnMouseOver += delegate
        {
            if (ReplaceString.Instance.importStates.TryGetValue(value.Name, out var state))
            {
                if (state == ImportState.Success && ReplaceString.Instance.importInfo.TryGetValue(value.Name, out var info))
                {
                    HoverString = info.ToString();
                }
                else
                {
                    HoverString = state.ToString();
                }
            }
        };
        text.OnMouseOut += delegate
        {
            HoverString = string.Empty;
        };

        var button = new UIImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay", ReLogic.Content.AssetRequestMode.ImmediateLoad))
        {
            Left = new StyleDimension(-28, 1),
            Top = new StyleDimension(13, 0)
        };
        button.OnClick += delegate
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            if (filePanel != null)
            {
                RemoveChild(filePanel);
                filePanel = null;
                Height.Pixels = MOD_HEIGHT;
                (Parent as ModDefinitionListElement).ResetChildrenTop();
                return;
            }
            ResetFiles();
        };
        Append(button);
    }
    public void ResetFiles()
    {
        var files = Directory.GetFiles(ReplaceString.BasePath).Where(p =>
        {
            var ext = Path.GetExtension(p);
            var name = Path.GetFileName(p);
            return name.StartsWith(value.Name) && (ext == ".hjson" || ext == ".loc");
        });
        filePanel = new UIPanel()
        {
            Top = new StyleDimension(MOD_HEIGHT, 0),
            Left = new StyleDimension(8, 0),
            Width = new StyleDimension(-16, 1),
            BorderColor = Color.Transparent
        };
        filePanel.SetPadding(6);
        Append(filePanel);
        int top = 0;
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var fileUI = new UIText($"◆ {name}")
            {
                TextOriginY = 0.5f,
                Height = new StyleDimension(30, 0),
                Top = new StyleDimension(top, 0),
                TextColor = Color.White
            };
            string hoverString = string.Empty;
            if (name.EndsWith(".loc"))
            {
                var meta = Zip.GetMetaData(file);
                fileUI.SetText($"◆ {meta.displayName}");
                hoverString = meta.ToString();
            }

            filePanel.Append(fileUI);
            top += 30 + 6;
            fileUI.OnUpdate += delegate
            {
                if (Data.DefaultTranslation.Get(value.Name) == name && fileUI.TextColor != Color.Yellow)
                {
                    fileUI.TextColor = Color.Green;
                }
                else if (fileUI.TextColor == Color.Green)
                {
                    fileUI.TextColor = Color.White;
                }
            };

            fileUI.OnMouseOver += delegate
            {
                fileUI.TextColor = Color.Yellow;
                HoverString = hoverString;
            };

            fileUI.OnMouseOut += delegate
            {
                if (fileUI.TextColor == Color.Yellow)
                {
                    fileUI.TextColor = Color.White;
                }
                HoverString = string.Empty;
            };

            fileUI.OnClick += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                Data.DefaultTranslation.Set(value.Name, name);
            };

        }

        foreach (var netFile in Network.GetModInfo(value.Name).Where(net => files.All(local => local != net.fileName)))
        {
            var fileUI = new UIText($"◇ {netFile.displayName}")
            {
                TextOriginY = 0.5f,
                Height = new StyleDimension(30, 0),
                Top = new StyleDimension(top, 0),
                TextColor = Color.Gray
            };
            filePanel.Append(fileUI);
            top += 30 + 6;

            fileUI.OnMouseOver += delegate
            {
                fileUI.TextColor = Color.White;
                HoverString = netFile.ToString();
            };

            fileUI.OnMouseOut += delegate
            {
                fileUI.TextColor = Color.Gray;
                HoverString = string.Empty;
            };

            fileUI.OnClick += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
`                Network.Download(ReplaceString.BasePath, netFile.fileName);
                ResetFiles();
            };
        }

        if (!files.Any())
        {
            var fileUI = new UIText($"(暂无内容)")
            {
                TextOriginY = 0.5f,
                Height = new StyleDimension(30, 0),
                Top = new StyleDimension(top, 0),
                TextColor = Color.Gray,
            };
            filePanel.Append(fileUI);
            top += 30 + 6;
        }

        filePanel.Height.Pixels = top + 6;
        Height.Pixels = filePanel.Height.Pixels + MOD_HEIGHT + 8;
        (Parent as ModDefinitionListElement).ResetChildrenTop();
        filePanel.Activate();
    }
}


