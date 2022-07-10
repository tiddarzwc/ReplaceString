using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _ReplaceString_.Config;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config.UI;

namespace _ReplaceString_.Translator.UI
{
    internal class ExportElement : ConfigElement<bool>
    {
        public ModDefinitionElement selected;
        public override void OnInitialize()
        {
            if(ReplaceString.Catcher.modInfos.Count == 0)
            {
                ModCatcher.ForceLoadIcon().Wait();
            }
            int height = 30;
            foreach(var mod in ModLoader.Mods.Where(mod => mod.Name != "ModLoader"))
            {
                var ui = new ModDefinitionElement(new ModDefinition(mod.Name, mod.DisplayName))
                {
                    MarginTop = height,
                    Height = new Terraria.UI.StyleDimension(Constant.ICON_SIZE, 0),
                    Width = Parent.Width
                };
                Append(ui);
                ui.OnClick += (evt, listeningElement) =>
                {
                    if(selected == null)
                    {
                        selected = ui;
                        ui.text.TextColor = Color.Red;
                    }
                    else if(selected == ui)
                    {
                        selected = null;
                        ui.text.TextColor = Color.Yellow;
                    }else
                    {
                        selected.text.TextColor = Color.White;
                        ui.text.TextColor = Color.Red;
                        selected = ui;
                    }

                };
                height += Constant.ICON_SIZE;
            }
        }

        public override void Recalculate()
        {
            Height.Pixels = 40 + Elements.Count * Constant.ICON_SIZE;
            if(Parent != null)
            {
                Parent.Height.Pixels = Height.Pixels;
            }
            base.Recalculate();
        }
    }
}
