using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoFrame
{
    static class Resolution
    {
        static public Matrix ScaleMatrix { get; set; }
        static public Vector2 Scale { get; set; }
        static public int GameWidth { get; set; }
        static public int GameHeight { get; set; }
        static public int ScreenWidth { get; set; }
        static public int ScreenHeight { get; set; }
        static public Boolean WasResized { get; set; }
        static private int PreviousWindowWidth;
        static private int PreviousWindowHeight;

        static public void Initialize(GraphicsDeviceManager graphics)
        {
            ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            PreviousWindowWidth = graphics.PreferredBackBufferWidth;
            PreviousWindowHeight = graphics.PreferredBackBufferHeight;
            WasResized = false;
            GameWidth = 720;
            GameHeight = 1080;
            CalculateMatrix(graphics);
        }

        static public void Update(MainGame game, GraphicsDeviceManager graphics)
        {
            if (WasResized && !game.IsExiting)
            {
                try
                {
                    if (graphics.PreferredBackBufferWidth < Resolution.GameWidth / 4)
                    {
                        if (graphics.PreferredBackBufferWidth == 0) graphics.PreferredBackBufferWidth = PreviousWindowWidth;
                        else graphics.PreferredBackBufferWidth = Resolution.GameWidth / 4;
                    }

                    if (graphics.PreferredBackBufferHeight < Resolution.GameHeight / 4)
                    {
                        if (graphics.PreferredBackBufferHeight == 0) graphics.PreferredBackBufferHeight = PreviousWindowHeight;
                        else graphics.PreferredBackBufferHeight = Resolution.GameHeight / 4;
                    }

                    graphics.ApplyChanges();
                    CalculateMatrix(graphics);
                    PreviousWindowWidth = graphics.PreferredBackBufferWidth;
                    PreviousWindowHeight = graphics.PreferredBackBufferHeight;
                    WasResized = false;

                    //re-initialize screens (Forced restart)
                    game.ScreenInitialize();
                }
                catch (Exception e)
                {
                    //log out the message. Likely we hit a snag resizing while exiting or closing the window
                }
            }
        }

        static void CalculateMatrix(GraphicsDeviceManager graphics)
        {
            ScaleMatrix = Matrix.CreateScale((float)graphics.PreferredBackBufferWidth / GameWidth, (float)graphics.PreferredBackBufferHeight / GameHeight, 1f);
            Scale = new Vector2(ScaleMatrix.M11, ScaleMatrix.M22);
        }
    }
}
