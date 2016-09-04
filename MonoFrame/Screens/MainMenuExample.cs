using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;
using MonoFrame.UI;
using MonoFrame.UI.Controls;
using MonoFrame.Screens;
using MonoFrame.ContentManager;
using MonoFrame.UI.Events;

namespace MonoFrame.Example.Screens
{
    public class MainMenuExample : Screen
    {
        private Texture2D background;

        private GuiPanel menuPanel;
        private GuiPanel loadPanel;
        private GuiPanel optionsPanel;
        private MenuButton newGameButton;
        private MenuButton loadGameButton;
        private MenuButton customGameButton;
        private MenuButton optionsButton;
        private MenuButton quitButton;

        private ListBox resolutionList;

        private bool settingsChanged = false;
        private DisplayMode mode;

        public MainMenuExample(long inID, ScreenType inType, MainGame inGame)
            : base(inID, inType, inGame)
        {
            Viewport viewport = ScreenManager.Instance.Game.GraphicsDevice.Viewport;

            Name = GetType().Name;

            // set the background
            background = ContentResourceManager.Instance.GetTexture2D("Textures/UI/woodBackground");

            //create the main menu
            Texture2D menuBox = ContentResourceManager.Instance.GetTexture2D("Textures/UI/Panels/MenuWindow");

            menuPanel = new GuiPanel(inGame);
            menuPanel.BackgroundTexture = menuBox;
            menuPanel.X = (viewport.Width / 2) - (menuBox.Width / 2);
            menuPanel.Y = 0;
            menuPanel.Width = menuBox.Width;
            menuPanel.Height = menuBox.Height;

            newGameButton = new MenuButton(inGame);
            newGameButton.Text = "New Game";
            newGameButton.X = (menuBox.Width - newGameButton.Width) / 2;
            newGameButton.Y = 100;
            newGameButton.MouseClick += NewGameButton_MouseClick;

            loadGameButton = new MenuButton(inGame);
            loadGameButton.Text = "Load Game";
            loadGameButton.X = (menuBox.Width - loadGameButton.Width) / 2;
            loadGameButton.Y = newGameButton.Y + loadGameButton.Height + 10;
            loadGameButton.MouseClick += LoadGameButton_MouseClick;

            customGameButton = new MenuButton(inGame);
            customGameButton.Text = "Custom Game";
            customGameButton.X = (menuBox.Width - customGameButton.Width) / 2;
            customGameButton.Y = loadGameButton.Y + customGameButton.Height + 10;
            customGameButton.MouseClick += CustomGameButton_MouseClick;

            optionsButton = new MenuButton(inGame);
            optionsButton.Text = "Options";
            optionsButton.X = (menuBox.Width - optionsButton.Width) / 2;
            optionsButton.Y = customGameButton.Y + optionsButton.Height + 10;
            optionsButton.MouseClick += OptionsButton_MouseClick;

            quitButton = new MenuButton(inGame);
            quitButton.Text = "Quit";
            quitButton.X = (menuBox.Width - quitButton.Width) / 2;
            quitButton.Y = optionsButton.Y + quitButton.Height + 10;
            quitButton.MouseClick += QuitButton_MouseClick;

            menuPanel.AddChild(newGameButton);
            menuPanel.AddChild(loadGameButton);
            menuPanel.AddChild(customGameButton);
            menuPanel.AddChild(optionsButton);
            menuPanel.AddChild(quitButton);

            GuiPanel.AddChild(menuPanel);

            // add any logos, images, etc.

            // hidden load menu
            loadPanel = new GuiPanel(inGame);
            loadPanel.IsVisible = false;
            loadPanel.BackgroundTexture = menuBox;
            loadPanel.X = (viewport.Width / 2) - (menuBox.Width / 2);
            loadPanel.Y = 0;
            loadPanel.Width = menuBox.Width;
            loadPanel.Height = menuBox.Height;

            MenuButton lpCloseButton = new MenuButton(inGame);
            lpCloseButton.Text = "Close";
            lpCloseButton.X = (loadPanel.Width - lpCloseButton.Width) / 2;
            lpCloseButton.Y = loadPanel.Height - lpCloseButton.Height - 20;
            lpCloseButton.MouseClick += LpCloseButton_MouseClick;

            loadPanel.AddChild(lpCloseButton);

            GuiPanel.AddChild(loadPanel);
            
            // hidden options menu
            optionsPanel = new GuiPanel(inGame);
            optionsPanel.IsVisible = false;
            optionsPanel.BackgroundTexture = menuBox;
            optionsPanel.X = (viewport.Width / 2) - (menuBox.Width / 2);
            optionsPanel.Y = 0;
            optionsPanel.Width = menuBox.Width;
            optionsPanel.Height = menuBox.Height;

            GuiPanel.AddChild(optionsPanel);

            MenuButton opCloseButton = new MenuButton(inGame);
            opCloseButton.Text = "Close";
            opCloseButton.X = (loadPanel.Width - lpCloseButton.Width) / 2;
            opCloseButton.Y = loadPanel.Height - lpCloseButton.Height - 15;
            opCloseButton.MouseClick += OpCloseButton_MouseClick;

            Label resolutionLabel = new Label(inGame);
            resolutionLabel.Text = "Resolution:";
            resolutionLabel.X = 20;
            resolutionLabel.Y = 100;

            resolutionList = new ListBox(inGame);

            optionsPanel.AddChild(resolutionList);

            resolutionList.X = 20;
            resolutionList.Y = resolutionLabel.Y + resolutionLabel.Height + 10;
            resolutionList.Width = 290;
            resolutionList.SelectionChanged += ResolutionList_SelectionChanged;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Width >= 800 && mode.Height >= 600)
                {
                    string res = mode.Width + " x " + mode.Height + " ratio: " + Math.Round(mode.AspectRatio, 1, MidpointRounding.ToEven).ToString();

                    if (resolutionList.Items.Count(r => r.Label.Equals(res)) == 0)
                    {
                        ListBoxItem newItem = new ListBoxItem(res, mode, inGame);
                        resolutionList.AddItem(newItem);

                        if (mode.Width == MainGame.Graphics.PreferredBackBufferWidth && mode.Height == MainGame.Graphics.PreferredBackBufferHeight)
                        {
                            resolutionList.SelectedItem = newItem;
                        }
                    }
                }
            }

            CheckBox borderlessCheck = new CheckBox(inGame);
            borderlessCheck.X = 20;
            borderlessCheck.Y = resolutionList.Y + resolutionList.Height + 10;
            borderlessCheck.Label = "Borderless";
            borderlessCheck.OnCheck += BorderlessCheck_OnCheck;
            borderlessCheck.IsChecked = MainGame.Window.IsBorderless;

            optionsPanel.AddChild(borderlessCheck);
            optionsPanel.AddChild(resolutionLabel);
            optionsPanel.AddChild(opCloseButton);
            
            GuiTextArea textArea = new GuiTextArea(inGame);
            textArea.TextColor = Color.White;
            textArea.X = 20;
            textArea.Y = loadPanel.Height - 175;
            textArea.Text = "This is a text area. This text area just shows a bunch of text, and should resize and wrap accordingly, unlike a standard label";
            textArea.Font = ScreenManager.Instance.DefaultFont;
            textArea.Width = loadPanel.Width - 40;
            textArea.AutoCalculateHeight = true;

            menuPanel.AddChild(textArea);

            ////// test text box
            //GuiTextBox tb = new GuiTextBox(inGame);
            //tb.X = 10;
            //tb.Y = 10;
            //tb.MaxCharacters = 200;
            //tb.BackgroundTexture = ScreenManager.Instance.BlankTexture;
            //tb.CaratTexture = ScreenManager.Instance.BlankTexture;
            //tb.CaratWidth = 1;
            //tb.CaratHeight = 30;
            //tb.Width = 200;
            //tb.Height = 30;
            //tb.Font = ScreenManager.Instance.DefaultFont;
            //tb.FontColor = Color.Black;
            //tb.IsActive = true;
            //tb.IsPassword = false;
            
            //GuiPanel.AddChild(tb);

            IsLoaded = true;
        }

        private void BorderlessCheck_OnCheck(object sender, EventArgs e)
        {
            Game.Window.IsBorderless = !Game.Window.IsBorderless;
            Game.Window.Position = new Point(0, 0);
        }

        private void ResolutionList_SelectionChanged(object sender, ListBoxSelectionChangedEventArgs e)
        {
            settingsChanged = true;
            mode = (DisplayMode)e.NewItem.Item;
        }

        private void OpCloseButton_MouseClick(object sender, InputEventArgs e)
        {
            optionsPanel.IsVisible = false;
            menuPanel.IsVisible = true;

            if(settingsChanged && mode != null)
            {
                if (mode.Width != MainGame.Graphics.PreferredBackBufferWidth && mode.Height != MainGame.Graphics.PreferredBackBufferHeight)
                {
                    MainGame.Graphics.PreferredBackBufferWidth = mode.Width;
                    MainGame.Graphics.PreferredBackBufferHeight = mode.Height;

                    float targetAspectRatio = ((DisplayMode)resolutionList.SelectedItem.Item).AspectRatio;

                    // figure out the largest area that fits in this resolution at the desired aspect ratio     
                    int width = MainGame.Graphics.PreferredBackBufferWidth;
                    int height = (int)(width / targetAspectRatio + 0.5f);

                    if (height > MainGame.Graphics.PreferredBackBufferHeight)
                    {
                        height = MainGame.Graphics.PreferredBackBufferHeight;
                        width = (int)(height * targetAspectRatio + 0.5f);
                    }

                    // set up the new viewport centered in the backbuffer 
                    Viewport viewport = new Viewport();
                    viewport.X = (MainGame.Graphics.PreferredBackBufferWidth / 2) - (width / 2);
                    viewport.Y = (MainGame.Graphics.PreferredBackBufferHeight / 2) - (height / 2);
                    viewport.Width = width;
                    viewport.Height = height;
                    viewport.MinDepth = 0;
                    viewport.MaxDepth = 1;

                    MainGame.GraphicsDevice.Viewport = viewport;
                }
            }

            MainGame.Graphics.ApplyChanges();
        }

        private void LpCloseButton_MouseClick(object sender, InputEventArgs e)
        {
            loadPanel.IsVisible = false;
            menuPanel.IsVisible = true;
        }

        private void QuitButton_MouseClick(object sender, InputEventArgs e)
        {
            Game.Exit();
        }

        private void OptionsButton_MouseClick(object sender, InputEventArgs e)
        {
            optionsPanel.IsVisible = true;
            menuPanel.IsVisible = false;
        }

        private void CustomGameButton_MouseClick(object sender, InputEventArgs e)
        {
        }

        private void LoadGameButton_MouseClick(object sender, InputEventArgs e)
        {
            loadPanel.IsVisible = true;
            menuPanel.IsVisible = false;
        }

        private void NewGameButton_MouseClick(object sender, InputEventArgs e)
        {
            ScreenManager.Instance.TransitionToScreen(2);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            ScreenManager.Instance.Game.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            ScreenManager.Instance.Game.SpriteBatch.Draw(background, new Vector2(0, 0), GuiPanel.HitBox, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            ScreenManager.Instance.Game.SpriteBatch.End();

            base.Draw(time);
        }
    }
}
