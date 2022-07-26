using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;
using static _ReplaceString_.ConfigUI.Constant;

namespace _ReplaceString_.ConfigUI.ModUI
{
    public class ModSelectedElement : UIElement
    {
        internal ModDefinitionListElement ModList => Parent as ModDefinitionListElement;
        public override void OnInitialize()
        {
            base.OnActivate();
            ResetChildren();
            OnUpdate += ModSelectedElement_OnUpdate;
        }
        public void ResetChildren()
        {
            Elements.Clear();
            foreach (var (mod, info) in ModList.GetUnAddedMod().OrderBy(mod => mod.Key))
            {
                var ui = new ModDefinitionElement(new ModDefinition(mod, info.displayName), false)
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
                    var modlist = ModList;
                    RemoveChild(ui);
                    modlist.ModList.Add(ui.value);
                    modlist.OnChange();
                    modlist.needUpdate = true;
                    UIFocusInputTextFieldReplaced.instance.SetText("");
                };
            }
        }
        private void ModSelectedElement_OnUpdate(UIElement affectedElement)
        {
            if (ModList.needUpdate)
            {
                ResetChildren();
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
