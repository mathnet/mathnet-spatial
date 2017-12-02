using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a standard 2 dimentional circle
    /// </summary>
    public struct Circle2D : IEquatable<Circle2D>
    {
        /// <summary>
        /// Creates a Circle of a given radius from a center point
        /// </summary>
        /// <param name="center">The location of the center</param>
        /// <param name="radius">The radius of the circle</param>
        public Circle2D(Point2D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// The center point of the circle
        /// </summary>
        public Point2D Center { get; }

        /// <summary>
        /// The radius of the circle
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Returns the circumference of the circle
        /// </summary>
        public double Circumference => 2 * Radius * Math.PI;

        /// <summary>
        /// Returns the diameter of the circle
        /// </summary>
        public double Diameter => 2 * Radius;

        /// <summary>
        /// Returns the area of the circle
        /// </summary>
        public double Area => Radius * Radius * Math.PI;

        public static bool operator ==(Circle2D left, Circle2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Circle2D left, Circle2D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines if two circle are equal to each other
        /// </summary>
        /// <param name="other">The other circle</param>
        /// <returns>true if the circles have the same radius and center</returns>
        public bool Equals(Circle2D other)
        {
            return Radius == other.Radius && Center == other.Center;
        }

        /// <summary>
        /// Determines if two objects are equal to each other
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>True if the objects are the same</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Circle2D && Equals((Circle2D)obj);
        }

        /// <summary>
        /// Returns the hashocde for a Circle
        /// </summary>
        /// <returns>A hashcode</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Center.GetHashCode() * 397) ^ Radius.GetHashCode();
            }
        }
    }
}
