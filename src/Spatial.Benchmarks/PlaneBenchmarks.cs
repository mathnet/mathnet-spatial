namespace Spatial.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    public class PlaneBenchmarks
    {
        private static readonly Plane Plane1 = new Plane(new Point3D(1, 2, 3), UnitVector3D.Create(1, 0, 0));
        private static readonly Plane Plane2 = new Plane(new Point3D(3, 4, 5), UnitVector3D.Create(0, 1, 0));
        private static readonly Point3D Point3D = new Point3D(1, 2, 3);
        private static readonly Point3D Point3D1 = new Point3D(0, 0, 0);
        private static readonly Point3D Point3D2 = new Point3D(1, 0, 0);
        private static readonly Point3D Point3D3 = new Point3D(0, 1, 0);

        [Benchmark]
        public double A()
        {
            return Plane1.A;
        }

        [Benchmark]
        public double B()
        {
            return Plane1.B;
        }

        [Benchmark]
        public double C()
        {
            return Plane1.C;
        }

        [Benchmark]
        public Point3D RootPoint()
        {
            return Plane1.RootPoint;
        }

        [Benchmark]
        public bool OperatorEqualityPlanePlane()
        {
            return Plane1 == Plane2;
        }

        [Benchmark]
        public Plane FromPoints()
        {
            return Plane.FromPoints(Point3D1, Point3D2, Point3D3);
        }

        [Benchmark]
        public Point3D PointFromPlanes()
        {
            return Plane.PointFromPlanes(Plane2, Plane2, Plane2);
        }

        [Benchmark]
        public double SignedDistanceTo()
        {
            return Plane1.SignedDistanceTo(Point3D);
        }

        [Benchmark]
        public double SignedDistanceTo()
        {
            return Plane1.SignedDistanceTo(Plane2);
        }

        [Benchmark]
        public double SignedDistanceTo()
        {
            return Plane1.SignedDistanceTo(Ray3D);
        }

        [Benchmark]
        public double AbsoluteDistanceTo()
        {
            return Plane1.AbsoluteDistanceTo(Point3D);
        }

        [Benchmark]
        public Point3D Project()
        {
            return Plane1.Project(Point3D, Nullable`1);
        }

        [Benchmark]
        public Line3D Project()
        {
            return Plane1.Project(Line3D);
        }

        [Benchmark]
        public Ray3D Project()
        {
            return Plane1.Project(Ray3D);
        }

        [Benchmark]
        public Ray3D Project()
        {
            return Plane1.Project(Vector3D);
        }

        [Benchmark]
        public Ray3D Project()
        {
            return Plane1.Project(UnitVector3D);
        }

        [Benchmark]
        public Ray3D IntersectionWith()
        {
            return Plane1.IntersectionWith(Plane2, 2);
        }

        [Benchmark]
        public Nullable`1 IntersectionWith()
        {
            return Plane1.IntersectionWith(Line3D, 2);
        }

        [Benchmark]
        public Point3D IntersectionWith()
        {
            return Plane1.IntersectionWith(Ray3D, 2);
        }

        [Benchmark]
        public Point3D MirrorAbout()
        {
            return Plane1.MirrorAbout(Point3D);
        }

        [Benchmark]
        public Plane Rotate()
        {
            return Plane1.Rotate(UnitVector3D, Angle.FromRadians(1));
        }

        [Benchmark]
        public bool Equals()
        {
            return Plane1.Equals(Plane2);
        }
    }
}
