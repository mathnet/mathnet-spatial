namespace Spatial.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BenchmarkDotNet.Attributes;
    using MathNet.Spatial.Euclidean;

    public class Polygon2DBenchmarks
    {
        private static readonly Random Random = new Random();
        private static readonly IReadOnlyList<Point2D> Point2D50 = Enumerable.Repeat<int>(0, 50).Select(_ => new Point2D(Random.Next(), Random.Next())).ToList();
        private static readonly IReadOnlyList<Point2D> Point2D500 = Enumerable.Repeat<int>(0, 50).Select(_ => new Point2D(Random.Next(), Random.Next())).ToList();
        private static readonly IReadOnlyList<Point2D> Point2D5000 = Enumerable.Repeat<int>(0, 50).Select(_ => new Point2D(Random.Next(), Random.Next())).ToList();

        [Benchmark]
        public Polygon2D GetConvexHullFromPoints50()
        {
            return Polygon2D.GetConvexHullFromPoints(Point2D50);
        }

        [Benchmark]
        public Polygon2D GetConvexHullFromPoints500()
        {
            return Polygon2D.GetConvexHullFromPoints(Point2D500);
        }

        [Benchmark]
        public Polygon2D GetConvexHullFromPoints5000()
        {
            return Polygon2D.GetConvexHullFromPoints(Point2D5000);
        }
    }
}
