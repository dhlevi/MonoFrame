using MonoFrame.Entities.Actors;
using System;

namespace MonoFrame.Messaging
{
    /// <summary>
    /// Helper factory class that assists in creating messages for the dispatcher
    /// by ensuring only valid BaseActor objects are used to send and recieve.
    /// </summary>
    public class MessageFactory
    {
        /// <summary>
        /// Attempts to place a message on the dispatcher priority queue.
        /// If the message cannot be placed, returns false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SendMessage(BaseActor sender, BaseActor reciever, object message)
        {
            return SendMessage(sender, reciever, 0, message);
        }

        /// <summary>
        /// Attempts to place a message on the dispatcher priority queue.
        /// If the message cannot be placed, returns false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        /// <param name="delay"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SendMessage(BaseActor sender, BaseActor reciever, long delay, object message)
        {
            bool wasSent = false;

            MessageType mt = MessageType.Object;

            if (message is string) mt = MessageType.String;
            else if (message is String) mt = MessageType.String;
            // integer types
            else if (message is int) mt = MessageType.NumericInteger;
            else if (message is short) mt = MessageType.NumericInteger;
            else if (message is long) mt = MessageType.NumericInteger;
            else if (message is ushort) mt = MessageType.NumericInteger;
            else if (message is uint) mt = MessageType.NumericInteger;
            else if (message is ulong) mt = MessageType.NumericInteger;
            // floating point types
            else if (message is double) mt = MessageType.NumericFloating;
            else if (message is float) mt = MessageType.NumericFloating;
            else if (message is decimal) mt = MessageType.NumericFloating;
            // boolean
            else if (message is bool) mt = MessageType.Boolean;
            else if (message is Boolean) mt = MessageType.Boolean;
            // byte
            else if (message is byte) mt = MessageType.Byte;
            else if (message is sbyte) mt = MessageType.Byte;

            if (ActorManager.Instance.GetActor(sender.ID) != null
                && ActorManager.Instance.GetActor(reciever.ID) != null
                && message != null)
            {
                MessageDispatcher.Instance.DispatchMessage(sender.ID, reciever.ID, mt, delay, message);
                wasSent = true;
            }
            
            return wasSent;
        }
    }
}
