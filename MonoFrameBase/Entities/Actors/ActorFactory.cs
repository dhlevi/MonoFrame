using Microsoft.Xna.Framework;
using MonoFrame.Entities.Actors;

namespace MonoFrame.Entities.Actors
{
    /// <summary>
    /// Factory helper for creating Vehicle and Non Vehicle entities
    /// This factory will create actors and automatically assign ID's for the actor manager.
    /// </summary>
    public class ActorFactory
    {
        // ##### Vehicle actor

        public static VehicleActor CreateVehicleActor(Game game)
        {
            return new VehicleActor(ActorManager.Instance.NextAvailableID, game);
        }

        public static VehicleActor CreateVehicleActor(Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
        {
            return new VehicleActor(ActorManager.Instance.NextAvailableID, game, inSide, inUp, inForward, inPosition);
        }

        public static VehicleActor CreateVehicleActor(Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
        {
            return new VehicleActor(ActorManager.Instance.NextAvailableID, game, inUp, inForward, inPosition);
        }

        // ##### Non vehicle actor

        public static NonVehicleActor CreateNonVehicleActor(Game game)
        {
            return new NonVehicleActor(ActorManager.Instance.NextAvailableID, game);
        }

        public static NonVehicleActor CreateNonVehicleActor(Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
        {
            return new NonVehicleActor(ActorManager.Instance.NextAvailableID, game, inSide, inUp, inForward, inPosition);
        }

        public static NonVehicleActor CreateNonVehicleActor(Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
        {
            return new NonVehicleActor(ActorManager.Instance.NextAvailableID, game, inUp, inForward, inPosition);
        }

        // ##### Spherical Obstacle actor

        public static SphericalObstacle CreateSphericalObstacleActor(Game game)
        {
            return new SphericalObstacle(ActorManager.Instance.NextAvailableID, game);
        }

        public static SphericalObstacle CreateSphericalObstacleActor(Game game, Vector3 position, float radius)
        {
            return new SphericalObstacle(ActorManager.Instance.NextAvailableID, game, radius, position);
        }

        public static SphericalObstacle CreateSphericalObstacleActor(Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float radius)
        {
            return new SphericalObstacle(ActorManager.Instance.NextAvailableID, game, inSide, inUp, inForward, inPosition, radius);
        }

        public static SphericalObstacle CreateSphericalObstacleActor(Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float radius)
        {
            return new SphericalObstacle(ActorManager.Instance.NextAvailableID, game, inUp, inForward, inPosition, radius);
        }

        // ##### Cube Obstacle actor

        public static CubeObstacle CreateCubeObstaclelActor(Game game)
        {
            return new CubeObstacle(ActorManager.Instance.NextAvailableID, game);
        }

        public static CubeObstacle CreateCubeObstacleActor(Game game, Vector3 position, float dimension)
        {
            return new CubeObstacle(ActorManager.Instance.NextAvailableID, game, dimension, position);
        }

        public static CubeObstacle CreateCubeObstacleActor(Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float dimension)
        {
            return new CubeObstacle(ActorManager.Instance.NextAvailableID, game, inSide, inUp, inForward, inPosition, dimension);
        }

        public static CubeObstacle CreateCubeObstacleActor(Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition, float dimension)
        {
            return new CubeObstacle(ActorManager.Instance.NextAvailableID, game, inUp, inForward, inPosition, dimension);
        }
    }
}
