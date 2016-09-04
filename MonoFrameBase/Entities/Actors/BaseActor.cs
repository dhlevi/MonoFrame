using Microsoft.Xna.Framework;
using MonoFrame.Messaging;

namespace MonoFrame.Entities.Actors
{
    /// <summary>
    /// Base entity/actor class. Extends DrawableGameComponent
    /// To "automatically" allow this class to call draw and update logic
    /// add to Game.Components
    /// Otherwise, handle drawing manually in the Draw loop
    /// by calling individual classes "Draw" method.
    /// </summary>
    public class BaseActor : DrawableGameComponent
    {
        public long ID { get; private set; }
        
        public BaseActor(long inID, Game game)
            : base(game)
        {
            ID = inID;
            ActorManager.Instance.RegisterActor(this);
        }

        public new bool Dispose()
        {
            base.Dispose();
            return ActorManager.Instance.RemoveActor(ID);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            base.Draw(time);
        }

        public virtual void HandleMessage(Message message)
        {
        }
    }
}

