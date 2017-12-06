namespace MathNet.Spatial.Euclidean
{
    using System;
    using Numerics;
    using Numerics.LinearAlgebra.Double;
    using Units;

    /// <summary>Quaternion Number</summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Quaternion
    /// http://mathworld.wolfram.com/Quaternion.html
    /// http://web.cs.iastate.edu/~cs577/handouts/quaternion.pdf
    /// http://www.lce.hut.fi/~ssarkka/pub/quat.pdf
    /// </remarks>
    public struct Quaternion : IEquatable<Quaternion>, IFormattable
    {
        /// <summary>
        /// Neutral element for multiplication
        /// </summary>
        public static readonly Quaternion One = new Quaternion(1, 0, 0, 0);

        /// <summary>
        /// Neutral element for sum
        /// </summary>
        public static readonly Quaternion Zero = new Quaternion(0, 0, 0, 0);

        private readonly double w; // real part
        private readonly double x;
        private readonly double y;
        private readonly double z; // imaginary part

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        public Quaternion(double real, double imagX, double imagY, double imagZ)
        {
            this.x = imagX;
            this.y = imagY;
            this.z = imagZ;
            this.w = real;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// Given a Vector (w,x,y,z), transforms it into a Quaternion = w+xi+yj+zk
        /// </summary>
        /// <param name="v">The vector to transform into a Quaternion</param>
        public Quaternion(DenseVector v)
            : this(v[0], v[1], v[2], v[3])
        {
        }

        /// <summary>
        /// Gets the real part of the quaternion.
        /// </summary>
        public double Real => this.w;

        /// <summary>
        /// Gets the imaginary X part (coefficient of complex I) of the quaternion.
        /// </summary>
        public double ImagX => this.x;

        /// <summary>
        /// Gets the imaginary Y part (coefficient of complex J) of the quaternion.
        /// </summary>
        public double ImagY => this.y;

        /// <summary>
        /// Gets the imaginary Z part (coefficient of complex K) of the quaternion.
        /// </summary>
        public double ImagZ => this.z;

        /// <summary>
        /// Gets the the sum of the squares of the four components.
        /// </summary>
        public double NormSquared => ToNormSquared(this.Real, this.ImagX, this.ImagY, this.ImagZ);

        /// <summary>
        /// Gets the norm of the quaternion q: square root of the sum of the squares of the four components.
        /// </summary>
        public double Norm => Math.Sqrt(this.NormSquared);

        /// <summary>
        /// Gets the argument phi = arg(q) of the quaternion q, such that q = r*(cos(phi) +
        /// u*sin(phi)) = r*exp(phi*u) where r is the absolute and u the unit vector of
        /// q.
        /// </summary>
        public double Arg => Math.Acos(this.Real / this.Norm);

        /// <summary>
        /// True if the quaternion q is of length |q| = 1.
        /// </summary>
        /// <remarks>
        /// To normalize a quaternion to a length of 1, use the <see cref="Normalized"/> method.
        /// All unit quaternions form a 3-sphere.
        /// </remarks>
        public bool IsUnitQuaternion => this.NormSquared.AlmostEqual(1);

        /// <summary>
        /// Returns a new Quaternion q with the Scalar part only.
        /// If you need a Double, use the Real-Field instead.
        /// </summary>
        public Quaternion Scalar => new Quaternion(this.w, 0, 0, 0);

        /// <summary>
        /// Returns a new Quaternion q with the Vector part only.
        /// </summary>
        public Quaternion Vector => new Quaternion(0, this.x, this.y, this.z);

        /// <summary>
        /// Returns a new normalized Quaternion u with the Vector part only, such that ||u|| = 1.
        /// Q may then be represented as q = r*(cos(phi) + u*sin(phi)) = r*exp(phi*u) where r is the absolute and phi the argument of q.
        /// </summary>
        public Quaternion NormalizedVector => ToUnitQuaternion(0, this.x, this.y, this.z);

        /// <summary>
        /// Returns a new normalized Quaternion q with the direction of this quaternion.
        /// </summary>
        public Quaternion Normalized => this == Zero ? this : ToUnitQuaternion(this.w, this.x, this.y, this.z);

        /// <summary>
        /// Inverts this quaternion. Inversing Zero returns Zero
        /// </summary>
        public Quaternion Inversed
        {
            get
            {
                if (this == Zero)
                {
                    return this;
                }

                var normSquared = this.NormSquared;
                return new Quaternion(this.w / normSquared, -this.x / normSquared, -this.y / normSquared, -this.z / normSquared);
            }
        }

        public bool IsNan => double.IsNaN(this.Real) ||
                             double.IsNaN(this.ImagX) ||
                             double.IsNaN(this.ImagY) ||
                             double.IsNaN(this.ImagZ);

        public bool IsInfinity => double.IsInfinity(this.Real) ||
                                  double.IsInfinity(this.ImagX) ||
                                  double.IsInfinity(this.ImagY) ||
                                  double.IsInfinity(this.ImagZ);

        /////// <summary>
        /////// Returns a new Quaternion q with the Sign of the components.
        /////// </summary>
        /////// <returns>
        /////// <list type="bullet">
        /////// <item>1 if Positive</item>
        /////// <item>0 if Neutral</item>
        /////// <item>-1 if Negative</item>
        /////// </list>
        /////// </returns>
        ////public Quaternion ComponentSigns()
        ////{
        ////    return new Quaternion(
        ////        Math.Sign(_x),
        ////        Math.Sign(_y),
        ////        Math.Sign(_z),
        ////        Math.Sign(_w));
        ////}

        /// <summary>
        /// (nop)
        /// </summary>
        public static Quaternion operator +(Quaternion q)
        {
            return q;
        }

        /// <summary>
        /// Negate a quaternion.
        /// </summary>
        public static Quaternion operator -(Quaternion q)
        {
            return q.Negate();
        }

        /// <summary>
        /// Add a quaternion to a quaternion.
        /// </summary>
        public static Quaternion operator +(Quaternion r, Quaternion q)
        {
            return new Quaternion(r.w + q.w, r.x + q.x, r.y + q.y, r.z + q.z);
        }

        /// <summary>
        /// Add a floating point number to a quaternion.
        /// </summary>
        public static Quaternion operator +(Quaternion q1, double d)
        {
            return new Quaternion(q1.Real + d, q1.ImagX, q1.ImagY, q1.ImagZ);
        }

        /// <summary>
        /// Add a quaternion to a floating point number.
        /// </summary>
        public static Quaternion operator +(double d, Quaternion q1)
        {
            return q1 + d;
        }

        /// <summary>
        /// Subtract a quaternion from a quaternion.
        /// </summary>
        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.w - q2.w, q1.x - q2.x, q1.y - q2.y, q1.z - q2.z);
        }

        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        public static Quaternion operator -(double d, Quaternion q)
        {
            return new Quaternion(d - q.Real, q.x, q.y, q.z);
        }

        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        public static Quaternion operator -(Quaternion q, double d)
        {
            return new Quaternion(q.Real - d, q.x, q.y, q.z);
        }

        /// <summary>
        /// Multiply a quaternion with a quaternion.
        /// </summary>
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            var ci = (q1.x * q2.w) + (q1.y * q2.z) - (q1.z * q2.y) + (q1.w * q2.x);
            var cj = (-q1.x * q2.z) + (q1.y * q2.w) + (q1.z * q2.x) + (q1.w * q2.y);
            var ck = (q1.x * q2.y) - (q1.y * q2.x) + (q1.z * q2.w) + (q1.w * q2.z);
            var cr = (-q1.x * q2.x) - (q1.y * q2.y) - (q1.z * q2.z) + (q1.w * q2.w);
            return new Quaternion(cr, ci, cj, ck);
        }

        /// <summary>
        /// Multiply a floating point number with a quaternion.
        /// </summary>
        public static Quaternion operator *(Quaternion q1, double d)
        {
            return new Quaternion(q1.Real * d, q1.ImagX * d, q1.ImagY * d, q1.ImagZ * d);
        }

        /// <summary>
        /// Multiply a floating point number with a quaternion.
        /// </summary>
        public static Quaternion operator *(double d, Quaternion q1)
        {
            return new Quaternion(q1.Real * d, q1.ImagX * d, q1.ImagY * d, q1.ImagZ * d);
        }

        /// <summary>
        /// Divide a quaternion by a quaternion.
        /// </summary>
        public static Quaternion operator /(Quaternion q, Quaternion r)
        {
            if (r == Zero)
            {
                if (q == Zero)
                {
                    return new Quaternion(double.NaN, double.NaN, double.NaN, double.NaN);
                }

                return new Quaternion(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            }

            var normSquared = r.NormSquared;
            var t0 = ((r.w * q.w) + (r.x * q.x) + (r.y * q.y) + (r.z * q.z)) / normSquared;
            var t1 = ((r.w * q.x) - (r.x * q.w) - (r.y * q.z) + (r.z * q.y)) / normSquared;
            var t2 = ((r.w * q.y) + (r.x * q.z) - (r.y * q.w) - (r.z * q.x)) / normSquared;
            var t3 = ((r.w * q.z) - (r.x * q.y) + (r.y * q.x) - (r.z * q.w)) / normSquared;
            return new Quaternion(t0, t1, t2, t3);
        }

        /// <summary>
        /// Divide a quaternion by a floating point number.
        /// </summary>
        public static Quaternion operator /(Quaternion q1, double d)
        {
            return new Quaternion(q1.Real / d, q1.ImagX / d, q1.ImagY / d, q1.ImagZ / d);
        }

        /// <summary>
        /// Raise a quaternion to a quaternion.
        /// </summary>
        public static Quaternion operator ^(Quaternion q1, Quaternion q2)
        {
            return q1.Pow(q2);
        }

        /// <summary>
        /// Raise a quaternion to a floating point number.
        /// </summary>
        public static Quaternion operator ^(Quaternion q1, double d)
        {
            return q1.Pow(d);
        }

        /// <summary>
        /// Equality operator for two quaternions
        /// </summary>
        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            return q1.Equals(q2);
        }

        /// <summary>
        /// Equality operator for quaternion and double
        /// </summary>
        public static bool operator ==(Quaternion q, double d)
        {
            return q.Real.AlmostEqual(d)
                   && q.ImagX.AlmostEqual(0)
                   && q.ImagY.AlmostEqual(0)
                   && q.ImagZ.AlmostEqual(0);
        }

        /// <summary>
        /// Equality operator for quaternion and double
        /// </summary>
        public static bool operator ==(double d, Quaternion q)
        {
            return q.Real.AlmostEqual(d)
                   && q.ImagX.AlmostEqual(0)
                   && q.ImagY.AlmostEqual(0)
                   && q.ImagZ.AlmostEqual(0);
        }

        /// <summary>
        /// Inequality operator for two quaternions
        /// </summary>
        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            return !(q1 == q2);
        }

        /// <summary>
        /// Inequality operator for quaternion and double
        /// </summary>
        public static bool operator !=(Quaternion q1, double d)
        {
            return !(q1 == d);
        }

        /// <summary>
        /// Inequality operator for quaternion and double
        /// </summary>
        public static bool operator !=(double d, Quaternion q1)
        {
            return !(q1 == d);
        }

        /// <summary>
        /// The quaternion expresses a relationship between two coordinate frames, A and B say. This relationship, if
        /// expressed using Euler angles, is as follows:
        /// 1) Rotate frame A about its z axis by angle gamma;
        /// 2) Rotate the resulting frame about its (new) y axis by angle beta;
        /// 3) Rotate the resulting frame about its (new) x axis by angle alpha, to arrive at frame B.
        /// </summary>
        /// <returns></returns>
        public EulerAngles ToEulerAngles()
        {
            return new EulerAngles(
                Angle.FromRadians(Math.Atan2(2 * ((this.w * this.x) + (this.y * this.z)), (this.w * this.w) + (this.z * this.z) - (this.x * this.x) - (this.y * this.y))),
                Angle.FromRadians(Math.Asin(2 * ((this.w * this.y) - (this.x * this.z)))),
                Angle.FromRadians(Math.Atan2(2 * ((this.w * this.z) + (this.x * this.y)), (this.w * this.w) + (this.x * this.x) - (this.y * this.y) - (this.z * this.z))));
        }

        /// <summary>
        /// Roatates the provided rotation quaternion with this quaternion
        /// </summary>
        /// <param name="rotation">The rotation quaternion to rotate</param>
        /// <returns></returns>
        public Quaternion RotateRotationQuaternion(Quaternion rotation)
        {
            if (!rotation.IsUnitQuaternion)
            {
                throw new ArgumentException("The quaternion provided is not a rotation", nameof(rotation));
            }

            return rotation * this;
        }

        /// <summary>
        /// Roatates the provided unit quaternion with this quaternion
        /// </summary>
        /// <param name="unitQuaternion">The unit quaternion to rotate</param>
        /// <returns></returns>
        public Quaternion RotateUnitQuaternion(Quaternion unitQuaternion)
        {
            if (!this.IsUnitQuaternion)
            {
                throw new InvalidOperationException("You cannot rotate with this quaternion as it is not a Unit Quaternion");
            }

            if (!unitQuaternion.IsUnitQuaternion)
            {
                throw new ArgumentException("The quaternion provided is not a Unit Quaternion", nameof(unitQuaternion));
            }

            return (this * unitQuaternion) * this.Conjugate();
        }

        /// <summary>
        /// Negate this quaternion.
        /// </summary>
        public Quaternion Negate()
        {
            return new Quaternion(-this.w, -this.x, -this.y, -this.z);
        }

        /// <summary>
        /// Returns the distance |a-b| of two quaternions, forming a metric space.
        /// </summary>
        public static double Distance(Quaternion a, Quaternion b)
        {
            return (a - b).Norm;
        }

        /// <summary>
        /// Conjugate this quaternion.
        /// </summary>
        public Quaternion Conjugate()
        {
            return new Quaternion(this.w, -this.x, -this.y, -this.z);
        }

        /// <summary>
        /// Logarithm to a given base.
        /// </summary>
        public Quaternion Log(double lbase)
        {
            return this.Log() / Math.Log(lbase);
        }

        /// <summary>
        /// Natural Logarithm to base E.
        /// </summary>
        public Quaternion Log()
        {
            if (this == One)
            {
                return One;
            }

            var quat = this.NormalizedVector * this.Arg;
            return new Quaternion(Math.Log(this.Norm), quat.ImagX, quat.ImagY, quat.ImagZ);
        }

        /// <summary>
        /// Common Logarithm to base 10.
        /// </summary>
        public Quaternion Log10()
        {
            return this.Log() / Math.Log(10);
        }

        /// <summary>
        /// Exponential Function.
        /// </summary>
        /// <returns></returns>
        public Quaternion Exp()
        {
            var real = Math.Pow(Math.E, this.Real);
            var vector = this.Vector;
            var vectorNorm = vector.Norm;
            var cos = Math.Cos(vectorNorm);
            var sgn = vector == Zero ? Zero : vector / vectorNorm;
            var sin = Math.Sin(vectorNorm);
            return real * (cos + (sgn * sin));
        }

        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        /// <remarks>
        /// This algorithm is not very accurate and works only for normalized quaternions
        /// </remarks>
        public Quaternion Pow(double power)
        {
            if (this == Zero)
            {
                return Zero;
            }

            if (this == One)
            {
                return One;
            }

            return (power * this.Log()).Exp();
        }

        public Quaternion Pow(int power)
        {
            var quat = new Quaternion(this.Real, this.ImagX, this.ImagY, this.ImagZ);
            if (power == 0)
            {
                return One;
            }

            if (power == 1)
            {
                return this;
            }

            if (this == Zero || this == One)
            {
                return this;
            }

            return quat * quat.Pow(power - 1);
        }

        /// <summary>
        /// Returns cos(n*arccos(x)) = 2*Cos((n-1)arccos(x))cos(arccos(x)) - cos((n-2)*arccos(x))
        /// </summary>
        public static double ChybyshevCosPoli(int n, double x)
        {
            if (n == 0)
            {
                return 1.0;
            }

            if (n == 1)
            {
                return x;
            }

            return (2 * ChybyshevCosPoli(n - 1, x) * x) - ChybyshevCosPoli(n - 2, x);
        }

        /// <summary>
        /// Returns sin(n*x)
        /// </summary>
        public static double ChybyshevSinPoli(int n, double x)
        {
            if (n == 0)
            {
                return 1;
            }

            if (n == 1)
            {
                return 2 * x;
            }

            return (2 * x * ChybyshevSinPoli(n - 1, x)) - ChybyshevSinPoli(n - 2, x);
        }

        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        public Quaternion Pow(Quaternion power)
        {
            if (this == Zero)
            {
                return Zero;
            }

            if (this == One)
            {
                return One;
            }

            return (power * this.Log()).Exp();
        }

        /// <summary>
        /// Square root of the Quaternion: q^(1/2).
        /// </summary>
        public Quaternion Sqrt()
        {
            var arg = this.Arg * 0.5;
            return this.NormalizedVector * (Math.Sin(arg) + (Math.Cos(arg) * Math.Sqrt(this.w)));
        }

        /// <summary>
        /// returns quaternion as real+ImagXi+ImagYj+ImagZk based on format provided
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(
                formatProvider,
                "{0}{1}{2}i{3}{4}j{5}{6}k",
                this.Real.ToString(format, formatProvider),
                (this.ImagX < 0) ? string.Empty : "+",
                this.ImagX.ToString(format, formatProvider),
                (this.ImagY < 0) ? string.Empty : "+",
                this.ImagY.ToString(format, formatProvider),
                (this.ImagZ < 0) ? string.Empty : "+",
                this.ImagZ.ToString(format, formatProvider));
        }

        /// <summary>
        /// returns quaternion as real+ImagXi+ImagYj+ImagZk
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                "{0}{1}{2}i{3}{4}j{5}{6}k",
                this.Real,
                (this.ImagX < 0) ? string.Empty : "+",
                this.ImagX,
                (this.ImagY < 0) ? string.Empty : "+",
                this.ImagY,
                (this.ImagZ < 0) ? string.Empty : "+",
                this.ImagZ);
        }

        /// <summary>
        /// Equality for quaternions
        /// </summary>
        public bool Equals(Quaternion other)
        {
            if ((other.IsNan && this.IsNan) ||
                (other.IsInfinity && this.IsInfinity))
            {
                return true;
            }

            return this.Real.AlmostEqual(other.Real)
                && this.ImagX.AlmostEqual(other.ImagX)
                && this.ImagY.AlmostEqual(other.ImagY)
                && this.ImagZ.AlmostEqual(other.ImagZ);
        }

        /// <summary>
        /// Equality for quaternion
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Quaternion && this.Equals((Quaternion)obj);
        }

        /// <summary>
        /// Quaternion hashcode based on all members.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.w.GetHashCode();
                hashCode = (hashCode * 397) ^ this.x.GetHashCode();
                hashCode = (hashCode * 397) ^ this.y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.z.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Calculates norm of quaternion from it's algebraical notation
        /// </summary>
        private static double ToNormSquared(double real, double imagX, double imagY, double imagZ)
        {
            return (imagX * imagX) + (imagY * imagY) + (imagZ * imagZ) + (real * real);
        }

        /// <summary>
        /// Creates unit quaternion (it's norm == 1) from it's algebraical notation
        /// </summary>
        private static Quaternion ToUnitQuaternion(double real, double imagX, double imagY, double imagZ)
        {
            var norm = Math.Sqrt(ToNormSquared(real, imagX, imagY, imagZ));
            return new Quaternion(real / norm, imagX / norm, imagY / norm, imagZ / norm);
        }
    }
}
