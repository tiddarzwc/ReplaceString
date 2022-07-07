using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace ReplaceString.Config
{
    internal class UIFocusInputTextFieldReplaced : UIElement
    {
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
				this.OnTextChange?.Invoke(this, new EventArgs());
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
				this.OnUnfocus?.Invoke(this, new EventArgs());
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
					this.OnTextChange?.Invoke(this, new EventArgs());
				}
				else
				{
					CurrentString = newString;
				}
				if (JustPressed(Keys.Tab))
				{
					if (UnfocusOnTab)
					{
						Focused = false;
						this.OnUnfocus?.Invoke(this, new EventArgs());
					}
					this.OnTab?.Invoke(this, new EventArgs());
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
