namespace MathNet.Spatial.Euclidean
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Spatial.Internals;
    using MathNet.Spatial.Units;

    /// <summary>
    /// Represents a point in 2 dimensional cartesian space
    /// </summary>
    [Serializable]
    public struct Point2D : IXmlSerializable, IEquatable<Point2D>, IFormattable
    {
        /// <summary>
        /// The x coordinate
        /// </summary>
        public readonly double X;

        /// <summary>
        /// The y coordinate
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> struct.
        /// Creates a point for given coordinates (x, y)
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        public Point2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> struct.
        /// Creates a point r from origin rotated a counterclockwise from X-Axis
        /// </summary>
        /// <param name="r">distance from origin</param>
        /// <param name="a">the angle</param>
        public Point2D(double r, Angle a)
            : this(r * Math.Cos(a.Radians), r * Math.Sin(a.Radians))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> struct.
        /// Creates a point from a list of coordinates (x, y)
        /// </summary>
        /// <param name="data">a pair of coordinates in the order x, y</param>
        /// <exception cref="ArgumentException">Exception thrown if more than 2 coordinates are passed</exception>
        [Obsolete("This constructor will be removed. Made obsolete 2017-12-03.")]
        public Point2D(IEnumerable<double> data)
            : this(data.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> struct.
        /// Creates a point from a list of coordinates (x, y)
        /// </summary>
        /// <param name="data">a pair of coordinates in the order x, y</param>
        /// <exception cref="ArgumentException">Exception thrown if more than 2 coordinates are passed</exception>
        [Obsolete("This constructor will be removed. Made obsolete 2017-12-03.")]
        public Point2D(double[] data)
            : this(data[0], data[1])
        {
            if (data.Length != 2)
            {
                throw new ArgumentException("data.Length != 2!");
            }
        }

        /// <summary>
        /// Gets a point at the origin (0,0)
        /// </summary>
        public static Point2D Origin => new Point2D(0, 0);

        public static Point2D operator +(Point2D point, Vector2D vector)
        {
            return new Point2D(point.X + vector.X, point.Y + vector.Y);
        }

        [Obsolete("This weird operator will be removed in a future version. Made obsolete 2017-12-03.")]
        public static Point3D operator +(Point2D point, Vector3D vector)
        {
            return new Point3D(point.X + vector.X, point.Y + vector.Y, vector.Z);
        }

        public static Point2D operator -(Point2D left, Vector2D right)
        {
            return new Point2D(left.X - right.X, left.Y - right.Y);
        }

        [Obsolete("This weird operator will be removed in a future version. Made obsolete 2017-12-03.")]
        public static Point3D operator -(Point2D left, Vector3D right)
        {
            return new Point3D(left.X - right.X, left.Y - right.Y, -1 * right.Z);
        }

        public static Vector2D operator -(Point2D left, Point2D right)
        {
            return new Vector2D(left.X - right.X, left.Y - right.Y);
        }

        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point2D left, Point2D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y into a point
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="result">A point at the coordinates specified</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, out Point2D result)
        {
            return TryParse(text, null, out result);
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y into a point
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <param name="result">A point at the coordinates specified</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, IFormatProvider formatProvider, out Point2D result)
        {
            if (Text.TryParse2D(text, formatProvider, out var x, out var y))
            {
                result = new Point2D(x, y);
                return true;
            }

            result = default(Point2D);
            return false;
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y into a point
        /// </summary>
        /// <param name="value">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <returns>A point at the coordinates specified</returns>
        public static Point2D Parse(string value, IFormatProvider formatProvider = null)
        {
            if (TryParse(value, formatProvider, out var p))
            {
                return p;
            }

            throw new FormatException($"Could not parse a Point2D from the string {value}");
        }

        /// <summary>
        /// Creates a point from xml with x and y coordinates either as attributes or elements
        /// </summary>
        /// <param name="reader">an xml reader</param>
        /// <returns>A point</returns>
        public static Point2D ReadFrom(XmlReader reader)
        {
            var v = default(Point2D);
            v.ReadXml(reader);
            return v;
        }

        /// <summary>
        /// Returns the centeroid or center of mass of any set of points
        /// </summary>
        /// <param name="points">a list of points</param>
        /// <returns>the centeroid point</returns>
        public static Point2D Centroid(IEnumerable<Point2D> points)
        {
            return Centroid(points.ToArray());
        }

        /// <summary>
        /// Returns the centeroid or center of mass of any set of points
        /// </summary>
        /// <param name="points">a list of points</param>
        /// <returns>the centeroid point</returns>
        public static Point2D Centroid(params Point2D[] points)
        {
            return new Point2D(
                points.Average(point => point.X),
                points.Average(point => point.Y));
        }

        /// <summary>
        /// Returns a point midway between the provided points <paramref name="point1"/> and <paramref name="point2"/>
        /// </summary>
        /// <param name="point1">point A</param>
        /// <param name="point2">point B</param>
        /// <returns>a new point midway between the provided points</returns>
        public static Point2D MidPoint(Point2D point1, Point2D point2)
        {
            return Centroid(point1, point2);
        }

        /// <summary>
        /// Create a new Point2D from a Math.NET Numerics vector of length 2.
        /// </summary>
        /// <param name="vector"> A vector with length 2 to populate the created instance with.</param>
        /// <returns> A <see cref="Point2D"/></returns>
        public static Point2D OfVector(Vector<double> vector)
        {
            if (vector.Count != 2)
            {
                throw new ArgumentException("The vector length must be 2 in order to convert it to a Point2D");
            }

            return new Point2D(vector.At(0), vector.At(1));
        }

        public Point2D TransformBy(Matrix<double> m)
        {
            return OfVector(m.Multiply(this.ToVector()));
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
            string separator = numberFormatInfo.NumberDecimalSeparator == "," ? ";" : ",";
            return string.Format("({0}{1} {2})", this.X.ToString(format, numberFormatInfo), separator, this.Y.ToString(format, numberFormatInfo));
        }

        /// <inheritdoc />
        public bool Equals(Point2D other)
        {
            //// ReSharper disable CompareOfFloatsByEqualityOperator
            return this.X == other.X && this.Y == other.Y;
            //// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public bool Equals(Point2D other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Point2D && this.Equals((Point2D)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.X.GetHashCode() * 397) ^ this.Y.GetHashCode();
            }
        }

        /// <inheritdoc />
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            this = new Point2D(
                XmlConvert.ToDouble(e.ReadAttributeOrElementOrDefault("X")),
                XmlConvert.ToDouble(e.ReadAttributeOrElementOrDefault("Y")));
        }

        /// <inheritdoc />
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("X", this.X);
            writer.WriteAttribute("Y", this.Y);
        }

        public Vector2D VectorTo(Point2D otherPoint)
        {
            return otherPoint - this;
        }

        /// <summary>
        /// Finds the straight line distance to another point
        /// </summary>
        /// <param name="otherPoint">The other point</param>
        /// <returns>a distance measure</returns>
        public double DistanceTo(Point2D otherPoint)
        {
            var vector = this.VectorTo(otherPoint);
            return vector.Length;
        }

        /// <summary>
        /// Converts this point into a vector from the origin
        /// </summary>
        /// <returns>A vector equivalent to this point</returns>
        public Vector2D ToVector2D()
        {
            return new Vector2D(this.X, this.Y);
        }

        /// <summary>
        /// return new Point3D(X, Y, 0);
        /// </summary>
        /// <returns>return new Point3D(X, Y, 0);</returns>
        public Point3D ToPoint3D()
        {
            return new Point3D(this.X, this.Y, 0);
        }

        public Point3D TransformBy(CoordinateSystem cs)
        {
            return cs.Transform(this.ToPoint3D());
        }

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 2.
        /// </summary>
        /// <returns> A <see cref="Vector{Double}"/> with the x and y values from this instance.</returns>
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { this.X, this.Y });
        }
    }
}
