namespace MathNet.Spatial.Projective
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Spatial.Units;

    /// <summary>
    /// An implementation of Matrix3DHomogeneous
    /// </summary>
    internal class Matrix3DHomogeneous
    {
        /// <summary>
        /// internal representation of matrix
        /// </summary>
        private readonly DenseMatrix matrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3DHomogeneous"/> class.
        /// </summary>
        public Matrix3DHomogeneous()
        {
            this.matrix = DenseMatrix.CreateIdentity(4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3DHomogeneous"/> class.
        /// </summary>
        /// <param name="matrix">An initial matrix</param>
        public Matrix3DHomogeneous(DenseMatrix matrix)
        {
            this.matrix = matrix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3DHomogeneous"/> class.
        /// </summary>
        /// <param name="m00">Element at position m[0,0]</param>
        /// <param name="m01">Element at position m[0,1]</param>
        /// <param name="m02">Element at position m[0,2]</param>
        /// <param name="m03">Element at position m[0,3]</param>
        /// <param name="m10">Element at position m[1,0]</param>
        /// <param name="m11">Element at position m[1,1]</param>
        /// <param name="m12">Element at position m[1,2]</param>
        /// <param name="m13">Element at position m[1,3]</param>
        /// <param name="m20">Element at position m[2,0]</param>
        /// <param name="m21">Element at position m[2,1]</param>
        /// <param name="m22">Element at position m[2,2]</param>
        /// <param name="m23">Element at position m[2,3]</param>
        /// <param name="m30">Element at position m[3,0]</param>
        /// <param name="m31">Element at position m[3,1]</param>
        /// <param name="m32">Element at position m[3,2]</param>
        /// <param name="m33">Element at position m[3,3]</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public Matrix3DHomogeneous(
            double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            this.matrix = DenseMatrix.CreateIdentity(4);
            this.matrix[0, 0] = m00;
            this.matrix[0, 1] = m01;
            this.matrix[0, 2] = m02;
            this.matrix[0, 3] = m03;
            this.matrix[1, 0] = m10;
            this.matrix[1, 1] = m11;
            this.matrix[1, 2] = m12;
            this.matrix[1, 3] = m13;
            this.matrix[2, 0] = m20;
            this.matrix[2, 1] = m21;
            this.matrix[2, 2] = m22;
            this.matrix[2, 3] = m23;
            this.matrix[3, 0] = m30;
            this.matrix[3, 1] = m31;
            this.matrix[3, 2] = m32;
            this.matrix[3, 3] = m33;
        }

        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="m1">The first matrix</param>
        /// <param name="m2">The second matrix</param>
        /// <returns>A Matrix3DHomogeneous</returns>
        public static Matrix3DHomogeneous operator *(Matrix3DHomogeneous m1, Matrix3DHomogeneous m2)
        {
            var result = new Matrix3DHomogeneous();
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    double element = 0;
                    for (var k = 0; k < 4; k++)
                    {
                        element += m1.matrix[i, k] * m2.matrix[k, j];
                    }

                    result.matrix[i, j] = element;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> identity matrix.</returns>
        public static Matrix3DHomogeneous Identity()
        {
            return new Matrix3DHomogeneous(DenseMatrix.CreateIdentity(4));
        }

        /// <summary>
        /// Create a translation matrix
        /// </summary>
        /// <param name="dx">The x component.</param>
        /// <param name="dy">The y component.</param>
        /// <param name="dz">The z component.</param>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the translation.</returns>
        public static Matrix3DHomogeneous CreateTranslation(double dx, double dy, double dz)
        {
            var result = new Matrix3DHomogeneous
                                         {
                                             matrix =
                                             {
                                                 [0, 3] = dx,
                                                 [1, 3] = dy,
                                                 [2, 3] = dz
                                             }
                                         };
            return result;
        }

        /// <summary>
        /// Create a translation matrix
        /// </summary>
        /// <param name="sx">The x component.</param>
        /// <param name="sy">The y component.</param>
        /// <param name="sz">The z component.</param>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the scale operation.</returns>
        public static Matrix3DHomogeneous CreateScale(double sx, double sy, double sz)
        {
            var result = new Matrix3DHomogeneous
                         {
                             matrix =
                             {
                                 [0, 0] = sx,
                                 [1, 1] = sy,
                                 [2, 2] = sz
                             }
                         };
            return result;
        }

        /// <summary>
        /// Create a matrix for rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the rotation.</returns>
        public static Matrix3DHomogeneous RotationAroundXAxis(Angle angle)
        {
            var result = new Matrix3DHomogeneous();
            var sinAngle = Math.Sin(angle.Radians);
            var cosAngle = Math.Cos(angle.Radians);
            result.matrix[1, 1] = cosAngle;
            result.matrix[1, 2] = -sinAngle;
            result.matrix[2, 1] = sinAngle;
            result.matrix[2, 2] = cosAngle;
            return result;
        }

        /// <summary>
        /// Create a matrix for rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the rotation.</returns>
        public static Matrix3DHomogeneous RotationAroundYAxis(Angle angle)
        {
            var result = new Matrix3DHomogeneous();
            var sinAngle = Math.Sin(angle.Radians);
            var cosAngle = Math.Cos(angle.Radians);
            result.matrix[0, 0] = cosAngle;
            result.matrix[0, 2] = sinAngle;
            result.matrix[2, 0] = -sinAngle;
            result.matrix[2, 2] = cosAngle;
            return result;
        }

        /// <summary>
        /// Create a matrix for rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the rotation.</returns>
        public static Matrix3DHomogeneous RotationAroundZAxis(Angle angle)
        {
            var result = new Matrix3DHomogeneous();
            var sinAngle = Math.Sin(angle.Radians);
            var cosAngle = Math.Cos(angle.Radians);
            result.matrix[0, 0] = cosAngle;
            result.matrix[0, 1] = -sinAngle;
            result.matrix[1, 0] = sinAngle;
            result.matrix[1, 1] = cosAngle;
            return result;
        }

        /// <summary>
        /// Create a matrix for reflecting about the xy-plane.
        /// </summary>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the reflection.</returns>
        public static Matrix3DHomogeneous ReflectionXY()
        {
            var result = CreateScale(1, 1, -1);
            return result;
        }

        /// <summary>
        /// Create a matrix for reflecting about the xz-plane.
        /// </summary>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the reflection.</returns>
        public static Matrix3DHomogeneous ReflectionXZ()
        {
            var result = CreateScale(1, -1, 1);
            return result;
        }

        /// <summary>
        /// Create a matrix for reflecting about the yz-plane.
        /// </summary>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the reflection.</returns>
        public static Matrix3DHomogeneous ReflectionYZ()
        {
            var result = CreateScale(-1, 1, 1);
            return result;
        }

        /// <summary>
        /// Create a front view projection matrix
        /// </summary>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the projection.</returns>
        public static Matrix3DHomogeneous FrontView()
        {
            var result = new Matrix3DHomogeneous();
            result.matrix[2, 2] = 0;
            return result;
        }

        /// <summary>
        /// Create a side view projection matrix
        /// </summary>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the projection.</returns>
        public static Matrix3DHomogeneous SideView()
        {
            var result = new Matrix3DHomogeneous();
            result.matrix[0, 0] = 0;
            result.matrix[2, 2] = 0;
            result.matrix[0, 2] = -1;
            return result;
        }

        /// <summary>
        /// Create a top view projection matrix
        /// </summary>
        /// <returns>A <see cref="Matrix3DHomogeneous"/> describing the projection.</returns>
        public static Matrix3DHomogeneous TopView()
        {
            var result = new Matrix3DHomogeneous();
            result.matrix[1, 1] = 0;
            result.matrix[2, 2] = 0;
            result.matrix[1, 2] = -1;
            return result;
        }

        /// <summary>
        /// Create an axonometric projection matrix
        /// </summary>
        /// <param name="alpha">The first angle</param>
        /// <param name="beta">The second angle</param>
        /// <returns>A Matrix3DHomogeneous</returns>
        public static Matrix3DHomogeneous Axonometric(Angle alpha, Angle beta)
        {
            var result = new Matrix3DHomogeneous();
            var sna = Math.Sin(alpha.Radians);
            var cosAlpha = Math.Cos(alpha.Radians);
            var sinBeta = Math.Sin(beta.Radians);
            var cosBeta = Math.Cos(beta.Radians);
            result.matrix[0, 0] = cosBeta;
            result.matrix[0, 2] = sinBeta;
            result.matrix[1, 0] = sna * sinBeta;
            result.matrix[1, 1] = cosAlpha;
            result.matrix[1, 2] = -sna * cosBeta;
            result.matrix[2, 2] = 0;
            return result;
        }

        /// <summary>
        /// Oblique projection matrix
        /// </summary>
        /// <param name="alpha">The first angle</param>
        /// <param name="theta">The second angle</param>
        /// <returns>A Matrix3DHomogeneous</returns>
        public static Matrix3DHomogeneous Oblique(Angle alpha, Angle theta)
        {
            var result = new Matrix3DHomogeneous();
            var tanAlpha = Math.Tan(alpha.Radians);
            var sinTheta = Math.Sin(theta.Radians);
            var cosTheta = Math.Cos(theta.Radians);
            result.matrix[0, 2] = -cosTheta / tanAlpha;
            result.matrix[1, 2] = -sinTheta / tanAlpha;
            result.matrix[2, 2] = 0;
            return result;
        }

        /// <summary>
        /// Create matrix from Euler Angles
        /// </summary>
        /// <param name="alpha">The first angle</param>
        /// <param name="beta">The second angle</param>
        /// <param name="gamma">The third angle</param>
        /// <returns>A Matrix3DHomogeneous</returns>
        public static Matrix3DHomogeneous Euler(Angle alpha, Angle beta, Angle gamma)
        {
            var result = new Matrix3DHomogeneous();

            var sinAlpha = Math.Sin(alpha.Radians);
            var cosAlpha = Math.Cos(alpha.Radians);
            var sinBeta = Math.Sin(beta.Radians);
            var cosBeta = Math.Cos(beta.Radians);
            var sinGamma = Math.Sin(gamma.Radians);
            var cosGamma = Math.Cos(gamma.Radians);

            result.matrix[0, 0] = (cosAlpha * cosGamma) - (sinAlpha * sinBeta * sinGamma);
            result.matrix[0, 1] = -sinBeta * sinGamma;
            result.matrix[0, 2] = (sinAlpha * cosGamma) - (cosAlpha * cosBeta * sinGamma);
            result.matrix[1, 0] = -sinAlpha * sinBeta;
            result.matrix[1, 1] = cosBeta;
            result.matrix[1, 2] = cosAlpha * sinBeta;
            result.matrix[2, 0] = (-cosAlpha * sinGamma) - (sinAlpha * cosBeta * cosGamma);
            result.matrix[2, 1] = -sinBeta * cosGamma;
            result.matrix[2, 2] = (cosAlpha * cosBeta * cosGamma) - (sinAlpha * sinBeta);
            return result;
        }

        /// <summary>
        /// Create matrix from azimuth and elevation
        /// </summary>
        /// <param name="elevation">The elevation</param>
        /// <param name="azimuth">The azimuth</param>
        /// <param name="oneOverd">The inverse of d</param>
        /// <returns>A Matrix3DHomogeneous</returns>
        public static Matrix3DHomogeneous AzimuthElevation(double elevation, double azimuth, double oneOverd)
        {
            var result = new Matrix3DHomogeneous();
            var rotate = new Matrix3DHomogeneous();

            // make sure elevation in the range of [-90, 90]:
            if (elevation > 90)
            {
                elevation = 90;
            }
            else if (elevation < -90)
            {
                elevation = -90;
            }

            // Make sure azimuth in the range of [-180, 180]:
            if (azimuth > 180)
            {
                azimuth = 180;
            }
            else if (azimuth < -180)
            {
                azimuth = -180;
            }

            elevation = elevation * Math.PI / 180.0f;
            var sne = Math.Sin(elevation);
            var cne = Math.Cos(elevation);
            azimuth = azimuth * Math.PI / 180.0f;
            var sna = Math.Sin(azimuth);
            var cna = Math.Cos(azimuth);
            rotate.matrix[0, 0] = cna;
            rotate.matrix[0, 1] = sna;
            rotate.matrix[0, 2] = 0;
            rotate.matrix[1, 0] = -sne * sna;
            rotate.matrix[1, 1] = sne * cna;
            rotate.matrix[1, 2] = cne;
            rotate.matrix[2, 0] = cne * sna;
            rotate.matrix[2, 1] = -cne * cna;
            rotate.matrix[2, 2] = sne;
            if (oneOverd <= 0)
            {
                result = rotate;
            }
            else if (oneOverd > 0)
            {
                // Point3DHomogeneous perspective = Point3DHomogeneous.Perspective(1 / oneOverd);
                // result = perspective * rotate;
            }

            return result;
        }
    }
}
