using Microsoft.Xna.Framework;
using MonoFrame.ContentManager;
using MonoFrame.Screens;

namespace MonoFrame.UI.Controls
{
    public class MenuButton : GuiButton
    {
        public MenuButton(MainGame inGame)
            : base(inGame)
        {
            //set defaults for menu button textures, fonts, etc.
            Font = ScreenManager.Instance.DefaultFont;
            FontClicked = ScreenManager.Instance.DefaultFont;
            FontHover = ScreenManager.Instance.DefaultFont;
            FontDisabled = ScreenManager.Instance.DefaultFont;

            Text = "Menu Button";

            ButtomClickedTexture = ContentResourceManager.Instance.GetTexture2D("Textures/UI/Buttons/MenuButtonPressed");
            ButtonHoverTexture = ContentResourceManager.Instance.GetTexture2D("Textures/UI/Buttons/MenuButtonPreLight");
            ButtonTexture = ContentResourceManager.Instance.GetTexture2D("Textures/UI/Buttons/MenuButtonInactiv");
            ButtonDisabledTexture = ContentResourceManager.Instance.GetTexture2D("Textures/UI/Buttons/MenuButtonGray");

            TextColor = Color.White;
            TextClickedColor = Color.White;
            TextHoverColor = Color.White;
            TextDisabledColor = Color.Gray;

            Width = ButtonTexture.Width;
            Height = ButtonTexture.Height;
        }
    }
}
