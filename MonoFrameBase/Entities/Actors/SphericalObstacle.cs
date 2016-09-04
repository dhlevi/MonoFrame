using Microsoft.Xna.Framework;
using MonoFrame.Messaging;

namespace MonoFrame.Entities.Actors
{
    public class SphericalObstacle : Obstacle
    {
        public float Radius { get; set; }

        public SphericalObstacle(long inID, Game game)
            : base(inID, game)
        {
            ResetLocalSpace();
            Radius = 1;
            Position = Vector3.Zero;
        }

        public SphericalObstacle(long inID, Game game, float radius, Vector3 position)
            : base(inID, game)
        {
            ResetLocalSpace();
            Radius = radius;
            Position = position;
        }

        public SphericalObstacle(long inID, Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float radius)
            : base(inID, game)
        {
            Side = inSide;
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            Radius = radius;
        }

        public SphericalObstacle(long inID, Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float radius)
            : base(inID, game)
        {
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            SetUnitSideFromForwardAndUp();
            Radius = radius;
        }

        /// <summary>
        /// Checks for intersection of the given spherical obstacle with a
        /// volume of "likely future vehicle positions": a cylinder along the
        /// current path, extending minTimeToCollision seconds along the
        /// forward axis from current position.
        ///
        /// If they intersect, a collision is imminent and this function returns
        /// a steering force pointing laterally away from the obstacle's center.
        ///
        /// Returns a zero vector if the obstacle is outside the cylinder.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="minTimeToCollision"></param>
        /// <returns></returns>
        public override Vector3 CollisionAvoidance(VehicleActor vehicle, float minTimeToCollision)
        {
            // minimum distance to obstacle before avoidance is required
            float minDistanceToCollision = minTimeToCollision * vehicle.Velocity;
            float minDistanceToCenter = minDistanceToCollision + Radius;

            // contact distance: sum of radii of obstacle and vehicle
            float totalRadius = Radius + vehicle.BoundingSphereRadius;

            // obstacle center relative to vehicle position
            Vector3 localOffset = Position - vehicle.Position;

            // distance along vehicle's forward axis to obstacle's center
            float forwardComponent = Vector3.Dot(localOffset, vehicle.Forward);
            Vector3 forwardOffset = vehicle.Forward * forwardComponent;

            // offset from forward axis to obstacle's center
            Vector3 offForwardOffset = localOffset - forwardOffset;

            // test to see if sphere overlaps with obstacle-free corridor
            bool inCylinder = offForwardOffset.Length() < totalRadius;
            bool nearby = forwardComponent < minDistanceToCenter;
            bool inFront = forwardComponent > 0;

            // if all three conditions are met, steer away from sphere center
            return inCylinder && nearby && inFront ? offForwardOffset * -1 : Vector3.Zero;
        }

        /// <summary>
        /// Check if a a specific vehicle has hit an obstacle, and return the point of collision. Returns Vector3.Zero if no collision detected.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public override Vector3 HasCollided(VehicleActor vehicle)
        {
            // contact distance: sum of radii of obstacle and vehicle
            float totalRadius = Radius + vehicle.BoundingSphereRadius;

            // obstacle center relative to vehicle position
            Vector3 localOffset = Position - vehicle.Position;

            // distance along vehicle's forward axis to obstacle's center
            float forwardComponent = Vector3.Dot(localOffset, vehicle.Forward);
            Vector3 forwardOffset = vehicle.Forward * forwardComponent;

            // offset from forward axis to obstacle's center
            Vector3 offForwardOffset = localOffset - forwardOffset;

            // test to see if sphere overlaps with obstacle-free corridor
            bool inCylinder = offForwardOffset.Length() < totalRadius;

            // if inCylinder, return point of impact, else return zero
            return inCylinder ? offForwardOffset : Vector3.Zero;
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
