using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using MathNet.Spatial.Internals;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A PolyLine is an ordered series of line segments in space represented as list of connected Point3Ds.
    /// </summary>
    [Serializable]
    public class PolyLine3D : IEquatable<PolyLine3D>, IXmlSerializable
    {
        /// <summary>
        /// An internal list of points
        /// </summary>
        private ReadOnlyCollection<Point3D> _points;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLine3D"/> class.
        /// Creates a PolyLine3D from a pre-existing IEnumerable of Point3Ds
        /// </summary>
        /// <param name="points">A list of points.</param>
        public PolyLine3D(IEnumerable<Point3D> points)
        {
            _points = new List<Point3D>(points).AsReadOnly();
        }

        /// <summary>
        /// Used only internally for XML deserialization
        /// </summary>
        internal PolyLine3D()
        {
            _points = new List<Point3D>().AsReadOnly();
        }

        /// <summary>
        /// Gets the number of vertices in the polyline.
        /// </summary>
        public int VertexCount => _points.Count;

        /// <summary>
        /// Gets the length of the polyline, computed as the sum of the lengths of every segment
        /// </summary>
        public double Length => GetPolyLineLength();

        /// <summary>
        /// Gets a list of vertices
        /// </summary>
        public IReadOnlyList<Point3D> Vertices => _points;

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified lines is equal.
        /// </summary>
        /// <param name="left">The first line to compare</param>
        /// <param name="right">The second line to compare</param>
        /// <returns>True if the lines are the same; otherwise false.</returns>
        public static bool operator ==(PolyLine3D left, PolyLine3D right)
        {
            return left?.Equals(right) == true;
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified lines is not equal.
        /// </summary>
        /// <param name="left">The first line to compare</param>
        /// <param name="right">The second line to compare</param>
        /// <returns>True if the lines are different; otherwise false.</returns>
        public static bool operator !=(PolyLine3D left, PolyLine3D right)
        {
            return left?.Equals(right) != true;
        }

        /// <summary>
        /// Get the point at a fractional distance along the curve.  For instance, fraction=0.5 will return
        /// the point halfway along the length of the polyline.
        /// </summary>
        /// <param name="fraction">The fractional length at which to compute the point</param>
        /// <returns>A point a fraction of the way along the line.</returns>
        public Point3D GetPointAtFractionAlongCurve(double fraction)
        {
            if (fraction > 1 || fraction < 0)
            {
                throw new ArgumentException("fraction must be between 0 and 1");
            }

            return GetPointAtLengthFromStart(fraction * Length);
        }

        /// <summary>
        /// Get the point at a specified distance along the curve.  A negative argument will return the first point,
        /// an argument greater than the length of the curve will return the last point.
        /// </summary>
        /// <param name="lengthFromStart">The distance from the first point along the curve at which to return a point</param>
        /// <returns>A point which is the specified distance along the line</returns>
        public Point3D GetPointAtLengthFromStart(double lengthFromStart)
        {
            var length = Length;
            if (lengthFromStart >= length)
            {
                return _points.Last();
            }

            if (lengthFromStart <= 0)
            {
                return _points.First();
            }

            double cumulativeLength = 0;
            var i = 0;
            while (true)
            {
                var nextLength = cumulativeLength + _points[i].DistanceTo(_points[i + 1]);
                if (cumulativeLength <= lengthFromStart && nextLength > lengthFromStart)
                {
                    var leftover = lengthFromStart - cumulativeLength;
                    var direction = _points[i].VectorTo(_points[i + 1]).Normalize();
                    return _points[i] + (leftover * direction);
                }

                cumulativeLength = nextLength;
                i++;
            }
        }

        /// <summary>
        /// Returns the closest point on the polyline to the given point.
        /// </summary>
        /// <param name="p">A point</param>
        /// <returns>A point which is the closest to the given point but still on the line.</returns>
        public Point3D ClosestPointTo(Point3D p)
        {
            var minError = double.MaxValue;
            var closest = default(Point3D);

            for (var i = 0; i < VertexCount - 1; i++)
            {
                var segment = new LineSegment(_points[i], _points[i + 1]);
                var projected = segment.ClosestPointTo(p);
                var error = p.DistanceTo(projected);
                if (error < minError)
                {
                    minError = error;
                    closest = projected;
                }
            }

            return closest;
        }

        /// <summary>
        /// Returns a value to indicate if a pair of polylines are equal
        /// </summary>
        /// <param name="other">The polyline to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the polylines are equal; otherwise false</returns>
        [Pure]
        public bool Equals(PolyLine3D other, double tolerance)
        {
            if (VertexCount != other?.VertexCount)
            {
                return false;
            }

            for (var i = 0; i < _points.Count; i++)
            {
                if (!_points[i].Equals(other._points[i], tolerance))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(PolyLine3D other)
        {
            if (VertexCount != other?.VertexCount)
            {
                return false;
            }

            for (var i = 0; i < _points.Count; i++)
            {
                if (!_points[i].Equals(other._points[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj)
        {
            return obj is PolyLine3D polyLine3D &&
                   Equals(polyLine3D);
        }

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode()
        {
            return HashCode.CombineMany(_points);
        }

        /// <summary>
        /// Returns the length of the polyline by summing the lengths of the individual segments
        /// </summary>
        /// <returns>The length of the line.</returns>
        private double GetPolyLineLength()
        {
            double length = 0;
            for (var i = 0; i < _points.Count - 1; ++i)
            {
                length += _points[i].DistanceTo(_points[i + 1]);
            }

            return length;
        }

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc/>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadToFirstDescendant();
            try
            {
                var e = (XElement)XNode.ReadFrom(reader);
                var xElements = e.ElementsNamed("Point");
                var points = xElements.Select(x => Point3D.ReadFrom(x.CreateReader())).ToList();
                _points = new List<Point3D>(points).AsReadOnly();
                reader.Skip();
                return;
            }
            catch
            {
                // ignore
            }

            throw new XmlException($"Could not read a {GetType()}");
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Points");
            foreach (var point in _points)
            {
                writer.WriteElement("Point", point);
            }
            writer.WriteEndElement();
        }
    }
}
