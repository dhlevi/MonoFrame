using Microsoft.Xna.Framework;

namespace MonoFrame.AI
{
    /// <summary>
    /// Vehicle Interface. These methods and attributes are required for
    /// any vehicle entity, and is implemented by the VehicleActor
    /// </summary>
    public interface IVehicle
    {
        float Mass { get; set; } // mass of the entity, for acceleration
        float BoundingSphereRadius { get; set; } // sphere around entity, for object avoidance, etc.
        float VisibilitySphereRadius { get; set; } // sphere around entity, For what it can 'see'
        float Velocity { get; set; } // Velocity in a forward direction
        float MaximumVelocity { get; set; } // the max speed the entity can travel at
        float MaximumSteeringForce { get; set; } // the max steering force of the entity

        Vector3 LastForward { get; set; }
        Vector3 LastPosition { get; set; }
        Vector3 SmoothedPosition { get; set; }

        float Curvature { get; set; }
        float SmoothedCurvature { get; set; }
        Vector3 SmoothedAcceleration { get; set; }

        Vector3 TrueVelocity { get; }
        Vector3 PredictFuturePosition(float predictionTime);
        void Reset();
    }
}
