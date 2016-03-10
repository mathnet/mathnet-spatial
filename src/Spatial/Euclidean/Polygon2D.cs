using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Class to represent a closed polygon. If the 
    /// </summary>
    public class Polygon2D : IEnumerable<Point2D>
    {
        private List<Point2D> _points;

        public int Count
        {
            get { return this._points.Count; }
        }

        // Constructors
        public Polygon2D()
            : this(Enumerable.Empty<Point2D>())
        {

        }

        public Polygon2D(IEnumerable<Point2D> points)
        {
            this._points = new List<Point2D>(points);
            if (this._points.First().Equals(this._points.Last()))
                this._points.RemoveAt(0);
        }

        // Methods
        public Point2D this[int key]
        {
            get { return this._points[key]; }
            set { this._points[key] = value; }
        }

        /// <summary>
        /// Test whether a point is enclosed within a polygon. Points on the polygon edges are not
        /// counted as contained within the polygon.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool EnclosesPoint(Point2D p)
        {
            return Polygon2D.IsPointInPolygon(p, this);
        }

        /// <summary>
        /// Compute whether or not two polygons are colliding based on whether or not the vertices of
        /// either are enclosed within the shape of the other. This is a simple means of detecting collisions
        /// that can fail if the two polygons are heavily overlapped in such a way that one protrudes through
        /// the other and out its opposing side without any vertices being enclosed.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ArePolygonVerticesColliding(Polygon2D a, Polygon2D b)
        {
            return a.Any(b.EnclosesPoint) || b.Any(a.EnclosesPoint);
        }

        /// <summary>
        /// Determine whether or not a point is inside a polygon using the intersection counting 
        /// method.  Return true if the point is contained, false if it is not. Points which lie
        /// on the edge are not counted as inside the polygon.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static bool IsPointInPolygon(Point2D p, Polygon2D poly)
        {
            // Algorithm from http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            // translated into C#
            bool c = false;
            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                if (((poly[i].Y > p.Y) != (poly[j].Y > p.Y)) &&
                    (p.X < (poly[j].X - poly[i].X) * (p.Y - poly[i].Y) / (poly[j].Y - poly[i].Y) + poly[i].X))
                    c = !c;
            }
            return c;
        }

        public Polygon2D ReduceComplexity(double singleStepTolerance)
        {
            return new Polygon2D(PolyLine2D.ReduceComplexity(this.ToPolyLine2D(), singleStepTolerance));
        }

        /// <summary>
        /// Using the recursive QuickHull algorithm, take an IEnumerable of Point2Ds and compute the 
        /// two dimensional convex hull, returning it as a Polygon2D object.  
        /// </summary>
        /// <param name="pointList"></param>
        /// <returns></returns>
        public static Polygon2D GetConvexHullFromPoints(IEnumerable<Point2D> pointList)
        {
            // Use the Quickhull algorithm to compute the convex hull of the given points, 
            // making the assumption that the points were delivered in no particular order.
            var points = new List<Point2D>(pointList);

            // Perform basic validation of the input point cloud for cases of less than
            // four points being given
            if (points.Count <= 1)
                return null;
            if (points.Count <= 3)
                return new Polygon2D(points);

            // Find the leftmost and rightmost points
            Point2D leftMost = points.First();
            Point2D rightMost = points.First();
            foreach (var point in points)
            {
                if (point.X < leftMost.X)
                    leftMost = point;
                if (point.X > rightMost.X)
                    rightMost = point;
            }

            // Remove the left and right points
            points.Remove(leftMost);
            points.Remove(rightMost);

            // Break the remaining cloud into upper and lower sets
            var upperPoints = new List<Point2D>();
            var lowerPoints = new List<Point2D>();
            Vector2D chord = leftMost.VectorTo(rightMost);
            foreach (var point2D in points)
            {
                Vector2D testVector = leftMost.VectorTo(point2D);
                if (chord.CrossProduct(testVector) > 0)
                    upperPoints.Add(point2D);
                else
                    lowerPoints.Add(point2D);
            }

            var hullPoints = new List<Point2D> { leftMost, rightMost };

            RecursiveHullComputation(leftMost, rightMost, upperPoints, hullPoints);
            RecursiveHullComputation(leftMost, rightMost, lowerPoints, hullPoints);

            // Order the hull points by angle to the centroid
            Point2D centroid = Point2D.Centroid(hullPoints);
            Vector2D xAxis = new Vector2D(1, 0);
            var results = (from x in hullPoints select new Tuple<Angle, Point2D>(centroid.VectorTo(x).AngleTo(xAxis), x)).ToList();
            results.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            return new Polygon2D(from x in results select x.Item2);
        }

        /// <summary>
        /// Recursive method to isolate the points from the working list which lie on the convex hull
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="workingList"></param>
        /// <param name="hullList"></param>
        private static void RecursiveHullComputation(Point2D a, Point2D b, List<Point2D> workingList, List<Point2D> hullList)
        {
            if (!workingList.Any())
                return;
            if (workingList.Count == 1)
            {
                hullList.Add(workingList.First());
                workingList.Remove(workingList.First());
                return;
            }

            // Find the furthest point from the line
            var chord = a.VectorTo(b);
            Point2D maxPoint = new Point2D();
            double maxDistance = double.MinValue;

            foreach (var point2D in workingList)
            {
                var testVector = a.VectorTo(point2D);
                var projection = testVector.ProjectOn(chord);
                var rejection = testVector - projection;
                if (rejection.Length > maxDistance)
                {
                    maxDistance = rejection.Length;
                    maxPoint = point2D;
                }
            }

            // Add the point to the hull and remove it from the working list
            hullList.Add(maxPoint);
            workingList.Remove(maxPoint);

            // Remove all points from the workinglist inside the new triangle
            Polygon2D exclusionTriangle = new Polygon2D(new Point2D[] { a, b, maxPoint });
            var removeList = workingList.Where(x => exclusionTriangle.EnclosesPoint(x)).ToList();
            foreach (var point2D in removeList)
            {
                workingList.Remove(point2D);
            }

            // Recurse to the next level
            RecursiveHullComputation(a, maxPoint, workingList, hullList);
            RecursiveHullComputation(maxPoint, b, workingList, hullList);
        }

        public Polygon2D Rotate(Angle angle)
        {
            IEnumerable<Point2D> newPoints = from p in this select new Point2D(0, 0) + p.ToVector2D().Rotate(angle);
            return new Polygon2D(newPoints);
        }

        /// <summary>
        /// Rotate the polygon around the specified point
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public Polygon2D RotateAround(Angle angle, Point2D center)
        {
            // Shift to the origin
            var shiftVector = center.VectorTo(Point2D.Origin);
            var tempPoly = shiftVector + this;

            // Rotate
            var rotatedPoly = tempPoly.Rotate(angle);

            // Shift back
            return -shiftVector + rotatedPoly;
        }

        public PolyLine2D ToPolyLine2D()
        {
            var points = this._points.ToList();
            points.Add(points.First());
            return new PolyLine2D(points);
        }

        // Operators
        public static Polygon2D operator +(Vector2D shift, Polygon2D poly)
        {
            IEnumerable<Point2D> newPoints = from p in poly select p + shift;
            return new Polygon2D(newPoints);
        }

        public static Polygon2D operator +(Polygon2D poly, Vector2D shift)
        {
            return shift + poly;
        }

        public IEnumerator<Point2D> GetEnumerator()
        {
            return this._points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}