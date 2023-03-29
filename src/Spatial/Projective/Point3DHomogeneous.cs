﻿using System;
using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Projective
{
    /// <summary>
    /// A Point3DHomogeneous struct
    /// </summary>
    internal struct Point3DHomogeneous : IEquatable<Point3DHomogeneous>
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
        /// Initializes a new instance of the <see cref="Point3DHomogeneous"/> struct.
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <param name="z">The z value</param>
        /// <param name="w">The w value</param>
        public Point3DHomogeneous(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point3DHomogeneous"/> struct.
        /// </summary>
        /// <param name="vector">A mathnet.numerics vector</param>
        private Point3DHomogeneous(Vector<double> vector)
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
        public static Point3DHomogeneous NaN => new Point3DHomogeneous(double.NaN, double.NaN, double.NaN, double.NaN);

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified points is equal.
        /// </summary>
        /// <param name="left">The first point to compare.</param>
        /// <param name="right">The second point to compare.</param>
        /// <returns>True if the points are the same; otherwise false.</returns>
        public static bool operator ==(Point3DHomogeneous left, Point3DHomogeneous right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified points is not equal.
        /// </summary>
        /// <param name="left">The first point to compare.</param>
        /// <param name="right">The second point to compare.</param>
        /// <returns>True if the points are different; otherwise false.</returns>
        public static bool operator !=(Point3DHomogeneous left, Point3DHomogeneous right)
        {
            return !left.Equals(right);
        }

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
            return string.Format("({0}{1} {2}{1} {3}{1} {4})", X.ToString(format, numberFormatInfo), separator, Y.ToString(format, numberFormatInfo), Z.ToString(format, numberFormatInfo), W.ToString(format, numberFormatInfo));
        }

        /// <summary>
        /// Returns a value to indicate if a pair of Point3DHomogeneous are equal
        /// </summary>
        /// <param name="other">The Point3DHomogeneous to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>True if the Point3DHomogeneouses are equal; otherwise false</returns>
        public bool Equals(Point3DHomogeneous other, double tolerance)
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

        /// <inheritdoc/>
        public bool Equals(Point3DHomogeneous other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Point3DHomogeneous p && Equals(p);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

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

        /// <summary>
        /// Convert to a Math.NET Numerics dense vector of length 4.
        /// </summary>
        /// <returns> A <see cref="Vector{Double}"/> with the x, y, z and w values from this instance.</returns>
        public Vector<double> ToVector()
        {
            return Vector<double>.Build.Dense(new[] { X, Y, Z, W });
        }

        /// <summary>
        /// return new Vector3DHomogeneous(X, Y, Z, W);
        /// </summary>
        /// <returns> A <see cref="Vector3DHomogeneous"/> with the same x, y, z and w as this instance.</returns>
        public Vector3DHomogeneous ToVector3DHomogeneous()
        {
            return new Vector3DHomogeneous(X, Y, Z, W);
        }

        /// <summary>
        /// Transforms by matrix
        /// </summary>
        /// <param name="m">A transform matrix</param>
        /// <returns>A transformed Point3DHomogeneous</returns>
        public Point3DHomogeneous TransformBy(Matrix<double> m)
        {
            return new Point3DHomogeneous(m.Multiply(Vector<double>.Build.Dense(new[] { X, Y, Z, W })));
        }
    }
}
