using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoFrame.UI
{
    /// <summary>
    /// The GuiTextArea control is similar to the text box control, however this
    /// defines an area, with multi-line handling, where you can enter text.
    /// </summary>
    public class GuiTextArea : GuiElement
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color TextColor { get; set; }

        public bool AutoCalculateHeight { get; set; }

        public GuiTextArea(MainGame inGame)
            : base(inGame)
        {
            Text = "";
            TextColor = Color.Black;
            AutoCalculateHeight = false;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            if (IsVisible && Font != null && !string.IsNullOrEmpty(Text))
            {
                RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = true };

                MainGame.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, rasterizerState, null, null);

                // get the current graphics device scissor rect. Set a new hitbox
                Rectangle previousRect = MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle;
                MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle = HitBox;

                string[] words = Text.Split(' ');

                int lineHeight = (int)Font.MeasureString(Text).Y;

                string lineToWrite = "";
                string lineToTest = "";
                int lineIndex = 0;
                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];

                    lineToTest += word + " ";

                    Vector2 stringLength = Font.MeasureString(lineToTest);

                    if (stringLength.X + 5 >= Width)
                    {
                        lineToTest = word + " ";
                        MainGame.SpriteBatch.DrawString(Font, lineToWrite, new Vector2(RelativeX + 5, (RelativeY + (lineIndex * lineHeight)) + 5), TextColor);
                        lineToWrite = word + " ";
                        lineIndex++;
                    }
                    else
                    {
                        lineToWrite += word + " ";
                    }
                }

                if(!string.IsNullOrEmpty(lineToWrite)) MainGame.SpriteBatch.DrawString(Font, lineToWrite, new Vector2(RelativeX + 5, (RelativeY + (lineIndex * lineHeight)) + 5), TextColor);

                if (AutoCalculateHeight) Height = (int)Font.MeasureString(Text).Y * lineIndex;

                //reset the scissor rect
                MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle = previousRect;

                MainGame.SpriteBatch.End();

                base.Draw(time);
            }
        }
    }
}
