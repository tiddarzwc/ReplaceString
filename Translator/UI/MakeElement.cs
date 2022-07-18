using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace _ReplaceString_.Translator.UI
{
    internal class MakeElement : ConfigElement<byte>
    {
        public override void OnInitialize()
        {
            DrawLabel = false;
            UIPanel panel = new UIPanel()
            {
                Width = new StyleDimension(50, 0),
                MarginLeft = 8,
                MarginTop = 4,
                HAlign = 0,
                Height = new StyleDimension(26, 0)
            };
            panel.OnMouseOver += (evt, listeningElement) =>
            {
                panel.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
            };
            panel.OnMouseOut += (evt, listeningElement) =>
            {
                panel.BackgroundColor = new Color(44, 57, 105, 178);
            };
            panel.OnClick += (evt, listeningElement) =>
            {
                
            };
            Append(panel);

            UIText text = new UIText("导出")
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            panel.Append(text);
        }

        public override void Recalculate()
        {
            Height.Pixels = 30 + 30;
            base.Recalculate();
        }

    }
}
