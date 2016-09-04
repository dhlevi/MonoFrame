using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoFrame.Entities.Actors
{
    /// <summary>
    /// A Manager helper singleton for Actor entities. When an actor is created
    /// the BaseActor class registers the object: ActorManager.Instance.RegisterActor(this);
    /// Actors can be identified via the manager by there ID
    /// Can be enhanced to autogenerate and apply IDs
    /// </summary>
    public class ActorManager
    {
        private static volatile ActorManager instance;
        private static object syncRoot = new Object();

        private HashSet<BaseActor> ActorCollection { get; set; }

        private long nextID;

        public ActorManager()
        {
            ActorCollection = new HashSet<BaseActor>();
            nextID = 1;
        }

        public static ActorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new ActorManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Fetch the next available ID number in the manager
        /// This method will also search the registered actors
        /// and fill in any gaps. If no gaps are found, the next
        /// available Id will be incremented and applied
        /// </summary>
        public long NextAvailableID
        {
            get
            {
                long nextAvailableID = 1;

                for(long i = 0; i < nextID; i++)
                {
                    if (IsIDAvailable(i))
                    {
                        nextAvailableID = i;
                        break;
                    }
                }

                if(nextAvailableID == 1) nextAvailableID = nextID++;

                return nextAvailableID;
            }
        }

        /// <summary>
        /// Check all registered actors and see if the ID is currently in use by another actor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsIDAvailable(long id)
        {
            return ActorCollection.Count(ent => ent.ID == id) == 0;
        }

        /// <summary>
        /// Clears the managers actor collection
        /// This WILL NOT dispose the actors within the collection
        /// </summary>
        public void ClearAllActors()
        {
            ActorCollection.Clear();
            ActorCollection = null;
            ActorCollection = new HashSet<BaseActor>();
        }

        /// <summary>
        /// Get the actor collection as a List object
        /// </summary>
        /// <returns></returns>
        public List<BaseActor> GetActorList()
        {
            return ActorCollection.ToList();
        }

        /// <summary>
        /// Registers an Actor with the Manager. If the registration
        /// is successful, return TRUE.
        /// Registration can only fail if the ID is in use or an ID <= 0 is used by an actor
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool RegisterActor(BaseActor actor)
        {
            if (ActorCollection.Count(ent => ent.ID == actor.ID) > 0) return false;
            else if (actor.ID <= 0) return false; // ID's must be 1 or greater. message dispatcher uses 0 and -1 as specials
            else
            {
                ActorCollection.Add(actor);
                return true;
            }
        }

        /// <summary>
        /// Get an actor from the Actor collection by their ID number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseActor GetActor(long id)
        {
            if (ActorCollection.Count(ent => ent.ID == id) > 0)
            {
                return ActorCollection.First(ent => ent.ID == id);
            }
            else return null;
        }

        /// <summary>
        /// Remove an actor from the Actor collection
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool RemoveActor(BaseActor actor)
        {
            if (ActorCollection.Contains(actor))
            {
                return ActorCollection.Remove(actor);
            }
            else return false;
        }

        /// <summary>
        /// Remove an actor from the actor collection by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveActor(long id)
        {
            if (ActorCollection.Count(ent => ent.ID == id) > 0)
            {
                return ActorCollection.Remove(ActorCollection.First(ent => ent.ID == id));
            }
            else return false;
        }
    }
}
