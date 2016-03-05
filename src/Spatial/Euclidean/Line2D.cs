using System;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// This structure represents a line between two points in 2-space.  It allows for operations such as 
    /// computing the length, direction, projections to, compairisons, and shifting by a vector.  
    /// </summary>
    public struct Line2D : IEquatable<Line2D>
    {
        private Vector2D _direction;
        private double _length;

        /// <summary>
        /// The starting point of the line
        /// </summary>
        public readonly Point2D StartPoint;

        /// <summary>
        /// The end point of the line
        /// </summary>
        public readonly Point2D EndPoint;

        /// <summary>
        /// A double precision number representing the distance between the startpoint and endpoint
        /// </summary>
        public double Length
        {
            get
            {
                if (this._length < 0)
                    ComputeLengthAndDirection();
                return this._length;
            }
        }

        /// <summary>
        /// A normalized Vector2D representing the direction from the startpoint to the endpoint
        /// </summary>
        public Vector2D Direction
        {
            get
            {
                if (this._length < 0)
                    ComputeLengthAndDirection();
                return this._direction;
            }
        }

        /// <summary>
        /// Constructor for the Line2D, throws an error if the startpoint is equal to the 
        /// endpoint.
        /// </summary>
        /// <param name="startPoint">the starting point of the line</param>
        /// <param name="endPoint">the ending point of the line</param>
        public Line2D(Point2D startPoint, Point2D endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;

            if (this.StartPoint == this.EndPoint)
            {
                throw new ArgumentException("The Line2D starting and ending points cannot be identical");
            }

            // Initialize the length and direction for lazy loading
            this._length = -1.0;
            this._direction = new Vector2D();
        }

        /// <summary>
        /// Compute and store the length and direction of the Line2D, used for lazy loading
        /// </summary>
        /// <returns></returns>
        private void ComputeLengthAndDirection()
        {
            var vectorBetween = this.StartPoint.VectorTo(this.EndPoint);
            this._length = vectorBetween.Length;
            this._direction = vectorBetween.Normalize();
        }

        /// <summary>
        /// Returns the shortest line between this line and a point.
        /// </summary>
        /// <param name="p">the point to create a line to</param>
        /// <param name="mustStartBetweenAndEnd">If false the startpoint can extend beyond the start and endpoint of the line</param>
        /// <returns></returns>
        public Line2D LineTo(Point2D p, bool mustStartBetweenAndEnd)
        {
            return new Line2D(this.ClosestPointTo(p, mustStartBetweenAndEnd), p);
        }

        /// <summary>
        /// Returns the closest point on the line to the given point.
        /// </summary>
        /// <param name="p">The point that the returned point is the closest point on the line to</param>
        /// <param name="mustBeOnSegment">If true the returned point is contained by the segment ends, otherwise it can be anywhere on the projected line</param>
        /// <returns></returns>
        public Point2D ClosestPointTo(Point2D p, bool mustBeOnSegment)
        {
            Vector2D v = this.StartPoint.VectorTo(p);
            double dotProduct = v.DotProduct(this.Direction);
            if (mustBeOnSegment)
            {
                if (dotProduct < 0)
                    dotProduct = 0;

                double l = this.Length;
                if (dotProduct > l)
                    dotProduct = l;
            }

            Vector2D alongVector = dotProduct * this.Direction;
            return this.StartPoint + alongVector;
        }

        /// <summary>
        /// Compute the intersection between two lines
        /// </summary>
        /// <param name="other">The other line to compute the intersection with</param>
        /// <returns>The point at the intersection of two lines, or null if the lines are parallelnu.</returns>
        public Point2D? IntersectWith(Line2D other)
        {
            // http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
            Point2D p = this.StartPoint;
            Point2D q = other.StartPoint;
            Vector2D r = this.StartPoint.VectorTo(this.EndPoint);
            Vector2D s = other.StartPoint.VectorTo(other.EndPoint);

            double t = (q - p).CrossProduct(s) / (r.CrossProduct(s));

            if (double.IsPositiveInfinity(t) || double.IsNegativeInfinity(t))
                return null;

            return p + t * r;
        }

        # region Operators 

        public static bool operator ==(Line2D left, Line2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Line2D left, Line2D right)
        {
            return !left.Equals(right);
        }

        public static Line2D operator +(Vector2D offset, Line2D line)
        {
            return new Line2D(line.StartPoint + offset, line.EndPoint + offset);
        }

        public static Line2D operator +(Line2D line, Vector2D offset)
        {
            return offset + line;
        }

        public static Line2D operator -(Line2D line, Vector2D offset)
        {
            return line + (-offset);
        }
        
        # endregion

        # region Serialization and Deserialization
        
        public override string ToString()
        {
            return string.Format("StartPoint: {0}, EndPoint: {1}", this.StartPoint, this.EndPoint);
        }

        public static Line2D Parse(string startPointString, string endPointString)
        {
            return new Line2D(Point2D.Parse(startPointString), Point2D.Parse(endPointString));
        }
        
        # endregion


        # region Equality and Hash Code

        public bool Equals(Line2D other)
        {
            return StartPoint.Equals(other.StartPoint) && EndPoint.Equals(other.EndPoint);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Line2D && Equals((Line2D)obj);
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
        # endregion
    }
}