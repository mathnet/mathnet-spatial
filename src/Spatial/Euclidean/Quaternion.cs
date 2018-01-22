namespace MathNet.Spatial.Euclidean
{
    using System;
    using System.Diagnostics.Contracts;
    using MathNet.Spatial.Internals;
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

        /// <summary>
        /// Specifies the rotation component of the Quaternion.
        /// </summary>
        private readonly double w; // real part

        /// <summary>
        /// Specifies the X-value of the vector component of the Quaternion
        /// </summary>
        private readonly double x;

        /// <summary>
        /// Specifies the Y-value of the vector component of the Quaternion
        /// </summary>
        private readonly double y;

        /// <summary>
        /// Specifies the Z-value of the vector component of the Quaternion
        /// </summary>
        private readonly double z; // imaginary part

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="real">The rotation component of the Quaternion</param>
        /// <param name="imagX">The X-value of the vector component of the Quaternion</param>
        /// <param name="imagY">The Y-value of the vector component of the Quaternion</param>
        /// <param name="imagZ">The Z-value of the vector component of the Quaternion</param>
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
        /// Gets the sum of the squares of the four components.
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
        /// Gets a value indicating whether the quaternion q has length |q| = 1.
        /// </summary>
        /// <remarks>
        /// To normalize a quaternion to a length of 1, use the <see cref="Normalized"/> method.
        /// All unit quaternions form a 3-sphere.
        /// </remarks>
        public bool IsUnitQuaternion => this.NormSquared.AlmostEqual(1);

        /// <summary>
        /// Gets a new Quaternion q with the Scalar part only.
        /// If you need a Double, use the Real-Field instead.
        /// </summary>
        public Quaternion Scalar => new Quaternion(this.w, 0, 0, 0);

        /// <summary>
        /// Gets a new Quaternion q with the Vector part only.
        /// </summary>
        public Quaternion Vector => new Quaternion(0, this.x, this.y, this.z);

        /// <summary>
        /// Gets a new normalized Quaternion u with the Vector part only, such that ||u|| = 1.
        /// Q may then be represented as q = r*(cos(phi) + u*sin(phi)) = r*exp(phi*u) where r is the absolute and phi the argument of q.
        /// </summary>
        public Quaternion NormalizedVector => ToUnitQuaternion(0, this.x, this.y, this.z);

        /// <summary>
        /// Gets a new normalized Quaternion q with the direction of this quaternion.
        /// </summary>
        public Quaternion Normalized => this == Zero ? this : ToUnitQuaternion(this.w, this.x, this.y, this.z);

        /// <summary>
        /// Gets an inverted quaternion. Inversing Zero returns Zero
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

        /// <summary>
        /// Gets a value indicating whether the quaternion is not a number
        /// </summary>
        public bool IsNan => double.IsNaN(this.Real) ||
                             double.IsNaN(this.ImagX) ||
                             double.IsNaN(this.ImagY) ||
                             double.IsNaN(this.ImagZ);

        /// <summary>
        /// Gets a value indicating whether the quaternion is not a number
        /// </summary>
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
        /// Negate a quaternion.
        /// </summary>
        /// <param name="q">The quaternion to negate</param>
        /// <returns>A negated quaternion</returns>
        public static Quaternion operator -(Quaternion q)
        {
            return q.Negate();
        }

        /// <summary>
        /// Add a quaternion to a quaternion.
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <returns>The sum of two quaternions</returns>
        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.w + q2.w, q1.x + q2.x, q1.y + q2.y, q1.z + q2.z);
        }

        /// <summary>
        /// Add a floating point number to a quaternion.
        /// </summary>
        /// <param name="q1">a quaternion</param>
        /// <param name="d">a number to add</param>
        /// <returns>A quaternion whose real value is increased by a scalar</returns>
        public static Quaternion operator +(Quaternion q1, double d)
        {
            return new Quaternion(q1.Real + d, q1.ImagX, q1.ImagY, q1.ImagZ);
        }

        /// <summary>
        /// Add a quaternion to a floating point number.
        /// </summary>
        /// <param name="d">a number to add</param>
        /// <param name="q">a quaternion</param>
        /// <returns>A quaternion whose real value is increased by a scalar</returns>
        public static Quaternion operator +(double d, Quaternion q)
        {
            return q + d;
        }

        /// <summary>
        /// Subtract a quaternion from a quaternion.
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <returns>The quaternion difference</returns>
        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.w - q2.w, q1.x - q2.x, q1.y - q2.y, q1.z - q2.z);
        }

        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        /// <param name="d">a number to subtract</param>
        /// <param name="q">a quaternion</param>
        /// <returns>A new quaternion</returns>
        public static Quaternion operator -(double d, Quaternion q)
        {
            return new Quaternion(d - q.Real, q.x, q.y, q.z);
        }

        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        /// <param name="q">a quaternion</param>
        /// <param name="d">a number to subtract</param>
        /// <returns>A new quaternion</returns>
        public static Quaternion operator -(Quaternion q, double d)
        {
            return new Quaternion(q.Real - d, q.x, q.y, q.z);
        }

        /// <summary>
        /// Multiply a quaternion with a quaternion.
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <returns>A new quaternion</returns>
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
        /// <param name="q">a quaternion</param>
        /// <param name="d">a scalar</param>
        /// <returns>A new quaternion</returns>
        public static Quaternion operator *(Quaternion q, double d)
        {
            return new Quaternion(q.Real * d, q.ImagX * d, q.ImagY * d, q.ImagZ * d);
        }

        /// <summary>
        /// Multiply a floating point number with a quaternion.
        /// </summary>
        /// <param name="d">a scalar</param>
        /// <param name="q">a quaternion</param>
        /// <returns>A new quaternion</returns>
        public static Quaternion operator *(double d, Quaternion q)
        {
            return new Quaternion(q.Real * d, q.ImagX * d, q.ImagY * d, q.ImagZ * d);
        }

        /// <summary>
        /// Divide a quaternion by a quaternion.
        /// </summary>
        /// <param name="q">The numerator quaternion</param>
        /// <param name="r">The denominator quaternion</param>
        /// <returns>A new divided quaternion</returns>
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
        /// <param name="q">a quaternion</param>
        /// <param name="d">a scalar</param>
        /// <returns>A new quaternion</returns>
        public static Quaternion operator /(Quaternion q, double d)
        {
            return new Quaternion(q.Real / d, q.ImagX / d, q.ImagY / d, q.ImagZ / d);
        }

        /// <summary>
        /// Raise a quaternion to a quaternion.
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <returns>A new quaternion</returns>
        public static Quaternion operator ^(Quaternion q1, Quaternion q2)
        {
            return q1.Pow(q2);
        }

        /// <summary>
        /// Raise a quaternion to a floating point number.
        /// </summary>
        /// <param name="q">a quaternion</param>
        /// <param name="d">a scalar</param>
        /// <returns>A new quaternion</returns>
        public static Quaternion operator ^(Quaternion q, double d)
        {
            return q.Pow(d);
        }

        /// <summary>
        /// Equality operator for two quaternions
        /// </summary>
        /// <param name="left">The first quaternion</param>
        /// <param name="right">The second quaternion</param>
        /// <returns>True if the quaternions are equal; otherwise false</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Equality operator for quaternion and double
        /// </summary>
        /// <param name="q">a quaternion</param>
        /// <param name="d">a scalar</param>
        /// <returns>True if the real part of the quaternion is almost equal to the double and the rest of the quaternion is almost 0; otherwise false</returns>
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
        /// <param name="d">a scalar</param>
        /// <param name="q">a quaternion</param>
        /// <returns>True if the real part of the quaternion is almost equal to the double and the rest of the quaternion is almost 0; otherwise false</returns>
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
        /// <param name="left">The first quaternion</param>
        /// <param name="right">The second quaternion</param>
        /// <returns>True if the quaternions are different; otherwise false</returns>
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Inequality operator for quaternion and double
        /// </summary>
        /// <param name="q">a quaternion</param>
        /// <param name="d">a scalar</param>
        /// <returns>False if the real part of the quaternion is almost equal to the double and the rest of the quaternion is almost 0; otherwise True</returns>
        public static bool operator !=(Quaternion q, double d)
        {
            return !(q == d);
        }

        /// <summary>
        /// Inequality operator for quaternion and double
        /// </summary>
        /// <param name="d">a scalar</param>
        /// <param name="q">a quaternion</param>
        /// <returns>False if the real part of the quaternion is almost equal to the double and the rest of the quaternion is almost 0; otherwise True</returns>
        public static bool operator !=(double d, Quaternion q)
        {
            return !(q == d);
        }

        /// <summary>
        /// Returns the distance |a-b| of two quaternions, forming a metric space.
        /// </summary>
        /// <param name="a">The first quaternion</param>
        /// <param name="b">The second quaternion</param>
        /// <returns>The distance between two quaternions.</returns>
        public static double Distance(Quaternion a, Quaternion b)
        {
            return (a - b).Norm;
        }

        /// <summary>
        /// Returns cos(n*arccos(x)) = 2*Cos((n-1)arccos(x))cos(arccos(x)) - cos((n-2)*arccos(x))
        /// </summary>
        /// <param name="n">an integer</param>
        /// <param name="x">a double</param>
        /// <returns>the polynomial result</returns>
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
        /// <param name="n">an integer</param>
        /// <param name="x">a double</param>
        /// <returns>the polynomial result</returns>
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
        /// The quaternion expresses a relationship between two coordinate frames, A and B say. This relationship, if
        /// expressed using Euler angles, is as follows:
        /// 1) Rotate frame A about its z axis by angle gamma;
        /// 2) Rotate the resulting frame about its (new) y axis by angle beta;
        /// 3) Rotate the resulting frame about its (new) x axis by angle alpha, to arrive at frame B.
        /// </summary>
        /// <returns>An EulerAngle</returns>
        public EulerAngles ToEulerAngles()
        {
            return new EulerAngles(
                Angle.FromRadians(Math.Atan2(2 * ((this.w * this.x) + (this.y * this.z)), (this.w * this.w) + (this.z * this.z) - (this.x * this.x) - (this.y * this.y))),
                Angle.FromRadians(Math.Asin(2 * ((this.w * this.y) - (this.x * this.z)))),
                Angle.FromRadians(Math.Atan2(2 * ((this.w * this.z) + (this.x * this.y)), (this.w * this.w) + (this.x * this.x) - (this.y * this.y) - (this.z * this.z))));
        }

        /// <summary>
        /// Rotates the provided rotation quaternion with this quaternion
        /// </summary>
        /// <param name="rotation">The rotation quaternion to rotate</param>
        /// <returns>A rotated quaternion</returns>
        public Quaternion RotateRotationQuaternion(Quaternion rotation)
        {
            if (!rotation.IsUnitQuaternion)
            {
                throw new ArgumentException("The quaternion provided is not a rotation", nameof(rotation));
            }

            return rotation * this;
        }

        /// <summary>
        /// Rotates the provided unit quaternion with this quaternion
        /// </summary>
        /// <param name="unitQuaternion">The unit quaternion to rotate</param>
        /// <returns>A rotated quaternion</returns>
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
        /// <returns>A new negated quaternion</returns>
        public Quaternion Negate()
        {
            return new Quaternion(-this.w, -this.x, -this.y, -this.z);
        }

        /// <summary>
        /// Conjugate this quaternion.
        /// </summary>
        /// <returns>a new conjugated quaternion</returns>
        public Quaternion Conjugate()
        {
            return new Quaternion(this.w, -this.x, -this.y, -this.z);
        }

        /// <summary>
        /// Logarithm to a given base.
        /// </summary>
        /// <param name="lbase">A base</param>
        /// <returns>A new quaternion</returns>
        public Quaternion Log(double lbase)
        {
            return this.Log() / Math.Log(lbase);
        }

        /// <summary>
        /// Natural Logarithm to base E.
        /// </summary>
        /// <returns>A new quaternion</returns>
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
        /// <returns>A new quaternion</returns>
        public Quaternion Log10()
        {
            return this.Log() / Math.Log(10);
        }

        /// <summary>
        /// Exponential Function.
        /// </summary>
        /// <returns>A new quaternion</returns>
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
        /// <param name="power">a number by which to raise the quaternion to</param>
        /// <returns>A new quaternion</returns>
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

        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        /// <param name="power">a number by which to raise the quaternion to</param>
        /// <returns>A new quaternion</returns>
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
        /// Raise the quaternion to a given power.
        /// </summary>
        /// <param name="power">a quaternion to use as the power</param>
        /// <returns>The quaternion raised to a power of another quaternion</returns>
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
        /// <returns>The square root of the quaternion</returns>
        public Quaternion Sqrt()
        {
            var arg = this.Arg * 0.5;
            return this.NormalizedVector * (Math.Sin(arg) + (Math.Cos(arg) * Math.Sqrt(this.w)));
        }

        /// <summary>
        /// returns quaternion as real+ImagXi+ImagYj+ImagZk based on format provided
        /// </summary>
        /// <param name="format">A format string to pass to the format provider</param>
        /// <param name="formatProvider">a format provider</param>
        /// <returns>A string representation of the quaternion</returns>
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
        /// <returns>a string representation of the quaternion</returns>
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
        /// Returns a value to indicate if this vector is equivalent to a given unit vector
        /// </summary>
        /// <param name="other">The unit vector to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the vectors are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Quaternion other, double tolerance)
        {
            if ((other.IsNan && this.IsNan) ||
                (other.IsInfinity && this.IsInfinity))
            {
                return true;
            }

            return Math.Abs(other.w - this.w) < tolerance
                && Math.Abs(other.x - this.x) < tolerance
                && Math.Abs(other.y - this.y) < tolerance
                && Math.Abs(other.z - this.z) < tolerance;
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Quaternion other)
        {
            if ((other.IsNan && this.IsNan) ||
                (other.IsInfinity && this.IsInfinity))
            {
                return true;
            }

            return this.w.Equals(other.w)
                && this.x.Equals(other.x)
                && this.y.Equals(other.y)
                && this.z.Equals(other.z);
        }

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Quaternion q && this.Equals(q);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(this.w, this.x, this.y, this.z);

        /// <summary>
        /// Calculates norm of quaternion from it's algebraical notation
        /// </summary>
        /// <param name="real">The rotation component of the Quaternion</param>
        /// <param name="imagX">The X-value of the vector component of the Quaternion</param>
        /// <param name="imagY">The Y-value of the vector component of the Quaternion</param>
        /// <param name="imagZ">The Z-value of the vector component of the Quaternion</param>
        /// <returns>a norm squared quaternion</returns>
        private static double ToNormSquared(double real, double imagX, double imagY, double imagZ)
        {
            return (imagX * imagX) + (imagY * imagY) + (imagZ * imagZ) + (real * real);
        }

        /// <summary>
        /// Creates unit quaternion (it's norm == 1) from it's algebraical notation
        /// </summary>
        /// <param name="real">The rotation component of the Quaternion</param>
        /// <param name="imagX">The X-value of the vector component of the Quaternion</param>
        /// <param name="imagY">The Y-value of the vector component of the Quaternion</param>
        /// <param name="imagZ">The Z-value of the vector component of the Quaternion</param>
        /// <returns>a unit quaternion</returns>
        private static Quaternion ToUnitQuaternion(double real, double imagX, double imagY, double imagZ)
        {
            var norm = Math.Sqrt(ToNormSquared(real, imagX, imagY, imagZ));
            return new Quaternion(real / norm, imagX / norm, imagY / norm, imagZ / norm);
        }
    }
}
