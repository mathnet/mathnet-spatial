namespace Spatial.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using MathNet.Spatial.Euclidean;

    public class Ray3DBenchmarks
    {
        private static readonly Ray3D Ray3D1 = new Ray3D(new Point3D(1, 2, 3), UnitVector3D.Create(1, 0, 0));
        private static readonly Ray3D Ray3D2 = new Ray3D(new Point3D(3, 4, 5), UnitVector3D.Create(0, 1, 0));
        private static readonly Point3D Point3D = new Point3D(3, 4, 5);
        private static readonly Plane Plane = new Plane(1, 2, 3, 4);
        private static readonly Plane PlaneXy = new Plane(Point3D.Origin, UnitVector3D.ZAxis);
        private static readonly Plane PlaneXz = new Plane(Point3D.Origin, UnitVector3D.YAxis);

        [Benchmark]
        public bool OperatorEqualityRay3DRay3D()
        {
            return Ray3D1 == Ray3D2;
        }

        [Benchmark]
        public Ray3D IntersectionOf()
        {
            return Ray3D.IntersectionOf(PlaneXy, PlaneXz);
        }

        [Benchmark]
        public LineSegment3D LineTo()
        {
            return Ray3D1.ShortestLineTo(Point3D);
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
