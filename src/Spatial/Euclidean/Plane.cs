using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    [Serializable]
    public struct Plane : IEquatable<Plane>, IXmlSerializable
    {
        public readonly UnitVector3D Normal;
        public readonly Point3D RootPoint;
        public readonly double D;

        public Plane(double a, double b, double c, double d)
            : this(new UnitVector3D(a, b, c), -1*d)
        {
        }

        public Plane(UnitVector3D normal, double offset = 0)
        {
            this.Normal = normal;
            this.RootPoint = (offset*normal).ToPoint3D();
            this.D = -1*offset;
        }

        public Plane(UnitVector3D normal, Point3D rootPoint)
            : this(rootPoint, normal)
        {
        }

        public Plane(Point3D rootPoint, UnitVector3D normal)
        {
            this.RootPoint = rootPoint;
            this.Normal = normal;
            this.D = -this.RootPoint.ToVector3D().DotProduct(this.Normal);
        }

        /// <summary>
        /// http://www.had2know.com/academics/equation-plane-through-3-points.html
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public Plane(Point3D p1, Point3D p2, Point3D p3)
        {
            if (p1 == p2 || p1 == p3 || p2 == p3)
            {
                throw new ArgumentException("Must use three different points");
            }
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            Vector3D v2 = new Vector3D(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);
            Vector3D cross = v1.CrossProduct(v2);

            if (cross.Length <= float.Epsilon)
            {
                throw new ArgumentException("The 3 points should not be on the same line");
            }
            this.RootPoint = p1;
            this.Normal = cross.Normalize();
            this.D = -this.RootPoint.ToVector3D().DotProduct(this.Normal);
        }

        /// <summary>
        /// Creates a Plane from its string representation
        /// </summary>
        /// <param name="s">The string representation of the Plane</param>
        /// <returns></returns>
        public static Plane Parse(string s)
        {
            return Parser.ParsePlane(s);
        }

        public double A
        {
            get { return this.Normal.X; }
        }

        public double B
        {
            get { return this.Normal.Y; }
        }

        public double C
        {
            get { return this.Normal.Z; }
        }

        public static bool operator ==(Plane left, Plane right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Plane left, Plane right)
        {
            return !left.Equals(right);
        }

        public double SignedDistanceTo(Point3D point)
        {
            var point3D = Project(point);
            var vectorTo = point3D.VectorTo(point);
            return vectorTo.DotProduct(this.Normal);
        }

        public double SignedDistanceTo(Plane otherPlane)
        {
            if (!this.Normal.IsParallelTo(otherPlane.Normal, tolerance: 1E-15))
            {
                throw new ArgumentException("Planes are not parallel");
            }

            return SignedDistanceTo(otherPlane.RootPoint);
        }

        public double SignedDistanceTo(Ray3D ray)
        {
            if (Math.Abs(ray.Direction.DotProduct(this.Normal) - 0) < 1E-15)
            {
                return SignedDistanceTo(ray.ThroughPoint);
            }

            return 0;
        }

        public double AbsoluteDistanceTo(Point3D point)
        {
            return Math.Abs(SignedDistanceTo(point));
        }

        public Point3D Project(Point3D p, UnitVector3D? projectionDirection = null)
        {
            double dotProduct = this.Normal.DotProduct(p.ToVector3D());
            var projectiononNormal = projectionDirection == null ? this.Normal : projectionDirection.Value;
            var projectionVector = (dotProduct + this.D)*projectiononNormal;
            return p - projectionVector;
        }

        public Line3D Project(Line3D line3DToProject)
        {
            var projectedStartPoint = Project(line3DToProject.StartPoint);
            var projectedEndPoint = Project(line3DToProject.EndPoint);
            return new Line3D(projectedStartPoint, projectedEndPoint);
        }

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
        public Ray3D Project(Vector3D vector3DToProject)
        {
            var projectedEndPoint = Project(vector3DToProject.ToPoint3D());
            var projectedZero = this.Project(new Point3D(0, 0, 0));
            return new Ray3D(projectedZero, projectedZero.VectorTo(projectedEndPoint).Normalize());
        }

        /// <summary>
        /// Project Vector3D onto this plane
        /// </summary>
        /// <param name="vector3DToProject">The Vector3D to project</param>
        /// <returns>The projected Vector3D</returns>
        public Ray3D Project(UnitVector3D vector3DToProject)
        {
            return Project(vector3DToProject.ToVector3D());
        }

        /// <summary>
        /// Finds the intersection of the two planes, throws if they are parallel
        /// http://mathworld.wolfram.com/Plane-PlaneIntersection.html
        /// </summary>
        /// <param name="intersectingPlane"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Ray3D IntersectionWith(Plane intersectingPlane, double tolerance = float.Epsilon)
        {
            var a = new DenseMatrix(2, 3);
            a.SetRow(0, this.Normal.ToVector());
            a.SetRow(1, intersectingPlane.Normal.ToVector());
            var svd = a.Svd(true);
            if (svd.S[1] < tolerance)
            {
                throw new ArgumentException("Planes are parallel");
            }

            var y = new DenseMatrix(2, 1);
            y[0, 0] = -1*this.D;
            y[1, 0] = -1*intersectingPlane.D;

            Matrix<double> pointOnIntersectionLine = svd.Solve(y);
            var throughPoint = new Point3D(pointOnIntersectionLine.Column(0));

            var direction = new UnitVector3D(svd.VT.Row(2));

            return new Ray3D(throughPoint, direction);
        }

        /// <summary>
        /// Find intersection between Line3D and Plane
        /// http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <param name="line"></param>
        /// <param name="tolerance"></param>
        /// <returns>Intersection Point or null</returns>
        public Point3D? IntersectionWith(Line3D line, double tolerance = float.Epsilon)
        {
            if (line.Direction.IsPerpendicularTo(this.Normal)) //either parallel or lies in the plane
            {
                Point3D projectedPoint = this.Project(line.StartPoint, line.Direction);
                if (projectedPoint == line.StartPoint) //Line lies in the plane
                {
                    throw new InvalidOperationException("Line lies in the plane"); //Not sure what should be done here
                }
                else // Line and plane are parallel
                {
                    return null;
                }
            }
            var d = SignedDistanceTo(line.StartPoint);
            var u = line.StartPoint.VectorTo(line.EndPoint);
            var t = -1 * d / u.DotProduct(this.Normal);
            if (t > 1 || t < 0) // They are not intersected
            {
                return null;
            }
            return line.StartPoint + (t * u);
        }

        /// <summary>
        /// http://www.cs.princeton.edu/courses/archive/fall00/cs426/lectures/raycast/sld017.htm
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Point3D IntersectionWith(Ray3D ray, double tolerance = float.Epsilon)
        {
            var d = SignedDistanceTo(ray.ThroughPoint);
            var t = -1*d/ray.Direction.DotProduct(this.Normal);
            return ray.ThroughPoint + (t*ray.Direction);
        }

        public Point3D MirrorAbout(Point3D p)
        {
            Point3D p2 = Project(p);
            double d = SignedDistanceTo(p);
            return p2 - (1*d*this.Normal);
        }

        public Plane Rotate(UnitVector3D aboutVector, Angle angle)
        {
            var rootPoint = this.RootPoint;
            var rotatedPoint = rootPoint.Rotate(aboutVector, angle);
            var rotatedPlaneVector = this.Normal.Rotate(aboutVector, angle);
            return new Plane(rotatedPlaneVector, rotatedPoint);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Plane other)
        {
            return this.RootPoint == other.RootPoint && this.Normal == other.Normal;
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

            return obj is Plane && this.Equals((Plane)obj);
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
                int result = this.A.GetHashCode();
                result = (result*397) ^ this.C.GetHashCode();
                result = (result*397) ^ this.B.GetHashCode();
                result = (result*397) ^ this.D.GetHashCode();
                return result;
            }
        }

        public static Point3D PointFromPlanes(Plane plane1, Plane plane2, Plane plane3)
        {
            return Point3D.IntersectionOf(plane1, plane2, plane3);
        }

        public override string ToString()
        {
            return string.Format("A:{0} B:{1} C:{2} D:{3}", Math.Round(this.A, 4), Math.Round(this.B, 4), Math.Round(this.C, 4), Math.Round(this.D, 4));
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            XmlExt.SetReadonlyField(ref this, l => l.RootPoint, Point3D.ReadFrom(e.SingleElement("RootPoint").CreateReader()));
            XmlExt.SetReadonlyField(ref this, l => l.Normal, UnitVector3D.ReadFrom(e.SingleElement("Normal").CreateReader()));
            XmlExt.SetReadonlyField(ref this, l => l.D, -this.RootPoint.ToVector3D().DotProduct(this.Normal));
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElement("RootPoint", this.RootPoint);
            writer.WriteElement("Normal", this.Normal);
        }
    }
}
