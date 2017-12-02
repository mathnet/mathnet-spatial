﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Represents a line conforming to the equation y = mx + b where m is the gradient and b is the y-intercept
    /// </summary>
    public struct StraightLine2D : ILine2D, IEquatable<StraightLine2D>
    {
        private readonly double yoffset;
        private readonly double xoffset;

        /// <summary>
        /// The gradient of the line
        /// </summary>
        public double Gradient { get; }
       
        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b where point is a point on the line and gradient is the slope of the line
        /// </summary>
        /// <param name="point">A point on the line</param>
        /// <param name="gradient">The gradient of the line</param>
        public static StraightLine2D Create(Point2D point, double gradient)
        {
            return new StraightLine2D(gradient, point.X, point.Y);
        }

        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b which passes through points p1 and p2
        /// </summary>
        /// <param name="p1">A point on the line</param>
        /// <param name="p2">A point on the line</param>
        public static StraightLine2D Create(Point2D p1, Point2D p2)
        {
            if (p1 == p2)
            {
                throw new ArgumentException("A line cannot be formed from two identical points");
            }

            double gradient = (p2.Y - p1.Y) / (p2.X - p1.X);
            return new StraightLine2D(gradient, p1.X, p1.Y);
        }

        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b which passes through the points provided
        /// </summary>
        /// <param name="points">The points though which the line should pass</param>
        /// <exception cref="ArgumentException">Thrown when less than 2 points are provided</exception>
        /// <exception cref="ArgumentException">Thrown when points are not collinear</exception>
        public static StraightLine2D CreateFromCollinear(IEnumerable<Point2D> points)
        {
            return CreateFromCollinear(points.ToArray());
        }

        /// <summary>
        /// Creates a Line conforming to the equation y = mx + b which passes through the points provided
        /// </summary>
        /// <param name="points">The points though which the line should pass</param>
        /// <exception cref="ArgumentException">Thrown when less than 2 points are provided</exception>
        /// <exception cref="ArgumentException">Thrown when points are not collinear</exception>
        public static StraightLine2D CreateFromCollinear(Point2D[] points)
        {
            if (points.Length < 2)
            {
                throw new ArgumentException("A line must be constructed from at least two points");
            }

            double gradient = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
            StraightLine2D line = new StraightLine2D(gradient, points[0].X, points[0].Y);
            for (int i = 2; i < points.Length; i++)
            {
                if (!line.IsThrough(points[i]))
                {
                    throw new ArgumentException("The points provided are not collinear");
                }
            }

            return line;
        }

        /// <summary>
        /// Constructs a straightline
        /// </summary>
        private StraightLine2D(double gradient, double x, double y)
        {
            Gradient = gradient;
            if (double.IsInfinity(gradient))
            {
                this.yoffset = double.NaN; // special case vertical line has no y-intercept
                this.xoffset = x;
            }
            else
            {
                this.yoffset = y - (gradient * x);
                this.xoffset = double.NaN;
            }
        }

        /// <summary>
        /// Determines if the line is horizontal
        /// </summary>
        /// <returns>True if the line is horizontal</returns>
        public bool IsHorizontal => Gradient == 0;

        /// <summary>
        /// Determines if the line is vertical
        /// </summary>
        /// <returns>returns true if the line is vertical</returns>
        public bool IsVertical => double.IsInfinity(Gradient);

        /// <summary>
        /// Tests if the other line is parallel to this one
        /// </summary>
        /// <returns>Returns true if lines are parallel</returns>
        public bool IsParallel<T>(T otherLine) where T : ILine2D
        {
            if (otherLine is StraightLine2D)
            {
                IsParallel(otherLine);
            }

            return false;
        }

        /// <summary>
        /// Tests if the other line is parallel to this one
        /// </summary>
        /// <returns>Returns true if lines are parallel</returns>
        public bool IsParallel(StraightLine2D otherLine)
        {
            return otherLine.Gradient == Gradient;
        }

        /// <summary>
        /// Finds the midpoint of two points along the current line
        /// </summary>
        /// <param name="pointA">a point on the line</param>
        /// <param name="pointB">a point on the line</param>
        /// <returns>A point midway between the two provided points</returns>
        public Point2D MidPoint(Point2D pointA, Point2D pointB)
        {
            if (IsThrough(pointA) && IsThrough(pointB))
            {
                return new Point2D((pointA.X + pointB.X) / 2, (pointA.Y + pointB.Y) / 2);
            }

            throw new ArgumentException("Provided points are not along the line");
        }

        /// <summary>
        /// Returns a line perpendicular to this line going through point p
        /// </summary>
        /// <param name="p">The point on the line which it passes through</param>
        /// <returns>A perpendicular straightline</returns>
        public StraightLine2D PerpendicularAt(Point2D p)
        {
            return StraightLine2D.Create(p, -1 / Gradient);
        }

        /// <summary>
        /// Find the intersection between two lines
        /// </summary>
        /// <param name="line">The line to intersect with</param>
        /// <returns>The point of intersection or null if the lines are parallel</returns>
        public Point2D? Intersection(StraightLine2D line)
        {
            if (IsParallel(line))
            {
                return null;
            }

            if (double.IsInfinity(Gradient))
            {
                return new Point2D(xoffset, (line.Gradient * xoffset) + line.yoffset);
            }
            else if (double.IsInfinity(line.Gradient))
            {
                return new Point2D(line.xoffset, (Gradient * line.xoffset) + yoffset);
            }

            var denominator = Gradient - line.Gradient;

            var x = (line.yoffset - yoffset) / denominator;
            var y = (Gradient * x) + yoffset;
            return new Point2D(x, y);
        }

        /// <summary>
        /// Returns the closest point on the line to the given point.
        /// </summary>
        /// <param name="p">The point that the returned point is the closest point on the line to</param>
        /// <returns></returns>
        public Point2D ClosestPointTo(Point2D p)
        {
            StraightLine2D line = StraightLine2D.Create(p, Gradient);
            var perpline = line.PerpendicularAt(p);
            return this.Intersection(perpline).Value;
        }

        /// <summary>
        /// Determines if a given point is on this line
        /// </summary>
        /// <param name="point">The point to be checked</param>
        /// <returns>True if the point is on this line</returns>
        public bool IsThrough(Point2D point)
        {
            return IsThrough(point.X, point.Y);
        }

        /// <summary>
        /// Determines if a set of coordinates (x,y) is on this line
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>True if the point is on the line</returns>
        public bool IsThrough(double x, double y)
        {
            if (double.IsInfinity(Gradient))
            {
                return x == xoffset;
            }
            else
            {
                return y == (Gradient * x) + yoffset;
            }
        }

        /// <summary>
        /// Returns the list of points which lie on a vertical line at coordinate x
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <returns>A list of points</returns>
        public Point2D[] XIntercept(double x = 0)
        {
            return new Point2D[] { new Point2D(x, (Gradient * x) + yoffset) };
        }

        /// <summary>
        /// Returns the list of points which lie on a horizontal line at coordinate y
        /// </summary>
        /// <param name="y">the y coordinate</param>
        /// <returns>A list of points</returns>
        public Point2D[] YIntercept(double y = 0)
        {
            return new Point2D[] { new Point2D((y - yoffset) / Gradient, y) };
        }

        public static bool operator ==(StraightLine2D left, StraightLine2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StraightLine2D left, StraightLine2D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines if two lines are equal to each other
        /// </summary>
        /// <param name="other">The other line</param>
        /// <returns>true if the lines have the same equation</returns>
        public bool Equals(ILine2D other)
        {
            if (other is StraightLine2D)
            {
                return Equals((StraightLine2D)other);
            }

            return false;
        }

        /// <summary>
        /// Determines if two lines are equal to each other
        /// </summary>
        /// <param name="other">The other line</param>
        /// <returns>true if the lines have the same equation</returns>
        public bool Equals(StraightLine2D other)
        {
            return Gradient == other.Gradient && yoffset == other.yoffset;
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

            return obj is StraightLine2D && Equals((StraightLine2D)obj);
        }

        /// <summary>
        /// Returns the hashocde for a Straightline
        /// </summary>
        /// <returns>A hashcode</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Gradient.GetHashCode() * 397) ^ (yoffset.GetHashCode() * 101) ^ xoffset.GetHashCode();
            }
        }
    }
}