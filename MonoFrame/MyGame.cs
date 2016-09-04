using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFrame.Screens;
using MonoFrame.Example.Screens;
using MonoFrame.ContentManager;

namespace MonoFrame
{
    /// <summary>
    /// This is the main type for your game.
    /// The MainGame class from MonoFrameBase handles much of the setup and instantiation
    /// </summary>
    public class MyGame : MainGame
    {
        public MyGame()
        {
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            this.Window.Title = "MonoFrame Example";

            //set to full resolution borderless
            Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            this.Window.Position = new Point(0, 0);
            this.Window.IsBorderless = true;

            Graphics.ApplyChanges();

            // Add items like so:
            // Components.Add(<DrawableGameComponent>);
            // if you're going to load in initialize here instead of using manager
            // Remeber draw order. First on the list draws first, so ensure your components are added in a sensible order
            // or the rendering will get a little wonky

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // ensure you load the base content
            // if this is ignored, the screen manager will not be initialized and default textures will not be loaded
            base.LoadContent();

            // TODO: use this.Content to load your game content here
            ContentLoader.LoadTexture2D(this, "Textures/splash");
        }

        public override void ScreenInitialize()
        {
            // Define screens for screen manager, load their content and add to the screen manager

            //first, just run a clear all, so we know we're starting clean
            ScreenManager.Instance.ClearAllScreens();
            ScreenManager.Instance.TransitionTime = 1000; // transitions between screens will take 1000 milliseconds
            // Instantiate the "Main Menu" Example screen.
            MainMenuExample mainMenu = new MainMenuExample(0, ScreenType.Screen, this);
            // Register the Screen with the screen manager. This will add it to the manager and allow us to access the screen by ID at any time
            ScreenManager.Instance.RegisterScreen(mainMenu);

            // Create a Splash Screen class (this is the default, but can be extended for more realistic purposes)
            CustomSplashScreen splash = new CustomSplashScreen(1, ScreenType.Screen, this);
            // we want the initial splash to automaticlaly transition to the main menu, once the display time has expired
            splash.ScreenToTransitionTo = mainMenu;
            splash.SplashStart = DateTime.Now; // start the splash display now
            splash.SplashDisplayTime = 1000; // transition from splash to main menu after 1000 milliseconds.

            // Register the splash screen
            ScreenManager.Instance.RegisterScreen(splash);
   
            // set the current active screen in the manager to the splash screen
            ScreenManager.Instance.ActiveScreen = splash;

            ScreensInitialized = true;
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Call the MainGame Update. This will trigger screen manager to process all screen updates.
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Call the MainGame draw. This will trigger screen manager to paint the active screens
            base.Draw(gameTime);

            // TODO: Add your drawing code here
        }
    }
}
