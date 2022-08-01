using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
namespace _ReplaceString_.ConfigUI.C_Work
{
    internal class FileSelectedUI : UIPanel
    {
        public UIPanel filePanel;
        public UIText selected;
        public UIText text;
        public string hintText;
        public bool IsSelected => text.Text != hintText;
        public string Text => text.Text;
        public FileSelectedUI(string hintText)
        {
            this.hintText = hintText;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            SetPadding(6);
            BorderColor = Color.Transparent;
            OnMouseOver += (evt, listeningElement) =>
            {
                BorderColor = Color.Gold;
            };
            OnMouseOut += (evt, listeningElement) =>
            {
                BorderColor = Color.Transparent;
            };
            text = new UIText(hintText)
            {
                Width = new StyleDimension(0, 1),
                Height = new StyleDimension(30, 0),
                TextOriginX = 0,
                TextOriginY = 0.5f,
                TextColor = Color.Gray
            };
            Append(text);

            text.OnClick += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Expand();
                if (filePanel == null)
                {
                    if (selected != null)
                    {
                        text.TextColor = Color.White;
                        text.SetText(selected.Text);
                    }
                    else
                    {
                        text.TextColor = Color.Gray;
                        text.SetText(hintText);
                    }
                }
                else
                {
                    if (selected != null)
                    {
                        selected = filePanel.Children.FirstOrDefault(u => u.Children.First() is UIText t && t.Text == selected.Text).Children.First() as UIText;
                        if (selected != null)
                        {
                            selected.TextColor = Color.Red;
                        }

                        text.TextColor = Color.Gray;
                        text.SetText(hintText);
                    }
                }
            };
            Height.Pixels = 30 + 6 * 2 + (filePanel?.Height.Pixels ?? 0);
        }

        public void Expand()
        {
            if (filePanel != null)
            {
                RemoveChild(filePanel);
                filePanel = null;
                Height.Pixels = 30 + 6 * 2;
                return;
            }
            filePanel = new UIPanel()
            {
                Top = new StyleDimension(30, 0),
                Width = new StyleDimension(0, 1),
                Height = new StyleDimension(40, 0),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            filePanel.OnMouseOver += (evt, listeningElement) =>
            {
                (listeningElement as UIPanel).BackgroundColor = Color.Transparent;
            };
            filePanel.OnMouseOut += (evt, listeningElement) =>
            {
                (listeningElement as UIPanel).BackgroundColor = Color.Transparent;
            };
            filePanel.SetPadding(6);
            int height = 0;
            foreach (var file in Directory.GetFiles(ReplaceString.BasePath)
                .Where(p => Path.GetExtension(p) == ".hjson")
                .Where(p => p.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()))
                .Select(p => Path.GetFileName(p)))
            {
                var ui = new UIPanel()
                {
                    Height = new StyleDimension(30, 0),
                    Top = new StyleDimension(height, 0),
                    Width = new StyleDimension(0, 1),
                    BackgroundColor = new Color(44 - 23 - 10, 57 - 23 - 5, 105 - 23, 178),
                    BorderColor = Color.Transparent
                };
                ui.SetPadding(6);
                var fileName = new UIText($"◆ {file}")
                {
                    TextOriginY = 0.5f
                };
                ui.OnMouseOver += (evt, listeningElement) =>
                {
                    ui.BackgroundColor = new Color(44 - 23 - 10, 57 - 23 - 5, 105 - 23, 178);
                    ui.BorderColor = Color.Gold;
                    //ui.BackgroundColor = Color.Blue;
                    if (fileName.TextColor == Color.White)
                    {
                        fileName.TextColor = Color.Yellow;
                    }
                };
                ui.OnMouseOut += (evt, listeningElement) =>
                {
                    ui.BackgroundColor = new Color(44 - 23, 57 - 23, 105 - 23, 178);
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
                height += 30;
                ui.Append(fileName);
                ui.Activate();
                filePanel.Append(ui);
            }

            if (!filePanel.Children.Any())
            {
                var ui = new UIPanel()
                {
                    Height = new StyleDimension(30, 0),
                    Top = new StyleDimension(height, 0),
                    Width = new StyleDimension(0, 1),
                    BackgroundColor = new Color(44 - 23 - 10, 57 - 23 - 5, 105 - 23, 178),
                    BorderColor = Color.Transparent
                };
                ui.SetPadding(6);
                var uiText = new UIText($"（暂无内容）")
                {
                    TextOriginY = 0.5f,
                    TextColor = Color.Gray
                };
                ui.Append(uiText);
                filePanel.Append(ui);
                height += 30;

            }

            filePanel.Height.Pixels = height + 6 * 2;
            if (filePanel.Children.Any())
            {
                Append(filePanel);
            }
            Height.Pixels = 30 + 6 * 2 + filePanel.Height.Pixels;
        }

        public override void Recalculate()
        {
            base.Recalculate();
        }
    }
}
