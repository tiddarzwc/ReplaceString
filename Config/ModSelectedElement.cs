using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;
using static _ReplaceString_.Config.Constant;

namespace _ReplaceString_.Config
{
    public class ModSelectedElement : UIElement
    {
        internal ModDefinitionListElement ModList => Parent as ModDefinitionListElement;
        public override void OnInitialize()
        {
            base.OnActivate();
            Elements.Clear();
            foreach (var (mod, info) in ModList.GetUnAddedMod().OrderBy(mod => mod.Key))
            {
                var ui = new ModDefinitionElement(new ModDefinition(mod, info.displayName))
                {
                    MarginTop = MOD_HEIGHT * Elements.Count + TEXT_HEIGHT,
                    Width = Width,
                    Height = new StyleDimension(MOD_HEIGHT, 0)
                };
                Append(ui);
            }
            OnUpdate += ModSelectedElement_OnUpdate;
        }

        private void ModSelectedElement_OnUpdate(UIElement affectedElement)
        {
            if (ModList.needUpdate)
            {
                Elements.Clear();
                foreach (var (mod, info) in ModList.GetUnAddedMod().OrderBy(mod => mod.Key))
                {
                    var ui = new ModDefinitionElement(new ModDefinition(mod, info.displayName))
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
                        var modlist = Parent as ModDefinitionListElement;
                        RemoveChild(this);
                        modlist.ModList.Add(ui.value);
                        modlist.OnChange();
                        modlist.uiFilter.SetText("");
                    };
                }
                ModList.needUpdate = false;
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
