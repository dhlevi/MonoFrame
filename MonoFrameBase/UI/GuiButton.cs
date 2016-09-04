using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoFrame.UI
{
    /// <summary>
    /// The base implementation of a Gui Button element provides an 
    /// extendable structure for building a simple button
    /// </summary>
    public class GuiButton : GuiElement
    {
        public Texture2D ButtonTexture { get; set; }
        public Texture2D ButtonHoverTexture { get; set; }
        public Texture2D ButtomClickedTexture { get; set; }
        public Texture2D ButtonDisabledTexture { get; set; }

        public Color ButtonMask { get; set; }
        public Color ButtonHoverMask { get; set; }
        public Color ButtonClickedMask { get; set; }
        public Color ButtonDisabledMask { get; set; }

        public SpriteFont Font { get; set; }
        public SpriteFont FontHover { get; set; }
        public SpriteFont FontClicked { get; set; }
        public SpriteFont FontDisabled { get; set; }

        public string Text { get; set; }

        public Color TextColor { get; set; }
        public Color TextHoverColor { get; set; }
        public Color TextClickedColor { get; set; }
        public Color TextDisabledColor { get; set; }

        public GuiButton(MainGame inGame)
            : base(inGame)
        {
            MainGame = inGame;

            ButtonMask = Color.White;
            ButtonHoverMask = Color.White;
            ButtonClickedMask = Color.White;
            ButtonDisabledMask = Color.White;
        }

        public override void Update(GameTime time)
        {
            if (IsVisible && IsEnabled)
            {
                base.Update(time);
            }
        }

        public override void Draw(GameTime time)
        {
            if (IsVisible)
            {
                MainGame.SpriteBatch.Begin();

                if (IsEnabled)
                {
                    // paint button
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && MouseOver)
                    {
                        MainGame.SpriteBatch.Draw(ButtomClickedTexture, HitBox, ButtonClickedMask);

                        Vector2 textSize = FontClicked.MeasureString(Text);
                        Vector2 textPosition = new Vector2((RelativeX + (Width / 2)) - (textSize.X / 2), (RelativeY + (Height / 2)) - (textSize.Y / 2));

                        MainGame.SpriteBatch.DrawString(FontClicked, Text, textPosition, TextClickedColor);
                    }
                    else if (MouseOver)
                    {
                        MainGame.SpriteBatch.Draw(ButtonHoverTexture, HitBox, ButtonHoverMask);

                        Vector2 textSize = FontHover.MeasureString(Text);
                        Vector2 textPosition = new Vector2((RelativeX + (Width / 2)) - (textSize.X / 2), (RelativeY + (Height / 2)) - (textSize.Y / 2));

                        MainGame.SpriteBatch.DrawString(FontHover, Text, textPosition, TextHoverColor);
                    }
                    else
                    {
                        MainGame.SpriteBatch.Draw(ButtonTexture, HitBox, ButtonMask);

                        Vector2 textSize = Font.MeasureString(Text);
                        Vector2 textPosition = new Vector2((RelativeX + (Width / 2)) - (textSize.X / 2), (RelativeY + (Height / 2)) - (textSize.Y / 2));

                        MainGame.SpriteBatch.DrawString(Font, Text, textPosition, TextColor);
                    }
                }
                else
                {
                    MainGame.SpriteBatch.Draw(ButtonDisabledTexture, HitBox, ButtonDisabledMask);

                    Vector2 textSize = FontHover.MeasureString(Text);
                    Vector2 textPosition = new Vector2((RelativeX + (Width / 2)) - (textSize.X / 2), (RelativeY + (Height / 2)) - (textSize.Y / 2));

                    MainGame.SpriteBatch.DrawString(FontDisabled, Text, textPosition, TextDisabledColor);
                }

                MainGame.SpriteBatch.End();

                base.Draw(time);
            }
        }
    }
}
