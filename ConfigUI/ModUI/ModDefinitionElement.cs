using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
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
        public static string HoverString { get; set; }
        public static Vector2 HoverPosition { get; set; }
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

            if(!showLoadInfo || !ReplaceString.Instance.importStates.TryGetValue(value.Name, out var state) || state == ImportState.HjsonNotExist)
            {
                return;
            }
            var button = new UIImageButton(ReplaceString.Instance.importStates[value.Name] switch
            {
                ImportState.None or ImportState.HjsonNotExist => throw new System.NotImplementedException(),
                _ => ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay", ReLogic.Content.AssetRequestMode.ImmediateLoad)
            })
            {
                Left = new StyleDimension(-28, 1),
                Height = new StyleDimension(-11, 0.5f)
            };
            Append(button);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var dimension = GetDimensions();
            Color panelColor = IsMouseHovering ? UICommon.DefaultUIBlue : UICommon.DefaultUIBlue.MultiplyRGBA(new Color(180, 180, 180));
            ConfigElement.DrawPanel2(spriteBatch, new Vector2(dimension.X, dimension.Y + 1), TextureAssets.SettingsPanel.Value, dimension.Width, dimension.Height - 2, panelColor);
            if (IsMouseHovering && showLoadInfo && ReplaceString.Instance.importStates.TryGetValue(value.Name, out var state))
            {
                if (state == ImportState.Success && ReplaceString.Instance.importInfo.TryGetValue(value.Name, out var info))
                {
                    HoverString = $"{info.name}\n{info.description}";
                }
                else
                {
                    HoverString = state.ToString();
                }
                HoverPosition = Main.MouseScreen + Vector2.One * 16;
            }
        }

        public ModDefinitionElement(ModDefinition value, bool showLoadInfo)
        {
            this.value = value;
            this.showLoadInfo = showLoadInfo;
        }
    }

}
