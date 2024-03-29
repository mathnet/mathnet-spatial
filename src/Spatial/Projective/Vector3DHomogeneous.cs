﻿using System;
using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Projective
{
    /// <summary>
    /// A Vector3DHomogeneous struct
    /// </summary>
    internal struct Vector3DHomogeneous : IEquatable<Vector3DHomogeneous>
    {
        /// <summary>
        /// Using public fields for performance.
        /// </summary>
        public readonly double X;

        /// <summary>
        /// Using public fields for performance.
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// Using public fields for performance.
        /// </summary>
        public readonly double Z;

        /// <summary>
        /// Using public fields for performance.
        /// </summary>
        public readonly double W;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3DHomogeneous"/> struct.
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <param name="z">The z value</param>
        /// <param name="w">The w value</param>
        public Vector3DHomogeneous(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3DHomogeneous"/> struct.
        /// </summary>
        /// <param name="vector">A mathnet.numerics vector</param>
        private Vector3DHomogeneous(Vector<double> vector)
        {
            if (vector.Count != 4)
            {
                throw new ArgumentException("Size must be 4");
            }
            X = vector[0];
            Y = vector[1];
            Z = vector[2];
            W = vector[3];
        }

        /// <summary>
        /// Gets a Vector3DHomogeneous with NaN values
        /// </summary>
        public static Vector3DHomogeneous NaN => new Vector3DHomogeneous(double.NaN, double.NaN, double.NaN, double.NaN);

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified vectors is equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are the same; otherwise false.</returns>
        public static bool operator ==(Vector3DHomogeneous left, Vector3DHomogeneous right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified vectors is not equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are different; otherwise false.</returns>
        public static bool operator !=(Vector3DHomogeneous left, Vector3DHomogeneous right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Create a new Vector3DHomogeneous from a Math.NET Numerics vector of length 4.
        /// </summary>
        /// <param name="vector"> A vector with length 4 to populate the created instance with.</param>
        /// <returns> A <see cref="Vector3DHomogeneous"/></returns>
        public static Vector3DHomogeneous OfVector(Vector<double> vector)
        {
            return new Vector3DHomogeneous(vector);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString("G15", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation of this instance using the provided <see cref="IFormatProvider"/>
        /// </summary>
        /// <param name="provider">A <see cref="IFormatProvider"/></param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(IFormatProvider provider)
        {
            return ToString("G15", provider);
        }

        /// <summary>
        /// Returns a string representation of this instance using the provided <see cref="IFormatProvider"/>
        /// </summary>
        /// <param name="format">A format for the string</param>
        /// <param name="provider">A <see cref="IFormatProvider"/></param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(string format, IFormatProvider provider = null)
        {
            var numberFormatInfo = provider != null ? NumberFormatInfo.GetInstance(provider) : CultureInfo.InvariantCulture.NumberFormat;
            var separator = numberFormatInfo.NumberDecimalSeparator == "," ? ";" : ",";
            return string.Format(
                "({1}{0} {2}{0} {3}{0} {4})",
                separator,
                X.ToString(format, numberFormatInfo),
                Y.ToString(format, numberFormatInfo),
                Z.ToString(format, numberFormatInfo),
                W.ToString(format, numberFormatInfo));
        }

        /// <summary>
        /// Returns a value to indicate if a pair of Vector3DHomogeneous are equal
        /// </summary>
        /// <param name="other">The Vector3DHomogeneous to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>True if the Vector3DHomogeneouses are equal; otherwise false</returns>
        public bool Equals(Vector3DHomogeneous other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - X) < tolerance &&
                   Math.Abs(other.Y - Y) < tolerance &&
                   Math.Abs(other.Z - Z) < tolerance &&
                   Math.Abs(other.W - W) < tolerance;
        }

        /// <summary>
        /// Returns a value to indicate if a pair of Vector3DHomogeneous are equal
        /// </summary>
        /// <param name="other">The Vector3DHomogeneous to compare against.</param>
        /// <returns>True if the Vector3DHomogeneouses are equal; otherwise false</returns>
        public bool Equals(Vector3DHomogeneous other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Vector3DHomogeneous v && Equals(v);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        /// <summary>
        /// return new Point3DHomogeneous(X, Y, Z, W);
        /// </summary>
        /// <returns> A <see cref="Point3DHomogeneous"/> with the same x, y, z and w as this instance.</returns>
        public Point3DHomogeneous ToPoint3DHomogeneous()
        {
            return new Point3DHomogeneous(X, Y, Z, W);
        }

        /// <summary>
        /// Transforms by matrix
        /// </summary>
        /// <param name="m">A transform matrix</param>
        /// <returns>A transformed Vector3DHomogeneous</returns>
        public Vector3DHomogeneous TransformBy(Matrix<double> m)
        {
            return new Vector3DHomogeneous(m.Multiply(ToVector()));
        }

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 4.
        /// </summary>
        /// <returns> A <see cref="Vector{Double}"/> with the x, y, z and w values from this instance.</returns>
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { X, Y, Z, W });
        }

        /// <summary>
        /// Gets a vector3D
        /// </summary>
        /// <returns>A vector</returns>
        public Vector3D ToVector3D()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (W != 0)
            {
                return new Vector3D(X / W, Y / W, Z / W);
            }

            return Vector3D.NaN;
        }
    }
}
