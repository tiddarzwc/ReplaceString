using System.Collections.Generic;
using System.Linq;
using _ReplaceString_.ConfigUI.ModUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using static _ReplaceString_.ConfigUI.Constant;

namespace _ReplaceString_.ConfigUI.A_Load
{
    internal class ModDefinitionListElement : ConfigElement<List<ModDefinition>>
    {
        public bool needUpdate = false;
        public bool filterHooked = false;
        public IEnumerable<KeyValuePair<string, ModInfo>> GetUnAddedMod()
        {
            return ReplaceString.Catcher.modInfos
                .Where(mod => mod.Key.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower())
                || mod.Value.displayName.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()))
                .Where(mod => Value.All(added => added.Name != mod.Key));
        }
        public List<ModDefinition> ModList => Value;
        public override void OnInitialize()
        {
            if (ModCatcher.IsLoading())
            {
                ModCatcher.OnFinish += () =>
                {
                    needUpdate = true;
                    DrawLabel = true;
                };
                return;
            }
            ResetChildren();
        }
        public void OnChange()
        {
            SetObject(GetObject());
        }
        public override void Update(GameTime gameTime)
        {
            if (!filterHooked)
            {
                UIFocusInputTextFieldReplaced.TryRepalce(Parent.Parent.Parent.Parent.Parent);
                filterHooked = true;
                UIFocusInputTextFieldReplaced.instance.hintText = "Fliter Mods";
            }
            UIFocusInputTextFieldReplaced.enable = true;

            if (UIFocusInputTextFieldReplaced.TextChanged || needUpdate)
            {
                UIFocusInputTextFieldReplaced.TextChanged = false;
                ResetChildren();
            }
            base.Update(gameTime);
        }
        public override void Recalculate()
        {
            if (ModCatcher.IsLoading())
            {
                DrawLabel = false;
                base.Recalculate();
                return;
            }

            Height.Set(Elements.Sum(ui => ui.Height.Pixels) + TEXT_HEIGHT + 8, 0);
            if (Parent != null)
            {
                Parent.Height = Height;
            }

            base.Recalculate();
        }
        public void ResetChildrenTop()
        {
            var it = Elements.GetEnumerator();
            if(!it.MoveNext())
            {
                return;
            }
            var last = it.Current;
            while(it.MoveNext())
            {
                var current = it.Current;
                current.Top.Pixels = last.Top.Pixels + last.Height.Pixels;
                last = current;
            }
        }
        public void ResetChildren()
        {
            Elements.Clear();
            float top = TEXT_HEIGHT;
            foreach (var mod in Value.Where(mod => mod.Name.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()) || mod.DisplayName.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower())).OrderBy(mod => mod.DisplayName))
            {
                var ui = new ModDefinitionElement(mod, true)
                {
                    Left = new StyleDimension(8, 0),
                    Top = new StyleDimension(top, 0),
                    Width = new StyleDimension(-16, 1),
                    Height = new StyleDimension(MOD_HEIGHT, 0)
                };
                Append(ui);
                ui.text.OnClick += delegate
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    ModList.Remove(ui.value);
                    OnChange();
                    UIFocusInputTextFieldReplaced.instance.SetText("");
                    needUpdate = true;
                };
                ui.Activate();
                top += ui.Height.Pixels;
            }
            var select = new ModSelectedElement
            {
                Top = new StyleDimension(top, 0),
                Width = Width,
                Height = new StyleDimension(TEXT_HEIGHT + GetUnAddedMod().Count() * MOD_HEIGHT, 0)
            };
            Append(select);
            select.Activate();
            Recalculate();
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (!string.IsNullOrWhiteSpace(ModDefinitionElement.HoverString))
            {
                var pos = Main.MouseScreen + Vector2.One * 16;
                var size = FontAssets.ItemStack.Value.MeasureString(ModDefinitionElement.HoverString);
                DrawPanel2(spriteBatch,
                    pos,
                    TextureAssets.SettingsPanel.Value,
                    size.X + 16,
                    size.Y + 12,
                    new Color(44, 57, 105, 255)
                    );
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value,
                    ModDefinitionElement.HoverString, pos + new Vector2(8,8),
                    Color.White, 0f, Vector2.Zero, Vector2.One, 0);
            }
        }
    }

}
