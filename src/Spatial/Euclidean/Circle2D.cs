namespace MathNet.Spatial.Euclidean
{
    using System;

    /// <summary>
    /// Describes a standard 2 dimensional circle
    /// </summary>
    public struct Circle2D : IEquatable<Circle2D>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Circle2D"/> struct.
        /// Creates a Circle of a given radius from a center point
        /// </summary>
        /// <param name="center">The location of the center</param>
        /// <param name="radius">The radius of the circle</param>
        public Circle2D(Point2D center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <summary>
        /// Gets the center point of the circle
        /// </summary>
        public Point2D Center { get; }

        /// <summary>
        /// Gets the radius of the circle
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets the circumference of the circle
        /// </summary>
        public double Circumference => 2 * this.Radius * Math.PI;

        /// <summary>
        /// Gets the diameter of the circle
        /// </summary>
        public double Diameter => 2 * this.Radius;

        /// <summary>
        /// Gets the area of the circle
        /// </summary>
        public double Area => this.Radius * this.Radius * Math.PI;

        public static bool operator ==(Circle2D left, Circle2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Circle2D left, Circle2D right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(Circle2D other)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return this.Radius == other.Radius && this.Center == other.Center;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Circle2D d && this.Equals(d);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Center.GetHashCode() * 397) ^ this.Radius.GetHashCode();
            }
        }
    }
}
