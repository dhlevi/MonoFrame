using Microsoft.Xna.Framework;
using MonoFrame.Screens;

namespace MonoFrame.UI.Controls
{
    public class Label : GuiLabel
    {
        public Label(MainGame inGame)
            : base(inGame)
        {
            Text = "";
            Font = ScreenManager.Instance.DefaultFont;
            TextColor = Color.White;
        }
    }
}
