namespace Spatial.Benchmarks
{
    using System.Globalization;
    using BenchmarkDotNet.Attributes;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    public class Point2DBenchmarks
    {
        private static readonly Point2D P1 = new Point2D(1, 2);
        private static readonly Point2D P2 = new Point2D(1, 2);
        private static readonly Point2D[] Points = { P1, P2 };
        private static readonly Vector2D V = new Vector2D(1, 2);

        [Benchmark]
        public Point2D OperatorAddition()
        {
            return P1 + V;
        }

        [Benchmark]
        public Point2D OperatorSubtractionVector()
        {
            return P1 - V;
        }

        [Benchmark]
        public Vector2D OperatorSubtractionPoint()
        {
            return P1 - P2;
        }

        [Benchmark]
        public bool OperatorEquality()
        {
            return P1 == P2;
        }

        [Benchmark]
        public Point2D FromPolar()
        {
            return Point2D.FromPolar(1, Angle.FromRadians(1));
        }

        [Benchmark]
        public Point2D Parse()
        {
            return Point2D.Parse("1; 2", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public Point2D Centroid()
        {
            return Point2D.Centroid(Points);
        }

        [Benchmark]
        public Point2D MidPoint()
        {
            return Point2D.MidPoint(P2, P2);
        }

        [Benchmark]
        public Vector2D VectorTo()
        {
            return P1.VectorTo(P2);
        }

        [Benchmark]
        public double DistanceTo()
        {
            return P1.DistanceTo(P2);
        }

        [Benchmark]
        public Vector2D ToVector2D()
        {
            return P1.ToVector2D();
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
        public bool EqualsWithTolerance()
        {
            return P1.Equals(P2, 1);
        }
    }
}
