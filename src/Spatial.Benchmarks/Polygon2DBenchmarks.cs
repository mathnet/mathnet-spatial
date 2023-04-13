using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using MathNet.Spatial.Euclidean;

namespace Spatial.Benchmarks
{
    public class Polygon2DBenchmarks
    {
        private static readonly Random Random = new Random();
        private static readonly IReadOnlyList<Point3D> Point3D10 = Enumerable.Repeat(0, 10).Select(_ => new Point3D(Random.Next(), Random.Next())).ToList();
        private static readonly IReadOnlyList<Point3D> Point3D100 = Enumerable.Repeat(0, 100).Select(_ => new Point3D(Random.Next(), Random.Next())).ToList();
        private static readonly IReadOnlyList<Point3D> Point3D1000 = Enumerable.Repeat(0, 1000).Select(_ => new Point3D(Random.Next(), Random.Next())).ToList();
        private static readonly IReadOnlyList<Point3D> Point3D10000 = Enumerable.Repeat(0, 10000).Select(_ => new Point3D(Random.Next(), Random.Next())).ToList();

        [Benchmark]
        public Polygon2D GetConvexHullFromPoints10()
        {
            return Polygon2D.GetConvexHullFromPoints(Point3D10);
        }

        [Benchmark]
        public Polygon2D GetConvexHullFromPoints100()
        {
            return Polygon2D.GetConvexHullFromPoints(Point3D100);
        }

        [Benchmark]
        public Polygon2D GetConvexHullFromPoints1000()
        {
            return Polygon2D.GetConvexHullFromPoints(Point3D1000);
        }

        [Benchmark]
        public Polygon2D GetConvexHullFromPoints10000()
        {
            return Polygon2D.GetConvexHullFromPoints(Point3D10000);
        }
    }
}
