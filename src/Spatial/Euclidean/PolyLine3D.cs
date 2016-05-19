using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathNet.Spatial.Euclidean
{

    /// <summary>
    /// A PolyLine is an ordered series of line segments in space represented as list of connected Point3Ds.
    /// </summary>
    public class PolyLine3D : IEnumerable<Point3D>
    {
        /// <summary>
        /// An integer representing the number of Point3D objects in the polyline
        /// </summary>
        public int Count => this._points.Count;

        /// <summary>
        /// The length of the polyline, computed as the sum of the lengths of every segment
        /// </summary>
        public double Length => this.GetPolyLineLength();

        /// <summary>
        /// Indicates whether or not the collection of points in the polyline are planar within
        /// the floating point tolerance
        /// </summary>
        public bool IsPlanar
        {
            get { throw new NotImplementedException();}
        }

        private List<Point3D> _points;

        public PolyLine3D(IEnumerable<Point3D> points)
        {
            this._points = new List<Point3D>(points);
        }

        // Operators
        public Point3D this[int key] => this._points[key];

        // Methods

        /// <summary>
        /// Computes the length of the polyline by summing the lengths of the individual segments
        /// </summary>
        /// <returns></returns>
        private double GetPolyLineLength()
        {
            double length = 0;
            for (int i = 0; i < this._points.Count - 1; ++i)
                length += this[i].DistanceTo(this[i + 1]);
            return length;
        }

        /// <summary>
        /// Get the point at a fractional distance along the curve.  For instance, fraction=0.5 will return
        /// the point halfway along the length of the polyline.
        /// </summary>
        /// <param name="fraction">The fractional length at which to compute the point</param>
        /// <returns></returns>
        public Point3D GetPointAtFractionAlongCurve(double fraction)
        {
            if (fraction > 1 || fraction < 0)
                throw new ArgumentException("fraction must be between 0 and 1");
            return this.GetPointAtLengthFromStart(fraction * this.Length);
        }

        /// <summary>
        /// Get the point at a specified distance along the curve.  A negative argument will return the first point,
        /// an argument greater than the length of the curve will return the last point.
        /// </summary>
        /// <param name="lengthFromStart">The distance from the first point along the curve at which to return a point</param>
        /// <returns></returns>
        public Point3D GetPointAtLengthFromStart(double lengthFromStart)
        {
            double length = this.Length;
            if (lengthFromStart >= length)
                return this.Last();
            if (lengthFromStart <= 0)
                return this.First();

            double cumulativeLength = 0;
            int i = 0;
            while (true)
            {
                double nextLength = cumulativeLength + this[i].DistanceTo(this[i + 1]);
                if (cumulativeLength <= lengthFromStart && nextLength > lengthFromStart)
                {
                    double leftover = lengthFromStart - cumulativeLength;
                    var direction = this[i].VectorTo(this[i + 1]).Normalize();
                    return this[i] + (leftover * direction);
                }
                cumulativeLength = nextLength;
                i++;
            }
        }

        /// <summary>
        /// Returns the closest point on the polyline to the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point3D ClosestPointTo(Point3D p)
        {
            var minError = double.MaxValue;
            var closest = new Point3D();

            for (int i = 0; i < this.Count - 1; i++)
            {
                var segment = new Line3D(this[i], this[i + 1]);
                var projected = segment.ClosestPointTo(p, true);
                double error = p.DistanceTo(projected);
                if (error < minError)
                {
                    minError = error;
                    closest = projected;
                }
            }
            return closest;
        }

        public bool IsPlanarWithinTol(double tolerance)
        {
            throw new NotImplementedException();
        }

        // IEnumerable<Point3D>
        public IEnumerator<Point3D> GetEnumerator()
        {
            return this._points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}