using System;
using System.IO;
using System.Linq;
using _ReplaceString_.Config;
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
using Terraria.UI.Chat;

namespace _ReplaceString_.Translator.UI
{
    internal class ExportElement : ConfigElement<bool>
    {
        public ModDefinitionElement selected;
        public bool fliterHooked = false;
        public override void OnInitialize()
        {
            if (ModCatcher.IsLoading())
            {
                DrawLabel = false;
                ModCatcher.OnFinish += OnInitialize;
                return;
            }

            DrawLabel = true;

            UIPanel panel = new UIPanel()
            {
                Width = new StyleDimension(50, 0),
                MarginRight = 8,
                MarginTop = 4,
                HAlign = 1f,
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
                    var mod = ModLoader.GetMod(selected.value.Name);
                    Export export = new Export(mod);
                    if (!Directory.Exists($"{Main.SavePath}/Mods/ReplaceString"))
                    {
                        Directory.CreateDirectory($"{Main.SavePath}/Mods/ReplaceString");
                    }
                    using FileStream file = new FileStream($"{Main.SavePath}/Mods/ReplaceString/{mod.Name}-{mod.Version.ToString().Replace(".", "")}-{Language.ActiveCulture.Name}.hjson", FileMode.Create);
                    export.Hjson(file);
                }
            };
            Append(panel);

            UIText text = new UIText("导出")
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            panel.Append(text);

            ResetChildren();


        }
        public override void Update(GameTime gameTime)
        {
            if(!fliterHooked)
            {
                UIFocusInputTextFieldReplaced.TryRepalce(Parent.Parent.Parent.Parent.Parent);
                fliterHooked = true;
            }
            if(UIFocusInputTextFieldReplaced.TextChanged)
            {
                ResetChildren();
                UIFocusInputTextFieldReplaced.TextChanged = false;
            }
            base.Update(gameTime);
        }
        public void ResetChildren()
        {
            int height = 30;
            selected = null;
            Elements.RemoveAll(ui => ui is ModDefinitionElement);
            foreach (var mod in ModLoader.Mods.Where(mod => mod.Name != "ModLoader")
                .Where(mod => mod.Name.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text) ||
                mod.DisplayName.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text)))
            {
                var ui = new ModDefinitionElement(new ModDefinition(mod.Name, mod.DisplayName))
                {
                    MarginLeft = 8,
                    MarginTop = height,
                    Height = new StyleDimension(Constant.MOD_HEIGHT, 0),
                    Width = new StyleDimension(-16, 1)
                };
                Append(ui);
                ui.OnClick += (evt, listeningElement) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    if (selected == null)
                    {
                        selected = ui;
                        ui.text.TextColor = Color.Red;
                    }
                    else if (selected == ui)
                    {
                        selected = null;
                        ui.text.TextColor = Color.Yellow;
                    }
                    else
                    {
                        selected.text.TextColor = Color.White;
                        ui.text.TextColor = Color.Red;
                        selected = ui;
                    }

                };
                height += Constant.MOD_HEIGHT;
                ui.Activate();
            }
        }
        public override void Recalculate()
        {
            if (ModCatcher.IsLoading())
            {
                base.Recalculate();
                return;
            }

            Height.Set(Elements.Count * Constant.MOD_HEIGHT -8 , 0);
            if (Parent != null)
            {
                Parent.Height.Pixels = Height.Pixels;
            }
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            float settingsWidth = dimensions.Width + 1f;
            Vector2 position = new Vector2(dimensions.X, dimensions.Y);
            if (!DrawLabel)
            {
                position.X += 8f;
                position.Y += 8f;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, "Loading...", position, Color.White, 0f, Vector2.Zero, new Vector2(0.8f), settingsWidth);
            }
         
        }
    }
}
