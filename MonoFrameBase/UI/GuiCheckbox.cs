using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFrame.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFrame.UI
{
    /// <summary>
    /// The Gui Check box control provides an extendable base for creating
    /// Check box elements
    /// </summary>
    public class GuiCheckbox : GuiElement
    {
        public bool IsChecked { get; set; }

        public Texture2D CheckedTexture { get; set; }
        public Texture2D UncheckedTexture { get; set; }
        public Color Mask { get; set; }

        public string Label { get; set; }
        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public event EventHandler<EventArgs> OnCheck;

        public GuiCheckbox(MainGame inGame)
            : base(inGame)
        {
            Mask = Color.White;
            FontColor = Color.White;
            IsChecked = false;
        }

        public override void Update(GameTime time)
        {
            if (IsVisible && IsEnabled)
            {
                MouseUp = false;

                base.Update(time);

                if(MouseUp && MouseOver)
                {
                    IsChecked = !IsChecked;
                    OnCheckEvent(new EventArgs(), OnCheck);
                }
            }
        }

        public override void Draw(GameTime time)
        {
            if (IsVisible && CheckedTexture != null && UncheckedTexture != null)
            {
                //set a scissor rasterizer so we can clip the text to paint so it doesn't spill outside of the box width
                MainGame.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, null, null, null);

                if(IsChecked) MainGame.SpriteBatch.Draw(CheckedTexture, HitBox, Mask);
                else MainGame.SpriteBatch.Draw(UncheckedTexture, HitBox, Mask);

                if(!string.IsNullOrEmpty(Label) && Font != null)
                {
                    MainGame.SpriteBatch.DrawString(Font, Label, new Vector2(RelativeX + HitBox.Width + 7 , RelativeY), FontColor);
                }

                MainGame.SpriteBatch.End();

                base.Draw(time);
            }
        }

        // Event Handling
        protected virtual void OnCheckEvent(EventArgs e, EventHandler<EventArgs> handler)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}