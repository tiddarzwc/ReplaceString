using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using static _ReplaceString_.Config.Constant;

namespace _ReplaceString_.Config
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

            Height.Set(MOD_HEIGHT * (Value.Where(mod => mod.Name.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()) || mod.DisplayName.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower())).Count() + GetUnAddedMod().Count()) + TEXT_HEIGHT * 2 + 8, 0);
            if (Parent != null)
            {
                Parent.Height = Height;
            }

            base.Recalculate();
        }
        public void ResetChildren()
        {
            Elements.Clear();
            foreach (var mod in Value.Where(mod => mod.Name.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()) || mod.DisplayName.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower())).OrderBy(mod => mod.DisplayName))
            {
                var ui = new ModDefinitionElement(mod)
                {
                    MarginLeft = 8,
                    MarginTop = MOD_HEIGHT * Elements.Count + TEXT_HEIGHT,
                    Width = new StyleDimension(-16, 1),
                    Height = new StyleDimension(MOD_HEIGHT, 0)
                };
                Append(ui);
                ui.Activate();
                ui.OnClick += (evt, listeningElement) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    ModList.Remove(ui.value);
                    OnChange();
                    UIFocusInputTextFieldReplaced.instance.SetText("");
                    needUpdate = true;
                };
            }
            var select = new ModSelectedElement
            {
                MarginTop = MOD_HEIGHT * Value.Where(mod => mod.Name.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower()) || mod.DisplayName.ToLower().StartsWith(UIFocusInputTextFieldReplaced.Text.ToLower())).Count() + TEXT_HEIGHT,
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
    }

}
