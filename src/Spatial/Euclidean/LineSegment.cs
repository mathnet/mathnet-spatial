using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using HashCode = MathNet.Spatial.Internals.HashCode;
using MathNet.Numerics;
using System.Drawing;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// This structure represents a line segment between two points in 3D-space.  It allows for operations such as
    /// computing the length, direction, comparisons, and shifting by a vector.
    /// </summary>
    [Serializable]
    public struct LineSegment : IEquatable<LineSegment>, IXmlSerializable
    {
        /// <summary>
        /// The starting point of the line segment
        /// </summary>
        public Point3D StartPoint { get; private set; }

        /// <summary>
        /// The end point of the line segment
        /// </summary>
        public Point3D EndPoint { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> struct.
        /// Throws an ArgumentException if the <paramref name="startPoint"/> is equal to the <paramref name="endPoint"/>.
        /// </summary>
        /// <param name="startPoint">the starting point of the line segment.</param>
        /// <param name="endPoint">the ending point of the line segment</param>
        public LineSegment(Point3D startPoint, Point3D endPoint)
        {
            if (startPoint == endPoint)
            {
                throw new ArgumentException("The segment starting and ending points cannot be identical");
            }

            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        /// <summary>
        /// Gets the distance from <see cref="StartPoint"/> to <see cref="EndPoint"/>
        /// </summary>
        [Pure]
        public double Length => StartPoint.DistanceTo(EndPoint);

        /// <summary>
        /// Gets a normalized vector in the direction from <see cref="StartPoint"/> to <see cref="EndPoint"/>
        /// </summary>
        [Pure]
        public Direction Direction => StartPoint.VectorTo(EndPoint).Normalize();

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified lines is equal.
        /// </summary>
        /// <param name="left">The first line to compare</param>
        /// <param name="right">The second line to compare</param>
        /// <returns>True if the lines are the same; otherwise false.</returns>
        public static bool operator ==(LineSegment left, LineSegment right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified lines is not equal.
        /// </summary>
        /// <param name="left">The first line to compare</param>
        /// <param name="right">The second line to compare</param>
        /// <returns>True if the lines are different; otherwise false.</returns>
        public static bool operator !=(LineSegment left, LineSegment right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a new <see cref="LineSegment"/> from a pair of strings which represent points.
        /// See <see cref="Point3D.Parse(string, IFormatProvider)" /> for details on acceptable formats.
        /// </summary>
        /// <param name="startPointString">The string representation of the first point.</param>
        /// <param name="endPointString">The string representation of the second point.</param>
        /// <returns>A line segment from the first point to the second point.</returns>
        public static LineSegment Parse(string startPointString, string endPointString)
        {
            return new LineSegment(Point3D.Parse(startPointString), Point3D.Parse(endPointString));
        }

        /// <summary>
        /// Translates a line according to a provided vector
        /// </summary>
        /// <param name="vector">A vector to apply</param>
        /// <returns>A new translated line segment</returns>
        public LineSegment TranslateBy(Vector3D vector)
        {
            return new LineSegment(StartPoint + vector, EndPoint + vector);
        }

        /// <summary>
        /// Returns the closest point on the line segment to the given point.
        /// </summary>
        /// <param name="p">The point that the returned point is the closest point on the line segment to</param>
        /// <param name="mustBeOnSegment">If true the returned point is contained by the segment ends, otherwise it can be anywhere on the projected line</param>
        /// <returns>The closest point on the line segment to the provided point</returns>
        [Pure]
        public Point3D ClosestPointTo(Point3D p, bool mustBeOnSegment = true)
        {
            var v = StartPoint.VectorTo(p);
            var dotProduct = v.DotProduct(Direction);

            if (mustBeOnSegment)
            {
                if (dotProduct < 0)
                {
                    dotProduct = 0;
                }

                if (dotProduct > Length)
                {
                    dotProduct = Length;
                }
            }

            var alongVector = dotProduct * Direction;
            return StartPoint + alongVector;
        }

        /// <summary>
        /// The line segment projected on a plane
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <returns>A projected line.</returns>
        [Pure]
        public LineSegment ProjectOn(Plane plane)
        {
            return plane.Project(this);
        }

        /// <summary>
        /// Returns a new line segment between the closest point on this line segment and a point.
        /// </summary>
        /// <param name="p">the point to create a line to</param>
        /// <returns>A line segment between the nearest point on this segment and the provided point.</returns>
        [Pure]
        public LineSegment LineTo(Point3D p)
        {
            return new LineSegment(ClosestPointTo(p), p);
        }

        /// <summary>
        /// Computes the pair of points which represent the closest distance between this LineSegment and another LineSegment
        /// </summary>
        /// <param name="other">Line segment to compute the closest points with</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <param name="closestLine">A line representing the endpoints of the shortest distance between the two segments</param>
        /// <returns>True if a line could be found, false if the lines intersect</returns>
        [Pure]
        public bool TryShortestLineTo(LineSegment other, Angle tolerance, out LineSegment closestLine)
        {
            // If the segments are parallel and the answer must be on the segments, we can skip directly to the ending
            // algorithm where the endpoints are projected onto the opposite segment and the smallest distance is
            // taken.  Otherwise we must first check if the infinite length line solution is valid.
            // If the lines aren't parallel
            if (!IsParallelTo(other, tolerance))
            {
                // Compute the unbounded result
                var result = ClosestPointsBetweenLines(other, tolerance);

                // A point that is known to be collinear with the line start and end points is on the segment if
                // its distance to both endpoints is less than the segment length.  If both projected points lie
                // within their segment, we can directly return the result.
                if (result.Item1.DistanceTo(StartPoint) <= Length &&
                    result.Item1.DistanceTo(EndPoint) <= Length &&
                    result.Item2.DistanceTo(other.StartPoint) <= other.Length &&
                    result.Item2.DistanceTo(other.EndPoint) <= other.Length)
                {
                    if (result.Item1 == result.Item2)
                    {
                        closestLine = default(LineSegment);
                        return false;
                    }

                    closestLine = new LineSegment(result.Item1, result.Item2);
                    return true;
                }
            }

            //// If we got here, we know that either we're doing a bounded distance on two parallel segments or one
            //// of the two closest span points is outside of the segment of the line it was projected on.  In either
            //// case we project each of the four endpoints onto the opposite segments and select the one with the
            //// smallest projected distance.

            var checkPoint = other.ClosestPointTo(StartPoint);
            var distance = checkPoint.DistanceTo(StartPoint);
            var closestPair = Tuple.Create(StartPoint, checkPoint);
            var minDistance = distance;

            checkPoint = other.ClosestPointTo(EndPoint);
            distance = checkPoint.DistanceTo(EndPoint);
            if (distance < minDistance)
            {
                closestPair = Tuple.Create(EndPoint, checkPoint);
                minDistance = distance;
            }

            checkPoint = ClosestPointTo(other.StartPoint);
            distance = checkPoint.DistanceTo(other.StartPoint);
            if (distance < minDistance)
            {
                closestPair = Tuple.Create(checkPoint, other.StartPoint);
                minDistance = distance;
            }

            checkPoint = ClosestPointTo(other.EndPoint);
            distance = checkPoint.DistanceTo(other.EndPoint);
            if (distance < minDistance)
            {
                closestPair = Tuple.Create(checkPoint, other.EndPoint);
            }

            if (closestPair.Item1 == closestPair.Item2)
            {
                closestLine = default(LineSegment);
                return false;
            }

            closestLine = new LineSegment(closestPair.Item1, closestPair.Item2);
            return true;
        }

        /// <summary>
        /// Checks to determine whether or not two line segments are parallel to each other, using the dot product within
        /// the double precision specified in the MathNet.Numerics package.
        /// </summary>
        /// <param name="other">The other line segment to check this one against</param>
        /// <returns>True if the line segments are parallel, false if they are not</returns>
        [Pure]
        public bool IsParallelTo(LineSegment other)
        {
            return Direction.IsParallelTo(other.Direction, Precision.DoublePrecision * 2);
        }

        /// <summary>
        /// Checks to determine whether or not two line segments are parallel to each other within a specified angle tolerance
        /// </summary>
        /// <param name="other">The other line to check this one against</param>
        /// <param name="tolerance">If the angle between line directions is less than this value, the method returns true</param>
        /// <returns>True if the lines are parallel within the angle tolerance, false if they are not</returns>
        [Pure]
        public bool IsParallelTo(LineSegment other, Angle tolerance)
        {
            return Direction.IsParallelTo(other.Direction, tolerance);
        }

        /// <inheritdoc/>
        [Pure]
        public override string ToString()
        {
            return $"StartPoint: {StartPoint}, EndPoint: {EndPoint}";
        }

        /// <summary>
        /// Returns a value to indicate if a pair of line segments are equal
        /// </summary>
        /// <param name="other">The line segment to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>True if the line segments are equal; otherwise false</returns>
        [Pure]
        public bool Equals(LineSegment other, double tolerance)
        {
            return StartPoint.Equals(other.StartPoint, tolerance) && EndPoint.Equals(other.EndPoint, tolerance);
        }

        /// <inheritdoc/>
        [Pure]
        public bool Equals(LineSegment l) => StartPoint.Equals(l.StartPoint) && EndPoint.Equals(l.EndPoint);

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is LineSegment l && Equals(l);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(StartPoint, EndPoint);

        /// <summary>
        /// Extends the segment to a infinite line and finds the closest point on that line to the provided point.
        /// </summary>
        /// <param name="p">a point</param>
        /// <returns>A point on the infinite line which extends the segment</returns>
        [Pure]
        private Point3D ClosestLinePointTo(Point3D p)
        {
            var alongVector = StartPoint.VectorTo(p).DotProduct(Direction) * Direction;
            return StartPoint + alongVector;
        }

        /// <summary>
        /// Compute the intersection between two line segments if the angle between them is greater than a specified
        /// angle tolerance.
        /// </summary>
        /// <param name="other">The other line segment to compute the intersection with</param>
        /// <param name="intersection">The intersection if it exists; otherwise null</param>
        /// <param name="tolerance">The tolerance used when checking if the line segments are parallel</param>
        /// <returns>True if an intersection exists; otherwise false</returns>
        [Pure]
        public bool TryIntersect(LineSegment other, out Point3D? intersection, Angle tolerance)
        {
            if (StartPoint.Z != 0 || EndPoint.Z != 0)
            {
                throw new ArgumentException($"The current segment {this} does not lie on the XY plane");
            }

            if (other.StartPoint.Z != 0 || other.EndPoint.Z != 0)
            {
                throw new ArgumentException($"The other segment {other} does not lie on the XY plane");
            }

            intersection = null;
            if (IsParallelTo(other, tolerance))
            {
                return false;
            }

            // http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
            var p = StartPoint;
            var q = other.StartPoint;
            var r = StartPoint.VectorTo(EndPoint);
            var s = other.StartPoint.VectorTo(other.EndPoint);

            var t = (q - p).CrossProduct(s).Length / r.CrossProduct(s).Length;
            var u = (p - q).CrossProduct(r).Length / s.CrossProduct(r).Length;

            var isIntersected = (0.0 <= t && t <= 1.0) && (0.0 <= u && u <= 1.0);
            if (isIntersected)
            {
                intersection = p + t * r;
            }

            return intersection != null;
        }

        /// <summary>
        /// Computes the pair of points which represent the closest distance between this LineSegment and another LineSegment, with the first
        /// point being the point on this LineSegment, and the second point being the corresponding point on the other LineSegment.  If the lines
        /// intersect the points will be identical, if the lines are parallel the first point will be the start point of this line.
        /// </summary>
        /// <param name="other">Segment to compute the closest points with</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>A tuple of two points representing the endpoints of the shortest distance between the two line segments</returns>
        [Pure]
        private Tuple<Point3D, Point3D> ClosestPointsBetweenLines(LineSegment other, Angle tolerance)
        {
            if (IsParallelTo(other, tolerance))
            {
                return Tuple.Create(StartPoint, other.ClosestLinePointTo(StartPoint));
            }

            // http://geomalgorithms.com/a07-_distance.html
            var point0 = StartPoint;
            var u = Direction;
            var point1 = other.StartPoint;
            var v = other.Direction;

            var w0 = point0 - point1;
            var a = u.DotProduct(u);
            var b = u.DotProduct(v);
            var c = v.DotProduct(v);
            var d = u.DotProduct(w0);
            var e = v.DotProduct(w0);

            var sc = ((b * e) - (c * d)) / ((a * c) - (b * b));
            var tc = ((a * e) - (b * d)) / ((a * c) - (b * b));

            return Tuple.Create(point0 + (sc * u), point1 + (tc * v));
        }

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc/>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            this = new LineSegment(
                Point3D.ReadFrom(e.SingleElement("StartPoint").CreateReader()),
                Point3D.ReadFrom(e.SingleElement("EndPoint").CreateReader()));
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("StartPoint", StartPoint);
            writer.WriteElement("EndPoint", EndPoint);
        }
    }
}
