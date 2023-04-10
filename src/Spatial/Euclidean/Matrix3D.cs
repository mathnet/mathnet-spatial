using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Helper class for working with 3D matrices
    /// </summary>
    public static class Matrix3D
    {
        /// <summary>
        /// Creates a rotation matrix around the X axis
        /// </summary>
        /// <param name="angle">The angle to rotate</param>
        /// <returns>A rotation matrix</returns>
        public static Matrix<double> RotationAroundXAxis(Angle angle)
        {
            var rotationMatrix = new DenseMatrix(3, 3)
            {
                [0, 0] = 1,
                [1, 1] = angle.Cos,
                [1, 2] = -angle.Sin,
                [2, 1] = angle.Sin,
                [2, 2] = angle.Cos
            };
            return rotationMatrix;
        }

        /// <summary>
        /// Creates a rotation matrix around the Y axis
        /// </summary>
        /// <param name="angle">The angle to rotate</param>
        /// <returns>A rotation matrix</returns>
        public static Matrix<double> RotationAroundYAxis(Angle angle)
        {
            var rotationMatrix = new DenseMatrix(3, 3)
            {
                [0, 0] = angle.Cos,
                [0, 2] = angle.Sin,
                [1, 1] = 1,
                [2, 0] = -angle.Sin,
                [2, 2] = angle.Cos
            };
            return rotationMatrix;
        }

        /// <summary>
        /// Creates a rotation matrix around the Z axis
        /// </summary>
        /// <param name="angle">The angle to rotate</param>
        /// <returns>A rotation matrix</returns>
        public static Matrix<double> RotationAroundZAxis(Angle angle)
        {
            var rotationMatrix = new DenseMatrix(3, 3)
            {
                [0, 0] = angle.Cos,
                [0, 1] = -angle.Sin,
                [1, 0] = angle.Sin,
                [1, 1] = angle.Cos,
                [2, 2] = 1
            };
            return rotationMatrix;
        }

        /// <summary>
        /// Sets to the matrix of rotation that would align the 'from' vector with the 'to' vector.
        /// The optional Axis argument may be used when the two vectors are parallel and in opposite directions to specify a specific solution, but is otherwise ignored.
        /// </summary>
        /// <param name="fromVector">Input Vector object to align from.</param>
        /// <param name="toVector">Input Vector object to align to.</param>
        /// <param name="axis">Input Vector object.</param>
        /// <returns>A transform matrix</returns>
        public static Matrix<double> RotationTo(
            Vector3D fromVector,
            Vector3D toVector,
            Direction? axis = null)
        {
            return RotationTo(fromVector.Normalize(), toVector.Normalize(), axis);
        }

        /// <summary>
        /// Sets to the matrix of rotation that would align the 'from' vector with the 'to' vector.
        /// The optional Axis argument may be used when the two vectors are parallel and in opposite directions to specify a specific solution, but is otherwise ignored.
        /// </summary>
        /// <param name="fromVector">Input Vector object to align from.</param>
        /// <param name="toVector">Input Vector object to align to.</param>
        /// <param name="axis">Input Vector object. </param>
        /// <returns>A transform matrix</returns>
        public static Matrix<double> RotationTo(Direction fromVector, Direction toVector, Direction? axis = null)
        {
            if (fromVector == toVector)
            {
                return DenseMatrix.CreateIdentity(3);
            }

            if (fromVector.IsParallelTo(toVector))
            {
                if (axis == null)
                {
                    axis = fromVector.Orthogonal;
                }
            }
            else
            {
                axis = fromVector.CrossProduct(toVector);
            }

            var signedAngleTo = fromVector.SignedAngleTo(toVector, axis.Value);
            return RotationAroundArbitraryVector(axis.Value, signedAngleTo);
        }

        /// <summary>
        /// Creates a rotation matrix around an arbitrary vector
        /// </summary>
        /// <param name="aboutVector">The vector</param>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>A transform matrix</returns>
        public static Matrix<double> RotationAroundArbitraryVector(Direction aboutVector, Angle angle)
        {
            // http://en.wikipedia.org/wiki/Rotation_matrix
            var unitTensorProduct = aboutVector.GetUnitTensorProduct();
            var crossProductMatrix = aboutVector.CrossProductMatrix;

            var r1 = DenseMatrix.CreateIdentity(3).Multiply(angle.Cos);
            var r2 = crossProductMatrix.Multiply(angle.Sin);
            var r3 = unitTensorProduct.Multiply(1 - angle.Cos);
            var totalR = r1.Add(r2).Add(r3);
            return totalR;
        }
    }
}
