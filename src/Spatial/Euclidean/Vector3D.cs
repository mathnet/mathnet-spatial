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

    [Serializable]
    public struct Vector3D : IXmlSerializable, IEquatable<Vector3D>, IEquatable<UnitVector3D>, IFormattable
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
        /// Initializes a new instance of the <see cref="Vector3D"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        [Obsolete("This constructor will be removed. Made obsolete 2017-12-05.")]
        public Vector3D(IEnumerable<double> data)
            : this(data.ToArray())
        {
        }

        [Obsolete("This constructor will be removed. Made obsolete 2017-12-05.")]
        public Vector3D(double[] data)
            : this(data[0], data[1], data[2])
        {
            if (data.Length != 3)
            {
                throw new ArgumentException("Size must be 3");
            }
        }

        public static Vector3D NaN { get; } = new Vector3D(double.NaN, double.NaN, double.NaN);

        /// <summary>
        /// Gets the Euclidean Norm.
        /// </summary>
        public double Length => Math.Sqrt((this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z));

        /// <summary>
        /// A vector orthogonal to this
        /// </summary>
        public UnitVector3D Orthogonal
        {
            get
            {
                if (-this.X - this.Y > 0.1)
                {
                    return new UnitVector3D(this.Z, this.Z, -this.X - this.Y);
                }

                return new UnitVector3D(-this.Y - this.Z, this.X, this.X);
            }
        }

        internal Matrix<double> CrossProductMatrix => Matrix<double>.Build.Dense(3, 3, new[] { 0d, this.Z, -this.Y, -this.Z, 0d, this.X, this.Y, -this.X, 0d });

        public static bool operator ==(Vector3D left, Vector3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3D left, Vector3D right)
        {
            return !left.Equals(right);
        }

        [Obsolete("Not sure this is nice")]
        public static Vector<double> operator *(Matrix<double> left, Vector3D right)
        {
            return left * right.ToVector();
        }

        [Obsolete("Not sure this is nice")]
        public static Vector<double> operator *(Vector3D left, Matrix<double> right)
        {
            return left.ToVector() * right;
        }

        public static double operator *(Vector3D left, Vector3D right)
        {
            return left.DotProduct(right);
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3D operator -(Vector3D v)
        {
            return v.Negate();
        }

        public static Vector3D operator *(double d, Vector3D v)
        {
            return new Vector3D(d * v.X, d * v.Y, d * v.Z);
        }

        public static Vector3D operator /(Vector3D v, double d)
        {
            return new Vector3D(v.X / d, v.Y / d, v.Z / d);
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a vector
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="result">A vector with the coordinates specified</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, out Vector3D result)
        {
            return TryParse(text, null, out result);
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a vector
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <param name="result">A point at the coordinates specified</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, IFormatProvider formatProvider, out Vector3D result)
        {
            if (Text.TryParse3D(text, formatProvider, out var x, out var y, out var z))
            {
                result = new Vector3D(x, y, z);
                return true;
            }

            result = default(Vector3D);
            return false;
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a vector
        /// </summary>
        /// <param name="value">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <returns>A point at the coordinates specified</returns>
        public static Vector3D Parse(string value, IFormatProvider formatProvider = null)
        {
            if (TryParse(value, formatProvider, out var p))
            {
                return p;
            }

            throw new FormatException($"Could not parse a Vector3D from the string {value}");
        }

        /// <summary>
        /// Create a new <see cref="Vector3D"/> from a Math.NET Numerics vector of length 3.
        /// </summary>
        /// <param name="vector"> A vector with length 2 to populate the created instance with.</param>
        /// <returns> A <see cref="Vector3D"/></returns>
        public static Vector3D OfVector(Vector<double> vector)
        {
            if (vector.Count != 3)
            {
                throw new ArgumentException("The vector length must be 3 in order to convert it to a Vector3D");
            }

            return new Vector3D(vector.At(0), vector.At(1), vector.At(2));
        }

        public static Vector3D ReadFrom(XmlReader reader)
        {
            return reader.ReadElementAs<Vector3D>();
        }

        ////public static explicit operator Vector3D(System.Windows.Media.Media3D.Vector3D v)
        ////{
        ////    return new Vector3D(v.X, v.Y, v.Z);
        ////}

        ////public static explicit operator System.Windows.Media.Media3D.Vector3D(Vector3D p)
        ////{
        ////    return new System.Windows.Media.Media3D.Vector3D(p.X, p.Y, p.Z);
        ////}

        /// <summary>
        /// Compute and return a unit vector from this vector
        /// </summary>
        /// <returns></returns>
        public UnitVector3D Normalize()
        {
            return UnitVector3D.Create(this.X, this.Y, this.Z);
        }

        public Vector3D ScaleBy(double scaleFactor)
        {
            return scaleFactor * this;
        }

        public Ray3D ProjectOn(Plane planeToProjectOn)
        {
            return planeToProjectOn.Project(this);
        }

        public Vector3D ProjectOn(UnitVector3D uv)
        {
            var pd = this.DotProduct(uv);
            return pd * uv;
        }

        /// <summary>
        /// Computes whether or not this vector is parallel to another vector using the dot product method and comparing it
        /// to within a specified tolerance.
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="tolerance">A tolerance value for the dot product method.  Values below 2*Precision.DoublePrecision may cause issues.</param>
        /// <returns>true if the vector dot product is within the given tolerance of unity, false if it is not</returns>
        public bool IsParallelTo(Vector3D othervector, double tolerance = 1e-10)
        {
            var @this = this.Normalize();
            return @this.IsParallelTo(othervector, tolerance);
        }

        /// <summary>
        /// Computes whether or not this vector is parallel to a unit vector using the dot product method and comparing it
        /// to within a specified tolerance.
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="tolerance">A tolerance value for the dot product method.  Values below 2*Precision.DoublePrecision may cause issues.</param>
        /// <returns>true if the vector dot product is within the given tolerance of unity, false if not</returns>
        public bool IsParallelTo(UnitVector3D othervector, double tolerance = 1e-10)
        {
            var @this = this.Normalize();
            return @this.IsParallelTo(othervector, tolerance);
        }

        /// <summary>
        /// Determine whether or not this vector is parallel to another vector within a given angle tolerance.
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="angleTolerance"></param>
        /// <returns>true if the vectors are parallel within the angle tolerance, false if they are not</returns>
        public bool IsParallelTo(Vector3D othervector, Angle angleTolerance)
        {
            var @this = this.Normalize();
            return @this.IsParallelTo(othervector, angleTolerance);
        }

        /// <summary>
        /// Determine whether or not this vector is parallel to a unit vector within a given angle tolerance.
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="angleTolerance"></param>
        /// <returns>true if the vectors are parallel within the angle tolerance, false if they are not</returns>
        public bool IsParallelTo(UnitVector3D othervector, Angle angleTolerance)
        {
            var @this = this.Normalize();
            return @this.IsParallelTo(othervector, angleTolerance);
        }

        /// <summary>
        /// Computes whether or not this vector is perpendicular to another vector using the dot product method and
        /// comparing it to within a specified tolerance
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="tolerance"></param>
        /// <returns>true if the vector dot product is within the given tolerance of zero, false if not</returns>
        public bool IsPerpendicularTo(Vector3D othervector, double tolerance = 1e-6)
        {
            var @this = this.Normalize();
            var other = othervector.Normalize();
            return Math.Abs(@this.DotProduct(other)) < tolerance;
        }

        /// <summary>
        /// Computes whether or not this vector is perpendicular to another vector using the dot product method and
        /// comparing it to within a specified tolerance
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="tolerance"></param>
        /// <returns>true if the vector dot product is within the given tolerance of zero, false if not</returns>
        public bool IsPerpendicularTo(UnitVector3D othervector, double tolerance = 1e-6)
        {
            var @this = this.Normalize();
            return Math.Abs(@this.DotProduct(othervector)) < tolerance;
        }

        /// <summary>
        /// Inverses the direction of the vector, equivalent to multiplying by -1
        /// </summary>
        /// <returns></returns>
        public Vector3D Negate()
        {
            return new Vector3D(-1 * this.X, -1 * this.Y, -1 * this.Z);
        }

        public double DotProduct(Vector3D v)
        {
            return (this.X * v.X) + (this.Y * v.Y) + (this.Z * v.Z);
        }

        public double DotProduct(UnitVector3D v)
        {
            return (this.X * v.X) + (this.Y * v.Y) + (this.Z * v.Z);
        }

        [Obsolete("Use - instead")]
        public Vector3D Subtract(Vector3D v)
        {
            return new Vector3D(this.X - v.X, this.Y - v.Y, this.Z - v.Z);
        }

        [Obsolete("Use + instead")]
        public Vector3D Add(Vector3D v)
        {
            return new Vector3D(this.X + v.X, this.Y + v.Y, this.Z + v.Z);
        }

        public Vector3D CrossProduct(Vector3D inVector3D)
        {
            var x = (this.Y * inVector3D.Z) - (this.Z * inVector3D.Y);
            var y = (this.Z * inVector3D.X) - (this.X * inVector3D.Z);
            var z = (this.X * inVector3D.Y) - (this.Y * inVector3D.X);
            var v = new Vector3D(x, y, z);
            return v;
        }

        public Vector3D CrossProduct(UnitVector3D inVector3D)
        {
            var x = (this.Y * inVector3D.Z) - (this.Z * inVector3D.Y);
            var y = (this.Z * inVector3D.X) - (this.X * inVector3D.Z);
            var z = (this.X * inVector3D.Y) - (this.Y * inVector3D.X);
            var v = new Vector3D(x, y, z);
            return v;
        }

        public Matrix<double> GetUnitTensorProduct()
        {
            // unitTensorProduct:matrix([ux^2,ux*uy,ux*uz],[ux*uy,uy^2,uy*uz],[ux*uz,uy*uz,uz^2]),
            var xy = this.X * this.Y;
            var xz = this.X * this.Z;
            var yz = this.Y * this.Z;
            return Matrix<double>.Build.Dense(3, 3, new[] { this.X * this.X, xy, xz, xy, this.Y * this.Y, yz, xz, yz, this.Z * this.Z });
        }

        /// <summary>
        /// Returns signed angle
        /// </summary>
        /// <param name="v">The vector to calculate the signed angle to </param>
        /// <param name="about">The vector around which to rotate to get the correct sign</param>
        public Angle SignedAngleTo(Vector3D v, UnitVector3D about)
        {
            return this.Normalize().SignedAngleTo(v.Normalize(), about);
        }

        /// <summary>
        /// Returns signed angle
        /// </summary>
        /// <param name="v">The vector to calculate the signed angle to </param>
        /// <param name="about">The vector around which to rotate to get the correct sign</param>
        public Angle SignedAngleTo(UnitVector3D v, UnitVector3D about)
        {
            return this.Normalize().SignedAngleTo(v, about);
        }

        /// <summary>
        /// Compute the angle between this vector and another using the arccosine of the dot product.
        /// </summary>
        /// <param name="v">The other vector</param>
        /// <returns>The angle between the vectors, with a range between 0° and 180°</returns>
        public Angle AngleTo(Vector3D v)
        {
            var uv1 = this.Normalize();
            var uv2 = v.Normalize();
            return uv1.AngleTo(uv2);
        }

        /// <summary>
        /// Compute the angle between this vector and a unit vector using the arccosine of the dot product.
        /// </summary>
        /// <param name="v">The other vector</param>
        /// <returns>The angle between the vectors, with a range between 0° and 180°</returns>
        public Angle AngleTo(UnitVector3D v)
        {
            var uv = this.Normalize();
            return uv.AngleTo(v);
        }

        /// <summary>
        /// Returns a vector that is this vector rotated the signed angle around the about vector
        /// </summary>
        /// <typeparam name="T">Constraining it like this does not box</typeparam>
        /// <param name="about"></param>
        /// <param name="angle"></param>
        /// <param name="angleUnit"></param>
        /// <returns></returns>
        public Vector3D Rotate<T>(UnitVector3D about, double angle, T angleUnit)
            where T : IAngleUnit
        {
            return this.Rotate(about, Angle.From(angle, angleUnit));
        }

        /// <summary>
        /// Returns a vector that is this vector rotated the signed angle around the about vector
        /// </summary>
        /// <param name="about"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector3D Rotate(Vector3D about, Angle angle)
        {
            return this.Rotate(about.Normalize(), angle);
        }

        /// <summary>
        /// Returns a vector that is this vector rotated the signed angle around the about vector
        /// </summary>
        /// <param name="about"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector3D Rotate(UnitVector3D about, Angle angle)
        {
            var cs = CoordinateSystem.Rotation(angle, about);
            return cs.Transform(this);
        }

        /// <summary>
        /// return new Point3D(this.X, this.Y, this.Z);
        /// </summary>
        /// <returns></returns>
        public Point3D ToPoint3D()
        {
            return new Point3D(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Transforms the vector by a coordinate system and returns the transformed.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public Vector3D TransformBy(CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Transform(this);
        }

        public Vector3D TransformBy(Matrix<double> m)
        {
            return new Vector3D(m.Multiply(this.ToVector()));
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
            return string.Format(
                "({1}{0} {2}{0} {3})",
                separator,
                this.X.ToString(format, numberFormatInfo),
                this.Y.ToString(format, numberFormatInfo),
                this.Z.ToString(format, numberFormatInfo));
        }

        /// <inheritdoc />
        public bool Equals(Vector3D other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <inheritdoc />
        public bool Equals(UnitVector3D other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public bool Equals(Vector3D other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance &&
                   Math.Abs(other.Z - this.Z) < tolerance;
        }

        public bool Equals(UnitVector3D other, double tolerance)
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

            return (obj is UnitVector3D && this.Equals((UnitVector3D)obj)) ||
                   (obj is Vector3D && this.Equals((Vector3D)obj));
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

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.TryReadAttributeAsDouble("X", out var x) &&
                reader.TryReadAttributeAsDouble("Y", out var y) &&
                reader.TryReadAttributeAsDouble("Z", out var z))
            {
                reader.Skip();
                this = new Vector3D(x, y, z);
                return;
            }

            if (reader.TryReadChildElementsAsDoubles("X", "Y", "Z", out x, out y, out z))
            {
                reader.Skip();
                this = new Vector3D(x, y, z);
                return;
            }

            throw new XmlException($"Could not read a {this.GetType()}");
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("X", this.X);
            writer.WriteAttribute("Y", this.Y);
            writer.WriteAttribute("Z", this.Z);
        }
    }
}
