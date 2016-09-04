using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFrame.UI.Events;
using System;

namespace MonoFrame.UI
{
    /// <summary>
    /// The base GUI label element provides an extendable base
    /// for creating simple labels
    /// </summary>
    public class GuiLabel : GuiElement
    {
        private string _Text;
        public SpriteFont Font { get; set; }
        public Color TextColor { get; set; }

        public GuiLabel(MainGame inGame)
            : base(inGame)
        {
            MainGame = inGame;
            _Text = "";
        }

        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (Font != null)
                {
                    Vector2 textSize = Font.MeasureString(value);
                    Width = (int)textSize.X;
                    Height = (int)textSize.Y;
                }
                _Text = value;
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            if (IsVisible)
            {
                MainGame.SpriteBatch.Begin();

                MainGame.SpriteBatch.DrawString(Font, Text, new Vector2(RelativeX, RelativeY), TextColor);

                MainGame.SpriteBatch.End();

                base.Draw(time);
            }
        }
    }
}
