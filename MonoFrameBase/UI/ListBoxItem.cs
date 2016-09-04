using Microsoft.Xna.Framework;
using System;

namespace MonoFrame.UI
{
    /// <summary>
    /// Basic list box item used by the GuiListBox
    /// </summary>
    public class ListBoxItem : GuiElement
    {
        public string Label { get; set; }
        public object Item { get; set; }

        public ListBoxItem(object inItem, MainGame inGame)
            : base(inGame)
        {
            Item = inItem;
            Label = inItem.ToString();
        }

        public ListBoxItem(String inLabel, object inItem, MainGame inGame)
            : base(inGame)
        {
            Item = inItem;
            Label = inLabel;
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
        }
    }
}
