using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static _ReplaceString_.ConfigUI.Constant;

namespace _ReplaceString_.ConfigUI.ModUI
{

    public class ModDefinitionElement : UIElement
    {
        public ModDefinition value;
        public UIText text;
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
                ImageScale = 0.5f,
            };

            Append(icon);

            text = new UIText($"{info.displayName}({value.Name})")
            {
                MarginLeft = MOD_HEIGHT,
                TextColor = Color.White,
                Height = new StyleDimension(0, 0),
                VAlign = 0.5f
            };
            Append(text);

            OnMouseOver += (evt, listeningElement) =>
            {
                if (text.TextColor == Color.White)
                {
                    text.TextColor = Color.Yellow;
                }
            };
            OnMouseOut += (evt, listeningElement) =>
            {
                if (text.TextColor == Color.Yellow)
                {
                    text.TextColor = Color.White;
                }
            };
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var dimension = GetDimensions();
            Color panelColor = IsMouseHovering ? UICommon.DefaultUIBlue : UICommon.DefaultUIBlue.MultiplyRGBA(new Color(180, 180, 180));
            ConfigElement.DrawPanel2(spriteBatch, new Vector2(dimension.X, dimension.Y + 1), TextureAssets.SettingsPanel.Value, dimension.Width, dimension.Height - 2, panelColor);
        }

        private void DeleteButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var list = Parent as ModDefinitionListElement;
            list.ModList.Remove(value);
        }

        public ModDefinitionElement(ModDefinition value)
        {
            this.value = value;
        }
    }

}
