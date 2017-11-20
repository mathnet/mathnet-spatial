using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathNet.Spatial.Euclidean.PlaneGeometry.Shapes
{
    /// <summary>
    /// Describes a standard 2 dimentional circle
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// The center point of the circle
        /// </summary>
        public Point2D Center { get; }

        /// <summary>
        /// The radius of the circle
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Creates a Circle of a given radius from a center point
        /// </summary>
        /// <param name="center">The location of the center</param>
        /// <param name="radius">The radius of the circle</param>
        public Circle(Point2D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Returns the circumference of the circle
        /// </summary>
        public double Circumference { get => 2 * Radius * Math.PI; }

        /// <summary>
        /// Returns the area of the circle
        /// </summary>
        public double Area { get => Radius * Radius * Math.PI; }
    }
}
