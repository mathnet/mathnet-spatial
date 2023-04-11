using BenchmarkDotNet.Attributes;
using MathNet.Spatial.Euclidean;

namespace Spatial.Benchmarks
{
    public class LineBenchmarks
    {
        private static readonly Line Line1 = new Line(new Point3D(1, 2, 3), Direction.Create(1, 0, 0));
        private static readonly Line Line2 = new Line(new Point3D(3, 4, 5), Direction.Create(0, 1, 0));
        private static readonly Point3D Point3D = new Point3D(3, 4, 5);
        private static readonly Plane Plane = new Plane(1, 2, 3, 4);
        private static readonly Plane PlaneXy = new Plane(Point3D.Origin, Direction.ZAxis);
        private static readonly Plane PlaneXz = new Plane(Point3D.Origin, Direction.YAxis);

        [Benchmark]
        public bool OperatorEqualityLineLine()
        {
            return Line1 == Line2;
        }

        [Benchmark]
        public Line IntersectionOf()
        {
            return Line.IntersectionOf(PlaneXy, PlaneXz);
        }

        [Benchmark]
        public LineSegment3D LineTo()
        {
            return Line1.ShortestLineSegmentTo(Point3D);
        }

        [Benchmark]
        public Point3D? IntersectionWith()
        {
            return Line1.IntersectionWith(Plane);
        }

        [Benchmark]
        public bool IsCollinear()
        {
            return Line1.IsCollinear(Line2, 2);
        }

        [Benchmark]
        public bool Equals()
        {
            return Line1.Equals(Line2);
        }

        [Benchmark]
        public bool EqualsWithTolerance()
        {
            return Line1.Equals(Line2, 2);
        }
    }
}
