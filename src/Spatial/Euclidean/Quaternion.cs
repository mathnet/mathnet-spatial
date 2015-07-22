using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>Quaternion Number</summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Quaternion
    /// </remarks>
    public struct Quaternion
    {
        readonly double _w; // real part
        readonly double _x, _y, _z; // imaginary part
        readonly double _abs, _norm; // norm
        readonly double _arg; // polar notation

        /// <summary>
        /// Initializes a new instance of the Quaternion.
        /// </summary>
        public Quaternion(double real, double imagX, double imagY, double imagZ)
        {
            _x = imagX;
            _y = imagY;
            _z = imagZ;
            _w = real;
            _norm = ToNorm(real, imagX, imagY, imagZ);
            _abs = Math.Sqrt(_norm);
            _arg = Math.Acos(real/_abs);
        }

        /// <summary>
        /// Given a Vector (x,y,z,w), transforms it into a Quaternion
        /// </summary>
        /// <param name="v">The vector to transform into a Quaternion</param>
        /// <returns></returns>
        public static Quaternion From4Vector(DenseVector v)
        {
            return new Quaternion(v[3],v[0],v[1],v[2]);
        }

        /// <summary>
        /// Given a quaternion (x, y, z, w), returns the quaternion (0, 0, 0, 1)
        /// </summary>
        /// <returns>A Quaternion structure that represents the identity quaternion.</returns>
        public static Quaternion Identity()
        {
            return 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the Quaternion.
        /// </summary>
        internal Quaternion(double real, double imagX, double imagY, double imagZ, double abs, double norm, double arg)
        {
            _x = imagX;
            _y = imagY;
            _z = imagZ;
            _w = real;
            _norm = norm;
            _abs = abs;
            _arg = arg;
        }

        static double ToNorm(double real, double imagX, double imagY, double imagZ)
        {
            return (imagX*imagX) + (imagY*imagY) + (imagZ*imagZ) + (real*real);
        }

        static Quaternion ToUnitQuaternion(double real, double imagX, double imagY, double imagZ)
        {
            double abs = Math.Sqrt(ToNorm(real, imagX, imagY, imagZ));
            return new Quaternion(real/abs, imagX/abs, imagY/abs, imagZ/abs, 1, 1, Math.Acos(real/abs));
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
        /// Gets the standard euclidean length |q| = sqrt(||q||) of the quaternion q: the
        /// square root of the sum of the squares of the four components. Q may then be
        /// represented as q = r*(cos(phi) + u * sin(phi)) = r*exp(phi*u) where u is the
        /// unit vector and phi the argument of q.
        /// </summary>
        public double Abs
        {
            get { return _abs; }
        }

        /// <summary>
        /// Gets the norm ||q|| = |q|^2 of the quaternion q: the sum of the squares of the four components.
        /// </summary>
        public double Norm
        {
            get { return _norm; }
        }

        /// <summary>
        /// Gets the argument phi = arg(q) of the quaternion q, such that q = r*(cos(phi) +
        /// u * sin(phi)) = r*exp(phi*u) where r is the absolute and u the unit vector of
        /// q.
        /// </summary>
        public double Arg
        {
            get { return _arg; }
        }

        /// <summary>
        /// True if the quaternion q is of length |q| = 1.
        /// </summary>
        /// <remarks>
        /// To normalize a quaternion to a length of 1, use the <see cref="Sign"/> method.
        /// All unit quaternions form a 3-sphere.
        /// </remarks>
        public bool IsUnitQuaternion
        {
            get { return _abs.AlmostEqual(1); }
        }

        /// <summary>
        /// True if the norm of the quaternion is around 1
        /// </summary>
        public bool IsRotation
        {
            get { return _norm.AlmostEqual(1.0, Precision.DoublePrecision); }
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
        public Quaternion Scalar()
        {
            return new Quaternion(_w, 0, 0, 0);
        }

        /// <summary>
        /// Returns a new Quaternion q with the Vector part only.
        /// </summary>
        public Quaternion Vector()
        {
            return new Quaternion(0, _x, _y, _z);
        }

        /// <summary>
        /// Returns a new normalized Quaternion u with the Vector part only, such that ||u|| = 1.
        /// Q may then be represented as q = r*(cos(phi) + u * sin(phi)) = r*exp(phi*u) where r is the absolute and phi the argument of q.
        /// </summary>
        public Quaternion UnitVector()
        {
            return ToUnitQuaternion(0, _x, _y, _z);
        }

        /// <summary>
        /// Returns a new normalized Quaternion q with the direction of this quaternion.
        /// </summary>
        public Quaternion Sign()
        {
            return ToUnitQuaternion(_w, _x, _y, _z);
        }

        /// <summary>
        /// Roatates the provided rotation quaternion with this quaternion
        /// </summary>
        /// <param name="rotation">The rotation quaternion to rotate</param>
        /// <returns></returns>
        public Quaternion RotateRotationQuaternion(Quaternion rotation)
        {
            if (!rotation.IsRotation) throw new ArgumentException("The quaternion provided is not a rotation", "rotation");

            return rotation*this;
        }

        /// <summary>
        /// Roatates the provided unit quaternion with this quaternion
        /// </summary>
        /// <param name="unitQuaternion">The unit quaternion to rotate</param>
        /// <returns></returns>
        public Quaternion RotateUnitQuaternion(Quaternion unitQuaternion)
        {
            if (!IsRotation) throw new InvalidOperationException("You cannot rotate with this quaternion as it is not a rotation");
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
        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return q1.Add(q2);
        }

        /// <summary>
        /// Add a floating point number to a quaternion.
        /// </summary>
        public static Quaternion operator +(Quaternion q1, double d)
        {
            return q1.Add(d);
        }

        /// <summary>
        /// Subtract a quaternion from a quaternion.
        /// </summary>
        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return q1.Subtract(q2);
        }

        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        public static Quaternion operator -(Quaternion q1, double d)
        {
            return q1.Subtract(d);
        }

        /// <summary>
        /// Multiply a quaternion with a quaternion.
        /// </summary>
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return q1.Multiply(q2);
        }

        /// <summary>
        /// Multiply a floating point number with a quaternion.
        /// </summary>
        public static Quaternion operator *(Quaternion q1, double d)
        {
            return q1.Multiply(d);
        }

        /// <summary>
        /// Divide a quaternion by a quaternion.
        /// </summary>
        public static Quaternion operator /(Quaternion q1, Quaternion q2)
        {
            return q1.Divide(q2);
        }

        /// <summary>
        /// Divide a quaternion by a floating point number.
        /// </summary>
        public static Quaternion operator /(Quaternion q1, double d)
        {
            return q1.Divide(d);
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
        /// Convert a floating point number to a quaternion.
        /// </summary>
        public static implicit operator Quaternion(double d)
        {
            return new Quaternion(d, 0, 0, 0);
        }

        /// <summary>
        /// Add a quaternion to this quaternion.
        /// </summary>
        public Quaternion Add(Quaternion q)
        {
            return new Quaternion(_w + q._w, _x + q._x, _y + q._y, _z + q._z);
        }

        /// <summary>
        /// Add a floating point number to this quaternion.
        /// </summary>
        public Quaternion Add(double r)
        {
            return new Quaternion(_w + r, _x, _y, _z);
        }

        /// <summary>
        /// Subtract a quaternion from this quaternion.
        /// </summary>
        public Quaternion Subtract(Quaternion q)
        {
            return new Quaternion(_w - q._w, _x - q._x, _y - q._y, _z - q._z);
        }

        /// <summary>
        /// Subtract a floating point number from this quaternion.
        /// </summary>
        public Quaternion Subtract(double r)
        {
            return new Quaternion(_w - r, _x, _y, _z);
        }

        /// <summary>
        /// Negate this quaternion.
        /// </summary>
        public Quaternion Negate()
        {
            return new Quaternion(-_w, -_x, -_y, -_z, _abs, _norm, Math.PI - _arg);
        }

        /// <summary>
        /// Multiply a quaternion with this quaternion.
        /// </summary>
        public Quaternion Multiply(Quaternion q)
        {
            double ci = (+_x*q._w) + (_y*q._z) - (_z*q._y) + (_w*q._x);
            double cj = (-_x*q._z) + (_y*q._w) + (_z*q._x) + (_w*q._y);
            double ck = (+_x*q._y) - (_y*q._x) + (_z*q._w) + (_w*q._z);
            double cr = (-_x*q._x) - (_y*q._y) - (_z*q._z) + (_w*q._w);
            return new Quaternion(cr, ci, cj, ck);
        }

        /// <summary>
        /// Multiply a floating point number to this quaternion.
        /// </summary>
        public Quaternion Multiply(double d)
        {
            return new Quaternion(d*_w, d*_x, d*_y, d*_z);
        }

        /// <summary>
        /// Multiplies a Quaternion with the inverse of another
        /// Quaternion (q*q<sup>-1</sup>). Note that for Quaternions
        /// q*q<sup>-1</sup> is not the same then q<sup>-1</sup>*q,
        /// because this will lead to a rotation in the other direction.
        /// </summary>
        public Quaternion Divide(Quaternion q)
        {
            return Multiply(q.Inverse());
        }

        /// <summary>
        /// Multiplies a Quaternion with the inverse of a real number.
        /// </summary>
        /// <remarks>
        /// Its also possible to cast a double to a Quaternion and make the division
        /// afterward, but that would be more expensive.
        /// </remarks>
        public Quaternion Divide(double d)
        {
            return new Quaternion(_w/d, _x/d, _y/d, _z/d);
        }

        /// <summary>
        /// Inverts this quaternion.
        /// </summary>
        public Quaternion Inverse()
        {
            if (_abs.AlmostEqual(1))
            {
                return new Quaternion(_w, -_x, -_y, -_z);
            }

            return new Quaternion(_w/_norm, -_x/_norm, -_y/_norm, -_z/_norm);
        }

        /// <summary>
        /// Returns the distance |a-b| of two quaternions, forming a metric space.
        /// </summary>
        public static double Distance(Quaternion a, Quaternion b)
        {
            return a.Subtract(b).Abs;
        }

        /// <summary>
        /// Conjugate this quaternion.
        /// </summary>
        public Quaternion Conjugate()
        {
            return new Quaternion(_w, -_x, -_y, -_z, _abs, _norm, _arg);
        }

        /// <summary>
        /// Logarithm to a given base.
        /// </summary>
        public Quaternion Log(double lbase)
        {
            return Ln().Divide(Math.Log(lbase));
        }

        /// <summary>
        /// Natural Logarithm to base E.
        /// </summary>
        public Quaternion Ln()
        {
            return UnitVector().Multiply(_arg).Add(Math.Log(_abs));
        }

        /// <summary>
        /// Common Logarithm to base 10.
        /// </summary>
        public Quaternion Lg()
        {
            return Ln().Divide(Math.Log(10));
        }

        /// <summary>
        /// Exponential Function.
        /// </summary>
        /// <returns></returns>
        public Quaternion Exp()
        {
            double vabs = Math.Sqrt(ToNorm(0, _x, _y, _z));
            return UnitVector().Multiply(Math.Sin(vabs)).Add(Math.Cos(vabs)).Multiply(Math.Exp(_w));
        }

        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        public Quaternion Pow(double power)
        {
            double arg = power*_arg;
            return UnitVector().Multiply(Math.Sin(arg)).Add(Math.Cos(arg)).Multiply(Math.Pow(_w, power));
        }

        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        public Quaternion Pow(Quaternion power)
        {
            return power.Multiply(Ln()).Exp();
        }

        /// <summary>
        /// Square of the Quaternion q: q^2.
        /// </summary>
        public Quaternion Sqr()
        {
            double arg = _arg*2;
            return UnitVector().Multiply(Math.Sin(arg)).Add(Math.Cos(arg)).Multiply(_w*_w);
        }

        /// <summary>
        /// Square root of the Quaternion: q^(1/2).
        /// </summary>
        public Quaternion Sqrt()
        {
            double arg = _arg*0.5;
            return UnitVector().Multiply(Math.Sin(arg)).Add(Math.Cos(arg)).Multiply(Math.Sqrt(_w));
        }
    }
}
