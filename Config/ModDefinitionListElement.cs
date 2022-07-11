using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using static _ReplaceString_.Config.Constant;

namespace _ReplaceString_.Config
{
    internal class ModDefinitionListElement : ConfigElement<List<ModDefinition>>
    {
        public string filterWord = string.Empty;
        public bool needUpdate = false;
        public bool filterHooked = false; 
        public UIFocusInputTextFieldReplaced uiFilter = null;
        public IEnumerable<KeyValuePair<string, ModInfo>> GetUnAddedMod()
        {
            return ReplaceString.Catcher.modInfos.Where(mod => mod.Key.ToLower().StartsWith(filterWord.ToLower()) || mod.Value.displayName.ToLower().StartsWith(filterWord.ToLower())).Where(mod => Value.All(added => added.Name != mod.Key));
        }
        public List<ModDefinition> ModList => Value;
        public override void OnInitialize()
        {
            if(ModCatcher.IsLoading())
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
                //TODO tml随便一动就可能炸掉
                UIElement panel = Parent.Parent.Parent.Parent.Parent.Children.First(t => t.GetType() == typeof(UIPanel));
                var list = (List<UIElement>)panel.GetType().GetField("Elements", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(panel);
                var replace = new UIFocusInputTextFieldReplaced("Filter Mods");
                replace.SetText("");
                replace.Top.Set(5f, 0f);
                replace.Left.Set(10f, 0f);
                replace.Width.Set(-20f, 1f);
                replace.Height.Set(20f, 0f);
                replace.OnTextChange += delegate
                {
                    needUpdate = true;
                    filterWord = replace.CurrentString;
                };
                replace.OnRightClick += delegate
                {
                    replace.SetText("");
                };
                list.Clear();
                panel.Append(replace);
                replace.Activate();
                uiFilter = replace;
                filterHooked = true;
            }

            if (needUpdate)
            {
                ResetChildren();
            }
            base.Update(gameTime);
        }
        public override void Recalculate()
        {
            if(ModCatcher.IsLoading())
            {
                DrawLabel = false;
                base.Recalculate();
                return;
            }

            Height.Set(MOD_HEIGHT * (Value.Where(mod => mod.Name.ToLower().StartsWith(filterWord.ToLower()) || mod.DisplayName.ToLower().StartsWith(filterWord.ToLower())).Count() + GetUnAddedMod().Count()) + TEXT_HEIGHT * 2, 0);
            if (Parent != null)
            {
                Parent.Height = Height;
            }

            base.Recalculate();
        }
        public void ResetChildren()
        {
            Elements.Clear();
            foreach (var mod in Value.Where(mod => mod.Name.ToLower().StartsWith(filterWord.ToLower()) || mod.DisplayName.ToLower().StartsWith(filterWord.ToLower())).OrderBy(mod => mod.DisplayName))
            {
                var ui = new ModDefinitionElement(mod)
                {
                    MarginTop = MOD_HEIGHT * Elements.Count + TEXT_HEIGHT,
                    Width = Width,
                    Height = new StyleDimension(MOD_HEIGHT, 0)
                };
                Append(ui);
                ui.Activate();
                ui.OnClick += (evt, listeningElement) =>
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    ModList.Remove(ui.value);
                    OnChange();
                    uiFilter.SetText("");
                    needUpdate = true;
                };
            }
            var select = new ModSelectedElement
            {
                MarginTop = MOD_HEIGHT * Value.Where(mod => mod.Name.ToLower().StartsWith(filterWord.ToLower()) || mod.DisplayName.ToLower().StartsWith(filterWord.ToLower())).Count() + TEXT_HEIGHT,
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
