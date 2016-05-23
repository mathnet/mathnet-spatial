using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Numerics;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A line between two points
    /// </summary>
    [Serializable]
    public struct Line3D : IEquatable<Line3D>, IXmlSerializable
    {
        /// <summary>
        /// The startpoint of the line
        /// </summary>
        public readonly Point3D StartPoint;

        /// <summary>
        /// The endpoint of the line
        /// </summary>
        public readonly Point3D EndPoint;
        private double _length;
        private UnitVector3D _direction;

        /// <summary>
        /// Throws if StartPoint == EndPoint
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public Line3D(Point3D startPoint, Point3D endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            if (this.StartPoint == this.EndPoint)
            {
                throw new ArgumentException("StartPoint == EndPoint");
            }

            this._length = -1.0;
            this._direction = new UnitVector3D();
        }

        /// <summary>
        /// Distance from startpoint to endpoint, the length of the line
        /// </summary>
        public double Length
        {
            get
            {
                if (this._length < 0)
                {
                    var vectorTo = this.StartPoint.VectorTo(this.EndPoint);
                    this._length = vectorTo.Length;
                    if (this._length > 0)
                    {
                        this._direction = vectorTo.Normalize();
                    }
                }

                return this._length;
            }
        }

        /// <summary>
        /// The direction from the startpoint to the endpoint
        /// </summary>
        public UnitVector3D Direction
        {
            get
            {
                if (this._length < 0)
                {
                    this._length = this.Length; // Side effect hack
                }

                return this._direction;
            }
        }

        /// <summary>
        /// Creates a Line from its string representation
        /// </summary>
        /// <param name="startPoint">The string representation of the startpoint</param>
        /// <param name="endPoint">The string representation of the endpoint</param>
        /// <returns></returns>
        public static Line3D Parse(string startPoint, string endPoint)
        {
            return new Line3D(Point3D.Parse(startPoint), Point3D.Parse(endPoint));
        }

        public static bool operator ==(Line3D left, Line3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Line3D left, Line3D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns the shortest line to a point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="mustStartBetweenStartAndEnd">If false the startpoint can be on the line extending beyond the start and endpoint of the line</param>
        /// <returns></returns>
        public Line3D LineTo(Point3D p, bool mustStartBetweenStartAndEnd)
        {
            return new Line3D(this.ClosestPointTo(p, mustStartBetweenStartAndEnd), p);
        }

        /// <summary>
        /// Returns the closest point on the line to the given point.
        /// </summary>
        /// <param name="p">The point which the returned point is the closest point on the line to</param>
        /// <param name="mustBeOnSegment">If true the returned point is contained by the segment ends, otherwise it can be anywhere on the projected line.</param>
        /// <returns></returns>
        public Point3D ClosestPointTo(Point3D p, bool mustBeOnSegment)
        {
            Vector3D v = (p - this.StartPoint);
            double dotProduct = v.DotProduct(this.Direction);
            if (mustBeOnSegment)
            {
                if (dotProduct < 0)
                    dotProduct = 0;

                if (dotProduct > this.Length)
                    dotProduct = this.Length;
            }

            Vector3D alongVector = dotProduct*this.Direction;
            return this.StartPoint + alongVector;
        }
        

        /// <summary>
        /// The line projected on a plane
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public Line3D ProjectOn(Plane plane)
        {
            return plane.Project(this);
        }

        /// <summary>
        /// Find the intersection between the line and a plane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Point3D? IntersectionWith(Plane plane, double tolerance = double.Epsilon)
        {
            return plane.IntersectionWith(this, tolerance);
        }

        /// <summary>
        /// Checks to determine whether or not two lines are parallel to each other, using the dot product within 
        /// the double precision specified in the MathNet.Numerics package.
        /// </summary>
        /// <param name="other">The other line to check this one against</param>
        /// <returns>True if the lines are parallel, false if they are not</returns>
        public bool IsParallelTo(Line3D other)
        {
            return this.Direction.IsParallelTo(other.Direction, Precision.DoublePrecision * 2);
        }

        /// <summary>
        /// Checks to determine whether or not two lines are parallel to each other within a specified angle tolerance
        /// </summary>
        /// <param name="other">The other line to check this one against</param>
        /// <param name="angleTolerance">If the angle between line directions is less than this value, the method returns true</param>
        /// <returns>True if the lines are parallel within the angle tolerance, false if they are not</returns>
        public bool IsParallelTo(Line3D other, Angle angleTolerance)
        {
            return this.Direction.IsParallelTo(other.Direction, angleTolerance);
        }

        /// <summary>
        /// Computes the pair of points which represent the closest distance between this Line3D and another Line3D, with the first
        /// point being the point on this Line3D, and the second point being the corresponding point on the other Line3D.  If the lines
        /// intersect the points will be identical, if the lines are parallel the first point will be the start point of this line.
        /// </summary>
        /// <param name="other">line to compute the closest points with</param>
        /// <returns>A tuple of two points representing the endpoints of the shortest distance between the two lines</returns>
        public Tuple<Point3D, Point3D> ClosestPointsBetween(Line3D other)
        {
            if (this.IsParallelTo(other))
            {
                return Tuple.Create(this.StartPoint, other.ClosestPointTo(this.StartPoint, false));
            }

            // http://geomalgorithms.com/a07-_distance.html
            var P0 = this.StartPoint;
            var u = this.Direction;
            var Q0 = other.StartPoint;
            var v = other.Direction;

            var w0 = P0 - Q0;
            var a = u.DotProduct(u);
            var b = u.DotProduct(v);
            var c = v.DotProduct(v);
            var d = u.DotProduct(w0);
            var e = v.DotProduct(w0);

            double sc = (b*e - c*d)/(a*c - b*b);
            double tc = (a*e - b*d)/(a*c - b*b);

            return Tuple.Create(P0 + sc*u, Q0 + tc*v);
        }

        /// <summary>
        /// Computes the pair of points which represents the closest distance between this Line3D and another Line3D, with the option
        /// of treating the lines as segments bounded by their start and end points.
        /// </summary>
        /// <param name="other">line to compute the closest points with</param>
        /// <param name="mustBeOnSegments">if true, the lines are treated as segments bounded by the start and end point</param>
        /// <returns>A tuple of two points representing the endpoints of the shortest distance between the two lines or segments</returns>
        public Tuple<Point3D, Point3D> ClosestPointsBetween(Line3D other, bool mustBeOnSegments)
        {
            // If the segments are parallel and the answer must be on the segments, we can skip directly to the ending
            // algorithm where the endpoints are projected onto the opposite segment and the smallest distance is 
            // taken.  Otherwise we must first check if the infinite length line solution is valid.
            if (!this.IsParallelTo(other) || !mustBeOnSegments)  // If the lines aren't parallel OR it doesn't have to be constrained to the segments
            {
                // Compute the unbounded result, and if mustBeOnSegments is false we can directly return the results
                // since this is the same as calling the other method.
                var result = this.ClosestPointsBetween(other);
                if (!mustBeOnSegments)
                    return result;

                // A point that is known to be colinear with the line start and end points is on the segment if
                // its distance to both endpoints is less than the segment length.  If both projected points lie 
                // within their segment, we can directly return the result.
                if (result.Item1.DistanceTo(this.StartPoint) <= this.Length &&
                    result.Item1.DistanceTo(this.EndPoint) <= this.Length &&
                    result.Item2.DistanceTo(other.StartPoint) <= other.Length &&
                    result.Item2.DistanceTo(other.EndPoint) <= other.Length)
                {
                    return result;
                }
            }

            // If we got here, we know that either we're doing a bounded distance on two parallel segments or one 
            // of the two closest span points is outside of the segment of the line it was projected on.  In either
            // case we project each of the four endpoints onto the opposite segments and select the one with the 
            // smallest projected distance.
            Point3D checkPoint;
            Tuple<Point3D, Point3D> closestPair;
            double distance;

            checkPoint = other.ClosestPointTo(this.StartPoint, true);
            distance = checkPoint.DistanceTo(this.StartPoint);
            closestPair = Tuple.Create(this.StartPoint, checkPoint);
            double minDistance = distance;
            

            checkPoint = other.ClosestPointTo(this.EndPoint, true);
            distance = checkPoint.DistanceTo(this.EndPoint);
            if (distance < minDistance)
            {
                closestPair = Tuple.Create(this.EndPoint, checkPoint);
                minDistance = distance;
            }

            checkPoint = this.ClosestPointTo(other.StartPoint, true);
            distance = checkPoint.DistanceTo(other.StartPoint);
            if (distance < minDistance)
            {
                closestPair = Tuple.Create(checkPoint, other.StartPoint);
                minDistance = distance;
            }

            checkPoint = this.ClosestPointTo(other.EndPoint, true);
            distance = checkPoint.DistanceTo(other.EndPoint);
            if (distance < minDistance)
            {
                closestPair = Tuple.Create(checkPoint, other.EndPoint);
            }

            return closestPair;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Line3D other)
        {
            return this.StartPoint.Equals(other.StartPoint) && this.EndPoint.Equals(other.EndPoint);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Line3D && this.Equals((Line3D)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.StartPoint.GetHashCode();
                hashCode = (hashCode * 397) ^ this.EndPoint.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("StartPoint: {0}, EndPoint: {1}", this.StartPoint, this.EndPoint);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }
        
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            var startPoint = Point3D.ReadFrom(e.SingleElement("StartPoint").CreateReader());
            XmlExt.SetReadonlyField(ref this, l => l.StartPoint, startPoint);
            var endPoint = Point3D.ReadFrom(e.SingleElement("EndPoint").CreateReader());
            XmlExt.SetReadonlyField(ref this, l => l.EndPoint, endPoint);
        }
        
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElement("StartPoint", this.StartPoint);
            writer.WriteElement("EndPoint", this.EndPoint);
        }
    }
}
