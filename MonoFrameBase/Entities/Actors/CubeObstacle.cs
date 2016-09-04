using Microsoft.Xna.Framework;
using MonoFrame.Messaging;

namespace MonoFrame.Entities.Actors
{
    public class CubeObstacle : Obstacle
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }

        public CubeObstacle(long inID, Game game)
            : base(inID, game)
        {
            ResetLocalSpace();
            Height = 1;
            Width = 1;
            Depth = 1;
        }

        public CubeObstacle(long inID, Game game, float dimension, Vector3 position)
            : base(inID, game)
        {
            ResetLocalSpace();
            Height = dimension;
            Width = dimension;
            Depth = dimension;
            Position = position;
        }

        public CubeObstacle(long inID, Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float dimension)
            : base(inID, game)
        {
            Side = inSide;
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            Height = dimension;
            Width = dimension;
            Depth = dimension;
        }

        public CubeObstacle(long inID, Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float dimension)
            : base(inID, game)
        {
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            SetUnitSideFromForwardAndUp();
            Height = dimension;
            Width = dimension;
            Depth = dimension;
        }

        /// <summary>
        /// Checks for intersection of the given cube obstacle with a
        /// volume of "likely future vehicle positions": a cube along the
        /// current path, extending minTimeToCollision seconds along the
        /// forward axis from current position.
        ///
        /// If they intersect, a collision is imminent and this function returns
        /// a steering force pointing laterally away from the obstacle's center.
        ///
        /// Returns a zero vector if the obstacle is outside the cube.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="minTimeToCollision"></param>
        /// <returns></returns>
        public override Vector3 CollisionAvoidance(VehicleActor vehicle, float minTimeToCollision)
        {
            return Vector3.Zero;
        }

        /// <summary>
        /// Check if a a specific vehicle has hit this obstacle, and return the point of collision. Returns Vector3.Zero if no collision detected.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public override Vector3 HasCollided(VehicleActor vehicle)
        {
            return Vector3.Zero;
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
