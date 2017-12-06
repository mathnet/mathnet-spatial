namespace MathNet.Spatial.Euclidean
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Spatial.Internals;
    using MathNet.Spatial.Units;

    [Serializable]
    public struct Point3D : IXmlSerializable, IEquatable<Point3D>, IFormattable
    {
        /// <summary>
        /// The x component.
        /// </summary>
        public readonly double X;

        /// <summary>
        /// The y component.
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// The z component.
        /// </summary>
        public readonly double Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        [Obsolete("This constructor will be removed. Made obsolete 2017-12-05.")]
        public Point3D(IEnumerable<double> data)
            : this(data.ToArray())
        {
        }

        [Obsolete("This constructor will be removed. Made obsolete 2017-12-05.")]
        public Point3D(double[] data)
            : this(data[0], data[1], data[2])
        {
            if (data.Length != 3)
            {
                throw new ArgumentException("Size must be 3");
            }
        }

        public static Point3D Origin { get; } = new Point3D(0, 0, 0);

        public static Point3D NaN { get; } = new Point3D(double.NaN, double.NaN, double.NaN);

        [Obsolete("Not sure this is nice")]
        public static Vector<double> operator *(Matrix<double> left, Point3D right)
        {
            return left * right.ToVector();
        }

        [Obsolete("Not sure this is nice")]
        public static Vector<double> operator *(Point3D left, Matrix<double> right)
        {
            return left.ToVector() * right;
        }

        public static Point3D operator +(Point3D p, Vector3D v)
        {
            return new Point3D(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
        }

        public static Point3D operator +(Point3D p, UnitVector3D v)
        {
            return new Point3D(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
        }

        public static Point3D operator -(Point3D p, Vector3D v)
        {
            return new Point3D(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
        }

        public static Point3D operator -(Point3D p, UnitVector3D v)
        {
            return new Point3D(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
        }

        public static Vector3D operator -(Point3D lhs, Point3D rhs)
        {
            return new Vector3D(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static bool operator ==(Point3D left, Point3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point3D left, Point3D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a point
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="result">A point with the coordinates specified</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, out Point3D result)
        {
            return TryParse(text, null, out result);
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a point
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <param name="result">A point at the coordinates specified</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, IFormatProvider formatProvider, out Point3D result)
        {
            if (Text.TryParse3D(text, formatProvider, out var x, out var y, out var z))
            {
                result = new Point3D(x, y, z);
                return true;
            }

            result = default(Point3D);
            return false;
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a point
        /// </summary>
        /// <param name="value">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <returns>A point at the coordinates specified</returns>
        public static Point3D Parse(string value, IFormatProvider formatProvider = null)
        {
            if (TryParse(value, formatProvider, out var p))
            {
                return p;
            }

            throw new FormatException($"Could not parse a Point3D from the string {value}");
        }

        /// <summary>
        /// Create a new <see cref="Point3D"/> from a Math.NET Numerics vector of length 3.
        /// </summary>
        /// <param name="vector"> A vector with length 2 to populate the created instance with.</param>
        /// <returns> A <see cref="Point3D"/></returns>
        public static Point3D OfVector(Vector<double> vector)
        {
            if (vector.Count != 3)
            {
                throw new ArgumentException("The vector length must be 3 in order to convert it to a Point3D");
            }

            return new Point3D(vector.At(0), vector.At(1), vector.At(2));
        }

        public static Point3D ReadFrom(XmlReader reader)
        {
            return reader.ReadElementAs<Point3D>();
        }

        public static Point3D Centroid(IEnumerable<Point3D> points)
        {
            return Centroid(points.ToArray());
        }

        public static Point3D Centroid(params Point3D[] points)
        {
            return new Point3D(
                points.Average(point => point.X),
                points.Average(point => point.Y),
                points.Average(point => point.Z));
        }

        public static Point3D MidPoint(Point3D p1, Point3D p2)
        {
            return Centroid(p1, p2);
        }

        public static Point3D IntersectionOf(Plane plane1, Plane plane2, Plane plane3)
        {
            var ray = plane1.IntersectionWith(plane2);
            return plane3.IntersectionWith(ray);
        }

        public static Point3D IntersectionOf(Plane plane, Ray3D ray)
        {
            return plane.IntersectionWith(ray);
        }

        public Point3D MirrorAbout(Plane plane)
        {
            return plane.MirrorAbout(this);
        }

        public Point3D ProjectOn(Plane plane)
        {
            return plane.Project(this);
        }

        public Point3D Rotate(Vector3D aboutVector, Angle angle)
        {
            return this.Rotate(aboutVector.Normalize(), angle);
        }

        public Point3D Rotate(UnitVector3D aboutVector, Angle angle)
        {
            var cs = CoordinateSystem.Rotation(angle, aboutVector);
            return cs.Transform(this);
        }

        [Pure]
        public Vector3D VectorTo(Point3D p)
        {
            return p - this;
        }

        [Pure]
        public double DistanceTo(Point3D p)
        {
            var vector = this.VectorTo(p);
            return vector.Length;
        }

        [Pure]
        public Vector3D ToVector3D()
        {
            return new Vector3D(this.X, this.Y, this.Z);
        }

        [Pure]
        public Point3D TransformBy(CoordinateSystem cs)
        {
            return cs.Transform(this);
        }

        public Point3D TransformBy(Matrix<double> m)
        {
            return OfVector(m.Multiply(this.ToVector()));
        }

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 3.
        /// </summary>
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { this.X, this.Y, this.Z });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString(null, CultureInfo.InvariantCulture);
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ToString(null, provider);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider provider = null)
        {
            var numberFormatInfo = provider != null ? NumberFormatInfo.GetInstance(provider) : CultureInfo.InvariantCulture.NumberFormat;
            var separator = numberFormatInfo.NumberDecimalSeparator == "," ? ";" : ",";
            return string.Format("({0}{1} {2}{1} {3})", this.X.ToString(format, numberFormatInfo), separator, this.Y.ToString(format, numberFormatInfo), this.Z.ToString(format, numberFormatInfo));
        }

        /// <inheritdoc />
        public bool Equals(Point3D other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public bool Equals(Point3D other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance &&
                   Math.Abs(other.Z - this.Z) < tolerance;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Point3D && this.Equals((Point3D)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.X.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Z.GetHashCode();
                return hashCode;
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.TryReadAttributeAsDouble("X", out var x) &&
                reader.TryReadAttributeAsDouble("Y", out var y) &&
                reader.TryReadAttributeAsDouble("Z", out var z))
            {
                reader.Skip();
                this = new Point3D(x, y, z);
                return;
            }

            if (reader.TryReadChildElementsAsDoubles("X", "Y", "Z", out x, out y, out z))
            {
                reader.Skip();
                this = new Point3D(x, y, z);
                return;
            }

            throw new XmlException($"Could not read a {this.GetType()}");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("X", this.X);
            writer.WriteAttribute("Y", this.Y);
            writer.WriteAttribute("Z", this.Z);
        }
    }
}
