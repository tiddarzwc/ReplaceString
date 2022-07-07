using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using static ReplaceString.Config.Constant;
namespace ReplaceString.Config
{
    internal static class Constant
    {
        public const int TEXT_HEIGHT = 30;
        public const int MOD_HEIGHT = 48;
        public const int ICON_SIZE = 40;
        public const float ICON_SPACE = (MOD_HEIGHT - ICON_SIZE) / 2f;
    }
    internal class ModDefinitionListElement : ConfigElement<List<ModDefinition>>
    {
        public IEnumerable<string> GetUnAddedMod()
        {
            return ReplaceString.Catcher.modInfos.Where(mod => Value.All(added => added.modName != mod.Key)).Select(mod => mod.Key);
        }
        public List<ModDefinition> ModList => Value;
        public override void OnInitialize()
        {
            base.OnInitialize();
            ResetChildren();
        }
        public void OnChange()
        {
            SetObject(GetObject());
        }
        public override void Update(GameTime gameTime)
        {
            Height.Set(MOD_HEIGHT * (Value.Count + GetUnAddedMod().Count()) + TEXT_HEIGHT * 2, 0);
            if (Parent != null && Parent.Height.Pixels < Height.Pixels)
            {
                Parent.Height.Pixels = Height.Pixels;
            }
            if (Value.Count != Elements.Count - 1)
            {
                ResetChildren();
            }
        }
        public void ResetChildren()
        {
            Elements.Clear();
            foreach (var mod in Value.OrderBy(mod => mod.DisplayName))
            {
                var ui = new ModDefinitionElement(mod)
                {
                    MarginTop = MOD_HEIGHT * Elements.Count + TEXT_HEIGHT,
                    Width = Width,
                    Height = new StyleDimension(MOD_HEIGHT, 0)
                };
                Append(ui);
            }
            var select = new ModSelectedElement
            {
                MarginTop = MOD_HEIGHT * Value.Count + TEXT_HEIGHT,
                Width = Width,
                Height = new StyleDimension(TEXT_HEIGHT + GetUnAddedMod().Count() * MOD_HEIGHT, 0)
            };
            Append(select);
            Recalculate();
            foreach (var child in Children)
            {
                child.Activate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            float settingsWidth = dimensions.Width + 1f;
            Color backgroundColor = Color.Blue;
            Color panelColor = IsMouseHovering ? backgroundColor : backgroundColor.MultiplyRGBA(new Color(180, 180, 180));
            Vector2 position = new Vector2(dimensions.X, dimensions.Y);
            DrawPanel2(spriteBatch, position, TextureAssets.SettingsPanel.Value, settingsWidth, dimensions.Height, panelColor);
            if (DrawLabel)
            {
                position.X += 8f;
                position.Y += 8f;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, TextDisplayFunction(), position, Color.White, 0f, Vector2.Zero, Vector2.One, settingsWidth);
            }
        }
    }

    public class ModDefinitionElement : UIElement
    {
        public ModDefinition value;
        public override void OnInitialize()
        {
            base.OnInitialize();
            if(!ReplaceString.Catcher.modInfos.TryGetValue(value.Name, out var info))
            {
                info = ModInfo.Default;
                var deleteButton = new UIImageButton(ModContent.Request<Texture2D>("ReplaceString/DeleteButton", ReLogic.Content.AssetRequestMode.ImmediateLoad))
                {
                    Width = new StyleDimension(32, 0),
                    Height = new StyleDimension(32, 0),
                    VAlign = 0.5f,
                    MarginLeft = Width.Pixels - 40
                };

                Append(deleteButton);
            }
            var icon = new UIImage(info.icon)
            {
                MarginLeft = ICON_SPACE - ICON_SIZE / 2f,
                MarginTop = ICON_SPACE - ICON_SIZE / 2f,
                ImageScale = 0.5f,
            };

            Append(icon);

            var text = new UIText($"{info.displayName}({value.modName})")
            {
                MarginLeft = MOD_HEIGHT,
                MarginTop = (MOD_HEIGHT - ICON_SIZE) / 2f,
                TextColor = Color.White,
                VAlign = 0.5f,
                Height = new StyleDimension(ICON_SIZE, 0)
            };
            Append(text);

            OnMouseOver += (evt, listeningElement) =>
            {
                text.TextColor = Color.Yellow;
            };
            OnMouseOut += (evt, listeningElement) =>
            {
                text.TextColor = Color.White;
            };
            OnClick += (evt, listeningElement) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                if (Parent is ModSelectedElement select)
                {
                    var modlist = select.Parent as ModDefinitionListElement;
                    select.RemoveChild(this);
                    modlist.ModList.Add(value);
                    modlist.OnChange();
                }
                else if (Parent is ModDefinitionListElement list)
                {
                    list.ModList.Remove(value);
                    list.OnChange();
                }
            };
        }


        public ModDefinitionElement(ModDefinition value)
        {
            this.value = value;
        }
    }

    public class ModSelectedElement : UIElement
    {
        public override void OnInitialize()
        {
            base.OnActivate();
            Elements.Clear();
            foreach (var mod in ((ModDefinitionListElement)Parent).GetUnAddedMod())
            {
                var ui = new ModDefinitionElement(new ModDefinition(mod))
                {
                    MarginTop = MOD_HEIGHT * Elements.Count + TEXT_HEIGHT,
                    Width = Width,
                    Height = new StyleDimension(MOD_HEIGHT, 0)
                };
                Append(ui);
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var dimemsion = GetDimensions();
            var position = new Vector2(dimemsion.X + 8, dimemsion.Y + 8);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, "Select Mod", position, Color.White, 0f, Vector2.Zero, Vector2.One);
        }
    }

}
