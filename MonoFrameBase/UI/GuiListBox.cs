using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFrame.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFrame.UI
{
    /// <summary>
    /// The Gui List Box provides an extendable base for basic list box functionality
    /// Extend in a custom class to provide defaults to text, images and hovers.
    /// </summary>
    public class GuiListBox : GuiElement
    {
        public List<ListBoxItem> Items { get; set; }
        private ListBoxItem _SelectedItem;

        public SpriteFont Font { get; set; }
        public Color TextColor { get; set; }
        public Color TextHoverColor { get; set; }
        public Color TextSelectedColor { get; set; }
        public Color SelectColor { get; set; }
        public Color HoverColor { get; set; }

        public Texture2D BackgroundTexture { get; set; }
        public Color BackgroundMask { get; set; }
        public Texture2D BorderTexture { get; set; }
        public Color BorderMask { get; set; }
        public Texture2D ItemBackgroundTexture { get; set; }
        public Texture2D ItemHoverBackgroundTexture { get; set; }
        public Texture2D ItemSelectedBackgroundTexture { get; set; }
        public Color ItemBackgroundMask { get; set; }

        public event EventHandler<ListBoxSelectionChangedEventArgs> SelectionChanged;

        private int YOffset { get; set; }

        public GuiListBox(MainGame inGame)
            : base(inGame)
        {
            Items = new List<ListBoxItem>();

            BackgroundMask = Color.White;
            BorderMask = Color.White;
            HoverColor = Color.LightBlue;
            SelectColor = Color.Blue;
            TextColor = Color.Black;
            TextHoverColor = Color.Black;
            TextSelectedColor = Color.Black;
            ItemBackgroundMask = Color.White;

            MouseScroll += GuiListBox_MouseScroll;
        }

        // Scroll wheel event handler
        private void GuiListBox_MouseScroll(object sender, Events.InputEventArgs e)
        {
            if (Items.Count > 0)
            {
                int scrollDelta = e.InputState.getMouseDeltaScrollWheelValue();

                YOffset += scrollDelta / 10;

                //get the height of each line
                int lineHeight = (int)Font.MeasureString(Items.First().Label).Y;
                int maxY = (lineHeight * Items.Count) - Height;

                if (YOffset > 0) YOffset = 0;
                if (YOffset < 0 && YOffset < -maxY) YOffset = -maxY;
            }
        }

        public ListBoxItem SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                ListBoxSelectionChangedEventArgs args = new ListBoxSelectionChangedEventArgs()
                {
                    OldItem = SelectedItem,
                    NewItem = value
                };

                _SelectedItem = value;
                OnSelectionChangeEvent(args, SelectionChanged);
            }
        }

        public void AddItem(ListBoxItem item)
        {
            int lineHeight = (int)Font.MeasureString(item.Label).Y;

            item.Parent = this;

            item.Y = RelativeY + (Items.Count * lineHeight);
            item.Width = Width;
            item.Height = lineHeight;
            
            Items.Add(item);
        }

        public override void Update(GameTime time)
        {
            if (IsVisible && IsEnabled)
            {
                MouseUp = false;

                base.Update(time);

                if (MouseUp && MouseOver)
                {
                    if(Items.Count(i => i.MouseOver) == 1)
                    {
                        ListBoxSelectionChangedEventArgs args = new ListBoxSelectionChangedEventArgs()
                        {
                            OldItem = SelectedItem,
                            NewItem = Items.First(i => i.MouseOver)
                        };

                        SelectedItem = Items.First(i => i.MouseOver);
                        OnSelectionChangeEvent(args, SelectionChanged);
                    }
                }
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
                    MainGame.SpriteBatch.Draw(BackgroundTexture, HitBox, BackgroundMask);

                // draw items
                int lineIndex = 0;
                foreach(ListBoxItem item in Items)
                {
                    int lineHeight = (int)Font.MeasureString(item.Label).Y;
                    int drawY = (RelativeY + (lineIndex * lineHeight)) + YOffset;

                    item.Y = drawY - Y;

                    // draw selection
                    if (SelectedItem == item)
                    {
                        MainGame.SpriteBatch.Draw(ItemSelectedBackgroundTexture, new Rectangle(RelativeX, drawY, Width, lineHeight), SelectColor);
                    }
                    // draw hover
                    else if (item.MouseOver)
                    {
                        MainGame.SpriteBatch.Draw(ItemHoverBackgroundTexture, new Rectangle(RelativeX, drawY, Width, lineHeight), HoverColor);
                    }
                    else
                    {
                        MainGame.SpriteBatch.Draw(ItemBackgroundTexture, new Rectangle(RelativeX, drawY, Width, lineHeight), ItemBackgroundMask);
                    }

                    MainGame.SpriteBatch.DrawString(Font, item.Label, new Vector2(RelativeX + 5, drawY), TextColor);

                    lineIndex++;
                }

                if (BorderTexture != null)
                    MainGame.SpriteBatch.Draw(BorderTexture, HitBox, BorderMask);
                
                MainGame.SpriteBatch.End();

                //reset the scissor rect
                MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle = previousRect;

                base.Draw(time);
            }
        }

        // Event Handling
        protected virtual void OnSelectionChangeEvent(ListBoxSelectionChangedEventArgs e, EventHandler<ListBoxSelectionChangedEventArgs> handler)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
