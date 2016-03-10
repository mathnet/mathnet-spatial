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
        private List<Point3D> _points;

        /// <summary>
        /// An integer representing the number of Point3D objects in the polyline
        /// </summary>
        public int Count
        {
            get { return this._points.Count; }
        }

        /// <summary>
        /// The length of the polyline, computed as the sum of the lengths of every segment
        /// </summary>
        public double Length
        {
            get { return this.GetPolyLineLength(); }
        }

        /// <summary>
        /// Indicates whether or not the collection of points in the polyline are planar within
        /// the floating point tolerance
        /// </summary>
        public bool IsPlanar
        {
            get { throw new NotImplementedException();}
        }

        // Constructors 
        public PolyLine3D()
            : this(Enumerable.Empty<Point3D>())
        {
        }

        public PolyLine3D(IEnumerable<Point3D> points)
        {
            this._points = new List<Point3D>(points);
        }

        // Operators
        public Point3D this[int key]
        {
            get { return this._points[key]; }
            set { this._points[key] = value; }
        }

        // Methods
        private double GetPolyLineLength()
        {
            double length = 0;
            for (int i = 0; i < this._points.Count - 1; ++i)
                length += this[i].DistanceTo(this[i + 1]);
            return length;
        }

        public bool IsPlanarWithinTol(double tolerance)
        {
            throw new NotImplementedException();
        }


        // IEnumerable<Point3D>
        public IEnumerator<Point3D> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}