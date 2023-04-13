using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A unit vector, this is used to describe a direction in 3D
    /// </summary>
    [Serializable]
    public struct Direction : IXmlSerializable, IEquatable<Direction>, IEquatable<Vector3D>, IFormattable
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
        /// Initializes a new instance of the <see cref="Direction"/> struct.
        /// The provided values are scaled to L2 norm == 1
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        private Direction(double x, double y, double z)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
            {
                throw new ArgumentOutOfRangeException(nameof(x), x, "Invalid value.");
            }

            if (double.IsNaN(y) || double.IsInfinity(y))
            {
                throw new ArgumentOutOfRangeException(nameof(y), y, "Invalid value.");
            }

            if (double.IsNaN(z) || double.IsInfinity(z))
            {
                throw new ArgumentOutOfRangeException(nameof(z), z, "Invalid value.");
            }

            var norm = Math.Sqrt((x * x) + (y * y) + (z * z));
            if (norm < float.Epsilon)
            {
                throw new ArgumentException("l < float.Epsilon");
            }

            X = x / norm;
            Y = y / norm;
            Z = z / norm;
        }

        /// <summary>
        /// Gets the X axis
        /// </summary>
        public static Direction XAxis { get; } = Create(1, 0, 0);

        /// <summary>
        /// Gets the Y axis
        /// </summary>
        public static Direction YAxis { get; } = Create(0, 1, 0);

        /// <summary>
        /// Gets the z Axis
        /// </summary>
        public static Direction ZAxis { get; } = Create(0, 0, 1);

        /// <summary>
        /// Gets a vector orthogonal to this
        /// </summary>
        [Pure]
        public Direction Orthogonal
        {
            get
            {
                if (-X - Y > 0.1)
                {
                    return Create(Z, Z, -X - Y);
                }

                return Create(-Y - Z, X, X);
            }
        }

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified vectors is equal.
        /// </summary>
        /// <param name="left">The first vector to compare</param>
        /// <param name="right">The second vector to compare</param>
        /// <returns>True if the vectors are the same; otherwise false.</returns>
        public static bool operator ==(Direction left, Direction right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified vectors is equal.
        /// </summary>
        /// <param name="left">The first vector to compare</param>
        /// <param name="right">The second vector to compare</param>
        /// <returns>True if the vectors are the same; otherwise false.</returns>
        public static bool operator ==(Vector3D left, Direction right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified vectors is equal.
        /// </summary>
        /// <param name="left">The first vector to compare</param>
        /// <param name="right">The second vector to compare</param>
        /// <returns>True if the vectors are the same; otherwise false.</returns>
        public static bool operator ==(Direction left, Vector3D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified vectors is not equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are different; otherwise false.</returns>
        public static bool operator !=(Direction left, Direction right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified vectors is not equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are different; otherwise false.</returns>
        public static bool operator !=(Vector3D left, Direction right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified vectors is not equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are different; otherwise false.</returns>
        public static bool operator !=(Direction left, Vector3D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Adds two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>A new summed vector</returns>
        public static Vector3D operator +(Direction v1, Direction v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        /// <summary>
        /// Adds two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>A new summed vector</returns>
        public static Vector3D operator +(Vector3D v1, Direction v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        /// <summary>
        /// Adds two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>A new summed vector</returns>
        public static Vector3D operator +(Direction v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        /// <summary>
        /// Subtracts two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>A new difference vector</returns>
        public static Vector3D operator -(Direction v1, Direction v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        /// <summary>
        /// Subtracts two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>A new difference vector</returns>
        public static Vector3D operator -(Vector3D v1, Direction v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        /// <summary>
        /// Subtracts two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>A new difference vector</returns>
        public static Vector3D operator -(Direction v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        /// <summary>
        /// Negates the vector
        /// </summary>
        /// <param name="v">A vector to negate</param>
        /// <returns>A new negated vector</returns>
        public static Vector3D operator -(Direction v)
        {
            return new Vector3D(-1 * v.X, -1 * v.Y, -1 * v.Z);
        }

        /// <summary>
        /// Multiplies a vector by a scalar
        /// </summary>
        /// <param name="d">A scalar</param>
        /// <param name="v">A vector</param>
        /// <returns>A scaled vector</returns>
        public static Vector3D operator *(double d, Direction v)
        {
            return new Vector3D(d * v.X, d * v.Y, d * v.Z);
        }

        /// <summary>
        /// Divides a vector by a scalar
        /// </summary>
        /// <param name="v">A vector</param>
        /// <param name="d">A scalar</param>
        /// <returns>A scaled vector</returns>
        public static Vector3D operator /(Direction v, double d)
        {
            return new Vector3D(v.X / d, v.Y / d, v.Z / d);
        }

        /// <summary>
        /// Returns the dot product of two vectors
        /// </summary>
        /// <param name="left">The first vector</param>
        /// <param name="right">The second vector</param>
        /// <returns>A scalar result</returns>
        public static double operator *(Direction left, Direction right)
        {
            return left.DotProduct(right);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direction"/> struct.
        /// The provided values are scaled to L2 norm == 1
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        /// <param name="tolerance">The allowed deviation from 1 for the L2-norm of x,y,z</param>
        /// <returns>The <see cref="Direction"/></returns>
        public static Direction Create(double x, double y, double z, double tolerance = double.PositiveInfinity)
        {
            var norm = Math.Sqrt((x * x) + (y * y) + (z * z));
            if (norm < float.Epsilon)
            {
                throw new InvalidOperationException("The Euclidean norm of x, y, z is less than float.Epsilon");
            }

            if (Math.Abs(norm - 1) > tolerance)
            {
                throw new InvalidOperationException("The Euclidean norm of x, y, z differs more than tolerance from 1");
            }

            return new Direction(x / norm, y / norm, z / norm);
        }

        /// <summary>
        /// Create a new <see cref="Direction"/> from a Math.NET Numerics vector of length 3.
        /// </summary>
        /// <param name="vector"> A vector with length 3 to populate the created instance with.</param>
        /// <returns> A <see cref="Direction"/></returns>
        public static Direction OfVector(Vector<double> vector)
        {
            if (vector.Count != 3)
            {
                throw new ArgumentException("The vector length must be 3 in order to convert it to a Vector3D");
            }

            return Create(vector.At(0), vector.At(1), vector.At(2));
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a vector
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="result">A vector with the coordinates specified</param>
        /// <param name="tolerance">The tolerance for how big deviation from Length = 1 is accepted</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, out Direction result, double tolerance = 0.1)
        {
            return TryParse(text, null, out result, tolerance);
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a unit vector
        /// First it is parsed to a vector then the length of the vector is compared to the tolerance and normalized if within.
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <param name="result">A point at the coordinates specified</param>
        /// <param name="tolerance">The tolerance for how big deviation from Length = 1 is accepted</param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, IFormatProvider formatProvider, out Direction result, double tolerance = 0.1)
        {
            if (Text.TryParse3D(text, formatProvider, out var x, out var y, out var z))
            {
                var temp = new Vector3D(x, y, z);
                if (Math.Abs(temp.Length - 1) < tolerance)
                {
                    result = temp.Normalize();
                    return true;
                }
            }

            result = default(Direction);
            return false;
        }

        /// <summary>
        /// Attempts to convert a string of the form x,y,z into a vector
        /// </summary>
        /// <param name="value">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <param name="tolerance">The tolerance for how big deviation from Length = 1 is accepted</param>
        /// <returns>A point at the coordinates specified</returns>
        public static Direction Parse(string value, IFormatProvider formatProvider = null, double tolerance = 0.1)
        {
            if (TryParse(value, formatProvider, out var p, tolerance))
            {
                return p;
            }

            throw new FormatException($"Could not parse a Direction from the string {value}");
        }

        /// <summary>
        /// Creates an <see cref="Direction"/> from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> positioned at the node to read into this <see cref="Direction"/>.</param>
        /// <returns>An <see cref="Direction"/> that contains the data read from the reader.</returns>
        public static Direction ReadFrom(XmlReader reader)
        {
            return reader.ReadElementAs<Direction>();
        }

        /// <summary>
        /// Scale this instance by <paramref name="factor"/>
        /// </summary>
        /// <param name="factor">The plane to project on.</param>
        /// <returns>The projected <see cref="Line"/></returns>
        [Pure]
        public Vector3D ScaleBy(double factor)
        {
            return factor * this;
        }

        /// <summary>
        /// Project this instance onto the plane
        /// </summary>
        /// <param name="plane">The plane to project on.</param>
        /// <returns>The projected <see cref="Line"/></returns>
        [Pure]
        public Line ProjectOn(Plane plane)
        {
            return plane.Project(ToVector3D());
        }

        /// <summary>
        /// Returns the Dot product of the current vector and a unit vector
        /// </summary>
        /// <param name="uv">A unit vector</param>
        /// <returns>Returns a new vector</returns>
        [Pure]
        public Vector3D ProjectOn(Direction uv)
        {
            var pd = DotProduct(uv);
            return pd * this;
        }

        /// <summary>
        /// Computes whether or not this vector is parallel to another vector using the dot product method and comparing it
        /// to within a specified tolerance.
        /// </summary>
        /// <param name="otherVector">The other <see cref="Vector3D"/></param>
        /// <param name="tolerance">A tolerance value for the dot product method.  Values below 2*Precision.DoublePrecision may cause issues.</param>
        /// <returns>true if the vector dot product is within the given tolerance of unity, false if it is not</returns>
        [Pure]
        public bool IsParallelTo(Vector3D otherVector, double tolerance = 1e-10)
        {
            var other = otherVector.Normalize();
            return IsParallelTo(other, tolerance);
        }

        /// <summary>
        /// Computes whether or not this vector is parallel to a unit vector using the dot product method and comparing it
        /// to within a specified tolerance.
        /// </summary>
        /// <param name="otherVector">The other <see cref="Direction"/></param>
        /// <param name="tolerance">A tolerance value for the dot product method.  Values below 2*Precision.DoublePrecision may cause issues.</param>
        /// <returns>true if the vector dot product is within the given tolerance of unity, false if not</returns>
        [Pure]
        public bool IsParallelTo(Direction otherVector, double tolerance = 1e-10)
        {
            // This is the master method for all Vector3D and Direction IsParallelTo comparisons.  Everything else
            // ends up here sooner or later.
            var dp = Math.Abs(DotProduct(otherVector));
            return Math.Abs(1 - dp) <= tolerance;
        }

        /// <summary>
        /// Determine whether or not this vector is parallel to another vector within a given angle tolerance.
        /// </summary>
        /// <param name="otherVector">The other <see cref="Vector3D"/></param>
        /// <param name="angleTolerance">The tolerance for when the vectors are considered parallel.</param>
        /// <returns>true if the vectors are parallel within the angle tolerance, false if they are not</returns>
        [Pure]
        public bool IsParallelTo(Direction otherVector, Angle angleTolerance)
        {
            // Compute the angle between these vectors
            var angle = AngleTo(otherVector);

            // Compute the 180° opposite of the angle
            var opposite = Angle.FromDegrees(180) - angle;

            // Check against the smaller of the two
            return ((angle < opposite) ? angle : opposite) < angleTolerance;
        }

        /// <summary>
        /// Determine whether or not this vector is parallel to a unit vector within a given angle tolerance.
        /// </summary>
        /// <param name="otherVector">The other <see cref="Direction"/></param>
        /// <param name="angleTolerance">The tolerance for when the vectors are considered parallel.</param>
        /// <returns>true if the vectors are parallel within the angle tolerance, false if they are not</returns>
        [Pure]
        public bool IsParallelTo(Vector3D otherVector, Angle angleTolerance)
        {
            var other = otherVector.Normalize();
            return IsParallelTo(other, angleTolerance);
        }

        /// <summary>
        /// Computes whether or not this vector is perpendicular to another vector using the dot product method and
        /// comparing it to within a specified tolerance
        /// </summary>
        /// <param name="otherVector">The other <see cref="Vector3D"/></param>
        /// <param name="tolerance">A tolerance value for the dot product method.  Values below 2*Precision.DoublePrecision may cause issues.</param>
        /// <returns>true if the vector dot product is within the given tolerance of zero, false if not</returns>
        [Pure]
        public bool IsPerpendicularTo(Vector3D otherVector, double tolerance = 1e-10)
        {
            var other = otherVector.Normalize();
            return Math.Abs(DotProduct(other)) < tolerance;
        }

        /// <summary>
        /// Computes whether or not this vector is perpendicular to another vector using the dot product method and
        /// comparing it to within a specified tolerance
        /// </summary>
        /// <param name="otherVector">The other <see cref="Direction"/></param>
        /// <param name="tolerance">A tolerance value for the dot product method.  Values below 2*Precision.DoublePrecision may cause issues.</param>
        /// <returns>true if the vector dot product is within the given tolerance of zero, false if not</returns>
        [Pure]
        public bool IsPerpendicularTo(Direction otherVector, double tolerance = 1e-10)
        {
            return Math.Abs(DotProduct(otherVector)) < tolerance;
        }

        /// <summary>
        /// Inverses the direction of the vector, equivalent to multiplying by -1
        /// </summary>
        /// <returns>A <see cref="Vector3D"/> pointing in the opposite direction.</returns>
        [Pure]
        public Direction Negate()
        {
            return Create(-1 * X, -1 * Y, -1 * Z);
        }

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v">The second vector.</param>
        /// <returns>The dot product.</returns>
        [Pure]
        public double DotProduct(Vector3D v)
        {
            return (X * v.X) + (Y * v.Y) + (Z * v.Z);
        }

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v">The second vector.</param>
        /// <returns>The dot product.</returns>
        [Pure]
        public double DotProduct(Direction v)
        {
            var dp = (X * v.X) + (Y * v.Y) + (Z * v.Z);
            return Math.Max(-1, Math.Min(dp, 1));
        }

        /// <summary>
        /// Returns the cross product of this vector and a vector
        /// </summary>
        /// <param name="other">A vector</param>
        /// <returns>A new vector with the cross product result</returns>
        [Pure]
        public Direction CrossProduct(Direction other)
        {
            var x = (Y * other.Z) - (Z * other.Y);
            var y = (Z * other.X) - (X * other.Z);
            var z = (X * other.Y) - (Y * other.X);
            var v = Create(x, y, z);
            return v;
        }

        /// <summary>
        /// Returns the cross product of this vector and a unit vector
        /// </summary>
        /// <param name="other">A vector</param>
        /// <returns>A new vector with the cross product result</returns>
        [Pure]
        public Vector3D CrossProduct(Vector3D other)
        {
            if (IsParallelTo(other))
            {
                throw new InvalidOperationException($"Vector {other} is parallel to {this}");
            }

            var x = (Y * other.Z) - (Z * other.Y);
            var y = (Z * other.X) - (X * other.Z);
            var z = (X * other.Y) - (Y * other.X);
            var v = new Vector3D(x, y, z);
            return v;
        }

        /// <summary>
        /// Returns a dense Matrix with the unit tensor product
        /// </summary>
        /// <returns>a dense matrix</returns>
        [Pure]
        public Matrix<double> GetUnitTensorProduct()
        {
            // unitTensorProduct:matrix([ux^2,ux*uy,ux*uz],[ux*uy,uy^2,uy*uz],[ux*uz,uy*uz,uz^2]),
            var xy = X * Y;
            var xz = X * Z;
            var yz = Y * Z;
            return Matrix<double>.Build.Dense(3, 3, new[] { X * X, xy, xz, xy, Y * Y, yz, xz, yz, Z * Z });
        }

        /// <summary>
        /// Returns signed angle
        /// </summary>
        /// <param name="v">The vector to calculate the signed angle to </param>
        /// <param name="about">The vector around which to rotate to get the correct sign</param>
        /// <returns>A signed Angle</returns>
        [Pure]
        public Angle SignedAngleTo(Vector3D v, Direction about)
        {
            return SignedAngleTo(v.Normalize(), about);
        }

        /// <summary>
        /// Returns signed angle
        /// </summary>
        /// <param name="v">The vector to calculate the signed angle to </param>
        /// <param name="about">The vector around which to rotate to get the correct sign</param>
        /// <returns>A signed Angle</returns>
        [Pure]
        public Angle SignedAngleTo(Direction v, Direction about)
        {
            if (IsParallelTo(about))
            {
                throw new ArgumentException("FromVector parallel to aboutVector");
            }

            if (v.IsParallelTo(about))
            {
                throw new ArgumentException("FromVector parallel to aboutVector");
            }

            var rp = new Plane(Point3D.Origin, about);
            var pfv = ProjectOn(rp).Direction;
            var ptv = v.ProjectOn(rp).Direction;
            var dp = pfv.DotProduct(ptv);
            if (Math.Abs(dp - 1) < 1E-15)
            {
                return Angle.FromRadians(0);
            }

            if (Math.Abs(dp + 1) < 1E-15)
            {
                return Angle.FromRadians(Math.PI);
            }

            var angle = Math.Acos(dp);
            var cpv = pfv.CrossProduct(ptv);
            var sign = cpv.DotProduct(rp.Normal);
            var signedAngle = sign * angle;
            return Angle.FromRadians(signedAngle);
        }

        /// <summary>
        /// The nearest angle between the vectors
        /// </summary>
        /// <param name="v">The other vector</param>
        /// <returns>The angle</returns>
        [Pure]
        public Angle AngleTo(Vector3D v)
        {
            return AngleTo(v.Normalize());
        }

        /// <summary>
        /// Compute the angle between this vector and a unit vector using the arccosine of the dot product.
        /// </summary>
        /// <param name="v">The other vector</param>
        /// <returns>The angle between the vectors, with a range between 0° and 180°</returns>
        [Pure]
        public Angle AngleTo(Direction v)
        {
            var dp = DotProduct(v);
            var angle = Math.Acos(dp);
            return Angle.FromRadians(angle);
        }

        /// <summary>
        /// Returns a vector that is this vector rotated the signed angle around the about vector
        /// </summary>
        /// <param name="about">The vector to rotate around.</param>
        /// <param name="angle">The angle positive according to right hand rule.</param>
        /// <returns>A rotated vector.</returns>
        [Pure]
        public Direction Rotate(Direction about, Angle angle)
        {
            var cs = CoordinateSystem.Rotation(angle, about);
            return cs.Transform(this);
        }

        /// <summary>
        /// Returns a point equivalent to the vector
        /// </summary>
        /// <returns>A point</returns>
        [Pure]
        public Point3D ToPoint3D()
        {
            return new Point3D(X, Y, Z);
        }

        /// <summary>
        /// Returns a Vector3D equivalent to this unit vector
        /// </summary>
        /// <returns>A vector</returns>
        [Pure]
        public Vector3D ToVector3D()
        {
            return new Vector3D(X, Y, Z);
        }

        /// <summary>
        /// Transforms the vector by a coordinate system and returns the transformed.
        /// </summary>
        /// <param name="coordinateSystem">A coordinate system</param>
        /// <returns>A new transformed vector</returns>
        [Pure]
        public Vector3D TransformBy(CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Transform(ToVector3D());
        }

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 3.
        /// </summary>
        /// <returns>A dense vector</returns>
        [Pure]
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { X, Y, Z });
        }

        /// <inheritdoc />
        [Pure]
        public override string ToString()
        {
            return ToString("G15", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation of this instance using the provided <see cref="IFormatProvider"/>
        /// </summary>
        /// <param name="provider">A <see cref="IFormatProvider"/></param>
        /// <returns>The string representation of this instance.</returns>
        [Pure]
        public string ToString(IFormatProvider provider)
        {
            return ToString("G15", provider);
        }

        /// <inheritdoc/>
        [Pure]
        public string ToString(string format, IFormatProvider provider = null)
        {
            var numberFormatInfo = provider != null ? NumberFormatInfo.GetInstance(provider) : CultureInfo.InvariantCulture.NumberFormat;
            var separator = numberFormatInfo.NumberDecimalSeparator == "," ? ";" : ",";
            return string.Format("({0}{1} {2}{1} {3})", X.ToString(format, numberFormatInfo), separator, Y.ToString(format, numberFormatInfo), Z.ToString(format, numberFormatInfo));
        }

        /// <summary>
        /// Returns a value to indicate if a pair of vectors are equal
        /// </summary>
        /// <param name="other">The vector to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>True if the vectors are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Direction other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - X) < tolerance &&
                   Math.Abs(other.Y - Y) < tolerance &&
                   Math.Abs(other.Z - Z) < tolerance;
        }

        /// <summary>
        /// Returns a value to indicate if a pair of vectors are equal
        /// </summary>
        /// <param name="other">The vector to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>True if the vectors are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Vector3D other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - X) < tolerance &&
                   Math.Abs(other.Y - Y) < tolerance &&
                   Math.Abs(other.Z - Z) < tolerance;
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Vector3D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Direction other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj)
        {
            return (obj is Direction u && Equals(u)) || (obj is Vector3D v && Equals(v));
        }

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            bool TryGetUnitVector(double xv, double yv, double zv, out Direction result)
            {
                var temp = new Vector3D(xv, yv, zv);
                if (Math.Abs(temp.Length - 1) < 1E-3)
                {
                    result = temp.Normalize();
                    return true;
                }

                result = default(Direction);
                return false;
            }

            if (reader.TryReadAttributeAsDouble("X", out var x) &&
                reader.TryReadAttributeAsDouble("Y", out var y) &&
                reader.TryReadAttributeAsDouble("Z", out var z) &&
                TryGetUnitVector(x, y, z, out var uv))
            {
                reader.Skip();
                this = uv;
                return;
            }

            if (reader.TryReadChildElementsAsDoubles("X", "Y", "Z", out x, out y, out z) &&
                TryGetUnitVector(x, y, z, out uv))
            {
                reader.Skip();
                this = uv;
                return;
            }

            throw new XmlException($"Could not read a {GetType()}");
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("X", X, "G17");
            writer.WriteElement("Y", Y, "G17");
            writer.WriteElement("Z", Z, "G17");
        }

        /// <summary>
        /// Returns the dot product of this vector with a second
        /// </summary>
        /// <param name="v">a second vector</param>
        /// <returns>The dot product</returns>
        [Pure]
        internal double DotProduct(Point3D v)
        {
            return (X * v.X) + (Y * v.Y) + (Z * v.Z);
        }
    }
}
