using Microsoft.Xna.Framework;
using System;

namespace MonoFrame.Screens
{
    public class SplashScreen : Screen
    {
        public DateTime SplashStart { get; set; }
        public double SplashDisplayTime { get; set; }
        public Screen ScreenToTransitionTo { get; set; }
        public bool IsTransitioned { get; set; }

        public SplashScreen(long inID, ScreenType inType, MainGame inGame)
            : base(inID, inType, inGame)
        {
            IsTransitioned = false;
            Name = GetType().Name;
            // this is a splash screen... disable the cursor
            Game.IsMouseVisible = false;
        }

        public override void Update(GameTime time)
        {
            if (!IsTransitioned)
            {
                DateTime now = DateTime.Now;
                TimeSpan ts = now - SplashStart;
                double splashWaitTime = ts.TotalMilliseconds;

                if (splashWaitTime >= SplashDisplayTime)
                {
                    ScreenManager.Instance.TransitionToScreen(ScreenToTransitionTo);
                    IsTransitioned = true;

                    // activate mouse pointer
                    Game.IsMouseVisible = true;
                }
            }

            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            base.Draw(time);
        }
    }
}
