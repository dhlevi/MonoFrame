using System;
using Microsoft.Xna.Framework;
using MonoFrame.AI;
using MonoFrame.Messaging;

namespace MonoFrame.Entities.Actors
{
    /// <summary>
    /// A GameWorldActor that implements the Vehicle interface
    /// This means the actor will be able to move in local or global space
    /// and can be used in steering behaviour AI.
    /// </summary>
    public class VehicleActor : GameWorldActor, IVehicle
    {
        public float Mass { get; set; } // mass of the entity, for acceleration
        public float BoundingSphereRadius { get; set; } // sphere around entity, for object avoidance, etc.
        public float VisibilitySphereRadius { get; set; } // sphere around entity, For what it can 'see'
        public float Velocity { get; set; } // Velocity in a forward direction
        public float MaximumVelocity { get; set; } // the max speed the entity can travel at
        public float MaximumSteeringForce { get; set; } // the max steering force of the entity

        public Vector3 LastForward { get; set; }
        public Vector3 LastPosition { get; set; }
        public Vector3 SmoothedPosition { get; set; }

        public float Curvature { get; set; }
        public float SmoothedCurvature { get; set; }
        public Vector3 SmoothedAcceleration { get; set; }

        public VehicleActor(long inID, Game game)
            : base(inID, game)
        {
            ResetLocalSpace();
        }

        public VehicleActor(long inID, Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Side = inSide;
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
        }

        public VehicleActor(long inID, Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            SetUnitSideFromForwardAndUp();
        }
        
        /// <summary>
        /// The true velocit of the vehicle (Forward vector * Velocity)
        /// </summary>
        public Vector3 TrueVelocity
        {
            get
            {
                return Forward * Velocity;
            }
        }

        /// <summary>
        /// Prediction of the location that this vehicle is headed given a point in time
        /// This function does not take any steering into account and assumes
        /// a constant velocity and heading
        /// </summary>
        /// <param name="predictionTime"></param>
        /// <returns></returns>
        public Vector3 PredictFuturePosition(float predictionTime)
        {
            return Position + (TrueVelocity * predictionTime);
        }

        /// <summary>
        /// Reset the vehicles location in space, velocity, and mass
        /// as well as local space base values
        /// </summary>
        public void Reset()
        {
            Mass = 1;
            Velocity = 0;
            BoundingSphereRadius = 0.5f;
            VisibilitySphereRadius = 0.5f;
            MaximumSteeringForce = 0.1f;
            MaximumVelocity = 1.0f;
            ResetLocalSpace();
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
