using System;

namespace MonoFrame.Messaging
{
    /// <summary>
    /// Message class used by the Message System.
    /// The message contains a tracking ID, the reciever ID, a type, time and an object package being sent to the reciever.
    /// Setting the reciever of a message to 0 will send to every object that is registered with the dispatcher.
    /// Setting the reciever of a message to -1 will send to every to every object that is registered with the dispatcher, including the sender.
    /// </summary>
    public class Message
    {
        public long SenderID { get; set; } // the Entity ID of the sending actor
        public long ReceiverID { get; set; } // which object should recieve this message. 0 == everyone but sender, -1 = everyone, including sender
        public MessageType MessageType { get; set; } // type of message being transmitted
        public TimeSpan DispatchTime { get; set; } // if there needs to be a delay, set it here. long value ticks
        public object Package { get; set; } // any other piece of data that might need to be tacked on

        public Message(long inSenderID, long inRecieverID, MessageType inMessageType)
        {
            SenderID = inSenderID;
            ReceiverID = inRecieverID;
            MessageType = inMessageType;
        }

        public Message(long inSenderID, long inRecieverID, MessageType inMessageType, long inDispatchTime)
            : this(inSenderID, inRecieverID, inMessageType)
        {
            DispatchTime = new TimeSpan(inDispatchTime);
        }

        public Message(long inSenderID, long inRecieverID, MessageType inMessageType, TimeSpan inDispatchTime)
            : this(inSenderID, inRecieverID, inMessageType)
        {
            DispatchTime = inDispatchTime;
        }

        public Message(long inSenderID, long inRecieverID, MessageType inMessageType, long inDispatchTime, object inPackage)
            : this(inSenderID, inRecieverID, inMessageType, inDispatchTime)
        {
            Package = inPackage;
        }

        public Message(long inSenderID, long inRecieverID, MessageType inMessageType, TimeSpan inDispatchTime, object inPackage)
            : this(inSenderID, inRecieverID, inMessageType, inDispatchTime)
        {
            Package = inPackage;
        }
    }
}
