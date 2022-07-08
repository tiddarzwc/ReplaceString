using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using static ReplaceString.Config.Constant;

namespace ReplaceString.Config
{
    internal class ModDefinitionListElement : ConfigElement<List<ModDefinition>>
    {
        public bool loading;
        public string filterWord = string.Empty;
        public bool needUpdate = true;
        public bool filterHooked = false;
        public UIFocusInputTextFieldReplaced uiFilter = null;
        public IEnumerable<KeyValuePair<string, ModInfo>> GetUnAddedMod()
        {
            return ReplaceString.Catcher.modInfos.Where(mod => mod.Key.ToLower().StartsWith(filterWord.ToLower()) || mod.Value.displayName.ToLower().StartsWith(filterWord.ToLower())).Where(mod => Value.All(added => added.Name != mod.Key));
        }
        public List<ModDefinition> ModList => Value;
        public override void OnInitialize()
        {
            base.OnInitialize();

            if (ReplaceString.Catcher.modInfos.Count == 0)
            {
                UIElement modsMenu = (UIElement)typeof(ModContent).Assembly.DefinedTypes.First(t => t.Name == "Interface")
                    .GetField("modsMenu", BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(null);
                modsMenu.GetType().GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(modsMenu, new CancellationTokenSource());
                modsMenu.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.Public).SetValue(modsMenu, true);
                modsMenu.GetType().GetMethod("Populate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(modsMenu, null);
                Height.Set(TEXT_HEIGHT * 2, 0);
                loading = true;
                var loadUI = new LoadingElement(modsMenu)
                {
                    Height = new StyleDimension(TEXT_HEIGHT, 0),
                    MarginTop = TEXT_HEIGHT
                };
                Append(loadUI);
                loadUI.Activate();
                Append(new UIElement());//加一个空UI凑数
            }
            else
            {
                loading = false;
                ResetChildren();
            }
        }
        public void OnChange()
        {
            SetObject(GetObject());
        }
        public override void Update(GameTime gameTime)
        {
            if (loading)
            {
                base.Update(gameTime);
                return;
            }
            if (!filterHooked)
            {
                //TODO tml随便一动就可能炸掉
                UIElement panel = Parent.Parent.Parent.Parent.Parent.Children.First(t => t.GetType() == typeof(UIPanel));
                var list = (List<UIElement>)panel.GetType().GetField("Elements", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(panel);
                var replace = new UIFocusInputTextFieldReplaced("Filter Mods");
                replace.SetText("");
                replace.Top.Set(5f, 0f);
                replace.Left.Set(10f, 0f);
                replace.Width.Set(-20f, 1f);
                replace.Height.Set(20f, 0f);
                replace.OnTextChange += delegate
                {
                    needUpdate = true;
                    filterWord = replace.CurrentString;
                };
                replace.OnRightClick += delegate
                {
                    replace.SetText("");
                };
                list.Clear();
                panel.Append(replace);
                replace.Activate();
                uiFilter = replace;
                filterHooked = true;
            }

            if (Value.Count != Elements.Count - 1)
            {
                ResetChildren();
            }
            base.Update(gameTime);
        }
        public override void Recalculate()
        {
            Height.Set(MOD_HEIGHT * (Value.Count + GetUnAddedMod().Count()) + TEXT_HEIGHT * 2, 0);
            if (Parent != null)
            {
                Parent.Height.Pixels = Height.Pixels;
            }
            base.Recalculate();
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
                ui.Activate();
            }
            var select = new ModSelectedElement
            {
                MarginTop = MOD_HEIGHT * Value.Count + TEXT_HEIGHT,
                Width = Width,
                Height = new StyleDimension(TEXT_HEIGHT + GetUnAddedMod().Count() * MOD_HEIGHT, 0)
            };
            Append(select);
            select.Activate();
            Recalculate();
        }
    }

}
