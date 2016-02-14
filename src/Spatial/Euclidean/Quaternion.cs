

namespace MathNet.Spatial.Euclidean
{
    using System;
    using Numerics;
    using Numerics.LinearAlgebra.Single;

    public struct Quaternion32 : IFormattable, IEquatable<Quaternion32>
    {
        private readonly float a1, a2, a3, a4;
        #region constructors and creators
        /// <summary>
        /// Initializes a new instance of the Quaternion. From algebraic numbers.
        /// </summary>
        public Quaternion32(float real, float imagX, float imagY, float imagZ)
        {
            a1 = real;
            a2 = imagX;
            a3 = imagY;
            a4 = imagZ;
        }
        /// <summary>
        /// Given a Vector (x,y,z,w), transforms it into a Quaternion
        /// </summary>
        /// <param name="v">The vector to transform into a Quaternion</param>
        /// <returns></returns>
        public static Quaternion32 From4Vector(DenseVector v)
        {
            return new Quaternion32(v[3], v[0], v[1], v[2]);
        }
        #endregion
        #region operations

        public float NormSquared
        {
            get { return a1 * a1 + a2 * a2 + a3 * a3 + a4 * a4; }
        }

        public float Norm
        {
            //TODO : create more robust norm - possible overflow
            get { return (float)Math.Sqrt(NormSquared); }
        }

        public static float DotProduct(Quaternion32 q1, Quaternion32 q2)
        {
            return q1.a2 * q2.a2 + q1.a3 * q2.a3 + q1.a4 * q2.a4 + q1.a1 * q2.a1;
        }
        public Quaternion32 Inverse()
        {
            return Conjugate() / NormSquared;
        }
        public Quaternion32 Normalize()
        {
            var norm = Norm;
            return new Quaternion32(a1 / norm, a2 / norm, a3 / norm, a4 / norm);
        }
        public static Quaternion32 Normalize(Quaternion32 quat)
        {
            return quat.Normalize();
        }
        public Quaternion32 Conjugate()
        {
            return new Quaternion32(a1, -a2, -a3, -a4);
        }
        public static Quaternion32 Conjugate(Quaternion32 quat)
        {
            return quat.Conjugate();
        }
        #endregion
        #region operators
        public static Quaternion32 operator +(Quaternion32 q1, Quaternion32 q2)
        {
            return new Quaternion32(q1.a1 + q2.a1, q1.a2 + q2.a2, q1.a3 + q2.a3, q1.a4 + q2.a4);
        }

        public static Quaternion32 operator +(Quaternion32 q1, float f)
        {
            return new Quaternion32(q1.a1 + f, q1.a2, q1.a3, q1.a4);
        }

        public static Quaternion32 operator +(float f, Quaternion32 q1)
        {
            return q1 + f;
        }
        public static Quaternion32 operator -(Quaternion32 q1, Quaternion32 q2)
        {
            return new Quaternion32(q1.a1 - q2.a1, q1.a2 - q2.a2, q1.a3 - q2.a3, q1.a4 - q2.a4);
        }

        public static Quaternion32 operator -(Quaternion32 q1, float f)
        {
            return new Quaternion32(q1.a1 - f, q1.a2, q1.a3, q1.a4);
        }

        public static Quaternion32 operator -(float f, Quaternion32 q1)
        {
            return new Quaternion32(f - q1.a1, q1.a2, q1.a3, q1.a4);
        }
        public static Quaternion32 operator *(Quaternion32 q1, Quaternion32 q2)
        {
            return new Quaternion32(
                 q1.a1 * q2.a1 - q1.a2 * q2.a2 - q1.a3 * q2.a3 - q1.a4 * q2.a4,
                q1.a1 * q2.a2 + q1.a2 * q2.a1 + q1.a3 * q2.a4 - q1.a4 * q2.a3,
                q1.a1 * q2.a3 - q1.a2 * q2.a4 + q1.a3 * q2.a1 + q1.a4 * q2.a2,
                q1.a1 * q2.a4 + q1.a2 * q2.a3 - q1.a3 * q2.a2 + q1.a4 * q2.a1
                );
        }
        public static Quaternion32 operator *(Quaternion32 q1, float f)
        {
            return new Quaternion32(q1.a1 * f, q1.a2 * f, q1.a3 * f, q1.a4 * f);
        }
        public static Quaternion32 operator *(float f, Quaternion32 q1)
        {
            return q1 * f;
        }

        public static Quaternion32 operator /(Quaternion32 q1, float f)
        {
            return new Quaternion32(q1.a1 / f, q1.a2 / f, q1.a3 / f, q1.a4 / f);
        }
        public static Quaternion32 operator /(float f, Quaternion32 q1)
        {
            //TODO : More robust division - possible overflow
            return f * q1.Inverse();
        }
        public static bool operator ==(Quaternion32 q1, Quaternion32 q2)
        {
            return q1.Equals(q2);
        }

        public static bool operator !=(Quaternion32 q1, Quaternion32 q2)
        {
            return !(q1 == q2);
        }

        public static Quaternion32 operator -(Quaternion32 q)
        {
            return new Quaternion32(-q.a1, -q.a2, -q.a3, -q.a4);
        }
        #endregion
        #region static members
        /// <summary>
        /// Neutral element for multiplication
        /// </summary>
        public static readonly Quaternion32 One = new Quaternion32(1, 0, 0, 0);
        /// <summary>
        /// Neutral element for sum
        /// </summary>
        public static readonly Quaternion32 Zero = new Quaternion32(0, 0, 0, 0);
        #endregion
        #region getters
        public float Real { get { return a1; } }
        public float ImagX { get { return a2; } }
        public float ImagY { get { return a3; } }
        public float ImagZ { get { return a4; } }
        /// <summary>
        /// Returns a new Quaternion q with the Scalar part only.
        /// If you need a Double, use the Real-Field instead.
        /// </summary>
        public Quaternion32 Scalar { get { return new Quaternion32(a1, 0, 0, 0); } }
        #endregion
        public bool IsNan
        {
            get { return float.IsNaN(a1) || float.IsNaN(a2) || float.IsNaN(a3) || float.IsNaN(a4); }
        }

        public bool IsInfinity
        {
            get { return float.IsInfinity(a1) || float.IsInfinity(a2) || float.IsInfinity(a3) || float.IsInfinity(a4); }
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "({0}, {1}, {2}, {3})",
                a1.ToString(format, formatProvider),
                a2.ToString(format, formatProvider),
                a3.ToString(format, formatProvider),
                a4.ToString(format, formatProvider));
        }

        public bool Equals(Quaternion32 other)
        {
            return a1.AlmostEqual(other.a1) && a2.AlmostEqual(other.a2) && a3.AlmostEqual(other.a3) && a4.AlmostEqual(other.a4);
        }
    }
}

