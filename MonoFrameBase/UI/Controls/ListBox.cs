using Microsoft.Xna.Framework;
using MonoFrame.Screens;
using System.Collections.Generic;

namespace MonoFrame.UI.Controls
{
    public class ListBox : GuiListBox
    {
        public ListBox(MainGame inGame)
            : base(inGame)
        {
            Initialize();
        }

        public ListBox(List<ListBoxItem> inItems, MainGame inGame)
            : base(inGame)
        {
            Items = inItems;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            IsEnabled = true;
            IsVisible = true;
            Width = 300;
            Height = 300;
            BackgroundTexture = ScreenManager.Instance.BlankTexture;
            ItemBackgroundTexture = ScreenManager.Instance.BlankTexture;
            ItemSelectedBackgroundTexture = ScreenManager.Instance.BlankTexture;
            ItemHoverBackgroundTexture = ScreenManager.Instance.BlankTexture;
            ItemBackgroundMask = Color.White;
            SelectColor = new Color(Color.DarkBlue, 100);
            HoverColor = new Color(Color.SlateBlue, 100);
            BackgroundMask = Color.AliceBlue;
            Font = ScreenManager.Instance.DefaultFont;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }
    }
}
