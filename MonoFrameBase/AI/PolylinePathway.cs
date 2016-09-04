using Microsoft.Xna.Framework;

namespace MonoFrame.AI
{
    /// <summary>
    /// PolylinePathway: a simple implementation of the Pathway protocol.  The path
    /// is a "polyline" a series of line segments between specified points.  A
    /// radius defines a volume for the path which is the union of a sphere at each
    /// point and a cylinder along each segment.
    /// </summary>
    public class PolylinePathway : Pathway
    {
        public int PointCount { get; private set; }
        public Vector3[] Points { get; private set; }
        public float Radius { get; private set; }
        public bool Cyclic { get; private set; }

        public float SegmentLength { get; private set; }
        public float SegmentProjection { get; private set; }
        public Vector3 Local { get; private set; }
        public Vector3 Chosen { get; private set; }
        public Vector3 SegmentNormal { get; private set; }

        public float[] Lengths { get; private set; }
        public Vector3[] Normals { get; private set; }
        public float TotalPathLength { get; private set; }

        /// <summary>
        /// Construct an empty polyline pathway. Will need to be manually initialized
        /// </summary>
        public PolylinePathway()
        { }

        /// <summary>
        /// Construct and initialize a polyline pathway
        /// </summary>
        /// <param name="inPointCount"></param>
        /// <param name="inPoints"></param>
        /// <param name="inRadius"></param>
        /// <param name="inIsCyclic"></param>
        public PolylinePathway(int inPointCount, Vector3[] inPoints, float inRadius, bool inIsCyclic)
        {
            Initialize(inPointCount, inPoints, inRadius, inIsCyclic);
        }

        /// <summary>
        /// Initialize the Polyline with the provided point count, vectors and radius
        /// </summary>
        /// <param name="inPointCount"></param>
        /// <param name="inPoints"></param>
        /// <param name="inRadius"></param>
        /// <param name="inCyclic"></param>
        public void Initialize(int inPointCount, Vector3[] inPoints, float inRadius, bool inIsCyclic)
        {
            // set data members, allocate arrays
            Radius = inRadius;
            Cyclic = inIsCyclic;
            PointCount = inPointCount;
            TotalPathLength = 0;
            if (Cyclic) PointCount++;
            Lengths = new float[PointCount];
            Points = new Vector3[PointCount];
            Normals = new Vector3[PointCount];

            // loop over all points
            for (int i = 0; i < PointCount; i++)
            {
                // copy in point locations, closing cycle when appropriate
                bool closeCycle = Cyclic && (i == PointCount - 1);
                int j = closeCycle ? 0 : i;
                Points[i] = inPoints[j];

                // for the end of each segment
                if (i > 0)
                {
                    // compute the segment length
                    Normals[i] = Points[i] - Points[i - 1];
                    Lengths[i] = Normals[i].Length();

                    // find the normalized vector parallel to the segment
                    Normals[i] *= 1 / Lengths[i];

                    // keep running total of segment lengths
                    TotalPathLength += Lengths[i];
                }
            }
        }

        /// <summary>
        /// Given an arbitrary point ("A"), returns the nearest point ("P") on
        /// this path.  Also returns, via output arguments, the path tangent at
        /// P and a measure of how far A is outside the Pathway's "tube".  Note
        /// that a negative distance indicates A is inside the Pathway.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="tangent"></param>
        /// <param name="outside"></param>
        /// <returns></returns>
        public Vector3 MapPointToPath(Vector3 point, out Vector3 tangent, out float outside)
        {
            float d;
            float minDistance = float.MaxValue;
            Vector3 onPath = Vector3.Zero;
            tangent = Vector3.Zero;

            // loop over all segments, find the one nearest to the given point
            for (int i = 1; i < PointCount; i++)
            {
                SegmentLength = Lengths[i];
                SegmentNormal = Normals[i];
                d = PointToSegmentDistance(point, Points[i - 1], Points[i]);
                if (d < minDistance)
                {
                    minDistance = d;
                    onPath = Chosen;
                    tangent = SegmentNormal;
                }
            }

            // measure how far original point is outside the Pathway's "tube"
            outside = Vector3.Distance(onPath, point) - Radius;

            // return point on path
            return onPath;
        }

        /// <summary>
        /// Given an arbitrary point, convert it to a distance along the path.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float MapPointToPathDistance(Vector3 point)
        {
            float d;
            float minDistance = float.MaxValue;
            float segmentLengthTotal = 0;
            float pathDistance = 0;

            for (int i = 1; i < PointCount; i++)
            {
                SegmentLength = Lengths[i];
                SegmentNormal = Normals[i];
                d = PointToSegmentDistance(point, Points[i - 1], Points[i]);
                if (d < minDistance)
                {
                    minDistance = d;
                    pathDistance = segmentLengthTotal + SegmentProjection;
                }
                segmentLengthTotal += SegmentLength;
            }

            // return distance along path of onPath point
            return pathDistance;
        }

        /// <summary>
        /// given a distance along the path, convert it to a point on the path
        /// </summary>
        /// <param name="pathDistance"></param>
        /// <returns></returns>
        public Vector3 MapPathDistanceToPoint(float pathDistance)
        {
            // clip or wrap given path distance according to cyclic flag
            float remaining = pathDistance;
            if (Cyclic)
            {
                remaining = pathDistance % TotalPathLength;//FIXME: (float)fmod(pathDistance, totalPathLength);
            }
            else
            {
                if (pathDistance < 0) return Points[0];
                if (pathDistance >= TotalPathLength) return Points[PointCount - 1];
            }

            // step through segments, subtracting off segment lengths until
            // locating the segment that contains the original pathDistance.
            // Interpolate along that segment to find 3d point value to return.
            Vector3 result = Vector3.Zero;
            for (int i = 1; i < PointCount; i++)
            {
                SegmentLength = Lengths[i];
                if (SegmentLength < remaining)
                {
                    remaining -= SegmentLength;
                }
                else
                {
                    float ratio = remaining / SegmentLength;
                    result = Utilities.Interpolate(ratio, Points[i - 1], Points[i]);
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Is a given point inside the path
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInsidePath(Vector3 point)
        {
            float outside;
            Vector3 tangent;
            MapPointToPath(point, out tangent, out outside);
            return outside < 0;
        }

        /// <summary>
        /// how far outside path tube is the given point?  (negative is inside)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float HowFarOutsidePath(Vector3 point)
        {
            float outside;
            Vector3 tangent;
            MapPointToPath(point, out tangent, out outside);
            return outside;
        }

        // utility methods

        /// <summary>
        /// compute minimum distance from a point to a line segment
        /// </summary>
        /// <param name="point"></param>
        /// <param name="ep0"></param>
        /// <param name="ep1"></param>
        /// <returns></returns>
        public float PointToSegmentDistance(Vector3 point, Vector3 ep0, Vector3 ep1)
        {
            // convert the test point to be "local" to ep0
            Local = point - ep0;

            // find the projection of "local" onto "segmentNormal"
            SegmentProjection = Vector3.Dot(SegmentNormal, Local);

            // handle boundary cases: when projection is not on segment, the
            // nearest point is one of the endpoints of the segment
            if (SegmentProjection < 0)
            {
                Chosen = ep0;
                SegmentProjection = 0;
                return Vector3.Distance(point, ep0);
            }
            if (SegmentProjection > SegmentLength)
            {
                Chosen = ep1;
                SegmentProjection = SegmentLength;
                return Vector3.Distance(point, ep1);
            }

            // otherwise nearest point is projection point on segment
            Chosen = SegmentNormal * SegmentProjection;
            Chosen += ep0;
            return Vector3.Distance(point, Chosen);
        }
    }
}
