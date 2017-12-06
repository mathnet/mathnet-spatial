namespace MathNet.Spatial.Euclidean
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using MathNet.Spatial.Units;

    /// <summary>
    /// Class to represent a closed polygon.
    /// </summary>
    public class Polygon2D : IEnumerable<Point2D>
    {
        /// <summary>
        /// A list of vertices.
        /// </summary>
        private readonly List<Point2D> points;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon2D"/> class.
        /// </summary>
        /// <param name="points">A list of vertices.</param>
        public Polygon2D(IEnumerable<Point2D> points)
        {
            this.points = new List<Point2D>(points);
            if (this.points.First().Equals(this.points.Last()))
            {
                this.points.RemoveAt(0);
            }
        }

        /// <summary>
        /// Gets the number of vertices in the polygon.
        /// </summary>
        public int Count => this.points.Count;

        /// <summary>
        /// A index into the list of vertices
        /// </summary>
        /// <param name="key">An index for the vertex number</param>
        /// <returns>A Vertex</returns>
        public Point2D this[int key] => this.points[key];

        /// <summary>
        /// Adds a vector to the each point on the polygon
        /// </summary>
        /// <param name="shift">The vector to add</param>
        /// <param name="poly">The polygon</param>
        /// <returns>A new <see cref="Polygon2D"/> at the adjusted points</returns>
        [Obsolete("Use Translate instance method instead, obsolete since 6/12/2017")]
        public static Polygon2D operator +(Vector2D shift, Polygon2D poly)
        {
            var newPoints = from p in poly select p + shift;
            return new Polygon2D(newPoints);
        }

        /// <summary>
        /// Adds a vector to the each point on the polygon
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="shift">The vector to add</param>
        /// <returns>A new <see cref="Polygon2D"/> at the adjusted points</returns>
        [Obsolete("Use Translate instance method instead, obsolete since 6/12/2017")]
        public static Polygon2D operator +(Polygon2D poly, Vector2D shift)
        {
            return shift + poly;
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
            return a.Any(b.EnclosesPoint) || b.Any(a.EnclosesPoint);
        }

        /// <summary>
        /// Determine whether or not a point is inside a polygon using the intersection counting
        /// method.  Return true if the point is contained, false if it is not. Points which lie
        /// on the edge are not counted as inside the polygon.
        /// </summary>
        /// <param name="p">A point</param>
        /// <param name="poly">A polygon</param>
        /// <returns>True if the point is inside the polygon; otherwise false.</returns>
        public static bool IsPointInPolygon(Point2D p, Polygon2D poly)
        {
            // Algorithm from http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            // translated into C#
            var c = false;
            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                if (((poly[i].Y > p.Y) != (poly[j].Y > p.Y)) &&
                    (p.X < ((poly[j].X - poly[i].X) * (p.Y - poly[i].Y) / (poly[j].Y - poly[i].Y)) + poly[i].X))
                {
                    c = !c;
                }
            }

            return c;
        }

        /// <summary>
        /// Using the recursive QuickHull algorithm, take an IEnumerable of Point2Ds and compute the
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
            // Use the Quickhull algorithm to compute the convex hull of the given points,
            // making the assumption that the points were delivered in no particular order.
            var points = new List<Point2D>(pointList);

            // Perform basic validation of the input point cloud for cases of less than
            // four points being given
            if (points.Count <= 2)
            {
                throw new ArgumentException("Must have at least 3 points in the polygon to compute the convex hull");
            }

            if (points.Count <= 3)
            {
                return new Polygon2D(points);
            }

            // Find the leftmost and rightmost points
            var leftMost = points.First();
            var rightMost = points.First();
            foreach (var point in points)
            {
                if (point.X < leftMost.X)
                {
                    leftMost = point;
                }

                if (point.X > rightMost.X)
                {
                    rightMost = point;
                }
            }

            // Remove the left and right points
            points.Remove(leftMost);
            points.Remove(rightMost);

            // Break the remaining cloud into upper and lower sets
            var upperPoints = new List<Point2D>();
            var lowerPoints = new List<Point2D>();
            var chord = leftMost.VectorTo(rightMost);
            foreach (var point2D in points)
            {
                var testVector = leftMost.VectorTo(point2D);
                if (chord.CrossProduct(testVector) > 0)
                {
                    upperPoints.Add(point2D);
                }
                else
                {
                    lowerPoints.Add(point2D);
                }
            }

            var hullPoints = new List<Point2D> { leftMost, rightMost };

            RecursiveHullComputation(leftMost, rightMost, upperPoints, hullPoints);
            RecursiveHullComputation(leftMost, rightMost, lowerPoints, hullPoints);

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
            return Polygon2D.IsPointInPolygon(p, this);
        }

        /// <summary>
        /// Creates a new polygon from the existing polygon by removing any edges whose adjacent segments are considered colinear within the provided tolerance
        /// </summary>
        /// <param name="singleStepTolerance">The tolerance by which adjactent edges should be considered collinear.</param>
        /// <returns>A polygon</returns>
        public Polygon2D ReduceComplexity(double singleStepTolerance)
        {
            return new Polygon2D(PolyLine2D.ReduceComplexity(this.ToPolyLine2D(), singleStepTolerance));
        }

        /// <summary>
        /// Returns a rotated polygon
        /// </summary>
        /// <param name="angle">The angle by which to rotate.</param>
        /// <returns>A new polygon that has been rotated.</returns>
        public Polygon2D Rotate(Angle angle)
        {
            var newPoints = from p in this select new Point2D(0, 0) + p.ToVector2D().Rotate(angle);
            return new Polygon2D(newPoints);
        }

        /// <summary>
        /// Returns a new polygon which is translated (moved) by a vector
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <returns>A new polygon that has been translated.</returns>
        public Polygon2D Translate(Vector2D vector)
        {
            var newPoints = from p in this.points select p + vector;
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
            var tempPoly = this.Translate(shiftVector);

            // Rotate
            var rotatedPoly = tempPoly.Rotate(angle);

            // Shift back
            return rotatedPoly.Translate(- shiftVector);
        }

        /// <summary>
        /// Converts the polygon into a PolyLine2D
        /// </summary>
        /// <returns>A polyline</returns>
        public PolyLine2D ToPolyLine2D()
        {
            var points = this.points.ToList();
            points.Add(points.First());
            return new PolyLine2D(points);
        }

        /// <summary>
        /// Returns an enumerator for the vertices
        /// </summary>
        /// <returns>An enumerator for the vertices</returns>
        public IEnumerator<Point2D> GetEnumerator()
        {
            return this.points.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator for the vertices
        /// </summary>
        /// <returns>An enumerator for the vertices</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Recursive method to isolate the points from the working list which lie on the convex hull
        /// </summary>
        /// <param name="a">The first point</param>
        /// <param name="b">The second point</param>
        /// <param name="workingList">A list of points to be evaluated</param>
        /// <param name="hullList">A list of points on the convexhull</param>
        private static void RecursiveHullComputation(Point2D a, Point2D b, List<Point2D> workingList, List<Point2D> hullList)
        {
            if (!workingList.Any())
            {
                return;
            }

            if (workingList.Count == 1)
            {
                hullList.Add(workingList.First());
                workingList.Remove(workingList.First());
                return;
            }

            // Find the furthest point from the line
            var chord = a.VectorTo(b);
            var maxPoint = default(Point2D);
            var maxDistance = double.MinValue;

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
            var exclusionTriangle = new Polygon2D(new Point2D[] { a, b, maxPoint });
            var removeList = workingList.Where(x => exclusionTriangle.EnclosesPoint(x)).ToList();
            foreach (var point2D in removeList)
            {
                workingList.Remove(point2D);
            }

            // Recurse to the next level
            RecursiveHullComputation(a, maxPoint, workingList, hullList);
            RecursiveHullComputation(maxPoint, b, workingList, hullList);
        }
    }
}
