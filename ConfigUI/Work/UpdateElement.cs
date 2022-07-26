using System.IO;
using _ReplaceString_.ConfigUI;
using _ReplaceString_.ConfigUI.Work;
using _ReplaceString_.Data;
using Hjson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
namespace _ReplaceString_.Translator.UI;

internal class UpdateElement : ConfigElement
{
    public FileSelectedUI oldHjson;
    public FileSelectedUI newHjson;
    public FileSelectedUI transHjson;
    public override void OnInitialize()
    {
        base.OnInitialize();
        DrawLabel = false;
        OnUpdate += delegate
        {
            if (UIFocusInputTextFieldReplaced.TextChanged)
            {
                ResetChildren();
                UIFocusInputTextFieldReplaced.TextChanged = false;
            }
        };
        ResetChildren();
    }

    private void ResetChildren()
    {
        Elements.Clear();
        UIPanel panel = new UIPanel()
        {
            Width = new StyleDimension(70, 0),
            MarginLeft = 8,
            MarginTop = 4,
            HAlign = 0,
            Height = new StyleDimension(26, 0)
        };
        panel.OnMouseOver += (evt, listeningElement) =>
        {
            if (oldHjson.IsSelected && newHjson.IsSelected && transHjson.IsSelected)
            {
                panel.BackgroundColor = new Color(44, 57, 105, 178).MultiplyRGBA(new Color(180, 180, 180));
            }
        };
        panel.OnMouseOut += (evt, listeningElement) =>
        {
            panel.BackgroundColor = new Color(44, 57, 105, 178);
        };
        panel.OnClick += (evt, listeningElement) =>
        {
            if (oldHjson.IsSelected && newHjson.IsSelected && transHjson.IsSelected)
            {
                var path = $"{Main.SavePath}/Mods/ReplaceString/";
                var o = TreeNode.ReadHjson(HjsonValue.Load(path + oldHjson.Text[2..]));
                var n = TreeNode.ReadHjson(HjsonValue.Load(path + newHjson.Text[2..]));
                var t = TreeNode.ReadHjson(HjsonValue.Load(path + transHjson.Text[2..]));
                Package.Update.CacheInfo.Clear();
                Package.Update.UpdateTree(o, n, t);
                File.WriteAllText(path + $"{Path.GetFileNameWithoutExtension(newHjson.Text[2..])}-update.hjson", n.BuildHjson(0).ToString());
                File.WriteAllText(path + "UpdateLog.txt", Package.Update.CacheInfo.ToString());
                SoundEngine.PlaySound(SoundID.MenuOpen);
            }
        };
        Append(panel);

        UIText text = new UIText("Update")
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            TextColor = Color.Gray
        };
        text.OnUpdate += delegate
        {
            text.TextColor = oldHjson.IsSelected && newHjson.IsSelected && transHjson.IsSelected ? Color.White : Color.Gray;
        };
        panel.Append(text);
        panel.Activate();

        oldHjson = new FileSelectedUI("旧版本Hjson")
        {
            Top = new StyleDimension(30 + 6, 0),
            Left = new StyleDimension(8, 0),
            Width = new StyleDimension(-16, 1),
        };
        Append(oldHjson);
        oldHjson.Activate();

        newHjson = new FileSelectedUI("新版本Hjson")
        {
            Left = new StyleDimension(8, 0),
            Width = new StyleDimension(-16, 1),
        };
        newHjson.OnUpdate += evt =>
        {
            evt.Top.Pixels = oldHjson.Height.Pixels + oldHjson.Top.Pixels + 6;
        };
        Append(newHjson);
        newHjson.Activate();

        transHjson = new FileSelectedUI("翻译后的Hjson")
        {
            Left = new StyleDimension(8, 0),
            Width = new StyleDimension(-16, 1),
        };
        transHjson.OnUpdate += evt =>
        {
            evt.Top.Pixels = newHjson.Top.Pixels + newHjson.Height.Pixels + 6;
        };
        Append(transHjson);
        transHjson.Activate();
    }

    public override void Recalculate()
    {
        float h = 10 + 8 + 18;
        foreach (var child in Elements)
        {
            h += child.Height.Pixels;
        }
        Height.Pixels = h;
        if (Parent != null)
        {
            Parent.Height.Pixels = Height.Pixels;
        }
        base.Recalculate();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        CalculatedStyle dimensions = GetDimensions();
        DrawPanel2(spriteBatch,
            new Vector2(dimensions.X, dimensions.Y),
            TextureAssets.SettingsPanel.Value,
            (float)(dimensions.Width + 1f),
            dimensions.Height,
            new Color(44, 57, 105, 178)
            );
    }
}
