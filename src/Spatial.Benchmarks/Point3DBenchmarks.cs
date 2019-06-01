using System.Globalization;
using BenchmarkDotNet.Attributes;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Spatial.Benchmarks
{
    public class Point3DBenchmarks
    {
        private static readonly Point3D P1 = new Point3D(1, 2, 3);
        private static readonly Point3D P2 = new Point3D(1, 2, 3);
        private static readonly Vector3D Vector3D = new Vector3D(1, 2, 3);
        private static readonly UnitVector3D UnitVector3D = UnitVector3D.Create(1, 0, 0);
        private static readonly Point3D[] Points = { P1, P2 };
        private static readonly Vector<double> Vector = P1.ToVector();
        private static readonly Ray3D Ray3D = new Ray3D(Point3D.Origin, UnitVector3D.ZAxis);
        private static readonly Plane Plane = new Plane(UnitVector3D.Create(0, 0, 1), Point3D.Origin);

        [Benchmark]
        public Point3D OperatorAdditionVector3D()
        {
            return P1 + Vector3D;
        }

        [Benchmark]
        public Point3D OperatorAdditionUnitVector3D()
        {
            return P1 + UnitVector3D;
        }

        [Benchmark]
        public Point3D OperatorSubtractionVector3D()
        {
            return P1 - Vector3D;
        }

        [Benchmark]
        public Point3D OperatorSubtractionUnitVector3D()
        {
            return P1 - UnitVector3D;
        }

        [Benchmark]
        public Vector3D OperatorSubtractionPoint3D()
        {
            return P1 - P2;
        }

        [Benchmark]
        public bool OperatorEquality()
        {
            return P1 == P2;
        }

        [Benchmark]
        public Point3D Parse()
        {
            return Point3D.Parse("1; 2; 3", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public Point3D OfVector()
        {
            return Point3D.OfVector(Vector);
        }

        [Benchmark]
        public Point3D Centroid()
        {
            return Point3D.Centroid(Points);
        }

        [Benchmark]
        public Point3D MidPoint()
        {
            return Point3D.MidPoint(P2, P2);
        }

        [Benchmark]
        public Point3D IntersectionOfPlanes()
        {
            return Point3D.IntersectionOf(Plane, Plane, Plane);
        }

        [Benchmark]
        public Point3D IntersectionOfPlaneAndRay()
        {
            return Point3D.IntersectionOf(Plane, Ray3D);
        }

        [Benchmark]
        public Point3D MirrorAbout()
        {
            return P1.MirrorAbout(Plane);
        }

        [Benchmark]
        public Point3D ProjectOn()
        {
            return P1.ProjectOn(Plane);
        }

        [Benchmark]
        public Point3D Rotate()
        {
            return P1.Rotate(Vector3D, Angle.FromRadians(1));
        }

        [Benchmark]
        public Point3D RotateAroundUnitVector()
        {
            return P1.Rotate(UnitVector3D, Angle.FromRadians(1));
        }

        [Benchmark]
        public Vector3D VectorTo()
        {
            return P1.VectorTo(P2);
        }

        [Benchmark]
        public double DistanceTo()
        {
            return P1.DistanceTo(P2);
        }

        [Benchmark]
        public Vector3D ToVector3D()
        {
            return P1.ToVector3D();
        }

        [Benchmark]
        public object ToVector()
        {
            return P1.ToVector();
        }

        [Benchmark]
        public bool Equals()
        {
            return P1.Equals(P2);
        }

        [Benchmark]
        public bool EqualsWIthTolerance()
        {
            return P1.Equals(P2, 2);
        }
    }
}
