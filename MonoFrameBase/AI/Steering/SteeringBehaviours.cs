using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoFrame.Entities.Actors;

namespace MonoFrame.AI.Steering
{
    /// <summary>
    /// Basic steering behaviours for vehicle AI.
    /// 
    /// Vehicle entities can execute one or more of these steering behaviours to
    /// adjust how you want them to behave in space, either individually
    /// or with additional actors (flocks, packs, targets)
    /// </summary>
    public class SteeringBehaviours
    {
        /// <summary>
        /// Moves a Vehicle Entity in a wandering pattern. Wander simulates a random walk behaviour
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="ElapsedTime"></param>
        /// <param name="WanderSide"></param>
        /// <param name="WanderUp"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 Wander(VehicleActor vehicle, float ElapsedTime, float WanderSide, float WanderUp)
        {
            // random walk WanderSide and WanderUp between -1 and +1
            float speed = 12 * ElapsedTime; // maybe this (12) should be an argument?
            WanderSide = Utilities.ScalarRandomWalk(WanderSide, speed, -1, +1);
            WanderUp = Utilities.ScalarRandomWalk(WanderUp, speed, -1, +1);

            // return a pure lateral steering vector: (+/-Side) + (+/-Up)
            return (vehicle.Side * WanderSide) + (vehicle.Up * WanderUp);
        }

        /// <summary>
        /// Moves a Vehicle Entity towards another target entity. This will not ignore obstacles by default, so ensure you include collision avoidance if needed
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 Seek(VehicleActor vehicle, Vector3 target)
        {
            Vector3 desiredVelocity = target - vehicle.Position;
            return desiredVelocity - vehicle.TrueVelocity;
        }

        /// <summary>
        /// Alternate Seek method. As the standard seek, but a smoother test for desired velocity.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 AlternateSeek(VehicleActor vehicle, Vector3 target)
        {
            Vector3 offset = target - vehicle.Position;
            Vector3 desiredVelocity = Vector3Helpers.TruncateLength(offset, vehicle.MaximumVelocity);
            return desiredVelocity - vehicle.TrueVelocity;
        }

        /// <summary>
        /// Moves a Vehicle Entity away from another target entity. The opposite of seek. This will not ignore obstacles by default, so ensure you include collision avoidance if needed
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 Flee(VehicleActor vehicle, Vector3 target)
        {
            Vector3 desiredVelocity = vehicle.Position - target;
            return desiredVelocity - vehicle.TrueVelocity;
        }

        /// <summary>
        /// Alternate Flee method. As the standard seek, but a smoother test for desired velocity.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 AlternateFlee(VehicleActor vehicle, Vector3 target)
        {
            Vector3 offset = vehicle.Position - target;
            Vector3 desiredVelocity = Vector3Helpers.TruncateLength(offset, vehicle.MaximumVelocity);
            return desiredVelocity - vehicle.TrueVelocity;
        }

        /// <summary>
        /// Instructs a Vehicle Entity to follow a defined path (pathway), within the defined predicted timeframe
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="direction"></param>
        /// <param name="predictionTime"></param>
        /// <param name="path"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 FollowPath(VehicleActor vehicle, int direction, float predictionTime, Pathway path)
        {
            // our goal will be offset from our path distance by this amount
            float pathDistanceOffset = direction * predictionTime * vehicle.Velocity;

            // predict our future position
            Vector3 futurePosition = vehicle.PredictFuturePosition(predictionTime);

            // measure distance along path of our current and predicted positions
            float nowPathDistance = path.MapPointToPathDistance(vehicle.Position);
            float futurePathDistance = path.MapPointToPathDistance(futurePosition);

            // are we facing in the correction direction?
            bool rightway = ((pathDistanceOffset > 0) ?
                                   (nowPathDistance < futurePathDistance) :
                                   (nowPathDistance > futurePathDistance));

            // find the point on the path nearest the predicted future position
            // XXX need to improve calling sequence, maybe change to return a
            // XXX special path-defined object which includes two Vector3s and a 
            // XXX bool (onPath,tangent (ignored), withinPath)
            Vector3 tangent;
            float outside;
            Vector3 onPath = path.MapPointToPath(futurePosition, out tangent, out outside);

            // no steering is required if (a) our future position is inside
            // the path tube and (b) we are facing in the correct direction
            if ((outside < 0) && rightway)
            {
                // all is well, return zero steering
                return Vector3.Zero;
            }
            else
            {
                // otherwise we need to steer towards a target point obtained
                // by adding pathDistanceOffset to our current path position

                float targetPathDistance = nowPathDistance + pathDistanceOffset;
                Vector3 target = path.MapPathDistanceToPoint(targetPathDistance);

                //log PathFollowing(futurePosition, onPath, target, outside);

                // return steering to seek target on path
                return Seek(vehicle, target);
            }
        }

        /// <summary>
        /// Instructs Vehicle Entity to always stay on the provided path. Entity will move to the nearest point
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="predictionTime"></param>
        /// <param name="path"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 StayOnPath(VehicleActor vehicle, float predictionTime, Pathway path)
        {
            // predict our future position
            Vector3 futurePosition = vehicle.PredictFuturePosition(predictionTime);

            // find the point on the path nearest the predicted future position
            Vector3 tangent;
            float outside;
            Vector3 onPath = path.MapPointToPath(futurePosition, out tangent, out outside);

            if (outside < 0)
            {
                // our predicted future position was in the path,
                // return zero steering.
                return Vector3.Zero;
            }
            else
            {
                // our predicted future position was outside the path, need to
                // steer towards it.  Use onPath projection of futurePosition
                // as seek target
                //log PathFollowing(futurePosition, onPath, onPath, outside);
                return Seek(vehicle, onPath);
            }
        }

        /// <summary>
        /// Instructs a Vehicle Entity to avoid an obstacle entity.
        /// Avoidance is required when (1) the obstacle intersects the this's current path, (2) it is in front of the vehicle entity
        /// and (3) is within minTimeToCollision seconds of travel at the vehicle entities current velocity.  
        /// Returns a zero vector value (Vector3::zero) when no avoidance is required.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="minTimeToCollision"></param>
        /// <param name="obstacle"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 AvoidObstacle(VehicleActor vehicle, float minTimeToCollision, Obstacle obstacle)
        {
            Vector3 avoidance = obstacle.CollisionAvoidance(vehicle, minTimeToCollision);

            //assumes spherical obstacle?
            if (avoidance != Vector3.Zero)
            {
                // log AvoidObstacle(minTimeToCollision * this.Speed);
            }

            return avoidance;
        }

        /// <summary>
        /// Instructs a Vehicle Entity to avoid collisions with neighbouring entities.
        /// Avoid colliding with other nearby vehicles moving in unconstrained directions.  Determine which other vehicle
        /// we would collide with first, then steers to avoid the site of that potential collision.  Returns a steering
        /// force vector, which is zero length if there is no impending collision.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="minTimeToCollision"></param>
        /// <param name="others"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 AvoidNeighbors(VehicleActor vehicle, float minTimeToCollision, List<VehicleActor> others)
        {
            // first priority is to prevent immediate interpenetration
            Vector3 separation = AvoidCloseNeighbors(vehicle, 0, others);
            if (separation != Vector3.Zero) return separation;

            // otherwise, go on to consider potential future collisions
            float steer = 0;
            VehicleActor threat = null;

            // Time (in seconds) until the most immediate collision threat found
            // so far.  Initial value is a threshold: don't look more than this
            // many frames into the future.
            float minTime = minTimeToCollision;

            Vector3 ThreatPositionAtNearestApproach = Vector3.Zero;
            //Vector3 OurPositionAtNearestApproach = Vector3.Zero;

            // for each of the other vehicles, determine which (if any)
            // pose the most immediate threat of collision.
            foreach (VehicleActor other in others)
            {
                if (other != vehicle)
                {
                    // avoid when future positions are this close (or less)
                    float collisionDangerThreshold = vehicle.BoundingSphereRadius * 2;

                    // predicted time until nearest approach of "this" and "other"
                    float time = PredictNearestApproachTime(vehicle, other);

                    // If the time is in the future, sooner than any other
                    // threatened collision...
                    if ((time >= 0) && (time < minTime))
                    {
                        // if the two will be close enough to collide,
                        // make a note of it
                        if (ComputeNearestApproachPositions(vehicle, other, time) < collisionDangerThreshold)
                        {
                            minTime = time;
                            threat = other;
                            Vector3 otherTravel = other.Forward * other.Velocity * time;
                            ThreatPositionAtNearestApproach = other.Position + otherTravel;
                            //ThreatPositionAtNearestApproach = hisPositionAtNearestApproach;
                            //OurPositionAtNearestApproach = ourPositionAtNearestApproach;
                        }
                    }
                }
            }

            // if a potential collision was found, compute steering to avoid
            if (threat != null)
            {
                // parallel: +1, perpendicular: 0, anti-parallel: -1
                float parallelness = Vector3.Dot(vehicle.Forward, threat.Forward);
                float angle = 0.707f;

                if (parallelness < -angle)
                {
                    // anti-parallel "head on" paths:
                    // steer away from future threat position
                    Vector3 offset = ThreatPositionAtNearestApproach - vehicle.Position;
                    float sideDot = Vector3.Dot(offset, vehicle.Side);
                    steer = (sideDot > 0) ? -1.0f : 1.0f;
                }
                else
                {
                    if (parallelness > angle)
                    {
                        // parallel paths: steer away from threat
                        Vector3 offset = threat.Position - vehicle.Position;
                        float sideDot = Vector3.Dot(offset, vehicle.Side);
                        steer = (sideDot > 0) ? -1.0f : 1.0f;
                    }
                    else
                    {
                        // perpendicular paths: steer behind threat
                        // (only the slower of the two does this)
                        if (threat.Velocity <= vehicle.Velocity)
                        {
                            float sideDot = Vector3.Dot(vehicle.Side, threat.TrueVelocity);
                            steer = (sideDot > 0) ? -1.0f : 1.0f;
                        }
                    }
                }
                //log AvoidNeighbor(threat, steer, OurPositionAtNearestApproach, ThreatPositionAtNearestApproach);
            }

            return vehicle.Side * steer;
        }

        /// <summary>
        /// Predict the nearest approach time between two vehicle entities
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="other"></param>
        /// <returns>The Vector to move to</returns>
        public static float PredictNearestApproachTime(VehicleActor vehicle, VehicleActor other)
        {
            // imagine we are at the origin with no velocity,
            // compute the relative velocity of the other this
            Vector3 myVelocity = vehicle.TrueVelocity;
            Vector3 otherVelocity = other.TrueVelocity;
            Vector3 relVelocity = otherVelocity - myVelocity;
            float relSpeed = relVelocity.Length();

            // for parallel paths, the vehicles will always be at the same distance,
            // so return 0 (aka "now") since "there is no time like the present"
            if (relSpeed == 0) return 0;

            // Now consider the path of the other this in this relative
            // space, a line defined by the relative position and velocity.
            // The distance from the origin (our this) to that line is
            // the nearest approach.

            // Take the unit tangent along the other this's path
            Vector3 relTangent = relVelocity / relSpeed;

            // find distance from its path to origin (compute offset from
            // other to us, find length of projection onto path)
            Vector3 relPosition = vehicle.Position - other.Position;
            float projection = Vector3.Dot(relTangent, relPosition);

            return projection / relSpeed;
        }

        /// <summary>
        /// Used with PredictNearestApproachTime, determine the position of each
        /// vehicle entity at the given time, and the distance between them
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="other"></param>
        /// <param name="time"></param>
        /// <returns>The Vector to move to</returns>
        public static float ComputeNearestApproachPositions(VehicleActor vehicle, VehicleActor other)
        {
            float time = PredictNearestApproachTime(vehicle, other);

            Vector3 myTravel = vehicle.Forward * vehicle.Velocity * time;
            Vector3 otherTravel = other.Forward * other.Velocity * time;

            Vector3 myFinal = vehicle.Position + myTravel;
            Vector3 otherFinal = other.Position + otherTravel;

            return Vector3.Distance(myFinal, otherFinal);
        }

        /// <summary>
        /// determine the position of each vehicle entity at the given time, and the distance between them
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="other"></param>
        /// <param name="time"></param>
        /// <returns>The Vector to move to</returns>
        public static float ComputeNearestApproachPositions(VehicleActor vehicle, VehicleActor other, float time)
        {
            Vector3 myTravel = vehicle.Forward * vehicle.Velocity * time;
            Vector3 otherTravel = other.Forward * other.Velocity * time;

            Vector3 myFinal = vehicle.Position + myTravel;
            Vector3 otherFinal = other.Position + otherTravel;

            return Vector3.Distance(myFinal, otherFinal);
        }

        /// <summary>
        /// Used primarily by SteerToAvoidNeighbors.
        /// Hard steer away from another vehicle entity who comes within the
        /// seperation distance. Returns Vector zero if no avoidance needed
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="minSeparationDistance"></param>
        /// <param name="others"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 AvoidCloseNeighbors(VehicleActor vehicle, float minSeparationDistance, List<VehicleActor> others)
        {
            // for each of the other vehicles...
            foreach (VehicleActor other in others)
            {
                if (other != vehicle)
                {
                    float sumOfRadii = vehicle.BoundingSphereRadius + other.BoundingSphereRadius;
                    float minCenterToCenter = minSeparationDistance + sumOfRadii;
                    Vector3 offset = other.Position - vehicle.Position;
                    float currentDistance = offset.Length();

                    if (currentDistance < minCenterToCenter)
                    {
                        //log AvoidCloseNeighbor(other, minSeparationDistance);
                        return Vector3Helpers.PerpendicularComponent(-offset, vehicle.Forward);
                    }
                }
            }

            // otherwise return zero
            return Vector3.Zero;
        }

        /// <summary>
        /// Boid behaviour. Determines if vehicle entity is within another vehicile entities
        /// neighbourhood, by min and max distance tolerance and angle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="other"></param>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        /// <param name="cosMaxAngle"></param>
        /// <returns>The Vector to move to</returns>
        public static bool IsInNeighborhood(VehicleActor vehicle, VehicleActor other, float minDistance, float maxDistance, float cosMaxAngle)
        {
            if (other == vehicle)
            {
                return false;
            }
            else
            {
                Vector3 offset = other.Position - vehicle.Position;
                float distanceSquared = offset.LengthSquared();

                // definitely in neighborhood if inside minDistance sphere
                if (distanceSquared < (minDistance * minDistance))
                {
                    return true;
                }
                else
                {
                    // definitely not in neighborhood if outside maxDistance sphere
                    if (distanceSquared > (maxDistance * maxDistance))
                    {
                        return false;
                    }
                    else
                    {
                        // otherwise, test angular offset from forward axis
                        Vector3 unitOffset = offset / (float)Math.Sqrt(distanceSquared);
                        float forwardness = Vector3.Dot(vehicle.Forward, unitOffset);
                        return forwardness > cosMaxAngle;
                    }
                }
            }
        }

        /// <summary>
        /// Boid behaviour. Determines the seperation from a vehicle entity and its flock
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="maxDistance"></param>
        /// <param name="cosMaxAngle"></param>
        /// <param name="flock"></param>
        /// <returns>The Vector to move to</returns>
        public static Vector3 Separation(VehicleActor vehicle, float maxDistance, float cosMaxAngle, List<VehicleActor> flock)
        {
            // steering accumulator and count of neighbors, both initially zero
            Vector3 steering = Vector3.Zero;
            int neighbors = 0;

            // for each of the other vehicles...
            for (int i = 0; i < flock.Count; i++)
            {
                VehicleActor other = flock[i];
                if (IsInNeighborhood(vehicle, other, vehicle.BoundingSphereRadius * 3, maxDistance, cosMaxAngle))
                {
                    // add in steering contribution
                    // (opposite of the offset direction, divided once by distance
                    // to normalize, divided another time to get 1/d falloff)
                    Vector3 offset = other.Position - vehicle.Position;
                    float distanceSquared = Vector3.Dot(offset, offset);
                    steering += (offset / -distanceSquared);

                    // count neighbors
                    neighbors++;
                }
            }

            // divide by neighbors, then normalize to pure direction
            if (neighbors > 0)
            {
                steering = (steering / (float)neighbors);
                steering.Normalize();
            }

            return steering;
        }

        /// <summary>
        /// Boid behaviour. Align vehicle entity with given flock, using max distance and angle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="maxDistance"></param>
        /// <param name="cosMaxAngle"></param>
        /// <param name="flock"></param>
        /// <returns></returns>
        public static Vector3 Alignment(VehicleActor vehicle, float maxDistance, float cosMaxAngle, List<VehicleActor> flock)
        {
            // steering accumulator and count of neighbors, both initially zero
            Vector3 steering = Vector3.Zero;
            int neighbors = 0;

            // for each of the other vehicles...
            for (int i = 0; i < flock.Count; i++)
            {
                VehicleActor other = flock[i];
                if (IsInNeighborhood(vehicle, other, vehicle.BoundingSphereRadius * 3, maxDistance, cosMaxAngle))
                {
                    // accumulate sum of neighbor's heading
                    steering += other.Forward;

                    // count neighbors
                    neighbors++;
                }
            }

            // divide by neighbors, subtract off current heading to get error-
            // correcting direction, then normalize to pure direction
            if (neighbors > 0)
            {
                steering = ((steering / (float)neighbors) - vehicle.Forward);
                steering.Normalize();
            }

            return steering;
        }

        /// <summary>
        /// Boid behaviour. Ensure flock cohesion with vehicile entity
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="maxDistance"></param>
        /// <param name="cosMaxAngle"></param>
        /// <param name="flock"></param>
        /// <returns></returns>
        public static Vector3 Cohesion(VehicleActor vehicle, float maxDistance, float cosMaxAngle, List<VehicleActor> flock)
        {
            // steering accumulator and count of neighbors, both initially zero
            Vector3 steering = Vector3.Zero;
            int neighbors = 0;

            // for each of the other vehicles...
            for (int i = 0; i < flock.Count; i++)
            {
                VehicleActor other = flock[i];
                if (IsInNeighborhood(vehicle, other, vehicle.BoundingSphereRadius * 3, maxDistance, cosMaxAngle))
                {
                    // accumulate sum of neighbor's positions
                    steering += other.Position;

                    // count neighbors
                    neighbors++;
                }
            }

            // divide by neighbors, subtract off current position to get error-
            // correcting direction, then normalize to pure direction
            if (neighbors > 0)
            {
                steering = ((steering / (float)neighbors) - vehicle.Position);
                steering.Normalize();
            }

            return steering;
        }

        /// <summary>
        /// Calculate target pursuit vector between two vehicle entities
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 Pursuit(VehicleActor vehicle, VehicleActor target)
        {
            return Pursuit(vehicle, target, float.MaxValue);
        }

        /// <summary>
        /// Calculate target pursuit vector between two vehicle entities, within a maximum time
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <param name="maxPredictionTime"></param>
        /// <returns></returns>
        public static Vector3 Pursuit(VehicleActor vehicle, VehicleActor target, float maxPredictionTime)
        {
            // offset from this to quarry, that distance, unit vector toward quarry
            Vector3 offset = target.Position - vehicle.Position;
            float distance = offset.Length();
            Vector3 unitOffset = offset / distance;

            // how parallel are the paths of "this" and the quarry
            // (1 means parallel, 0 is pependicular, -1 is anti-parallel)
            float parallelness = Vector3.Dot(vehicle.Forward, target.Forward);

            // how "forward" is the direction to the quarry
            // (1 means dead ahead, 0 is directly to the side, -1 is straight back)
            float forwardness = Vector3.Dot(vehicle.Forward, unitOffset);

            float directTravelTime = distance / vehicle.Velocity;
            int f = Utilities.IntervalComparison(forwardness, -0.707f, 0.707f);
            int p = Utilities.IntervalComparison(parallelness, -0.707f, 0.707f);

            float timeFactor = 0;   // to be filled in below

            // Break the pursuit into nine cases, the cross product of the
            // quarry being [ahead, aside, or behind] us and heading
            // [parallel, perpendicular, or anti-parallel] to us.
            switch (f)
            {
                case +1:
                    switch (p)
                    {
                        case +1:          // ahead, parallel
                            timeFactor = 4;
                            break;
                        case 0:           // ahead, perpendicular
                            timeFactor = 1.8f;
                            break;
                        case -1:          // ahead, anti-parallel
                            timeFactor = 0.85f;
                            break;
                    }
                    break;
                case 0:
                    switch (p)
                    {
                        case +1:          // aside, parallel
                            timeFactor = 1;
                            break;
                        case 0:           // aside, perpendicular
                            timeFactor = 0.8f;
                            break;
                        case -1:          // aside, anti-parallel
                            timeFactor = 4;
                            break;
                    }
                    break;
                case -1:
                    switch (p)
                    {
                        case +1:          // behind, parallel
                            timeFactor = 0.5f;
                            break;
                        case 0:           // behind, perpendicular
                            timeFactor = 2;
                            break;
                        case -1:          // behind, anti-parallel
                            timeFactor = 2;
                            break;
                    }
                    break;
            }

            // estimated time until intercept of target
            float et = directTravelTime * timeFactor;

            // experiment, if kept, this limit should be an argument
            float etl = (et > maxPredictionTime) ? maxPredictionTime : et;

            // estimated position of quarry at intercept
            Vector3 targetLocation = target.PredictFuturePosition(etl);

            return Seek(vehicle, targetLocation);
        }

        /// <summary>
        /// Determine evasion vector between two vehicle entities
        /// Often used by the target of a pursuit to aid in avoidance of pursuing vehicle
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="menace"></param>
        /// <param name="maxPredictionTime"></param>
        /// <returns></returns>
        public static Vector3 Evasion(VehicleActor vehicle, VehicleActor menace, float maxPredictionTime)
        {
            // offset from this to menace, that distance, unit vector toward menace
            Vector3 offset = menace.Position - vehicle.Position;
            float distance = offset.Length();

            float roughTime = distance / menace.Velocity;
            float predictionTime = ((roughTime > maxPredictionTime) ? maxPredictionTime : roughTime);

            Vector3 target = menace.PredictFuturePosition(predictionTime);

            return Flee(vehicle, target);
        }

        /// <summary>
        /// Instruct a vehicle entity to maintain a given speed
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="targetSpeed"></param>
        /// <returns></returns>
        public static Vector3 TargetSpeed(VehicleActor vehicle, float targetSpeed)
        {
            float mf = vehicle.MaximumSteeringForce;
            float speedError = targetSpeed - vehicle.Velocity;
            return vehicle.Forward * Utilities.Clip(speedError, -mf, +mf);
        }

        // ####################### UTILITIES ######################## //

        /// <summary>
        ///  Utility method to determine if a vehicle entity is ahead of a target
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsAhead(VehicleActor vehicle, Vector3 target)
        {
            return IsAhead(vehicle, target, 0.707f);
        }

        /// <summary>
        ///  Utility method to determine if a vehicle entity is beside a target
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsBeside(VehicleActor vehicle, Vector3 target)
        {
            return IsBeside(vehicle, target, 0.707f);
        }

        /// <summary>
        ///  Utility method to determine which vehicle entity is behind a target
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsBehind(VehicleActor vehicle, Vector3 target)
        {
            return IsBehind(vehicle, target, -0.707f);
        }

        /// <summary>
        ///  Utility method to determine if a vehicle entity is ahead of a target within a threshold
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <param name="cosThreshold"></param>
        /// <returns></returns>
        public static bool IsAhead(VehicleActor vehicle, Vector3 target, float cosThreshold)
        {
            Vector3 targetDirection = (target - vehicle.Position);
            targetDirection.Normalize();
            return Vector3.Dot(vehicle.Forward, targetDirection) > cosThreshold;
        }


        /// <summary>
        ///  Utility method to determine if a vehicle entity is beside of a target within a threshold
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <param name="cosThreshold"></param>
        /// <returns></returns>
        public static bool IsBeside(VehicleActor vehicle, Vector3 target, float cosThreshold)
        {
            Vector3 targetDirection = (target - vehicle.Position);
            targetDirection.Normalize();
            float dp = Vector3.Dot(vehicle.Forward, targetDirection);
            return (dp < cosThreshold) && (dp > -cosThreshold);
        }


        /// <summary>
        ///  Utility method to determine if a vehicle entity is ahead of a target behind a threshold
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="target"></param>
        /// <param name="cosThreshold"></param>
        /// <returns></returns>
        public static bool IsBehind(VehicleActor vehicle, Vector3 target, float cosThreshold)
        {
            Vector3 targetDirection = (target - vehicle.Position);
            targetDirection.Normalize();
            return Vector3.Dot(vehicle.Forward, targetDirection) < cosThreshold;
        }
    }
}
