using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _ReplaceString_.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace _ReplaceString_.Translator.UI
{
    internal class ExportElement : ConfigElement<bool>
    {
        public ModDefinitionElement selected;
        public override void OnInitialize()
        {
            if(ModCatcher.IsLoading())
            {
                DrawLabel = false;
                ModCatcher.OnFinish += OnInitialize;
                return;
            }

            DrawLabel = true;
            int height = 30;
            foreach(var mod in ModLoader.Mods.Where(mod => mod.Name != "ModLoader"))
            {
                var ui = new ModDefinitionElement(new ModDefinition(mod.Name, mod.DisplayName))
                {
                    MarginTop = height,
                    Height = new StyleDimension(Constant.ICON_SIZE, 0),
                    Width = Parent.Width
                };
                Append(ui);
                ui.OnClick += (evt, listeningElement) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    if(selected == null)
                    {
                        selected = ui;
                        ui.text.TextColor = Color.Red;
                    }
                    else if(selected == ui)
                    {
                        selected = null;
                        ui.text.TextColor = Color.Yellow;
                    }else
                    {
                        selected.text.TextColor = Color.White;
                        ui.text.TextColor = Color.Red;
                        selected = ui;
                    }

                };
                height += Constant.ICON_SIZE;
                ui.Activate();
            }
        }

        public override void Recalculate()
        {
            if(ModCatcher.IsLoading())
            {
                base.Recalculate();
                return;
            }

            Height.Set(40 + Elements.Count * Constant.ICON_SIZE, 0);
            if(Parent != null)
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
            Vector2 baseScale = new Vector2(0.8f);
            if (!DrawLabel)
            {
                position.X += 8f;
                position.Y += 8f;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, "Loading...", position, Color.White, 0f, Vector2.Zero, baseScale, settingsWidth);
            }
        }
    }
}
