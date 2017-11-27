using System;
using MathNet.Numerics;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// This represents a line segment between two points.
    /// </summary>
    public class LineSegment<T> : IEquatable<LineSegment<T>> where T : ILine
    {

        private T line;

        /// <summary>
        /// The starting point of the line
        /// </summary>
        public readonly Point2D StartPoint;

        /// <summary>
        /// The end point of the line
        /// </summary>
        public readonly Point2D EndPoint;

        public LineSegment(T line, Point2D startPoint, Point2D endPoint)
        {
            this.line = line;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public bool IsParallel(LineSegment<T> otherLine)
        {
            return IsParallel(otherLine.line);
        }

        public bool IsParallel(ILine otherLine)
        {
            return this.line.IsParallel(otherLine);
        }

        #region Operators 

        public static bool operator ==(LineSegment<T> left, LineSegment<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LineSegment<T> left, LineSegment<T> right)
        {
            return !left.Equals(right);
        }
       
        # endregion

        # region Equality and Hash Code

        /// <summary>
        /// Checks if the line segements are equal
        /// </summary>
        /// <param name="other">The line segment to be checked</param>
        /// <returns>True if the lineSegements are equal</returns>
        public bool Equals(LineSegment<T> other)
        {
            return StartPoint.Equals(other.StartPoint) && EndPoint.Equals(other.EndPoint);
        }

        /// <summary>
        /// Checks if the objects are equal
        /// </summary>
        /// <param name="obj">The object to be checked</param>
        /// <returns>True if the object is a LineSegment and it is equal to this one</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LineSegment<T> && Equals((LineSegment<T>)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object"/></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (StartPoint.GetHashCode() * 397) ^ EndPoint.GetHashCode();
            }
        }

        #endregion
    }
}
