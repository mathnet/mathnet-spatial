using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Projective
{
    class Matrix3DHomogeneous
    {
        public DenseMatrix Matrix;

        public Matrix3DHomogeneous()
        {
            Matrix = DenseMatrix.CreateIdentity(4);
        }

        public Matrix3DHomogeneous(double m00, double m01, double m02, double m03,
                        double m10, double m11, double m12, double m13,
                        double m20, double m21, double m22, double m23,
                        double m30, double m31, double m32, double m33)
        {
            Matrix[0, 0] = m00;
            Matrix[0, 1] = m01;
            Matrix[0, 2] = m02;
            Matrix[0, 3] = m03;
            Matrix[1, 0] = m10;
            Matrix[1, 1] = m11;
            Matrix[1, 2] = m12;
            Matrix[1, 3] = m13;
            Matrix[2, 0] = m20;
            Matrix[2, 1] = m21;
            Matrix[2, 2] = m22;
            Matrix[2, 3] = m23;
            Matrix[3, 0] = m30;
            Matrix[3, 1] = m31;
            Matrix[3, 2] = m32;
            Matrix[3, 3] = m33;
        }

        // Define a Identity matrix
        public void Identity3DHomegeneous()
        {
            Matrix = DenseMatrix.CreateIdentity(4);
        }

        // Multiply two matrices together
        public static Matrix3DHomogeneous operator *(Matrix3DHomogeneous m1, Matrix3DHomogeneous m2)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    double element = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        element += m1.Matrix[i, k] * m2.Matrix[k, j];
                    }
                    result.Matrix[i, j] = element;
                }
            }
            return result;
        }

        ////Multiply a matrix with a vector
        //public double[] VectorMultiply(double[] vector)
        //{
        //    double[] result = new double[4];
        //    for (int i = 0; i < 4; i++)
        //    {
        //        for (int j = 0; j < 4; j++)
        //        {
        //             result[i] += Matrix[i, j] * vector[j];
        //        }
        //    }
        //    return result;
        //}

        //public double[] VectorMultiply(Vector3DHomogeneous vector)
        //{
        //    double[] _vector= new double[4];
        //    _vector[0] = vector.X;
        //    _vector[1] = vector.Y;
        //    _vector[2] = vector.Z;
        //    _vector[3] = vector.W;

        //    return VectorMultiply(_vector);
        //}

        ////Multiply a matrix with a point
        //public double[] VectorMultiply(Point3DHomogeneous point)
        //{
        //    double[] _point = new double[4];
        //    _point[0] = point.X;
        //    _point[1] = point.Y;
        //    _point[2] = point.Z;
        //    _point[3] = point.W;

        //    return VectorMultiply(_point);
        //}


        //Create a translation matrix
        public static Matrix3DHomogeneous CreateTraslation(double dx, double dy, double dz)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            result.Matrix[0, 3] = dx;
            result.Matrix[1, 3] = dy;
            result.Matrix[2, 3] = dz;
            return result;
        }


        // Create a scaling matrix
        public static Matrix3DHomogeneous CreateScale(double sx, double sy, double sz)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            result.Matrix[0, 0] = sx;
            result.Matrix[1, 1] = sy;
            result.Matrix[2, 2] = sz;
            return result;
        }

        // Create a rotation matrix around the x axis:
        public static Matrix3DHomogeneous RotationAroundXAxis(Angle angle)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            double sinAngle = Math.Sin(angle.Radians);
            double cosAngle = Math.Cos(angle.Radians);           
            result.Matrix[1, 1] = cosAngle;
            result.Matrix[1, 2] = -sinAngle;
            result.Matrix[2, 1] = sinAngle;
            result.Matrix[2, 2] = cosAngle;
            return result;
        }

        // Create a rotation matrix around the y axis:
        public static Matrix3DHomogeneous RotationAroundYAxis(Angle angle)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            double sinAngle = Math.Sin(angle.Radians);
            double cosAngle = Math.Cos(angle.Radians);
            result.Matrix[0, 0] = cosAngle;
            result.Matrix[0, 2] = sinAngle;
            result.Matrix[2, 0] = -sinAngle;
            result.Matrix[2, 2] = cosAngle;
            return result;
        }

        // Create a rotation matrix around the z axis:
        public static Matrix3DHomogeneous RotationAroundZAxis(Angle angle)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            double sinAngle = Math.Sin(angle.Radians);
            double cosAngle = Math.Cos(angle.Radians);
            result.Matrix[0, 0] = cosAngle;
            result.Matrix[0, 1] = -sinAngle;
            result.Matrix[1, 0] = sinAngle;
            result.Matrix[1, 1] = cosAngle;
            return result;
        }

        // Create a reflection matrix across the X-Y plane
        public static Matrix3DHomogeneous ReflectionXY()
        {
            Matrix3DHomogeneous result = CreateScale(1, 1, -1);
            return result;
        }

        // Create a reflection matrix across the X-Z plane
        public static Matrix3DHomogeneous ReflectionXZ()
        {
            Matrix3DHomogeneous result = CreateScale(1, -1, 1);
            return result;
        }

        // Create a reflection matrix across the Y-Z plane
        public static Matrix3DHomogeneous ReflectionYZ()
        {
            Matrix3DHomogeneous result = CreateScale(-1, 1, 1);
            return result;
        }


        // Front view projection matrix
        public static Matrix3DHomogeneous FrontView()
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            result.Matrix[2, 2] = 0;
            return result;
        }

        // Side view projection matrix
        public static Matrix3DHomogeneous SideView()
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            result.Matrix[0, 0] = 0;
            result.Matrix[2, 2] = 0;
            result.Matrix[0, 2] = -1;
            return result;
        }

        // Top view projection matrix
        public static Matrix3DHomogeneous TopView()
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            result.Matrix[1, 1] = 0;
            result.Matrix[2, 2] = 0;
            result.Matrix[1, 2] = -1;
            return result;
        }

        // Axonometric projection matrix
        public static Matrix3DHomogeneous Axonometric(Angle alpha, Angle beta)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            double sna = Math.Sin(alpha.Radians);
            double cosAlpha = Math.Cos(alpha.Radians);
            double sinBeta = Math.Sin(beta.Radians);
            double cosBeta = Math.Cos(beta.Radians);
            result.Matrix[0, 0] = cosBeta;
            result.Matrix[0, 2] = sinBeta;
            result.Matrix[1, 0] = sna * sinBeta;
            result.Matrix[1, 1] = cosAlpha;
            result.Matrix[1, 2] = -sna * cosBeta;
            result.Matrix[2, 2] = 0;
            return result;
        }

        // Oblique projection matrix
        public static Matrix3DHomogeneous Oblique(Angle alpha, Angle theta)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            double tanAlpha = Math.Tan(alpha.Radians);
            double sinTheta = Math.Sin(theta.Radians);
            double cosTheta = Math.Cos(theta.Radians);
            result.Matrix[0, 2] = -cosTheta / tanAlpha;
            result.Matrix[1, 2] = -sinTheta / tanAlpha;
            result.Matrix[2, 2] = 0;
            return result;
        }

        // Create matrix from Euler Angles
        public static Matrix3DHomogeneous Euler(Angle alpha, Angle beta, Angle gamma)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            
            double sinAlpha = Math.Sin(alpha.Radians);
            double cosAlpha = Math.Cos(alpha.Radians);
            double sinBeta = Math.Sin(beta.Radians);
            double cosBeta = Math.Cos(beta.Radians);
            double sinGamma = Math.Sin(gamma.Radians);
            double cosGamma = Math.Cos(gamma.Radians);

            result.Matrix[0, 0] = cosAlpha * cosGamma - sinAlpha * sinBeta * sinGamma;
            result.Matrix[0, 1] = -sinBeta * sinGamma;
            result.Matrix[0, 2] = sinAlpha * cosGamma - cosAlpha * cosBeta * sinGamma;
            result.Matrix[1, 0] = -sinAlpha * sinBeta;
            result.Matrix[1, 1] = cosBeta;
            result.Matrix[1, 2] = cosAlpha * sinBeta;
            result.Matrix[2, 0] = -cosAlpha * sinGamma - sinAlpha * cosBeta * cosGamma;
            result.Matrix[2, 1] = -sinBeta * cosGamma;
            result.Matrix[2, 2] = cosAlpha * cosBeta * cosGamma - sinAlpha * sinBeta;
            return result;
        }

        // Create matrix from azimuth and elevation
        public static Matrix3DHomogeneous AzimuthElevation(double elevation, double azimuth, double oneOverd)
        {
            Matrix3DHomogeneous result = new Matrix3DHomogeneous();
            Matrix3DHomogeneous rotate = new Matrix3DHomogeneous();
            // make sure elevation in the range of [-90, 90]:
            if (elevation > 90)
                elevation = 90;
            else if (elevation < -90)
                elevation = -90;
            // Make sure azimuth in the range of [-180, 180]:
            if (azimuth > 180)
                azimuth = 180;
            else if (azimuth < -180)
                azimuth = -180;
            elevation = elevation * Math.PI / 180.0f;
            double sne = Math.Sin(elevation);
            double cne = Math.Cos(elevation);
            azimuth = azimuth * Math.PI / 180.0f;
            double sna = Math.Sin(azimuth);
            double cna = Math.Cos(azimuth);
            rotate.Matrix[0, 0] = cna;
            rotate.Matrix[0, 1] = sna;
            rotate.Matrix[0, 2] = 0;
            rotate.Matrix[1, 0] = -sne * sna;
            rotate.Matrix[1, 1] = sne * cna;
            rotate.Matrix[1, 2] = cne;
            rotate.Matrix[2, 0] = cne * sna;
            rotate.Matrix[2, 1] = -cne * cna;
            rotate.Matrix[2, 2] = sne;
            if (oneOverd <= 0)
                result = rotate;
            else if (oneOverd > 0)
            {
                //Point3DHomogeneous perspective = Point3DHomogeneous.Perspective(1 / oneOverd);
                //result = perspective * rotate;
            }
            return result;
        }


    }
}
