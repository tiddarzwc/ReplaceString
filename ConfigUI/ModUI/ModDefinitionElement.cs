using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using static _ReplaceString_.ConfigUI.Constant;

namespace _ReplaceString_.ConfigUI.ModUI
{

    public class ModDefinitionElement : UIElement
    {
        public ModDefinition value;
        public UIText text;
        public bool showLoadInfo;
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
        public override void OnInitialize()
        {
            base.OnInitialize();
            if (!ReplaceString.Catcher.modInfos.TryGetValue(value.Name, out var info))
            {
                info = ModInfo.Default with { displayName = value.DisplayName };
                var tex = TextureAssets.Trash;
                tex.Wait();
                var deleteButton = new UIImageButton(tex)
                {
                    MarginLeft = Parent.GetDimensions().Width - ICON_SPACE - 32,
                    MarginTop = ICON_SPACE
                };
                deleteButton.OnClick += DeleteButton_OnClick;
                Append(deleteButton);
            }
            var icon = new UIImage(info.icon)
            {
                MarginLeft = ICON_SPACE - ICON_SIZE / 2f,
                MarginTop = ICON_SPACE - ICON_SIZE / 2f,
                ImageScale = 0.5f
            };

            Append(icon);

            text = new UIText($"{info.displayName} ({value.Name})")
            {
                MarginLeft = MOD_HEIGHT,
                TextColor = DefaultColor,
                Height = new StyleDimension(0, 0),
                VAlign = 0.5f
            };
            Append(text);

            OnMouseOver += (evt, listeningElement) =>
            {
                if (text.TextColor == DefaultColor)
                {
                    text.TextColor = Color.Yellow;
                }
            };
            OnMouseOut += (evt, listeningElement) =>
            {
                if (text.TextColor == Color.Yellow)
                {
                    text.TextColor = DefaultColor;
                }
            };
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var dimension = GetDimensions();
            Color panelColor = IsMouseHovering ? UICommon.DefaultUIBlue : UICommon.DefaultUIBlue.MultiplyRGBA(new Color(180, 180, 180));
            ConfigElement.DrawPanel2(spriteBatch, new Vector2(dimension.X, dimension.Y + 1), TextureAssets.SettingsPanel.Value, dimension.Width, dimension.Height - 2, panelColor);
            if (IsMouseHovering && showLoadInfo && ReplaceString.Instance.importStates.TryGetValue(value.Name, out var state))
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, state.ToString(), Main.MouseScreen + Vector2.One * 16, Color.White, 0f, Vector2.Zero, Vector2.One, 0);
            }
        }

        private void DeleteButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var list = Parent as ModDefinitionListElement;
            list.ModList.Remove(value);
        }

        public ModDefinitionElement(ModDefinition value, bool showLoadInfo)
        {
            this.value = value;
            this.showLoadInfo = showLoadInfo;
        }
    }

}
