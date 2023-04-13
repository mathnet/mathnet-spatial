using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Spatial.Internals;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A infinite line in 3D space.
    /// </summary>
    [Serializable]
    public struct Line : IEquatable<Line>, IXmlSerializable, IFormattable
    {
        /// <summary>
        /// A given through point of the line
        /// </summary>
        public readonly Point3D ThroughPoint;

        /// <summary>
        /// The direction of the line
        /// </summary>
        public readonly Direction Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> struct.
        /// </summary>
        /// <param name="throughPoint">A through point of the line.</param>
        /// <param name="direction">The direction of the line.</param>
        public Line(Point3D throughPoint, Direction direction)
        {
            ThroughPoint = throughPoint;
            Direction = direction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> struct.
        /// </summary>
        /// <param name="throughPoint">A through point of the line.</param>
        /// <param name="direction">A vector indicating the direction of the line.</param>
        public Line(Point3D throughPoint, Vector3D direction)
            : this(throughPoint, direction.Normalize())
        {
        }

        /// <summary>
        /// Returns a value that indicates whether the two lines are equal.
        /// </summary>
        /// <param name="left">The first line to compare</param>
        /// <param name="right">The second line to compare</param>
        /// <returns>True if the lines are the same; otherwise false.</returns>
        public static bool operator ==(Line left, Line right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether the two lines are not equal.
        /// </summary>
        /// <param name="left">The first line to compare</param>
        /// <param name="right">The second line to compare</param>
        /// <returns>True if the lines are different; otherwise false.</returns>
        public static bool operator !=(Line left, Line right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// The intersection of the two planes
        /// </summary>
        /// <param name="plane1">The first plane</param>
        /// <param name="plane2">The second plane</param>
        /// <returns>A line at the common intersection of two the planes</returns>
        public static Line IntersectionOf(Plane plane1, Plane plane2)
        {
            return plane1.IntersectionWith(plane2);
        }

        /// <summary>
        /// Parses string representation of through point and direction
        /// See <see cref="Point3D.Parse(string, IFormatProvider)" /> and  <see cref="Euclidean.Direction.Parse(string, IFormatProvider, double)" /> for details on acceptable formats.
        /// This is mainly meant for tests
        /// </summary>
        /// <param name="point">a string representing a through point for the line.</param>
        /// <param name="direction">a string representing a direction for the line.</param>
        /// <returns>A line.</returns>
        public static Line Parse(string point, string direction)
        {
            return new Line(Point3D.Parse(point), Direction.Parse(direction));
        }

        /// <summary>
        /// Returns the distance of a <param name="point"/> from this line
        /// </summary>
        /// <param name="point">A point in space, whose distance from this line is requested</param>
        /// <returns>The (normal) distance of the point to this line</returns>
        [Pure]
        public double DistanceTo(Point3D point)
        {
            return (PerpendicularFootTo(point) - point).Length;
        }

        /// <summary>
        /// Returns the shortest line segment from a point to the line
        /// </summary>
        /// <param name="point3D">A point.</param>
        /// <returns>A line segment from the point to the closest point on the line</returns>
        [Pure]
        public LineSegment ShortestLineSegmentTo(Point3D point3D)
        {
            return new LineSegment(PerpendicularFootTo(point3D), point3D);
        }

        /// <summary>
        /// Returns the perpendicular foot on this line from a given <param name="point"></param>
        /// https://mathworld.wolfram.com/PerpendicularFoot.html
        /// </summary>
        /// <param name="point">The point from which the perpendicular on this line passes through</param>
        /// <returns>The perpendicular foot on this line</returns>
        [Pure]
        public Point3D PerpendicularFootTo(Point3D point)
        {
            var v = ThroughPoint.VectorTo(point);
            var alongVector = v.ProjectOn(Direction);
            return ThroughPoint + alongVector;
        }

        /// <summary>
        /// Returns the point at which this line intersects with the plane
        /// </summary>
        /// <param name="plane">A geometric plane.</param>
        /// <returns>A point of intersection if such an intersection exists; otherwise null.</returns>
        [Pure]
        public Point3D? IntersectionWith(Plane plane)
        {
            return plane.IntersectionWith(this);
        }

        /// <summary>
        /// Returns a value to indicate if a pair of lines are collinear
        /// </summary>
        /// <param name="other">The line to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>True if the lines are collinear; otherwise false.</returns>
        [Pure]
        public bool IsCollinear(Line other, double tolerance = float.Epsilon)
        {
            return Direction.IsParallelTo(other.Direction, tolerance);
        }

        /// <summary>
        /// Returns a value to indicate if a pair of lines are equal
        /// </summary>
        /// <param name="other">The line to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>True if the lines are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Line other, double tolerance)
        {
            return Direction.IsParallelTo(other.Direction, tolerance)
                   && DistanceTo(other.ThroughPoint) < tolerance;
        }

        /// <inheritdoc/>
        [Pure]
        public bool Equals(Line r) => Direction.IsParallelTo(r.Direction)
                                      && DistanceTo(r.ThroughPoint) == 0;

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) => obj is Line r && Equals(r);

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => HashCode.Combine(ThroughPoint, Direction);

        /// <inheritdoc/>
        [Pure]
        public override string ToString()
        {
            return ToString("G15", CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        [Pure]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(
                "ThroughPoint: {0}, Direction: {1}",
                ThroughPoint.ToString(format, formatProvider),
                Direction.ToString(format, formatProvider));
        }

        /// <inheritdoc/>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc/>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            this = new Line(
                Point3D.ReadFrom(e.SingleElement("ThroughPoint").CreateReader()),
                Direction.ReadFrom(e.SingleElement("Direction").CreateReader()));
        }

        /// <inheritdoc/>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("ThroughPoint", ThroughPoint);
            writer.WriteElement("Direction", Direction);
        }
    }
}
