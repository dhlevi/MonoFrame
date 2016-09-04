using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFrame.Screens
{
    /// <summary>
    /// The Screen Manager allows you to register screens, popup windows and context menus
    /// The manager will control calling update and paint (activate in MainGame Update/Paint methods
    /// Order of types is Screen, popup, then menus
    /// Any screen can technically be a popup. In this event, the popup screen is painted over
    /// the active screen. This is used primarily for messages, inventory windows, etc.
    /// </summary>
    public class ScreenManager
    {
        private static volatile ScreenManager instance;
        private static object syncRoot = new Object();

        // collection of all screens
        public HashSet<Screen> ScreenCollection { get; set; }
        // list of popup messages to display
        public HashSet<Screen> ActivePopups { get; set; }
        // list of context menu's to display
        public HashSet<Screen> ContextMenus { get; set; }

        // the active screen to paint
        public Screen ActiveScreen { get; set; }
        // the screen we should be transitioning to
        public Screen ScreenToTransitionTo { get; set; }

        // The game object this manager is registered to
        public MainGame Game { get; set; }

        // Default font
        public SpriteFont DefaultFont { get; set; }
        // Blank texture
        public Texture2D BlankTexture { get; set; }

        // transition details
        public bool IsTransitioning { get; set; }
        public DateTime TransitionStartTime { get; set; }
        // the length of time a transition sould take in milliseconds
        public float TransitionTime { get; set; }

        public ScreenManager()
        {
            ScreenCollection = new HashSet<Screen>();
            ActivePopups = new HashSet<Screen>();
            ContextMenus = new HashSet<Screen>();

            IsTransitioning = false;
        }

        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new ScreenManager();
                    }
                }
                return instance;
            }
        }

        public void ClearAllScreens()
        {
            ScreenCollection.Clear();
            ScreenCollection = null;
            ScreenCollection = new HashSet<Screen>();

            ActiveScreen = null;
            ScreenToTransitionTo = null;
        }

        public List<Screen> GetScreensAsList()
        {
            return ScreenCollection.ToList();
        }

        public bool RegisterScreen(Screen screen)
        {
            if (ScreenCollection.Count(ent => ent.ID == screen.ID) > 0) return false;
            else
            {
                ScreenCollection.Add(screen);
                return true;
            }
        }

        public Screen GetScreen(string name)
        {
            if (ScreenCollection.Count(ent => ent.Name == name) > 0)
            {
                return ScreenCollection.First(ent => ent.Name == name);
            }
            else return null;
        }

        public Screen GetScreen(long id)
        {
            if (ScreenCollection.Count(ent => ent.ID == id) > 0)
            {
                return ScreenCollection.First(ent => ent.ID == id);
            }
            else return null;
        }

        public bool RemoveScreen(Screen screen)
        {
            if (ScreenCollection.Contains(screen))
            {
                return ScreenCollection.Remove(screen);
            }
            else return false;
        }

        public bool RemoveScreen(long id)
        {
            if (ScreenCollection.Count(ent => ent.ID == id) > 0)
            {
                return ScreenCollection.Remove(ScreenCollection.First(ent => ent.ID == id));
            }
            else return false;
        }

        public void AddPopup(Screen popup)
        {
            ActivePopups.Add(popup);
        }

        public void RemovePopup(Screen popup)
        {
            ActivePopups.Add(popup);
        }

        public void TransitionToScreen(long id)
        {
            Screen screen = GetScreen(id);
            TransitionToScreen(screen);
        }

        public void TransitionToScreen(Screen screen)
        {
            ScreenToTransitionTo = screen;
            IsTransitioning = true;
            TransitionStartTime = DateTime.Now;

            // We're transitioning screens, so any active menus should be disposed
            ContextMenus.Clear();

            // We don't want to force close popups though, because they may be waiting for input
            // the screen should handle this occurance for safety
        }

        public void Update(GameTime time)
        {
            if (ScreenCollection.Count > 0 && ActiveScreen != null)
            {
                ActiveScreen.Update(time);
            }

            if (ActivePopups.Count > 0 && ActiveScreen != null)
            {
                foreach (Screen popupScreen in ActivePopups)
                {
                    popupScreen.Update(time);
                }
            }

            if (ContextMenus.Count > 0 && ActiveScreen != null)
            {
                foreach (Screen contextScreen in ContextMenus)
                {
                    contextScreen.Update(time);
                }
            }
        }

        public void Draw(GameTime time)
        {
            // draw the main active screen
            if (ScreenCollection.Count > 0 && ActiveScreen != null)
            {
                ActiveScreen.Draw(time);
            }

            // draw any active popup windows
            if (ActivePopups.Count > 0 && ActiveScreen != null)
            {
                foreach (Screen popupScreen in ActivePopups)
                {
                    popupScreen.Draw(time);
                }
            }

            // draw any active context menus
            if (ContextMenus.Count > 0 && ActiveScreen != null)
            {
                foreach (Screen contextScreen in ContextMenus)
                {
                    contextScreen.Draw(time);
                }
            }

            // if a screen transition has been triggered, fade in
            // you can swap screens without activating a transition
            if (IsTransitioning)
            {
                DateTime now = DateTime.Now;
                TimeSpan ts = now - TransitionStartTime;
                double timeTransitioning = ts.TotalMilliseconds;

                if (timeTransitioning >= TransitionTime && ScreenToTransitionTo != null)
                {
                    ActiveScreen = ScreenToTransitionTo;
                    ScreenToTransitionTo = null;
                }

                float alphaPct = Convert.ToSingle(timeTransitioning) / TransitionTime;

                if (alphaPct > 1.0f) alphaPct = 2.0f - alphaPct;

                int alpha = Convert.ToInt32(255f * alphaPct);

                if (timeTransitioning >= TransitionTime * 2)
                {
                    IsTransitioning = false;
                    alpha = 0;
                }

                ScreenTransitionBlack(alpha);
            }
        }

        public void ScreenTransitionBlack(int alpha)
        {
            Viewport viewport = Game.GraphicsDevice.Viewport;

            Game.SpriteBatch.Begin();

            Game.SpriteBatch.Draw(BlankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             new Color(0, 0, 0, (byte)alpha));

            Game.SpriteBatch.End();
        }
    }
}
