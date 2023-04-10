﻿using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Helper class for creating matrices for manipulating 2D-elements
    /// </summary>
    public static class Matrix2D
    {
        /// <summary>
        /// Creates a rotation about the z-axis
        /// </summary>
        /// <param name="rotation">The angle of rotation</param>
        /// <returns>A transform matrix</returns>
        public static DenseMatrix Rotation(Angle rotation)
        {
            var c = rotation.Cos;
            var s = rotation.Sin;
            return Create(c, -s, s, c);
        }

        /// <summary>
        /// Creates an arbitrary 2D transform
        /// </summary>
        /// <param name="m11">Element at m[1,1]</param>
        /// <param name="m12">Element at m[1,2]</param>
        /// <param name="m21">Element at m[2,1]</param>
        /// <param name="m22">Element at m[2,2]</param>
        /// <returns>A transform matrix</returns>
        public static DenseMatrix Create(double m11, double m12, double m21, double m22)
        {
            return new DenseMatrix(2, 2, new[] { m11, m21, m12, m22 });
        }
    }
}
