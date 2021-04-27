using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a 3 dimensional sphere.
    /// </summary>
    [Serializable]
    public struct Sphere3D : IEquatable<Sphere3D>
    {
        /// <summary>
        /// The center of the sphere.
        /// </summary>
        public readonly Point3D CenterPoint;
        
        /// <summary>
        /// The radius of the sphere.
        /// </summary>
        public readonly double Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sphere3D"/>.
        /// </summary>
        /// <param name="centerPoint">The center of the Sphere</param>
        /// <param name="radius">The radius of the Sphere</param>
        public Sphere3D(Point3D centerPoint, double radius)
        {
            // Anypoint is same to other point.
            if (radius <= 0)
            {
                throw new ArgumentException("The radius is negative.");
            }

            this.CenterPoint = centerPoint;
            this.Radius = radius;
        }

        /// <summary>
        /// Gets the diameter of the sphere.
        /// </summary>
        [Pure]
        public double Diameter => 2 * Radius;

        /// <summary>
        /// Gets the circumference of the sphere.
        /// </summary>
        [Pure]
        public double Circumference => 2 * Math.PI * Radius;

        /// <summary>
        /// Gets the volume of the sphere.
        /// </summary>
        [Pure]
        public double Volume => 4 / 3 * Math.PI * this.Radius * this.Radius * this.Radius;

        /// <summary>
        /// Gets the area of the sphere.
        /// </summary>
        [Pure]
        public double Area => 4 * Math.PI * this.Radius * this.Radius;

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified sphere is equal.
        /// </summary>
        /// <param name="left">The first sphere to compare</param>
        /// <param name="right">The second sphere to compare</param>
        /// <returns>True if the sphere are the same; otherwise false.</returns>
        public static bool operator ==(Sphere3D left, Sphere3D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified sphere is not equal.
        /// </summary>
        /// <param name="left">The first sphere to compare</param>
        /// <param name="right">The second sphere to compare</param>
        /// <returns>True if the sphere are different; otherwise false.</returns>
        public static bool operator !=(Sphere3D left, Sphere3D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Creates an instance of <see cref="Sphere3D"/>sphere which lie along its circumference of tetrahydron made of four points.
        /// </summary>
        /// <param name="p1">The first point on the sphere</param>
        /// <param name="p2">The second point on the sphere</param>
        /// <param name="p3">The third point on the sphere</param>
        /// <param name="p4">The last point on the sphere</param>
        public static Sphere3D FromFourPoints(Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            // https://mathworld.wolfram.com/Circumsphere.html
            //
            // The equation for the circumsphere of the tetrahedron with polygon vertices p1 = { x1, y1, z1 }, p2 = { x2, y2, z2 }, p3 = { x3, y3, z3 }, p4 = { x4, y4, z4 },
            //    | x1^2 + y1^2 + z1^2    x1    y1    z1    1 | = 0
            //    | x2^2 + y2^2 + z2^2    x2    y2    z2    1 |
            //    | x3^2 + y3^2 + z3^2    x3    y3    z3    1 |
            //    | x4^2 + y4^2 + z4^2    x4    y4    z4    1 |
            //
            // gives 
            //    a(x^2 + y^2 + z^2) - (Dx*x + Dy*y + Dz*z) + c = 0,
            //
            // where
            //    A = | x1 y2 z3 1 | = 0
            //        | x2 y2 z2 1 |
            //        | x3 y3 z3 1 |
            //        | x4 y4 z4 1 |,
            // and
            //    Dx = | x1^2+y1^2+z1^2 y1 z1 1 |   Dy= - | x1^2+y1^2+z1^2x1z1 1 |   Dz = | x1^2+y1^2+z1^2 x1 y1 1 |   c=  | x1^2+y1^2+z1^2 x1 y1 z1 |   
            //         | x2^2+y2^2+z2^2 y2 z2 1 |         | x2^2+y2^2+z2^2x2z2 1 |        | x2^2+y2^2+z2^2 x2 y2 1 |       | x2^2+y2^2+z2^2 x2 y2 z2 |     
            //         | x3^2+y3^2+z3^2 y3 z3 1 |         | x3^2+y3^2+z3^2x3z3 1 |        | x3^2+y3^2+z3^2 x3 y3 1 |       | x3^2+y3^2+z3^2 x3 y3 z3 |    
            //         | x4^2+y4^2+z4^2 y4 z4 1 |         | x4^2+y4^2+z4^2x4z4 1 |        | x4^2+y4^2+z4^2 x4 y4 1 |       | x4^2+y4^2+z4^2 x4 y4 z4 | .  
            //
            // Completing the square gives,   
            //    a(x-(Dx)/(2a))^2 + a(y-(Dy)/(2a))^2 + a(z-(Dz)/(2a))^2 - (Dx^2+Dy^2+Dz^2)/(4a)+c = 0,
            // which is a sphere of the form,
            //    (x-x0)^2 + (y-y0)^2 + (z-z0)^2 = r^2,
            //
            // With circumcenter and circumradius.
            //    x0 = (Dx)/(2a), y0 = (Dy)/(2a), z0 =(Dz)/(2a), and r = (sqrt(Dx^2+Dy^2+Dz^2-4ac))/(2|a|).
                       
            if (p1 == p2 || p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4 || p3 ==p4)
            {
                throw new ArgumentException("All points must be differrent.");
            }
            
            var Dx = Matrix<double>.Build.DenseOfArray(new[,]
            {
                { p1.X*p1.X + p1.Y*p1.Y + p1.Z*p1.Z, p1.Y,p1.Z, 1 },
                { p2.X*p2.X + p2.Y*p2.Y + p2.Z*p2.Z, p2.Y,p2.Z, 1 },
                { p3.X*p3.X + p3.Y*p3.Y + p3.Z*p3.Z, p3.Y,p3.Z, 1 },
                { p4.X*p4.X + p4.Y*p4.Y + p4.Z*p4.Z, p4.Y,p4.Z, 1 }
            });
            var Dy = Matrix<double>.Build.DenseOfArray(new[,]
            {
                { p1.X*p1.X + p1.Y*p1.Y + p1.Z*p1.Z, p1.X,p1.Z, 1 },
                { p2.X*p2.X + p2.Y*p2.Y + p2.Z*p2.Z, p2.X,p2.Z, 1 },
                { p3.X*p3.X + p3.Y*p3.Y + p3.Z*p3.Z, p3.X,p3.Z, 1 },
                { p4.X*p4.X + p4.Y*p4.Y + p4.Z*p4.Z, p4.X,p4.Z, 1 }
            });
            var Dz = Matrix<double>.Build.DenseOfArray(new[,]
            {
                { p1.X*p1.X + p1.Y*p1.Y + p1.Z*p1.Z, p1.X,p1.Y, 1 },
                { p2.X*p2.X + p2.Y*p2.Y + p2.Z*p2.Z, p2.X,p2.Y ,1 },
                { p3.X*p3.X + p3.Y*p3.Y + p3.Z*p3.Z, p3.X,p3.Y, 1 },
                { p4.X*p4.X + p4.Y*p4.Y + p4.Z*p4.Z, p4.X,p4.Y, 1 }
            });
            var a = Matrix<double>.Build.DenseOfArray(new[,]
            {
                { p1.X, p1.Y, p1.Z, 1 },
                { p2.X, p2.Y, p2.Z, 1 },
                { p3.X, p3.Y, p3.Z, 1 },
                { p4.X, p4.Y, p4.Z, 1 }
            });
            var c = Matrix<double>.Build.DenseOfArray(new[,]
            {
                { p1.X*p1.X + p1.Y*p1.Y + p1.Z*p1.Z, p1.X, p1.Y, p1.Z },
                { p2.X*p2.X + p2.Y*p2.Y + p2.Z*p2.Z, p2.X, p2.Y, p2.Z },
                { p3.X*p3.X + p3.Y*p3.Y + p3.Z*p3.Z, p3.X, p3.Y, p3.Z },
                { p4.X*p4.X + p4.Y*p4.Y + p4.Z*p4.Z, p4.X, p4.Y, p4.Z }
            });
            
            var detDx = Dx.Determinant();
            var detDy = -Dy.Determinant();
            var detDz = Dz.Determinant();
            var deta = a.Determinant();
            var detc = c.Determinant();
     
            if (deta == 0)
            {
                throw new ArgumentException("A circumcenter cannot be created from these points, are they collinear?");
            }

            // Finally, we can get the circumcenter and radius.
            var x0 = detDx / (2 * deta);
            var y0 = detDy / (2 * deta);
            var z0 = detDz / (2 * deta);
            var center = new Point3D(x0, y0, z0);
            var radius = Math.Sqrt(detDx * detDx + detDy * detDy + detDz * detDz - 4 * deta * detc) / (2 * Math.Abs(deta));           

            return new Sphere3D(center,radius);
        }

        /// <summary>
        /// Creates an instance of <see cref="Sphere3D"/> from 2 points of diameter.
        /// </summary>
        /// <param name="p1">The start point of diameter.</param>
        /// <param name="p2">The end point of diameter.</param>
        /// <returns></returns>
        public static Sphere3D FromTwoPoints(Point3D p1, Point3D p2)
        {
            if (p1 == p2)
            {
                throw new ArgumentException("A sphere cannot be created from two identical points.");
            }

            var center = Point3D.MidPoint(p1, p2);
            var radius = center.DistanceTo(p1);

            return new Sphere3D(center, radius);
        }

        /// <summary>
        /// Returns a value to indicate if a pair of sphere are equal.
        /// </summary>
        /// <param name="c">The sphere to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error.</param>
        /// <returns>True if the points are equal; otherwise false.</returns>
        public bool Equals(Sphere3D c, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(c.Radius - this.Radius) < tolerance
                   && this.CenterPoint.Equals(c.CenterPoint, tolerance);
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Sphere3D c)
        {
            return this.CenterPoint.Equals(c.CenterPoint)
                   && this.Radius.Equals(c.Radius);
        }

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Sphere3D c && this.Equals(c);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(this.CenterPoint, this.Radius);
    }


}



