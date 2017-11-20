using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathNet.Spatial.Euclidean.PlaneGeometry.Lines
{
    /// <summary>
    /// Represents a line conforming to the equation y = mx + b where m is the gradient and b is the y-intercept
    /// </summary>
    public class Line
    {
        /// <summary>
        /// The y-intercept of the line
        /// </summary>
        public Point2D YIntercept { get; }

        /// <summary>
        /// The gradient of the line
        /// </summary>
        public double Gradient { get; }

        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b where point is a point on the line and gradient is the slope of the line
        /// </summary>
        /// <param name="point">A point on the line</param>
        /// <param name="gradient">The gradient of the line</param>
        public Line(Point2D point, double gradient)
        {
            Gradient = gradient;
            YIntercept = new Point2D(0.0, point.Y - (Gradient * point.X));
        }

        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b which passes through points p1 and p2
        /// </summary>
        /// <param name="p1">A point on the line</param>
        /// <param name="p2">A point on the line</param>
        public Line(Point2D p1, Point2D p2)
        {
            if (p1 == p2)
                throw new ArgumentException("A line cannot be formed from two identical points");
            Gradient = (p2.Y - p1.Y) / (p2.X - p1.X);
            if (double.IsInfinity(Gradient))
                YIntercept = new Point2D(0.0, double.NaN); // special case vertical line has no y-intercept
            else
                YIntercept = new Point2D(0.0, p1.Y - (Gradient * p1.X));
        }

        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b which passes through the points provided
        /// </summary>
        /// <param name="points">The points though which the line should pass</param>
        /// <exception cref="ArgumentException">Thrown when less than 2 points are provided</exception>
        /// <exception cref="ArgumentException">Thrown when points are not collinear</exception>
        public Line(IEnumerable<Point2D> points) : this (points.ToArray())
        {
        }

        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b which passes through the points provided
        /// </summary>
        /// <param name="points">The points though which the line should pass</param>
        /// <exception cref="ArgumentException">Thrown when less than 2 points are provided</exception>
        /// <exception cref="ArgumentException">Thrown when points are not collinear</exception>
        public Line(Point2D[] points)
        {
            if (points.Length < 2)
                throw new ArgumentException("A line must be constructed from at least two points");
            Gradient = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
            YIntercept = new Point2D(0.0, points[0].Y - (Gradient * points[0].X));
            for (int i = 2; i < points.Length; i++)
            {
                if (!IsOnLine(points[i]))
                {
                    throw new ArgumentException("The points provided are not collinear");
                }
            }
        }

        /// <summary>
        /// Tests if the other line is parallel to this one
        /// </summary>
        /// <returns>Returns true if lines are parallel</returns>
        public bool IsParallel(Line otherLine)
        {
            return (this.Gradient == otherLine.Gradient);
        }

        /// <summary>
        /// Determines if a given point is on this line
        /// </summary>
        /// <param name="point">The point to be checked</param>
        /// <returns>True if the point is on this line</returns>
        public bool IsOnLine(Point2D point)
        {
            return IsOnLine(point.X, point.Y);
        }

        /// <summary>
        /// Determines if a set of coordinates (x,y) is on this line
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>True if the point is on the line</returns>
        public bool IsOnLine(double x, double y)
        {
            return (y == (Gradient * x) + YIntercept.Y);
        }
    }
}
