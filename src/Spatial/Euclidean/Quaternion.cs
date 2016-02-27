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
        readonly double _w; // real part 
        readonly double _x, _y, _z; // imaginary part   
        /// <summary>
        /// Initializes a new instance of the Quatpow
        /// 
        /// ernion.
        /// </summary>
        public Quaternion(double real, double imagX, double imagY, double imagZ)
        {
            _x = imagX;
            _y = imagY;
            _z = imagZ;
            _w = real;
        }
        /// <summary>
        /// Given a Vector (w,x,y,z), transforms it into a Quaternion = w+xi+yj+zk
        /// </summary>
        /// <param name="v">The vector to transform into a Quaternion</param>
        public Quaternion(DenseVector v) : this(v[0], v[1], v[2], v[3])
        { }
        /// <summary>
        /// Neutral element for multiplication
        /// </summary>
        public static readonly Quaternion One = new Quaternion(1, 0, 0, 0);
        /// <summary>
        /// Neutral element for sum
        /// </summary>
        public static readonly Quaternion Zero = new Quaternion(0, 0, 0, 0);
        /// <summary>
        /// Calculates norm of quaternion from it's algebraical notation
        /// </summary> 
        private static double ToNormSquared(double real, double imagX, double imagY, double imagZ)
        {
            return (imagX*imagX) + (imagY*imagY) + (imagZ*imagZ) + (real*real);
        }
        /// <summary>
        /// Creates unit quaternion (it's norm == 1) from it's algebraical notation
        /// </summary> 
        private static Quaternion ToUnitQuaternion(double real, double imagX, double imagY, double imagZ)
        {
            double norm = Math.Sqrt(ToNormSquared(real, imagX, imagY, imagZ));
            return new Quaternion(real / norm, imagX / norm, imagY / norm, imagZ / norm);
        }

        /// <summary>
        /// Gets the real part of the quaternion.
        /// </summary>
        public double Real
        {
            get { return _w; }
        }

        /// <summary>
        /// Gets the imaginary X part (coefficient of complex I) of the quaternion.
        /// </summary>
        public double ImagX
        {
            get { return _x; }
        }

        /// <summary>
        /// Gets the imaginary Y part (coefficient of complex J) of the quaternion.
        /// </summary>
        public double ImagY
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets the imaginary Z part (coefficient of complex K) of the quaternion.
        /// </summary>
        public double ImagZ
        {
            get { return _z; }
        }

        /// <summary>
        /// Gets the the sum of the squares of the four components.
        /// </summary>
        public double NormSquared
        {
            get { return ToNormSquared(Real, ImagX, ImagY, ImagZ); }
        }

        /// <summary>
        /// Gets the norm of the quaternion q: square root of the sum of the squares of the four components.
        /// </summary>
        public double Norm
        {
            get { return Math.Sqrt(NormSquared); } //TODO : robust Norm calculation
        }

        /// <summary>
        /// Gets the argument phi = arg(q) of the quaternion q, such that q = r*(cos(phi) +
        /// u*sin(phi)) = r*exp(phi*u) where r is the absolute and u the unit vector of
        /// q.
        /// </summary>
        public double Arg
        {
            get { return Math.Acos(Real / Norm); }
        }

        /// <summary>
        /// True if the quaternion q is of length |q| = 1.
        /// </summary>
        /// <remarks>
        /// To normalize a quaternion to a length of 1, use the <see cref="Normalized"/> method.
        /// All unit quaternions form a 3-sphere.
        /// </remarks>
        public bool IsUnitQuaternion
        {
            get { return NormSquared.AlmostEqual(1); }
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
                Angle.FromRadians(Math.Atan2(2*(_w*_x + _y*_z), ((_w*_w) + (_z*_z) - (_x*_x) - (_y*_y)))),
                Angle.FromRadians(Math.Asin(2*(_w*_y - _x*_z))),
                Angle.FromRadians(Math.Atan2(2*(_w*_z + _x*_y), ((_w*_w) + (_x*_x) - (_y*_y) - (_z*_z)))));
        }

        /// <summary>
        /// Returns a new Quaternion q with the Scalar part only.
        /// If you need a Double, use the Real-Field instead.
        /// </summary>
        public Quaternion Scalar
        {
            get { return new Quaternion(_w, 0, 0, 0); }
        }

        /// <summary>
        /// Returns a new Quaternion q with the Vector part only.
        /// </summary>
        public Quaternion Vector
        {
            get { return new Quaternion(0, _x, _y, _z); }
        }

        /// <summary>
        /// Returns a new normalized Quaternion u with the Vector part only, such that ||u|| = 1.
        /// Q may then be represented as q = r*(cos(phi) + u*sin(phi)) = r*exp(phi*u) where r is the absolute and phi the argument of q.
        /// </summary>
        public Quaternion NormalizedVector
        {
            get { return ToUnitQuaternion(0, _x, _y, _z); }
        }

        /// <summary>
        /// Returns a new normalized Quaternion q with the direction of this quaternion.
        /// </summary>
        public Quaternion Normalized
        {
            get
            {
                return this == Zero ? this : ToUnitQuaternion(_w, _x, _y, _z);
            }
        }

        /// <summary>
        /// Roatates the provided rotation quaternion with this quaternion
        /// </summary>
        /// <param name="rotation">The rotation quaternion to rotate</param>
        /// <returns></returns>
        public Quaternion RotateRotationQuaternion(Quaternion rotation)
        {
            if (!rotation.IsUnitQuaternion) throw new ArgumentException("The quaternion provided is not a rotation", "rotation");
            return rotation*this;
        }

        /// <summary>
        /// Roatates the provided unit quaternion with this quaternion
        /// </summary>
        /// <param name="unitQuaternion">The unit quaternion to rotate</param>
        /// <returns></returns>
        public Quaternion RotateUnitQuaternion(Quaternion unitQuaternion)
        {
            if (!IsUnitQuaternion) throw new InvalidOperationException("You cannot rotate with this quaternion as it is not a Unit Quaternion");
            if (!unitQuaternion.IsUnitQuaternion) throw new ArgumentException("The quaternion provided is not a Unit Quaternion");

            return (this*unitQuaternion)*Conjugate();
        }

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

            return new Quaternion(r._w + q._w, r._x + q._x, r._y + q._y, r._z + q._z);
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
            return new Quaternion(q1._w - q2._w, q1._x - q2._x, q1._y - q2._y, q1._z - q2._z);
        }
        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        public static Quaternion operator -(double d, Quaternion q)
        {
            return new Quaternion(d - q.Real, q._x, q._y, q._z);
        }
        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        public static Quaternion operator -(Quaternion q, double d)
        {
            return new Quaternion(q.Real - d, q._x, q._y, q._z);
        }
        /// <summary>
        /// Multiply a quaternion with a quaternion.
        /// </summary>
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            double ci = (q1._x*q2._w) + (q1._y*q2._z) - (q1._z*q2._y) + (q1._w*q2._x);
            double cj = (-q1._x*q2._z) + (q1._y*q2._w) + (q1._z*q2._x) + (q1._w*q2._y);
            double ck = (q1._x*q2._y) - (q1._y*q2._x) + (q1._z*q2._w) + (q1._w*q2._z);
            double cr = (-q1._x*q2._x) - (q1._y*q2._y) - (q1._z*q2._z) + (q1._w*q2._w);
            return new Quaternion(cr, ci, cj, ck);
        }
        /// <summary>
        /// Multiply a floating point number with a quaternion.
        /// </summary>
        public static Quaternion operator *(Quaternion q1, double d)
        {
            return new Quaternion(q1.Real*d, q1.ImagX*d, q1.ImagY*d, q1.ImagZ*d);
        }
        /// <summary>
        /// Multiply a floating point number with a quaternion.
        /// </summary>
        public static Quaternion operator *(double d, Quaternion q1)
        {
            return new Quaternion(q1.Real*d, q1.ImagX*d, q1.ImagY*d, q1.ImagZ*d);
        }

        /// <summary>
        /// Divide a quaternion by a quaternion.
        /// </summary> 
        public static Quaternion operator /(Quaternion q, Quaternion r)
        {

            if (r == Zero)
            {
                if (q == Zero)
                    return new Quaternion(double.NaN, double.NaN, double.NaN, double.NaN);
                return new Quaternion(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            }
            double normSquared = r.NormSquared;
            var t0 = (r._w*q._w + r._x*q._x + r._y*q._y + r._z*q._z) / normSquared;
            var t1 = (r._w*q._x - r._x*q._w - r._y*q._z + r._z*q._y) / normSquared;
            var t2 = (r._w*q._y + r._x*q._z - r._y*q._w - r._z*q._x) / normSquared;
            var t3 = (r._w*q._z - r._x*q._y + r._y*q._x - r._z*q._w) / normSquared;
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
        ///// <summary>
        ///// Convert a floating point number to a quaternion.
        ///// </summary>
        //public static implicit operator Quaternion(double d)
        //{
        //    return new Quaternion(d, 0, 0, 0);
        //}
        /// <summary>
        /// Negate this quaternion.
        /// </summary>
        public Quaternion Negate()
        {
            return new Quaternion(-_w, -_x, -_y, -_z);
        }
        /// <summary>
        /// Inverts this quaternion. Inversing Zero returns Zero
        /// </summary>
        public Quaternion Inversed
        {
            get
            {
                if (this == Zero)
                    return this;
                var normSquared = NormSquared;
                return new Quaternion(_w / normSquared, -_x / normSquared, -_y / normSquared, -_z / normSquared);
            }
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
            return new Quaternion(_w, -_x, -_y, -_z);
        }

        /// <summary>
        /// Logarithm to a given base.
        /// </summary>
        public Quaternion Log(double lbase)
        {
            return Log() / Math.Log(lbase);
        }

        /// <summary>
        /// Natural Logarithm to base E.
        /// </summary>
        public Quaternion Log()
        {
            if (this == One)
                return One;
            var quat = NormalizedVector*Arg;
            return new Quaternion(Math.Log(Norm), quat.ImagX, quat.ImagY, quat.ImagZ);
        }

        /// <summary>
        /// Common Logarithm to base 10.
        /// </summary>
        public Quaternion Log10()
        {
            return Log() / Math.Log(10);
        }

        /// <summary>
        /// Exponential Function.
        /// </summary>
        /// <returns></returns>
        public Quaternion Exp()
        {
            double real = Math.Pow(Math.E, Real);
            var vector = Vector;
            double vectorNorm = vector.Norm;
            double cos = Math.Cos(vectorNorm);
            var sgn = vector == Zero ? Zero : vector / vectorNorm;
            double sin = Math.Sin(vectorNorm);
            return real*(cos + sgn*sin);
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
                return Zero;
            if (this == One)
                return One;
            return (power*Log()).Exp();
        }

        public Quaternion Pow(int power)
        {
            Quaternion quat = new Quaternion(this.Real, this.ImagX, this.ImagY, this.ImagZ);
            if (power == 0)
                return One;
            if (power == 1)
                return this;
            if (this == Zero || this == One)
                return this;
            return quat*quat.Pow(power - 1);
        }
        /// <summary>
        /// Returns cos(n*arccos(x)) = 2*Cos((n-1)arccos(x))cos(arccos(x)) - cos((n-2)*arccos(x))
        /// </summary> 
        public static double ChybyshevCosPoli(int n, double x)
        {
            if (n == 0)
                return 1.0;
            if (n == 1)
                return x;
            return 2*ChybyshevCosPoli(n - 1, x)*x - ChybyshevCosPoli(n - 2, x);
        }
        /// <summary>
        /// Returns sin(n*x)
        /// </summary> 
        public static double ChybyshevSinPoli(int n, double x)
        {
            if (n == 0)
                return 1;
            if (n == 1)
                return 2*x;
            return 2*x*ChybyshevSinPoli(n - 1, x) - ChybyshevSinPoli(n - 2, x);
        }
        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        public Quaternion Pow(Quaternion power)
        {
            if (this == Zero)
                return Zero;
            if (this == One)
                return One;
            return (power*Log()).Exp();
        }

        /// <summary>
        /// Square root of the Quaternion: q^(1/2).
        /// </summary>
        public Quaternion Sqrt()
        {
            double arg = Arg*0.5;
            return NormalizedVector*((Math.Sin(arg)) + (Math.Cos(arg))*(Math.Sqrt(_w)));
        }

        public bool IsNan
        {
            get
            {
                return
                    double.IsNaN(Real) ||
                    double.IsNaN(ImagX) ||
                    double.IsNaN(ImagY) ||
                    double.IsNaN(ImagZ);

            }
        }

        public bool IsInfinity
        {
            get
            {
                return
                    double.IsInfinity(Real) ||
                    double.IsInfinity(ImagX) ||
                    double.IsInfinity(ImagY) ||
                    double.IsInfinity(ImagZ);
            }
        }
        /// <summary>
        /// returns quaternion as real+ImagXi+ImagYj+ImagZk based on format provided 
        /// </summary> 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "{0}{1}{2}i{3}{4}j{5}{6}k",
                Real.ToString(format, formatProvider),
                (ImagX < 0) ? "" : "+",
                ImagX.ToString(format, formatProvider),
                (ImagY < 0) ? "" : "+",
                ImagY.ToString(format, formatProvider),
                (ImagZ < 0) ? "" : "+",
                ImagZ.ToString(format, formatProvider));
        }
        /// <summary>
        /// returns quaternion as real+ImagXi+ImagYj+ImagZk 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}{1}{2}i{3}{4}j{5}{6}k",
                Real,
                (ImagX < 0) ? "" : "+",
                ImagX,
                (ImagY < 0) ? "" : "+",
                ImagY,
                (ImagZ < 0) ? "" : "+",
                ImagZ);
        }
        /// <summary>
        /// Equality for quaternions
        /// </summary> 
        public bool Equals(Quaternion other)
        {
            if (other.IsNan && IsNan || other.IsInfinity && IsInfinity)
                return true;
            return Real.AlmostEqual(other.Real)
                && ImagX.AlmostEqual(other.ImagX)
                && ImagY.AlmostEqual(other.ImagY)
                && ImagZ.AlmostEqual(other.ImagZ);
        }
        /// <summary>
        /// Equality for quaternion
        /// </summary> 
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Quaternion && Equals((Quaternion)obj);
        }
        /// <summary>
        /// Quaternion hashcode based on all members.
        /// </summary> 
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _w.GetHashCode();
                hashCode = (hashCode*397) ^ _x.GetHashCode();
                hashCode = (hashCode*397) ^ _y.GetHashCode();
                hashCode = (hashCode*397) ^ _z.GetHashCode();
                return hashCode;
            }
        }
    }
}