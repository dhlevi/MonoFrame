using Microsoft.Xna.Framework;
using MonoFrame.UI;
using System;
using System.Collections.Generic;

namespace MonoFrame.Screens
{
    public enum ScreenType { Screen, Popup, Menu }
    /// <summary>
    /// Screen base class. All screens for use in the screen manager will extend this base.
    /// The Screen class contains basic components (ID, name, type, root GUI element and components)
    /// Inheriting classes can override base draw/update, but must call this base to ensure
    /// GUI elements and components are drawn and updated.
    /// </summary>
    public abstract class Screen : DrawableGameComponent
    {
        public long ID { get; private set; }
        public string Name { get; set; }
        public ScreenType ScreenType { get; set; }

        public GuiElement GuiPanel { get; set; }
        public List<DrawableGameComponent> Components { get; set; }

        public bool IsLoaded { get; set; }

        public MainGame MainGame { get; set; }

        public Screen(long inID, ScreenType inType, MainGame game)
            : base(game)
        {
            ID = inID;
            ScreenManager.Instance.RegisterScreen(this);
            Components = new List<DrawableGameComponent>();
            ScreenType = inType;
            
            GuiPanel = new GuiPanel(game);
            GuiPanel.X = 0;
            GuiPanel.Y = 0;
            GuiPanel.Width = ScreenManager.Instance.Game.GraphicsDevice.Viewport.Width;
            GuiPanel.Height = ScreenManager.Instance.Game.GraphicsDevice.Viewport.Height;
            // resize panel if the window size has changed
            game.Window.ClientSizeChanged += Window_ClientSizeChanged;

            MainGame = game;

            IsLoaded = true;
        }

        // subscribed handler to window client size changed to reset panel size
        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            GuiPanel.Width = ScreenManager.Instance.Game.GraphicsDevice.Viewport.Width;
            GuiPanel.Height = ScreenManager.Instance.Game.GraphicsDevice.Viewport.Height;
        }

        protected override void LoadContent()
        {
            IsLoaded = false;

            try
            {
                base.LoadContent();

                IsLoaded = true;
            }
            catch (Exception e)
            {
                IsLoaded = false;
            }
        }

        protected override void UnloadContent()
        {
            PurgeComponents();
            base.UnloadContent();
        }

        public void PurgeComponents()
        {
            foreach (DrawableGameComponent component in Components)
            {
                component.Dispose();
            }

            Components.Clear();
        }

        public override void Update(GameTime time)
        {
            if (IsLoaded)
            {
                foreach (DrawableGameComponent component in Components)
                {
                    component.Update(time);
                }

                GuiPanel.Update(time);

                base.Update(time);
            }
        }

        public override void Draw(GameTime time)
        {
            if (IsLoaded)
            {
                foreach (DrawableGameComponent component in Components)
                {
                    component.Draw(time);
                }

                GuiPanel.Draw(time);

                base.Draw(time);
            }
        }
    }
}
