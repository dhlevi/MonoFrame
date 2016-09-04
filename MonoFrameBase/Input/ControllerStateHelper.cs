using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoFrame.Input
{
    /// <summary>
    /// Keyboard and mouse state helper for keeping
    /// track of mouse and keypresses between updates
    /// </summary>
    public class ControllerStateHelper
    {
        #region Attribute
        private KeyboardState kbState1;
        private KeyboardState kbState2;
        private MouseState msState1;
        private MouseState msState2;
        private int mouseScrollWheelValue1;
        private int mouseScrollWheelValue2;
        #endregion

        public ControllerStateHelper(KeyboardState kbState, MouseState msState)
        {
            this.kbState1 = kbState;
            this.kbState2 = kbState;
            this.msState1 = msState;
            this.msState2 = msState;
            this.mouseScrollWheelValue1 = msState.ScrollWheelValue;
            this.mouseScrollWheelValue2 = msState.ScrollWheelValue;
        }

        public void Update(KeyboardState kbState, MouseState msState)
        {
            this.kbState1 = this.kbState2;
            this.kbState2 = kbState;
            this.msState1 = this.msState2;
            this.msState2 = msState;
            this.mouseScrollWheelValue1 = this.mouseScrollWheelValue2;
            this.mouseScrollWheelValue2 = msState.ScrollWheelValue;
        }

        public Boolean isKeyDown(Keys key)
        {
            return kbState2.IsKeyDown(key);
        }

        public Boolean isKeyUp(Keys key)
        {
            return kbState2.IsKeyUp(key);
        }

        public Boolean isKeyPressed(Keys key)
        {
            return kbState1.IsKeyUp(key) && kbState2.IsKeyDown(key);
        }

        public Boolean isKeyReleased(Keys key)
        {
            return kbState1.IsKeyDown(key) && kbState2.IsKeyUp(key);
        }

        public Keys[] getPressedKeys()
        {
            return kbState2.GetPressedKeys();
        }

        public Boolean isButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return msState2.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return msState2.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return msState2.RightButton == ButtonState.Pressed;
                case MouseButton.Special1:
                    return msState2.XButton1 == ButtonState.Pressed;
                case MouseButton.Special2:
                    return msState2.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public Boolean isButtonUp(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return msState2.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return msState2.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return msState2.RightButton == ButtonState.Released;
                case MouseButton.Special1:
                    return msState2.XButton1 == ButtonState.Released;
                case MouseButton.Special2:
                    return msState2.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }

        public Boolean isButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return msState1.LeftButton == ButtonState.Released && msState2.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return msState1.MiddleButton == ButtonState.Released && msState2.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return msState1.RightButton == ButtonState.Released && msState2.RightButton == ButtonState.Pressed;
                case MouseButton.Special1:
                    return msState1.XButton1 == ButtonState.Released && msState2.XButton1 == ButtonState.Pressed;
                case MouseButton.Special2:
                    return msState1.XButton2 == ButtonState.Released && msState2.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public Boolean isButtonReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return msState1.LeftButton == ButtonState.Pressed && msState2.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return msState1.MiddleButton == ButtonState.Pressed && msState2.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return msState1.RightButton == ButtonState.Pressed && msState2.RightButton == ButtonState.Released;
                case MouseButton.Special1:
                    return msState1.XButton1 == ButtonState.Pressed && msState2.XButton1 == ButtonState.Released;
                case MouseButton.Special2:
                    return msState1.XButton2 == ButtonState.Pressed && msState2.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }

        public int getMouseScrollWheelValue()
        {
            return this.mouseScrollWheelValue2;
        }

        public int getMouseX()
        {
            return msState2.X;
        }

        public int getMouseY()
        {
            return msState2.Y;
        }

        public Vector2 getMousePosition()
        {
            return new Vector2(msState2.X, msState2.Y);
        }

        public int getMouseDeltaScrollWheelValue()
        {
            return mouseScrollWheelValue2 - mouseScrollWheelValue1;
        }

        public int getMouseDeltaX()
        {
            return msState2.X - msState1.X;
        }

        public int getMouseDeltaY()
        {
            return msState2.Y - msState1.Y;
        }

        public Vector2 getMouseDeltaPosition()
        {
            return new Vector2(getMouseDeltaX(), getMouseDeltaY());
        }

        public Vector3 getMouseDeltaVector()
        {
            return new Vector3(getMouseDeltaX(), getMouseDeltaY(), getMouseDeltaScrollWheelValue());
        }
    }

    public enum MouseButton
    {
        Left, Middle, Right, Special1, Special2
    }
}
