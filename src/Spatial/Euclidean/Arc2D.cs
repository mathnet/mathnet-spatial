using System;
using System.Diagnostics.Contracts;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a standard 2 dimensional arc
    /// </summary>
    public struct Arc2D : IEquatable<Arc2D>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Arc2D"/> struct.
        /// Creates an Arc of a given radius from a center point
        /// </summary>
        /// <param name="center">The location of the center</param>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="startAngle">The start angle of the arc</param>
        /// <param name="sweepAngle">The sweep angle of the arc</param>
        public Arc2D(Point2D center, double radius, double startAngle, double sweepAngle)
        {
            this.Center = center;
            this.Radius = radius;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
        }

        /// <summary>
        /// Gets the center point of the arc
        /// </summary>
        public Point2D Center { get; }

        /// <summary>
        /// Gets the radius of the arc
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets the start angle of the arc
        /// </summary>
        public Angle StartAngle { get; }

        /// <summary>
        /// Gets the sweep angle of the arc
        /// </summary>
        public Angle SweepAngle { get; }

        /// <summary>
        /// Gets the circumference of the arc
        /// </summary>
        [Pure]
        public double Circumference => this.Radius * this.SweepAngle * Math.PI / 180;

        /// <summary>
        /// Gets the diameter of the arc
        /// </summary>
        [Pure]
        public double Diameter => 2 * this.Radius;

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified arcs is equal.
        /// </summary>
        /// <param name="left">The first arc to compare.</param>
        /// <param name="right">The second arc to compare.</param>
        /// <returns>True if the arcs are the same; otherwise false.</returns>
        public static bool operator ==(Arc2D left, Arc2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two arcs for inequality.
        /// </summary>
        /// <param name="left">The left arc.</param>
        /// <param name="right">The right arc.</param>
        /// <returns>True if the arcs are not equal; otherwise, false.</returns>
        public static bool operator !=(Arc2D left, Arc2D right)
        {
            return !left.Equals(right);
        }

        /// <summary> Returns a arc fromt a start point, end point and center point </summary>
        /// <param name="start">The start point of the arc</param>
        /// <param name="end">The end point of the arc</param>
        /// <param name="center">The center point of the arc</param>
        /// <returns>A arc</returns>
        [Pure]
        public static Arc2D FromPoints(Point2D start, Point2D end, Point2D center)
        {
            // Calculate the radius of the arc
            var radius = (start - center).Length;
            // Calculate the start angle of the arc
            var startAngle = (start - center).AngleTo(Vector2D.XAxis);
            // Calculate the end angle of the arc
            var endAngle = (end - center).AngleTo(Vector2D.XAxis);
            // Calculate the sweep angle of the arc
            var sweepAngle = endAngle - startAngle;

            // If the sweep angle is greater than 180 degrees, then we need to change the direction of the arc
            if (sweepAngle > Math.PI)
            {
                sweepAngle -= 2 * Math.PI;
            }
            // If the sweep angle is less than -180 degrees, then we need to change the direction of the arc
            else if (sweepAngle < -Math.PI)
            {
                sweepAngle += 2 * Math.PI;
            }

            // Return the arc
            return new Arc2D(center, radius, startAngle, sweepAngle);
        }

        /// <summary> Returns a arc from center point, start point and angle </summary>
        /// <param name="center">The center point of the arc</param>
        /// <param name="startPoint">The start point of the arc</param>
        /// <param name="angle">The angle of the arc</param>
        /// <returns>A arc</returns>
        [Pure]
        public static Arc2D FromCenterPointStartPointAndAngle(Point2D center, Point2D startPoint, Angle angle)
        {
            // Create a Vector2D from startPoint to center
            var radius = (startPoint - center).Length;
            // Create a Vector2D from startPoint to center and get the angle to the XAxis
            var startAngle = (startPoint - center).AngleTo(Vector2D.XAxis);
            // Create a Vector2D from startPoint to center and get the angle to the XAxis
            var sweepAngle = angle;
            // Return a new Arc2D with the parameters
            return new Arc2D(center, radius, startAngle, sweepAngle);
        }

        /// <summary> Checks to determine if the arc is equal to another arc </summary>
        /// <param name="other">The other arc</param>
        /// <returns>True if the arcs are equal; otherwise false.</returns>
        [Pure]
        public bool Equals(Arc2D other, double tolerance = Precision.DoublePrecision * 2)
        {
            return
            this.Center.Equals(other.Center) &&
            Math.Abs(other.Radius - this.Radius) < tolerance &&
            this.StartAngle.Equals(other.StartAngle) &&
            this.SweepAngle.Equals(other.SweepAngle);
        }

        /// <summary> Checks to determine if the arc is equal to another arc </summary>
        /// <param name="obj">The other arc</param>
        [Pure]
        public override bool Equals(object obj)
        {
            if (!(obj is Arc2D arc)) return false;
            return this.Equals(arc);
        }
    }
}