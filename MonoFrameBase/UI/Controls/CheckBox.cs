using MonoFrame.ContentManager;
using MonoFrame.Screens;

namespace MonoFrame.UI.Controls
{
    public class CheckBox : GuiCheckbox
    {
        public CheckBox(MainGame inGame)
            : base(inGame)
        {
            CheckedTexture = ContentResourceManager.Instance.GetTexture2D("Textures/UI/Buttons/checkbox_checked");
            UncheckedTexture = ContentResourceManager.Instance.GetTexture2D("Textures/UI/Buttons/checkbox_unchecked");
            Width = CheckedTexture.Width;
            Height = CheckedTexture.Height;
            Label = "Checkbox";
            Font = ScreenManager.Instance.DefaultFont;
        }
    }
}
