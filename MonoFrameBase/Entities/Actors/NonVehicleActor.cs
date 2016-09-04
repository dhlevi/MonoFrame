using Microsoft.Xna.Framework;
using MonoFrame.Messaging;

namespace MonoFrame.Entities.Actors
{
    /// <summary>
    /// A GameWorldActor that does not contain a Vehicle implementation
    /// This means the actor will not be able to move in local or global space
    /// but otherwise behaves in the same way as any other actor
    /// </summary>
    public class NonVehicleActor : GameWorldActor
    {
        public NonVehicleActor(long inID, Game game)
            : base(inID, game)
        {
            ResetLocalSpace();
        }

        public NonVehicleActor(long inID, Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Side = inSide;
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
        }

        public NonVehicleActor(long inID, Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            SetUnitSideFromForwardAndUp();
        }

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
