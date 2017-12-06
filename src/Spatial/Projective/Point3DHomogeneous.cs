namespace MathNet.Spatial.Projective
{
    using System;
    using System.Globalization;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Spatial.Euclidean;

    internal struct Point3DHomogeneous
    {
        /// <summary>
        /// Using public fields cos: http://blogs.msdn.com/b/ricom/archive/2006/08/31/performance-quiz-11-ten-questions-on-value-based-programming.aspx
        /// </summary>
        public readonly double X;

        /// <summary>
        /// Using public fields cos: http://blogs.msdn.com/b/ricom/archive/2006/08/31/performance-quiz-11-ten-questions-on-value-based-programming.aspx
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// Using public fields cos: http://blogs.msdn.com/b/ricom/archive/2006/08/31/performance-quiz-11-ten-questions-on-value-based-programming.aspx
        /// </summary>
        public readonly double Z;

        /// <summary>
        /// Using public fields cos: http://blogs.msdn.com/b/ricom/archive/2006/08/31/performance-quiz-11-ten-questions-on-value-based-programming.aspx
        /// </summary>
        public readonly double W;

        public Point3DHomogeneous(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        private Point3DHomogeneous(Vector<double> vector)
            : this(vector.At(0), vector.At(1), vector.At(2), vector.At(3))
        {
            if (vector.Count != 4)
            {
                throw new ArgumentException("Size must be 4");
            }
        }

        public static Point3DHomogeneous NaN => new Point3DHomogeneous(double.NaN, double.NaN, double.NaN, double.NaN);

        /// <summary>
        /// Create a new Point3DHomogeneous from a Math.NET Numerics vector of length 4.
        /// </summary>
        /// <param name="vector"> A vector with length 4 to populate the created instance with.</param>
        /// <returns> A <see cref="Point3DHomogeneous"/></returns>
        public static Point3DHomogeneous OfVector(Vector<double> vector)
        {
            return new Point3DHomogeneous(vector);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation of this instance using the provided <see cref="IFormatProvider"/>
        /// </summary>
        /// <param name="provider">A <see cref="IFormatProvider"/></param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(IFormatProvider provider)
        {
            return this.ToString(null, provider);
        }

        public string ToString(string format, IFormatProvider provider = null)
        {
            var numberFormatInfo = provider != null ? NumberFormatInfo.GetInstance(provider) : CultureInfo.InvariantCulture.NumberFormat;
            var separator = numberFormatInfo.NumberDecimalSeparator == "," ? ";" : ",";
            return string.Format("({0}{1} {2}{1} {3}{1} {4})", this.X.ToString(format, numberFormatInfo), separator, this.Y.ToString(format, numberFormatInfo), this.Z.ToString(format, numberFormatInfo), this.W.ToString(format, numberFormatInfo));
        }

        public bool Equals(Point3DHomogeneous other)
        {
            //// ReSharper disable CompareOfFloatsByEqualityOperator
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
            //// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public bool Equals(Point3DHomogeneous other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance &&
                   Math.Abs(other.Z - this.Z) < tolerance &&
                   Math.Abs(other.W - this.W) < tolerance;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Point3DHomogeneous homogeneous && this.Equals(homogeneous);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.X.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Z.GetHashCode();
                hashCode = (hashCode * 397) ^ this.W.GetHashCode();
                return hashCode;
            }
        }

        public Vector3D ToVector3D()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (this.W != 0)
            {
                return new Vector3D(this.X / this.W, this.X / this.W, this.X / this.W);
            }

            return Vector3D.NaN;
        }

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 4.
        /// </summary>
        /// <returns> A <see cref="Vector{Double}"/> with the x, y, z and w values from this instance.</returns>
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { this.X, this.Y, this.Z, this.W });
        }

        public Vector3DHomogeneous ToVector3DHomogeneous()
        {
            return new Vector3DHomogeneous(this.X, this.Y, this.Z, this.W);
        }

        // Apply a transformation to a point
        public Point3DHomogeneous TransformBy(Matrix<double> m)
        {
           return new Point3DHomogeneous(m.Multiply(Vector<double>.Build.Dense(new[] { this.X, this.Y, this.Z, this.W })));
        }
    }
}
