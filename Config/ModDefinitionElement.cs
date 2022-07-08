using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using static _ReplaceString_.Config.Constant;

namespace _ReplaceString_.Config
{
    internal static class Constant
    {
        public const int TEXT_HEIGHT = 30;
        public const int MOD_HEIGHT = 48;
        public const int ICON_SIZE = 40;
        public const float ICON_SPACE = (MOD_HEIGHT - ICON_SIZE) / 2f;
    }
    internal class LoadingElement : UIElement
    {
        public int timer = 0;
        public UIText uiText;
        public UIElement modsMenu;

        public LoadingElement(UIElement modsMenu)
        {
            this.modsMenu = modsMenu;
        }

        public override void OnInitialize()
        {
            uiText = new UIText("Loading")
            {
                VAlign = 0.5f,
                HAlign = 0.5f,
                MarginTop = 8,
                MarginLeft = 8
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timer++;
            uiText.SetText(timer switch
            {
                < 20 => "Loading",
                < 40 => "Loading.",
                < 60 => "Loading..",
                _ => "Loading..."
            });
            if (timer >= 80)
            {
                timer = 0;
            }
            if (!(bool)modsMenu.GetType().GetField("loading", BindingFlags.Public | BindingFlags.Instance).GetValue(modsMenu))
            {
                var modlist = Parent as ModDefinitionListElement;
                modlist.loading = false;
            }
        }
    }

    public class ModDefinitionElement : UIElement
    {
        public ModDefinition value;
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

            var text = new UIText($"{info.displayName}({value.Name})")
            {
                MarginLeft = MOD_HEIGHT,
                TextColor = Color.White,
                VAlign = 0.5f,
                Height = new StyleDimension(0, 0)
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
                    modlist.uiFilter.SetText("");
                }
                else if (Parent is ModDefinitionListElement list)
                {
                    list.ModList.Remove(value);
                    list.OnChange();
                    list.uiFilter.SetText("");
                }
            };
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
