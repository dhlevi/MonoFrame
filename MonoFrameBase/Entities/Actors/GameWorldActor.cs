using Microsoft.Xna.Framework;
using MonoFrame.Messaging;

namespace MonoFrame.Entities.Actors
{
    /// <summary>
    /// An Extentsion of the Base Actor to a GameWorldActor
    /// The game world actor contains the base actor elements, and includes the required
    /// parameters for the actor entity to "live" in the game space.
    /// The GameWorldActor is then extended further by vehicle and non vehicle entities
    /// </summary>
    public class GameWorldActor : BaseActor // Extend Base Actor, Then vehicle/non vehicle extend local? 
    {
        public Vector3 Side { get; set; } // side-pointing unit basis vector
        public Vector3 Up { get; set; } // upward-pointing unit basis vector
        public Vector3 Forward { get; set; } // forward-pointing unit basis vector
        public Vector3 Position { get; set; } // origin of local space

        // Global compile-time switch to control handedness/chirality: should
        // LocalSpace use a left- or right-handed coordinate system?  This can be
        // overloaded in derived types (e.g. vehicles) to change handedness.
        public bool IsRightHanded { get { return true; } }

        public GameWorldActor(long inID, Game game)
            : base(inID, game)
        {
            ResetLocalSpace();
        }

        public GameWorldActor(long inID, Game game, Vector3 inSide, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Side = inSide;
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
        }

        public GameWorldActor(long inID, Game game, Vector3 inUp, Vector3 inForward, Vector3 inPosition)
            : base(inID, game)
        {
            Up = inUp;
            Forward = inForward;
            Position = inPosition;
            SetUnitSideFromForwardAndUp();
        }

        /// <summary>
        /// Resets the actors position in local space
        /// </summary>
        public void ResetLocalSpace()
        {
            Forward = Vector3.Backward;
            Side = LocalRotateForwardToSide(Forward);
            Up = Vector3.Up;
            Position = Vector3.Zero;
        }

        /// <summary>
        /// Transform a direction in global space to its equivalent in local space
        /// </summary>
        /// <param name="globalDirection"></param>
        /// <returns></returns>
        public Vector3 LocalizeDirection(Vector3 globalDirection)
        {
            // dot offset with local basis vectors to obtain local coordiantes
            return new Vector3(Vector3.Dot(globalDirection, Side), Vector3.Dot(globalDirection, Up), Vector3.Dot(globalDirection, Forward));
        }

        /// <summary>
        /// Transform a point in global space to its equivalent in local space
        /// </summary>
        /// <param name="globalPosition"></param>
        /// <returns></returns>
        public Vector3 LocalizePosition(Vector3 globalPosition)
        {
            // global offset from local origin
            Vector3 globalOffset = globalPosition - Position;

            // dot offset with local basis vectors to obtain local coordiantes
            return LocalizeDirection(globalOffset);
        }

        /// <summary>
        /// Transform a point in local space to its equivalent in global space
        /// </summary>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public Vector3 GlobalizePosition(Vector3 localPosition)
        {
            return Position + GlobalizeDirection(localPosition);
        }

        /// <summary>
        /// Transform a direction in local space to its equivalent in global space
        /// </summary>
        /// <param name="localDirection"></param>
        /// <returns></returns>
        public Vector3 GlobalizeDirection(Vector3 localDirection)
        {
            return ((Side * localDirection.X) +
                    (Up * localDirection.Y) +
                    (Forward * localDirection.Z));
        }

        /// <summary>
        /// Set "side" basis vector to normalized cross product of forward and up
        /// </summary>
        public void SetUnitSideFromForwardAndUp()
        {
            // derive new unit side basis vector from forward and up
            if (IsRightHanded)
                Side = Vector3.Cross(Forward, Up);
            else
                Side = Vector3.Cross(Up, Forward);

            Side.Normalize();
        }

        /// <summary>
        /// Regenerate the orthonormal basis vectors given a new forward
        /// </summary>
        /// <param name="newUnitForward"></param>
        public void RegenerateOrthonormalBasisUF(Vector3 newUnitForward)
        {
            Forward = newUnitForward;

            // derive new side basis vector from NEW forward and OLD up
            SetUnitSideFromForwardAndUp();

            // derive new Up basis vector from new Side and new Forward
            //(should have unit length since Side and Forward are
            // perpendicular and unit length)
            if (IsRightHanded)
                Up = Vector3.Cross(Side, Forward);
            else
                Up = Vector3.Cross(Forward, Side);
        }

        /// <summary>
        /// For when the new forward is NOT know to have unit length
        /// </summary>
        /// <param name="newForward"></param>
        public void RegenerateOrthonormalBasis(Vector3 newForward)
        {
            newForward.Normalize();

            RegenerateOrthonormalBasisUF(newForward);
        }

        /// <summary>
        /// For supplying both a new forward and and new up
        /// </summary>
        /// <param name="newForward"></param>
        /// <param name="newUp"></param>
        public void RegenerateOrthonormalBasis(Vector3 newForward, Vector3 newUp)
        {
            Up = newUp;
            newForward.Normalize();
            RegenerateOrthonormalBasis(newForward);
        }

        /// <summary>
        /// Rotate a vector in the canonical direction pointing in the "forward"(+Z) direction to the "side"(+/-X) direction
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vector3 LocalRotateForwardToSide(Vector3 value)
        {
            return new Vector3(IsRightHanded ? -value.Z : +value.Z, value.Y, value.X);
        }

        public Vector3 GlobalRotateForwardToSide(Vector3 value)
        {
            Vector3 localForward = LocalizeDirection(value);
            Vector3 localSide = LocalRotateForwardToSide(localForward);
            return GlobalizeDirection(localSide);
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
