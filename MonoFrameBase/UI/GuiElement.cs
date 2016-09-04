using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoFrame.Input;
using MonoFrame.UI.Events;
using System;
using System.Collections.Generic;

namespace MonoFrame.UI
{
    /// <summary>
    /// The Root object of all GUI components. The GuiElement class extends drawableComponent, and contains the
    /// basic default UI properties that all other elements will share, inlcuding Click, enter and leave events for the mouse
    /// GUI elements utilize the ControllerStateHelper class for state handling
    /// </summary>
    public class GuiElement : DrawableGameComponent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public MainGame MainGame { get; set; }

        public GuiElement Parent { get; set; }
        public List<GuiElement> Children { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActive { get; set; }
        public ControllerStateHelper InputState { get; set; }
        // mouse state
        public bool MouseEntered { get; set; }
        public bool MouseLeft { get; set; }
        public bool MouseDown { get; set; }
        public bool MouseUp { get; set; }
        public int Clicks { get; set; }
        public int LastScrollPosition { get; set; }

        // Event Handling
        public event EventHandler<InputEventArgs> MouseClick;
        public event EventHandler<InputEventArgs> MouseEnter;
        public event EventHandler<InputEventArgs> MouseLeave;
        public event EventHandler<InputEventArgs> MouseScroll;

        public GuiElement(MainGame inGame)
            : base(inGame)
        {
            MainGame = inGame;
            Children = new List<GuiElement>();

            IsVisible = true;
            IsEnabled = true;

            InputState = new ControllerStateHelper(Keyboard.GetState(), Mouse.GetState());
        }

        public void SetXAsPct(int pct)
        {
            int parentWidth = Parent != null ? Parent.Width : MainGame.GraphicsDevice.Viewport.Width;
            X = (int)(Convert.ToSingle(parentWidth) * (Convert.ToSingle(pct) / 100f));
        }

        public void SetYAsPct(int pct)
        {
            int parentHeight = Parent != null ? Parent.Height : MainGame.GraphicsDevice.Viewport.Height;
            Y = (int)(Convert.ToSingle(parentHeight) * (Convert.ToSingle(pct) / 100f));
        }

        public void AddChild(GuiElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(GuiElement child)
        {
            if(Children.Contains(child))
            {
                child.Parent = null;
                Children.Remove(child);
            }
        }

        public Rectangle HitBox
        {
            get
            {
                return new Rectangle(RelativeX, RelativeY, Width, Height);
            }
        }

        public int RelativeX
        {
            get
            {
                if (Parent != null) return X + Parent.RelativeX;
                else return X;
            }
        }

        public int RelativeY
        {
            get
            {
                if (Parent != null) return Y + Parent.RelativeY;
                else return Y;
            }
        }

        public bool MouseOver
        {
            get
            {
                MouseState state = Mouse.GetState();
                return HitBox.Contains(state.X, state.Y);
            }
        }

        public bool IsMousePressed
        {
            get
            {
                MouseState state = Mouse.GetState();
                return MouseOver && state.LeftButton == ButtonState.Pressed;
            }
        }
  
        // Draw and Update Handling
        public override void Update(GameTime time)
        {
            if (IsVisible && IsEnabled)
            {
                base.Update(time);

                // process child element updates
                if (Children.Count > 0)
                {
                    foreach (GuiElement child in Children)
                    {
                        child.Update(time);
                    }
                }

                // Check Mouse Event Handlers
                InputState.Update(Keyboard.GetState(), Mouse.GetState());
                InputEventArgs args = new InputEventArgs();
                args.InputState = InputState;

                // Mouse Enter
                if (!MouseEntered && MouseOver)
                {
                    MouseLeft = false;
                    MouseEntered = true;
                    MouseDown = false;
                    MouseUp = false;
                    OnInputEvent(args, MouseEnter);
                }
                else
                // Mouse Leave
                if (MouseEntered && !MouseOver)
                {
                    MouseEntered = false;
                    MouseLeft = true;
                    MouseDown = false;
                    MouseUp = false;
                    OnInputEvent(args, MouseLeave);
                }

                // Mouse Click
                if (IsMousePressed)
                {
                    MouseUp = false;
                    MouseDown = true;
                    IsActive = true;
                }

                if (MouseDown && Mouse.GetState().LeftButton == ButtonState.Released && MouseOver)
                {
                    MouseDown = false;
                    MouseUp = true;
                    OnInputEvent(args, MouseClick);
                }

                if (!MouseOver && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    IsActive = false;
                }

                // Mouse Scroll
                if (MouseEntered && InputState.getMouseDeltaScrollWheelValue() != 0)
                {
                    OnInputEvent(args, MouseScroll);
                }
            }
        }

        public override void Draw(GameTime time)
        {
            // Drawing an Element will likely be different
            // depending on mouse state (click, enter, leave, etc.)
            // However, that can be handled by the Extended objects
            // directly, not here.
            if (IsVisible)
            {
                if (Children.Count > 0)
                {
                    foreach (GuiElement child in Children)
                    {
                        child.Draw(time);
                    }
                }

                base.Draw(time);
            }
        }

        // Event Handling
        protected virtual void OnInputEvent(InputEventArgs e, EventHandler<InputEventArgs> handler)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
