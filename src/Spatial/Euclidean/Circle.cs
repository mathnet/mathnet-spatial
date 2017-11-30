using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a standard 2 dimentional circle
    /// </summary>
    public class Circle : IEquatable<Circle>
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
        /// Creates a Circle from three points which are not colinnear
        /// </summary>
        /// <param name="center">The location of the center</param>
        /// <param name="radius">The radius of the circle</param>
        public Circle(Point2D pointA, Point2D pointB, Point2D pointC)
        {
            StraightLine lineAB = new StraightLine(pointA, pointB);
            StraightLine lineBC = new StraightLine(pointB, pointA);
            var midpointAB = Point2D.MidPoint(pointA, pointB);
            var midpointBC = Point2D.MidPoint(pointB, pointC);
            var line1 = lineAB.PerpendicularAt(midpointAB);
            var line2 = lineBC.PerpendicularAt(midpointBC);
            var centerpoint = lineAB.Intersection(lineBC);
            if (!centerpoint.HasValue)
                throw new ArgumentException("Points cannot form a circle, are they colinnear?");
            Center = centerpoint.Value;
            Radius = Center.DistanceTo(pointA);
        }

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

        /// <summary>
        /// Returns the tanget at a point on the circle
        /// </summary>
        /// <param name="point">a point on the circle</param>
        /// <returns>A straightline tangent to the circle</returns>
        public StraightLine TangentAt(Point2D point)
        {
            StraightLine radii = new StraightLine(Center, point);
            return radii.PerpendicularAt(point);
        }

        /// <summary>
        /// Returns the normal to the circle passing through point p
        /// </summary>
        /// <param name="p">a point on the circle</param>
        /// <returns>a straightline normal to the circle</returns>
        public StraightLine NormalAt(Point2D p)
        {
            return new StraightLine(Center, p);
        }

        /// <summary>
        /// Returns a secant which passes through the points A and B
        /// </summary>
        /// <param name="pointA">A point on the circle</param>
        /// <param name="pointB">A point on the circle</param>
        /// <returns>A straightline</returns>
        public StraightLine SecantAt(Point2D pointA, Point2D pointB)
        {
            return new StraightLine(pointA, pointB);
        }

        /// <summary>
        /// Determines if two circle are equal to each other
        /// </summary>
        /// <param name="other">The other circle</param>
        /// <returns>true if the circles have the same radius and center</returns>
        public bool Equals(Circle other)
        {
            var circle = other as Circle;
            return (Radius == circle?.Radius && Center == circle?.Center);
        }

        /// <summary>
        /// Determines if two objects are equal to each other
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>True if the objects are the same</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Circle && Equals((Circle)obj);
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
