using MonoFrame.Input;
using System;

namespace MonoFrame.UI.Events
{
    /// <summary>
    /// Input event arguments for the Input State
    /// </summary>
    public class InputEventArgs : EventArgs
    {
        public ControllerStateHelper InputState { get; set; }
    }
}
