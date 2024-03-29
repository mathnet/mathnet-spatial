﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Class to represent a closed polygon.
    /// </summary>
    [Serializable]
    public class Polygon2D : IEquatable<Polygon2D>, IXmlSerializable
    {
        /// <summary>
        /// A list of vertices.
        /// </summary>
        private ImmutableList<Point2D> _points;

        /// <summary>
        /// A list of edges.  This list is lazy loaded on demand.
        /// </summary>
        private ImmutableList<LineSegment2D> _edges;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon2D"/> class.
        /// At least three points are needed to construct a polygon.  If less are passed an ArgumentException is thrown.
        /// </summary>
        /// <param name="vertices">A list of vertices.</param>
        public Polygon2D(IEnumerable<Point2D> vertices)
            : this(vertices.ToArray())
        {
        }

        /// <summary>
        /// Used only internally for XML deserialization
        /// </summary>
        internal Polygon2D()
        {
            _points = ImmutableList<Point2D>.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon2D"/> class.
        /// At least three points are needed to construct a polygon.  If less are passed an ArgumentException is thrown.
        /// </summary>
        /// <param name="vertices">A list of vertices.</param>
        public Polygon2D(params Point2D[] vertices)
        {
            if (vertices.Length < 3)
            {
                throw new ArgumentException("Cannot create a polygon out of less than three points");
            }

            if (vertices[0].Equals(vertices[vertices.Length - 1]))
            {
                _points = ImmutableList.Create(vertices.Skip(1).ToArray());
            }
            else
            {
                _points = ImmutableList.Create(vertices);
            }
        }

        /// <summary>
        /// Gets a list of vertices
        /// </summary>
        public IEnumerable<Point2D> Vertices
        {
            get
            {
                foreach (var point in _points)
                {
                    yield return point;
                }
            }
        }

        /// <summary>
        /// Gets a list of Edges
        /// </summary>
        public IEnumerable<LineSegment2D> Edges
        {
            get
            {
                if (_edges == null)
                {
                    PopulateEdgeList();
                }

                foreach (var edge in _edges)
                {
                    yield return edge;
                }
            }
        }

        /// <summary>
        /// Gets the number of vertices in the polygon.
        /// </summary>
        public int VertexCount => _points.Count;

        /// <summary>
        /// Returns a value that indicates whether each point in two specified polygons is equal.
        /// </summary>
        /// <param name="left">The first polygon to compare</param>
        /// <param name="right">The second polygon to compare</param>
        /// <returns>True if the polygons are the same; otherwise false.</returns>
        public static bool operator ==(Polygon2D left, Polygon2D right)
        {
            return left?.Equals(right) == true;
        }

        /// <summary>
        /// Returns a value that indicates whether any point in two specified polygons is not equal.
        /// </summary>
        /// <param name="left">The first polygon to compare</param>
        /// <param name="right">The second polygon to compare</param>
        /// <returns>True if the polygons are different; otherwise false.</returns>
        public static bool operator !=(Polygon2D left, Polygon2D right)
        {
            return left?.Equals(right) != true;
        }

        /// <summary>
        /// Compute whether or not two polygons are colliding based on whether or not the vertices of
        /// either are enclosed within the shape of the other. This is a simple means of detecting collisions
        /// that can fail if the two polygons are heavily overlapped in such a way that one protrudes through
        /// the other and out its opposing side without any vertices being enclosed.
        /// </summary>
        /// <param name="a">The first polygon.</param>
        /// <param name="b">The second polygon</param>
        /// <returns>True if the vertices collide; otherwise false.</returns>
        public static bool ArePolygonVerticesColliding(Polygon2D a, Polygon2D b)
        {
            return a._points.Any(b.EnclosesPoint) || b._points.Any(a.EnclosesPoint);
        }

        /// <summary>
        /// Using algorithm from Ouellet - https://www.codeproject.com/Articles/1210225/Fast-and-improved-D-Convex-Hull-algorithm-and-its, take an IEnumerable of Point2Ds and computes the
        /// two dimensional convex hull, returning it as a Polygon2D object.
        /// </summary>
        /// <param name="pointList">A list of points</param>
        /// <param name="clockWise">
        /// In which direction to return the points on the convex hull.
        /// If true, clockwise. Otherwise counter clockwise
        /// </param>
        /// <returns>A polygon.</returns>
        public static Polygon2D GetConvexHullFromPoints(IEnumerable<Point2D> pointList, bool clockWise = true)
        {
            int count = pointList.Count();

            // Perform basic validation of the input point cloud for cases of less than
            // four points being given
            if (count <= 2)
            {
                throw new ArgumentException("Must have at least 3 points in the polygon to compute the convex hull");
            }

            if (count <= 3)
            {
                return new Polygon2D(pointList);
            }

            var chull = new ConvexHull(pointList, false);
            chull.CalcConvexHull();
            var hullPoints = chull.GetResultsAsArrayOfPoint();

            // Order the hull points by angle to the centroid
            var centroid = Point2D.Centroid(hullPoints);
            var xAxis = new Vector2D(1, 0);
            var results = (from x in hullPoints select new Tuple<Angle, Point2D>(centroid.VectorTo(x).SignedAngleTo(xAxis, clockWise), x)).ToList();
            results.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            return new Polygon2D(from x in results select x.Item2);
        }

        /// <summary>
        /// Test whether a point is enclosed within a polygon. Points on the polygon edges are not
        /// counted as contained within the polygon.
        /// </summary>
        /// <param name="p">A point.</param>
        /// <returns>True if the point is inside the polygon; otherwise false.</returns>
        public bool EnclosesPoint(Point2D p)
        {
            var c = false;
            for (int i = 0, j = _points.Count - 1; i < _points.Count; j = i++)
            {
                if (((_points[i].Y > p.Y) != (_points[j].Y > p.Y)) &&
                    (p.X < ((_points[j].X - _points[i].X) * (p.Y - _points[i].Y) / (_points[j].Y - _points[i].Y)) + _points[i].X))
                {
                    c = !c;
                }
            }

            return c;
        }

        /// <summary>
        /// Creates a new polygon from the existing polygon by removing any edges whose adjacent segments are considered colinear within the provided tolerance
        /// </summary>
        /// <param name="singleStepTolerance">The tolerance by which adjacent edges should be considered collinear.</param>
        /// <returns>A polygon</returns>
        public Polygon2D ReduceComplexity(double singleStepTolerance)
        {
            return new Polygon2D(PolyLine2D.ReduceComplexity(ToPolyLine2D().Vertices, singleStepTolerance).Vertices);
        }

        /// <summary>
        /// Returns a polygon rotated around the origin
        /// </summary>
        /// <param name="angle">The angle by which to rotate.</param>
        /// <returns>A new polygon that has been rotated.</returns>
        public Polygon2D Rotate(Angle angle)
        {
            var rotated = _points.Select(t => Point2D.Origin + t.ToVector2D().Rotate(angle)).ToArray();
            return new Polygon2D(rotated);
        }

        /// <summary>
        /// Returns a new polygon which is translated (moved) by a vector
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <returns>A new polygon that has been translated.</returns>
        public Polygon2D TranslateBy(Vector2D vector)
        {
            var newPoints = from p in _points select p + vector;
            return new Polygon2D(newPoints);
        }

        /// <summary>
        /// Rotate the polygon around the specified point
        /// </summary>
        /// <param name="angle">The angle by which to rotate</param>
        /// <param name="center">A point at which to rotate around</param>
        /// <returns>A new polygon that has been rotated.</returns>
        public Polygon2D RotateAround(Angle angle, Point2D center)
        {
            // Shift to the origin
            var shiftVector = center.VectorTo(Point2D.Origin);
            var tempPoly = TranslateBy(shiftVector);

            // Rotate
            var rotatedPoly = tempPoly.Rotate(angle);

            // Shift back
            return rotatedPoly.TranslateBy(-shiftVector);
        }

        /// <summary>
        /// Converts the polygon into a PolyLine2D
        /// </summary>
        /// <returns>A polyline</returns>
        public PolyLine2D ToPolyLine2D()
        {
            var points = _points.ToList();
            points.Add(points.First());
            return new PolyLine2D(points);
        }

        /// <summary>
        /// Returns a value to indicate if a pair of polygons are equal
        /// </summary>
        /// <param name="other">The polygon to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the polygons are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Polygon2D other, double tolerance)
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
        public bool Equals(Polygon2D other)
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
            if (obj is null)
            {
                return false;
            }

            return obj is Polygon2D d && Equals(d);
        }

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode()
        {
            return HashCode.CombineMany(_points);
        }

        /// <summary>
        /// Populates the edge list
        /// </summary>
        private void PopulateEdgeList()
        {
            var localEdges = new List<LineSegment2D>(_points.Count);
            for (var i = 0; i < _points.Count - 1; i++)
            {
                var edge = new LineSegment2D(_points[i], _points[i + 1]);
                localEdges.Add(edge);
            }

            localEdges.Add(new LineSegment2D(_points[_points.Count - 1], _points[0])); // complete loop
            _edges = ImmutableList.Create(localEdges);
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
                var points = xElements.Select(x => Point2D.ReadFrom(x.CreateReader())).ToList();
                _points = ImmutableList.Create(points);
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
