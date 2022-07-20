using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;
namespace _ReplaceString_.ConfigUI.Work
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
            OnMouseOver += (evt, listeningElement) =>
            {
                BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
            };
            OnMouseOut += (evt, listeningElement) =>
            {
                BackgroundColor = new Color(44, 57, 105, 178);
            };
            text = new UIText(hintText)
            {
                MarginLeft = 4,
                Width = new StyleDimension(0, 0),
                Height = new StyleDimension(30, 0),
                TextOriginX = 0.5f,
                TextOriginY = 0.5f,
                TextColor = Color.Gray
            };
            Append(text);

            var expandButton = new UIImageButton(UICommon.ButtonCollapsedTexture)
            {
                Left = new StyleDimension(-22, 1),
                Top = new StyleDimension((30 - UICommon.ButtonCollapsedTexture.Value.Height) / 2, 0)
            };
            expandButton.OnClick += delegate
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Expand();
                if (filePanel == null)
                {
                    expandButton.SetImage(UICommon.ButtonCollapsedTexture);
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
                    expandButton.SetImage(UICommon.ButtonExpandedTexture);
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
            Append(expandButton);
        }

        public void Expand()
        {
            if (filePanel != null)
            {
                RemoveChild(filePanel);
                filePanel = null;
                return;
            }
            filePanel = new UIPanel()
            {
                Top = new StyleDimension(30, 0),
                Width = new StyleDimension(0, 1),
                Height = new StyleDimension(40, 0)
            };
            filePanel.OnMouseOver += (evt, listeningElement) =>
            {
                filePanel.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
            };
            filePanel.OnMouseOut += (evt, listeningElement) =>
            {
                filePanel.BackgroundColor = new Color(44, 57, 105, 178);
            };
            filePanel.SetPadding(6);
            int height = 0;
            foreach (var file in Directory.GetFiles($"{Main.SavePath}/Mods/ReplaceString")
                .Where(p => Path.GetExtension(p) == ".hjson")
                .Where(p => p.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()))
                .Select(p => Path.GetFileName(p)))
            {
                var ui = new UIPanel()
                {
                    Height = new StyleDimension(30, 0),
                    Top = new StyleDimension(height, 0),
                    Width = new StyleDimension(0, 1),
                };
                ui.SetPadding(6);
                var fileName = new UIText(file)
                {
                    TextOriginY = 0.5f
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
                height += 30;
                ui.Append(fileName);
                ui.Activate();
                filePanel.Append(ui);
            }
            filePanel.Height.Pixels = height + 6 * 2;
            if (filePanel.Children.Any())
            {
                Append(filePanel);
            }
        }

        public override void Recalculate()
        {
            Height.Pixels = 30 + 6 * 2 + (filePanel?.Height.Pixels ?? 0);
            base.Recalculate();
        }
    }
}
