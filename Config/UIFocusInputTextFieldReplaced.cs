using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace _ReplaceString_.Config
{
    internal class UIFocusInputTextFieldReplaced : UIElement
    {
        public static bool TextChanged { get; set; }
        public static string Text { get; private set; } = string.Empty;
        public static UIFocusInputTextFieldReplaced instance;
        public static bool TryRepalce(UIElement parent)
        {
            Queue<UIElement> queue = new Queue<UIElement>();
            queue.Enqueue(parent);
            while(queue.Count != 0)
            {
                var ui = queue.Dequeue();
                var list = (List<UIElement>)typeof(UIElement).GetField("Elements", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ui);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].GetType().Name == "UIFocusInputTextField")
                    {
                        list.RemoveAt(i);
                        var replace = new UIFocusInputTextFieldReplaced("Filter Mods");
                        replace.SetText("");
                        replace.Top.Set(5f, 0f);
                        replace.Left.Set(10f, 0f);
                        replace.Width.Set(-20f, 1f);
                        replace.Height.Set(20f, 0f);
                        replace.OnRightClick += delegate
                        {
                            replace.SetText("");
                        };
                        replace.OnTextChange += delegate
                        {
                            TextChanged = true;
                        };
                        ui.Append(replace);
                        replace.Activate();
                        return true;
                    }
                    else if (list[i].GetType().Name == "UIFocusInputTextFieldReplaced")
                    {
                        return false;
                    }
                    queue.Enqueue(list[i]);
                }
            }
            return false;
        }
        internal bool Focused;

        internal string CurrentString = "";

        private readonly string _hintText;

        private int _textBlinkerCount;

        private int _textBlinkerState;

        public bool UnfocusOnTab
        {
            get;
            internal set;
        }

        public event EventHandler OnTextChange;

        public event EventHandler OnUnfocus;

        public event EventHandler OnTab;

        public UIFocusInputTextFieldReplaced(string hintText)
        {
            _hintText = hintText;
            instance = this;
        }

        public void SetText(string text)
        {
            if (text == null)
            {
                text = "";
            }
            if (CurrentString != text)
            {
                CurrentString = text;
                Text = text ?? string.Empty;
                OnTextChange?.Invoke(this, new EventArgs());
            }
        }

        public override void Click(UIMouseEvent evt)
        {
            Main.clrInput();
            Focused = true;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 MousePosition = new Vector2(Main.mouseX, Main.mouseY);
            if (!ContainsPoint(MousePosition) && Main.mouseLeft)
            {
                Focused = false;
                OnUnfocus?.Invoke(this, new EventArgs());
            }
            base.Update(gameTime);
        }

        private static bool JustPressed(Keys key)
        {
            if (Main.inputText.IsKeyDown(key))
            {
                return !Main.oldInputText.IsKeyDown(key);
            }
            return false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Focused)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newString = Main.GetInputText(CurrentString);
                if (!newString.Equals(CurrentString))
                {
                    CurrentString = newString;
                    Text = newString ?? string.Empty;
                    OnTextChange?.Invoke(this, new EventArgs());
                }
                else
                {
                    CurrentString = newString;
                    Text = newString ?? string.Empty;
                }
                if (JustPressed(Keys.Tab))
                {
                    if (UnfocusOnTab)
                    {
                        Focused = false;
                        OnUnfocus?.Invoke(this, new EventArgs());
                    }
                    OnTab?.Invoke(this, new EventArgs());
                }
                if (++_textBlinkerCount >= 20)
                {
                    _textBlinkerState = (_textBlinkerState + 1) % 2;
                    _textBlinkerCount = 0;
                }
            }
            string displayString = CurrentString;
            if (_textBlinkerState == 1 && Focused)
            {
                displayString += "|";
            }
            CalculatedStyle space = GetDimensions();
            if (CurrentString.Length == 0 && !Focused)
            {
                Utils.DrawBorderString(spriteBatch, _hintText, new Vector2(space.X, space.Y), Color.Gray);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), Color.White);
            }
        }
    }
}
