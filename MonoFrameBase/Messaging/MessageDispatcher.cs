using MonoFrame.Entities.Actors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoFrame.Messaging
{
    public enum MessageType { String, NumericInteger, NumericFloating, Boolean, Byte, Object, Nothing }

    /// <summary>
    /// The Message Dispatcher and Priority Queue singleton.
    /// Message dispatcher collectes messages from and sends messages to all registered actors.
    /// By default, anything that extends the BaseActor class can recieve messages from the dispatcher
    /// via the "HandleMessage" method
    /// </summary>
    public class MessageDispatcher
    {
        private static volatile MessageDispatcher instance;
        private static object syncRoot = new Object();

        public HashSet<Message> PriorityQueue { get; set; }

        public MessageDispatcher()
        {
            PriorityQueue = new HashSet<Message>();
        }

        public static MessageDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new MessageDispatcher();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Dispatch an empty message. This is useful as a "WakeUp" signal from sender to reciever.
        /// </summary>
        /// <param name="inSenderID"></param>
        /// <param name="inRecieverID"></param>
        public void DispatchMessage(long inSenderID, long inRecieverID)
        {
            DispatchMessage(inSenderID, inRecieverID, 0);
        }

        /// <summary>
        /// Dispatch an empty message with a dispatch delay. This is useful as a "WakeUp" signal from sender to reciever.
        /// </summary>
        /// <param name="inSenderID"></param>
        /// <param name="inRecieverID"></param>
        /// <param name="inDispatchTime"></param>
        public void DispatchMessage(long inSenderID, long inRecieverID, long inDispatchTime)
        {
            DispatchMessage(inSenderID, inRecieverID, MessageType.Nothing, inDispatchTime, null);
        }

        /// <summary>
        /// Dispatch a message containing an object package and defined type to a reciever
        /// This can be used by a sender to pass information via message handling between other BaseActors
        /// </summary>
        /// <param name="inSenderID"></param>
        /// <param name="inRecieverID"></param>
        /// <param name="inMessageType"></param>
        /// <param name="inDispatchTime"></param>
        /// <param name="inPackage"></param>
        public void DispatchMessage(long inSenderID, long inRecieverID, MessageType inMessageType, long inDispatchTime, object inPackage)
        {
            Message message = new Message(inSenderID, inRecieverID, inMessageType, inDispatchTime, inPackage);

            if (inDispatchTime <= 0) DispatchMessage(message);
            else PriorityQueue.Add(message);
        }

        /// <summary>
        /// Dispatches queued message from the priority queue after the delay has expired
        /// This must be called from the Update loop
        /// </summary>
        public void DispatchDelayedMessages()
        {
            if (MessageDispatcher.Instance.PriorityQueue.Count > 0)
            {
                long currentTime = DateTime.Now.Ticks;
                List<Message> priorityMessages = PriorityQueue.Where(msg => msg.DispatchTime.Ticks < currentTime).ToList();

                foreach (Message message in priorityMessages)
                {
                    PriorityQueue.Remove(message);
                    DispatchMessage(message);
                }

                priorityMessages.Clear();
                priorityMessages = null;
            }
        }

        /// <summary>
        /// Dispatches messages from the priority queue
        /// </summary>
        /// <param name="message"></param>
        private void DispatchMessage(Message message)
        {
            BaseActor sender = ActorManager.Instance.GetActor(message.SenderID);
            BaseActor reciever = ActorManager.Instance.GetActor(message.ReceiverID);

            if (reciever == null)
            {
                if (message.ReceiverID == -1) Send(ActorManager.Instance.GetActorList().Where(entity => entity.ID != sender.ID).ToList(), message);
                else if (message.ReceiverID == 0) Send(ActorManager.Instance.GetActorList(), message);
                // else log out message not sent error
            }
            else Send(reciever, message);
        }

        private void Send(List<BaseActor> recievers, Message message)
        {
            foreach (BaseActor entity in recievers) Send(entity, message);
        }

        private void Send(BaseActor reciever, Message message)
        {
            reciever.HandleMessage(message);
        }
    }
}

