using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;

namespace MathNet.Spatial.Projective
{
    class Point3DHomogeneous
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
        
        public Point3DHomogeneous()
        {
        }

        public Point3DHomogeneous(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Point3DHomogeneous(IEnumerable<double> data)
            : this(data.ToArray())
        {
        }

        public Point3DHomogeneous(double[] data) : this(data[0], data[1], data[2], data[3])
        {
            if (data.Length != 4)
            {
                throw new ArgumentException("Size must be 4");
            }
        }

        public override string ToString()
        {
            return this.ToString(null, CultureInfo.InvariantCulture);
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ToString(null, provider);
        }

        public string ToString(string format, IFormatProvider provider = null)
        {
            var numberFormatInfo = provider != null ? NumberFormatInfo.GetInstance(provider) : CultureInfo.InvariantCulture.NumberFormat;
            string separator = numberFormatInfo.NumberDecimalSeparator == "," ? ";" : ",";
            return string.Format("({0}{1} {2}{1} {3}{1} {4})", this.X.ToString(format, numberFormatInfo), separator, this.Y.ToString(format, numberFormatInfo), this.Z.ToString(format, numberFormatInfo), this.W.ToString(format, numberFormatInfo));
        }

        public bool Equals(Point3DHomogeneous other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Point3DHomogeneous && this.Equals((Point3DHomogeneous)obj);
        }

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

        public static Point3DHomogeneous NaN
        {
            get { return new Point3DHomogeneous(double.NaN, double.NaN, double.NaN, double.NaN); }
        }

        /// <summary>
        /// Create a new Point3D from a Math.NET Numerics vector of length 3.
        /// </summary>
        public static Point3DHomogeneous OfVector(Vector<double> vector)
        {
            if (vector.Count != 4)
            {
                throw new ArgumentException("The vector length must be 3 in order to convert it to a Point3D");
            }

            return new Point3DHomogeneous(vector.At(0), vector.At(1), vector.At(2), vector.At(3));
        }

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 3.
        /// </summary>
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { X, Y, Z, W });
        }

        public Vector3D ToVector3D()
        {
            if (this.W != 0)
            {
                return new Vector3D(this.X / this.W, this.X / this.W, this.X / this.W);
            }
            return Vector3D.NaN;
        }

        public Vector3DHomogeneous ToVector3DHomogeneous()
        {
            return new Vector3DHomogeneous(this.X, this.Y, this.Z, this.W);
        }

        // Apply a transformation to a point
        public Point3DHomogeneous TransformBy(Matrix<double> m)
        {
           return new Point3DHomogeneous(m.Multiply(this.ToVector()));
        }
    }
}
