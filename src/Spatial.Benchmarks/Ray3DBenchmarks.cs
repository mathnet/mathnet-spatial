namespace Spatial.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using MathNet.Spatial.Euclidean;

    public class PlaneBenchmarks
    {
        private static readonly Plane Plane1 = new Plane(new Point3D(1, 2, 3), UnitVector3D.Create(1, 0, 0));
        private static readonly Plane Plane2 = new Plane(new Point3D(3, 4, 5), UnitVector3D.Create(0, 1, 0));
    }

    public class Ray3DBenchmarks
    {
        private static readonly Ray3D Ray3D1 = new Ray3D(new Point3D(1, 2, 3), UnitVector3D.Create(1, 0, 0));
        private static readonly Ray3D Ray3D2 = new Ray3D(new Point3D(3, 4, 5), UnitVector3D.Create(0, 1, 0));
        private static readonly Point3D Point3D = new Point3D(3, 4, 5);
        private static readonly Plane Plane = new Plane(1, 2, 3, 4);

        [Benchmark]
        public bool OperatorEqualityRay3DRay3D()
        {
            return Ray3D1 == Ray3D2;
        }

        [Benchmark]
        public Ray3D IntersectionOf()
        {
            return Ray3D.IntersectionOf(Plane, Plane);
        }

        [Benchmark]
        public Line3D LineTo()
        {
            return Ray3D1.LineTo(Point3D);
        }

        [Benchmark]
        public Point3D? IntersectionWith()
        {
            return Ray3D1.IntersectionWith(Plane);
        }

        [Benchmark]
        public bool IsCollinear()
        {
            return Ray3D1.IsCollinear(Ray3D2, 2);
        }

        [Benchmark]
        public bool Equals()
        {
            return Ray3D1.Equals(Ray3D2);
        }

        [Benchmark]
        public bool EqualsWithTolerance()
        {
            return Ray3D1.Equals(Ray3D2, 2);
        }
    }
}
