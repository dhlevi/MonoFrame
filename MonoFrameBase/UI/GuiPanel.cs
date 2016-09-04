using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFrame.UI
{
    /// <summary>
    /// Gui Panel is the "base" extention of the GuiElement, and many other Gui Elements can be built from
    /// extending the panel and adding custom functionality. The panel has very little function itself, but 
    /// can be used to store collections of child elements when you want to control the background and edge
    /// images.
    /// </summary>
    public class GuiPanel : GuiElement
    {
        public Texture2D BackgroundTexture { get; set; }

        public Texture2D TopLeftCornerTexture { get; set; }
        public Texture2D BottomLeftCornerTexture { get; set; }
        public Texture2D TopRightCornerTexture { get; set; }
        public Texture2D BottomRightCornerTexture { get; set; }

        public Texture2D TopTexture { get; set; }
        public Texture2D BottomTexture { get; set; }
        public Texture2D RightTexture { get; set; }
        public Texture2D LeftTexture { get; set; }
        
        public GuiPanel(MainGame inGame)
            : base(inGame)
        {
        }

        public void MatchPanelDimensionsToBackground()
        {
            if(BackgroundTexture != null)
            {
                Width = BackgroundTexture.Width;
                Height = BackgroundTexture.Height;
            }
        }

        public bool HasBorders
        {
            get
            {
                return TopLeftCornerTexture != null &&
                        BottomLeftCornerTexture != null &&
                        TopRightCornerTexture != null &&
                        BottomRightCornerTexture != null &&
                        TopTexture != null &&
                        BottomTexture != null &&
                        RightTexture != null &&
                        LeftTexture != null;
            }
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
                //set a scissor rasterizer so we can clip the text to paint so it doesn't spill outside of the box width
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.ScissorTestEnable = true;

                //get the current graphics device scissor rect.
                Rectangle previousRect = MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle;

                //set the scissor rect to the box
                MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle = HitBox;

                MainGame.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, rasterizerState, null, null);

                // draw panel background
                if (BackgroundTexture != null)
                    MainGame.SpriteBatch.Draw(BackgroundTexture, HitBox, Color.White);

                if (HasBorders)
                {
                    //draw corners
                    MainGame.SpriteBatch.Draw(TopLeftCornerTexture, new Rectangle(RelativeX, RelativeY, TopLeftCornerTexture.Width, TopLeftCornerTexture.Height), Color.White);
                    MainGame.SpriteBatch.Draw(TopRightCornerTexture, new Rectangle(RelativeX + Width - TopRightCornerTexture.Width, RelativeY, TopRightCornerTexture.Width, TopRightCornerTexture.Height), Color.White);
                    MainGame.SpriteBatch.Draw(BottomLeftCornerTexture, new Rectangle(RelativeX, Height - BottomLeftCornerTexture.Height, BottomLeftCornerTexture.Width, BottomLeftCornerTexture.Height), Color.White);
                    MainGame.SpriteBatch.Draw(BottomRightCornerTexture, new Rectangle(RelativeX + Width - BottomRightCornerTexture.Width, Height - BottomRightCornerTexture.Height, BottomRightCornerTexture.Width, BottomRightCornerTexture.Height), Color.White);

                    //draw sides
                    MainGame.SpriteBatch.Draw(TopTexture, new Rectangle(RelativeX + TopLeftCornerTexture.Width, RelativeY, Width - TopLeftCornerTexture.Width - TopRightCornerTexture.Width, TopTexture.Height), Color.White);
                    MainGame.SpriteBatch.Draw(BottomTexture, new Rectangle(RelativeX + BottomLeftCornerTexture.Width, RelativeY + Height - BottomTexture.Height, Width - BottomLeftCornerTexture.Width - BottomRightCornerTexture.Width, BottomTexture.Height), Color.White);
                    MainGame.SpriteBatch.Draw(RightTexture, new Rectangle(RelativeX + Width - RightTexture.Width, RelativeY + TopRightCornerTexture.Height, RightTexture.Width, Height - TopRightCornerTexture.Height - BottomRightCornerTexture.Height), Color.White);
                    MainGame.SpriteBatch.Draw(LeftTexture, new Rectangle(RelativeX, RelativeY + TopLeftCornerTexture.Height, LeftTexture.Width, Height - TopLeftCornerTexture.Height - BottomLeftCornerTexture.Height), Color.White);
                }

                MainGame.SpriteBatch.End();

                //reset the scissor rect
                MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle = previousRect;

                base.Draw(time);
            }
        }
    }
}
