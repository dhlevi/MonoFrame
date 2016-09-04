using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFrame.ContentManager;
using MonoFrame.Entities.Actors;
using MonoFrame.Messaging;
using MonoFrame.Screens;
using System;

namespace MonoFrame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public abstract class MainGame : Game
    {
        public GraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public bool IsExiting { get; set; }
        public bool ScreensInitialized { get; set; }

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // associate the game with the screen manager
            // screen objects can get the graphics and sprite batch via the screen manager
            ScreenManager.Instance.Game = this;

            this.Window.ClientSizeChanged += delegate { Resolution.WasResized = true; };

            ScreensInitialized = false;

            IsExiting = false;
            this.Exiting += MainGame_Exiting;

        }

        private void MainGame_Exiting(object sender, EventArgs e)
        {
            IsExiting = true;
        }

        // MultiSamping logic
        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            PresentationParameters pp = e.GraphicsDeviceInformation.PresentationParameters;
            GraphicsAdapter adapter = e.GraphicsDeviceInformation.Adapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;

            return;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Resolution.Initialize(Graphics);

            base.Initialize();
        }

        /// <summary>
        /// Screen initialization, which must be defined and implemented by the extending class
        /// This method will be fired after the load content is triggered automatically
        /// If base.LoadContent() is not used, it must be called manually
        /// </summary>
        public abstract void ScreenInitialize();

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Screen manager defaults
            ContentLoader.LoadTexture2D(this, "blank");
            ContentLoader.LoadSpriteFont(this, "DefaultFont");

            ScreenManager.Instance.BlankTexture = ContentResourceManager.Instance.GetTexture2D("blank");
            ScreenManager.Instance.DefaultFont = ContentResourceManager.Instance.GetFont("DefaultFont");

            // Load default GUI content examples
            // You can move this to the extended MainGame class, or leave these here for defaults
            // and apply your own in the main.
            // buttons
            ContentLoader.LoadTexture2D(this, "Textures/UI/Buttons/MenuButtonGray");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Buttons/MenuButtonInactiv");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Buttons/MenuButtonPreLight");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Buttons/MenuButtonPressed");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Buttons/checkbox_checked");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Buttons/checkbox_unchecked");
            // panels
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/BackBaseColor");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/IndCreatures");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/logMax");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/logMin");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/MapBack");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/MenuWindow");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/PanelAlpha");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/PanelBottom");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/PopUpMenu");
            ContentLoader.LoadTexture2D(this, "Textures/UI/Panels/WindowBorder");
            // general
            ContentLoader.LoadTexture2D(this, "Textures/UI/woodBackground");
            ContentLoader.LoadSpriteFont(this, "Fonts/ArialRounded");

            ScreenInitialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // call the resource manager to completely purge the Game content
            ScreenManager.Instance.ClearAllScreens();
            MessageDispatcher.Instance.PriorityQueue.Clear();
            ActorManager.Instance.ClearAllActors();
            ContentResourceManager.Instance.Purge(this);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Only handle the main Update calls if the application is running
            if (!IsExiting)
            {
                // Call the Messaging Queue and trigger a dispatch of any delayed messages (if any are queued)
                if (MessageDispatcher.Instance.PriorityQueue.Count > 0) MessageDispatcher.Instance.DispatchDelayedMessages();
                // Any screen specific updates (may have additional input handling)
                if (ScreenManager.Instance.ScreenCollection.Count > 0) ScreenManager.Instance.Update(gameTime);
                // Resolution update/refresh
                Resolution.Update(this, Graphics);
                // Base update call
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // only draw the screen if we're not exiting
            if (!IsExiting)
            {
                // force a clear
                GraphicsDevice.Clear(Color.Black);
                // Draw via the Screen Manager, but no sense drawing anything if there are no screens!
                if (ScreenManager.Instance.ScreenCollection.Count > 0) ScreenManager.Instance.Draw(gameTime);
                // Base draw call
                base.Draw(gameTime);
            }
        }
    }
}
