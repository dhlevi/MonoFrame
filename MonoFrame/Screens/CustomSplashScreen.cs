using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFrame.ContentManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFrame.Screens
{
    public class CustomSplashScreen : SplashScreen
    {
        private Texture2D splashImage;

        public CustomSplashScreen(long inID, ScreenType inType, MainGame inGame)
            : base(inID, inType, inGame)
        {
            IsTransitioned = false;
            Name = GetType().Name;
            // this is a splash screen... disable the cursor
            Game.IsMouseVisible = false;
        }

        public override void Update(GameTime time)
        {
            if(splashImage == null)
            {
                splashImage = ContentResourceManager.Instance.GetTexture2D("Textures/splash");
            }

            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            Game.GraphicsDevice.Clear(Color.Black);

            if (splashImage != null)
            {
                MainGame.SpriteBatch.Begin();

                int width = MainGame.Graphics.PreferredBackBufferWidth;
                int height = MainGame.Graphics.PreferredBackBufferHeight;

                MainGame.SpriteBatch.Draw(splashImage, new Vector2(width / 2 - splashImage.Width / 2, height / 2 - splashImage.Height / 2));

                MainGame.SpriteBatch.End();
            }

            base.Draw(time);
        }
    }
}
