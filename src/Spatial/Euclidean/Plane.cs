using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A geometric plane
    /// </summary>
    [Serializable]
    public struct Plane : IEquatable<Plane>, IXmlSerializable
    {
        /// <summary>
        /// The normal vector of the Plane.
        /// </summary>
        public readonly UnitVector3D Normal;

        /// <summary>
        /// The distance to the Plane along its normal from the origin.
        /// </summary>
        public readonly double D;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// Constructs a Plane from the X, Y, and Z components of its normal, and its distance from the origin on that normal.
        /// </summary>
        /// <param name="x">The X-component of the normal.</param>
        /// <param name="y">The Y-component of the normal.</param>
        /// <param name="z">The Z-component of the normal.</param>
        /// <param name="d">The distance of the Plane along its normal from the origin.</param>
        public Plane(double x, double y, double z, double d)
            : this(UnitVector3D.Create(x, y, z), -d)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// Constructs a Plane from the given normal and distance along the normal from the origin.
        /// </summary>
        /// <param name="normal">The Plane's normal vector.</param>
        /// <param name="offset">The Plane's distance from the origin along its normal vector.</param>
        public Plane(UnitVector3D normal, double offset = 0)
        {
            Normal = normal;
            D = -offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// Constructs a Plane from the given normal and distance along the normal from the origin.
        /// </summary>
        /// <param name="normal">The Plane's normal vector.</param>
        /// <param name="rootPoint">A point in the plane.</param>
        public Plane(UnitVector3D normal, Point3D rootPoint)
            : this(normal, normal.DotProduct(rootPoint))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// Constructs a Plane from the given normal and distance along the normal from the origin.
        /// </summary>
        /// <param name="normal">The Plane's normal vector.</param>
        /// <param name="rootPoint">A point in the plane.</param>
        public Plane(Point3D rootPoint, UnitVector3D normal)
            : this(normal, normal.DotProduct(rootPoint))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// Constructs a Plane from the given points.
        /// </summary>
        /// <param name="points">The points</param>
        /// <returns>Fitted <see cref="Plane"/></returns>
        /// <remarks>this method uses SVD to fit the plane.</remarks>
        public static Plane CreateFittedPlaneFrom(IEnumerable<Point3D> points)
        {
            var throughPoint = Point3D.Centroid(points);

            var relativePointMatrix = CreateMatrix.DenseOfRowVectors(
                points.Select(p => p - throughPoint)
                    .Select(p => CreateVector.DenseOfArray(new double[] { p.X, p.Y, p.Z })));

            var svd = relativePointMatrix.Svd(true);
            var matV = svd.VT.Transpose();
            var theIndex = svd.S.Count-1; // in this case, theIndex = 2.
            var normal = UnitVector3D.OfVector(matV.Column(theIndex));

            var result = new Plane(normal, throughPoint);
            return result;
        }

        /// <summary>
        /// Gets the <see cref="Normal"/> x component.
        /// </summary>
        public double A => Normal.X;

        /// <summary>
        /// Gets the <see cref="Normal"/> y component.
        /// </summary>
        public double B => Normal.Y;

        /// <summary>
        /// Gets the <see cref="Normal"/> y component.
        /// </summary>
        public double C => Normal.Z;

        /// <summary>
        /// Gets the point on the plane closest to origin.
        /// </summary>
        public Point3D RootPoint => (-D * Normal).ToPoint3D();

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified geometric planes is equal.
        /// </summary>
        /// <param name="left">The first plane to compare.</param>
        /// <param name="right">The second plane to compare.</param>
        /// <returns>True if the geometric planes are the same; otherwise false.</returns>
        public static bool operator ==(Plane left, Plane right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified geometric planes is not equal.
        /// </summary>
        /// <param name="left">The first plane to compare.</param>
        /// <param name="right">The second plane to compare.</param>
        /// <returns>True if the geometric planes are different; otherwise false.</returns>
        public static bool operator !=(Plane left, Plane right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// Creates a plane that contains the three given points.
        /// </summary>
        /// <param name="p1">The first point on the plane.</param>
        /// <param name="p2">The second point on the plane.</param>
        /// <param name="p3">The third point on the plane.</param>
        /// <returns>The plane containing the three points.</returns>
        public static Plane FromPoints(Point3D p1, Point3D p2, Point3D p3)
        {
            // http://www.had2know.com/academics/equation-plane-through-3-points.html
            if (p1 == p2 || p1 == p3 || p2 == p3)
            {
                throw new ArgumentException("Must use three different points");
            }

            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            var v2 = new Vector3D(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);
            var cross = v1.CrossProduct(v2);

            if (cross.Length <= float.Epsilon)
            {
                throw new ArgumentException("The 3 points should not be on the same line");
            }

            var normal = cross.Normalize();
            var distanceFromOrigin = normal.DotProduct(p1);
            if (distanceFromOrigin < 0)
            {
                // make sure the plane is defined in its Hesse normal form
                // https://en.wikipedia.org/wiki/Hesse_normal_form
                normal = normal.Negate();
            }

            return new Plane(normal, p1);
        }

        /// <summary>
        /// Returns a point of intersection between three planes
        /// </summary>
        /// <param name="plane1">The first plane</param>
        /// <param name="plane2">The second plane</param>
        /// <param name="plane3">The third plane</param>
        /// <returns>The intersection point</returns>
        public static Point3D PointFromPlanes(Plane plane1, Plane plane2, Plane plane3)
        {
            return Point3D.IntersectionOf(plane1, plane2, plane3);
        }

        /// <summary>
        /// Get the distance to the point along the <see cref="Normal"/>
        /// </summary>
        /// <param name="point">The <see cref="Point3D"/></param>
        /// <returns>The distance.</returns>
        [Pure]
        public double SignedDistanceTo(Point3D point)
        {
            var p = Project(point);
            var v = p.VectorTo(point);
            return v.DotProduct(Normal);
        }

        /// <summary>
        /// Get the distance to the plane along the <see cref="Normal"/>
        /// This assumes the planes are parallel
        /// </summary>
        /// <param name="other">The <see cref="Point3D"/></param>
        /// <returns>The distance.</returns>
        [Pure]
        public double SignedDistanceTo(Plane other)
        {
            if (!Normal.IsParallelTo(other.Normal, tolerance: 1E-15))
            {
                throw new ArgumentException("Planes are not parallel");
            }

            return SignedDistanceTo(other.RootPoint);
        }

        /// <summary>
        /// Get the distance to the ThroughPoint of <paramref name="ray"/>  along the <see cref="Normal"/>
        /// This assumes the ray is parallel to the plane.
        /// </summary>
        /// <param name="ray">The <see cref="Point3D"/></param>
        /// <returns>The distance.</returns>
        [Pure]
        public double SignedDistanceTo(Ray3D ray)
        {
            if (Math.Abs(ray.Direction.DotProduct(Normal) - 0) < 1E-15)
            {
                return SignedDistanceTo(ray.ThroughPoint);
            }

            return 0;
        }

        /// <summary>
        /// Get the distance to the point.
        /// </summary>
        /// <param name="point">The <see cref="Point3D"/></param>
        /// <returns>The distance.</returns>
        [Pure]
        public double AbsoluteDistanceTo(Point3D point)
        {
            return Math.Abs(SignedDistanceTo(point));
        }

        /// <summary>
        /// Projects a point onto the plane
        /// </summary>
        /// <param name="p">A point</param>
        /// <param name="projectionDirection">The direction of projection</param>
        /// <returns>a projected point</returns>
        [Pure]
        public Point3D Project(Point3D p, UnitVector3D? projectionDirection = null)
        {
            var direction = projectionDirection ?? Normal;
            var distance = (RootPoint - p).DotProduct(Normal) / direction.DotProduct(Normal);
            return p + distance * direction;
        }

        /// <summary>
        /// Projects a line onto the plane
        /// </summary>
        /// <param name="line3DToProject">The line to project</param>
        /// <returns>A projected line</returns>
        public Line3D Project(Line3D line3DToProject)
        {
            var projectedStartPoint = Project(line3DToProject.StartPoint);
            var projectedEndPoint = Project(line3DToProject.EndPoint);
            return new Line3D(projectedStartPoint, projectedEndPoint);
        }

        /// <summary>
        /// Projects a line onto the plane
        /// </summary>
        /// <param name="line3DToProject">The line to project</param>
        /// <returns>A projected line</returns>
        [Pure]
        public LineSegment3D Project(LineSegment3D line3DToProject)
        {
            var projectedStartPoint = Project(line3DToProject.StartPoint);
            var projectedEndPoint = Project(line3DToProject.EndPoint);
            return new LineSegment3D(projectedStartPoint, projectedEndPoint);
        }

        /// <summary>
        /// Projects a ray onto the plane
        /// </summary>
        /// <param name="rayToProject">The ray to project</param>
        /// <returns>A projected ray</returns>
        [Pure]
        public Ray3D Project(Ray3D rayToProject)
        {
            var projectedThroughPoint = Project(rayToProject.ThroughPoint);
            var projectedDirection = Project(rayToProject.Direction.ToVector3D());
            return new Ray3D(projectedThroughPoint, projectedDirection.Direction);
        }

        /// <summary>
        /// Project Vector3D onto this plane
        /// </summary>
        /// <param name="vector3DToProject">The Vector3D to project</param>
        /// <returns>The projected Vector3D</returns>
        [Pure]
        public Ray3D Project(Vector3D vector3DToProject)
        {
            var projectedEndPoint = Project(vector3DToProject.ToPoint3D());
            var projectedZero = Project(new Point3D(0, 0, 0));
            return new Ray3D(projectedZero, projectedZero.VectorTo(projectedEndPoint).Normalize());
        }

        /// <summary>
        /// Project Vector3D onto this plane
        /// </summary>
        /// <param name="vector3DToProject">The Vector3D to project</param>
        /// <returns>The projected Vector3D</returns>
        [Pure]
        public Ray3D Project(UnitVector3D vector3DToProject)
        {
            return Project(vector3DToProject.ToVector3D());
        }

        /// <summary>
        /// Finds the intersection of the two planes, throws if they are parallel
        /// http://mathworld.wolfram.com/Plane-PlaneIntersection.html
        /// </summary>
        /// <param name="intersectingPlane">a plane which intersects</param>
        /// <param name="tolerance">A tolerance (epsilon) to account for floating point error.</param>
        /// <returns>A ray at the intersection.</returns>
        [Pure]
        public Ray3D IntersectionWith(Plane intersectingPlane, double tolerance = float.Epsilon)
        {
            var a = new DenseMatrix(2, 3);
            a.SetRow(0, Normal.ToVector());
            a.SetRow(1, intersectingPlane.Normal.ToVector());
            var svd = a.Svd();
            if (svd.S[1] < tolerance)
            {
                throw new ArgumentException("Planes are parallel");
            }

            var y = new DenseMatrix(2, 1)
            {
                [0, 0] = -1 * D,
                [1, 0] = -1 * intersectingPlane.D
            };

            var pointOnIntersectionLine = svd.Solve(y);
            var throughPoint = Point3D.OfVector(pointOnIntersectionLine.Column(0));
            var direction = UnitVector3D.OfVector(svd.VT.Row(2));
            return new Ray3D(throughPoint, direction);
        }

        /// <summary>
        /// Find intersection between Line3D and Plane
        /// http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <param name="line">A line segment</param>
        /// <param name="tolerance">A tolerance (epsilon) to account for floating point error.</param>
        /// <returns>Intersection Point or null</returns>
        public Point3D? IntersectionWith(Line3D line, double tolerance = float.Epsilon)
        {
            if (line.Direction.IsPerpendicularTo(Normal, tolerance))
            {
                // either parallel or lies in the plane
                var projectedPoint = Project(line.StartPoint, line.Direction);
                if (projectedPoint == line.StartPoint)
                {
                    throw new InvalidOperationException("Line lies in the plane");
                }

                // Line and plane are parallel
                return null;
            }

            var d = SignedDistanceTo(line.StartPoint);
            var u = line.StartPoint.VectorTo(line.EndPoint);
            var t = -1 * d / u.DotProduct(Normal);
            if (t > 1 || t < 0)
            {
                // They are not intersected
                return null;
            }

            return line.StartPoint + (t * u);
        }

        /// <summary>
        /// Find intersection between LineSegment3D and Plane
        /// http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <param name="line">A line segment</param>
        /// <param name="tolerance">A tolerance (epsilon) to account for floating point error.</param>
        /// <returns>Intersection Point or null</returns>
        [Pure]
        public Point3D? IntersectionWith(LineSegment3D line, double tolerance = float.Epsilon)
        {
            if (line.Direction.IsPerpendicularTo(Normal, tolerance))
            {
                // either parallel or lies in the plane
                var projectedPoint = Project(line.StartPoint, line.Direction);
                if (projectedPoint == line.StartPoint)
                {
                    throw new InvalidOperationException("Line lies in the plane");
                }

                // Line and plane are parallel
                return null;
            }

            var d = SignedDistanceTo(line.StartPoint);
            var u = line.StartPoint.VectorTo(line.EndPoint);
            var t = -1 * d / u.DotProduct(Normal);
            if (t > 1 || t < 0)
            {
                // They are not intersected
                return null;
            }

            return line.StartPoint + (t * u);
        }

        /// <summary>
        /// http://www.cs.princeton.edu/courses/archive/fall00/cs426/lectures/raycast/sld017.htm
        /// </summary>
        /// <param name="ray">A ray</param>
        /// <param name="tolerance">A tolerance (epsilon) to account for floating point error.</param>
        /// <returns>The point of intersection.</returns>
        [Pure]
        public Point3D IntersectionWith(Ray3D ray, double tolerance = float.Epsilon)
        {
            if (Normal.IsPerpendicularTo(ray.Direction, tolerance))
            {
                throw new InvalidOperationException("Ray is parallel to the plane.");
            }

            var d = SignedDistanceTo(ray.ThroughPoint);
            var t = -1 * d / ray.Direction.DotProduct(Normal);
            return ray.ThroughPoint + (t * ray.Direction);
        }

        /// <summary>
        /// Returns <paramref name="p"/> mirrored about the plane.
        /// </summary>
        /// <param name="p">The <see cref="Point3D"/></param>
        /// <returns>The mirrored point.</returns>
        [Pure]
        public Point3D MirrorAbout(Point3D p)
        {
            var p2 = Project(p);
            var d = SignedDistanceTo(p);
            return p2 - (1 * d * Normal);
        }

        /// <summary>
        /// Rotates a plane
        /// </summary>
        /// <param name="aboutVector">The vector about which to rotate</param>
        /// <param name="angle">An angle to rotate</param>
        /// <returns>A rotated plane</returns>
        [Pure]
        public Plane Rotate(UnitVector3D aboutVector, Angle angle)
        {
            var rootPoint = RootPoint;
            var rotatedPoint = rootPoint.Rotate(aboutVector, angle);
            var rotatedPlaneVector = Normal.Rotate(aboutVector, angle);
            return new Plane(rotatedPlaneVector, rotatedPoint);
        }

        /// <summary>
        /// Returns a value to indicate if a pair of geometric planes are equal
        /// </summary>
        /// <param name="other">The geometric plane to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the geometric planes are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Plane other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.D - D) < tolerance && Normal.Equals(other.Normal, tolerance);
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Plane p) => D.Equals(p.D) && Normal.Equals(p.Normal);

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Plane p && Equals(p);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(Normal, D);

        /// <inheritdoc />
        [Pure]
        public override string ToString()
        {
            return $"A:{Math.Round(A, 4)} B:{Math.Round(B, 4)} C:{Math.Round(C, 4)} D:{Math.Round(D, 4)}";
        }

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            this = new Plane(
                UnitVector3D.ReadFrom(e.SingleElement("Normal").CreateReader()),
                Point3D.ReadFrom(e.SingleElement("RootPoint").CreateReader()));
        }

        /// <inheritdoc/>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("RootPoint", RootPoint);
            writer.WriteElement("Normal", Normal);
        }
    }
}
