using System;

namespace MonoFrame.UI.Events
{
    /// <summary>
    /// Input event args for OnChanged events to hold old and new listBoxItems
    /// </summary>
    public class ListBoxSelectionChangedEventArgs : EventArgs
    {
        public ListBoxItem OldItem { get; set; }
        public ListBoxItem NewItem { get; set; }
    }
}
