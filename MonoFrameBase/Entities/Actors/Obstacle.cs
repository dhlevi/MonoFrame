using Microsoft.Xna.Framework;
using MonoFrame.Entities.Actors;
using MonoFrame.Messaging;

namespace MonoFrame.Entities.Actors
{
    public enum ObstacleSeenFromState
    {
        Outside,
        Inside,
        Both
    }

    public abstract class Obstacle : NonVehicleActor
    {
        public ObstacleSeenFromState SeenFrom { get; set; }

        public Obstacle(long inID, Game game)
            : base(inID, game)
        {
            ResetLocalSpace();
        }

        public Obstacle(long inID, Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Side = inSide;
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
        }

        public Obstacle(long inID, Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            SetUnitSideFromForwardAndUp();
        }

        public abstract Vector3 CollisionAvoidance(VehicleActor vehicle, float minTimeToCollision);
        public abstract Vector3 HasCollided(VehicleActor vehicle);

        public new bool Dispose()
        {
            return base.Dispose();
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        public override void Draw(GameTime time)
        {
            base.Draw(time);
        }

        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);
        }
    }
}
