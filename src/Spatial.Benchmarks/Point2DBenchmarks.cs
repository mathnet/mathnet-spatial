namespace Spatial.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using MathNet.Spatial.Euclidean;

    public class Point2DBenchmarks
    {
        private static readonly Point2D P1 = new Point2D(1, 2);
        private static readonly Point2D P2 = new Point2D(1, 2);
        private static readonly Vector2D V = new Vector2D(1, 2);

        [Benchmark]
        public Point2D OperatorAddition()
        {
            return P1 + V;
        }

        [Benchmark]
        public Vector2D OperatorSubtraction()
        {
            return P1 - P2;
        }

        [Benchmark]
        public bool OperatorEquality()
        {
            return P1 == P2;
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
        public Point3D ToPoint3D()
        {
            return P1.ToPoint3D();
        }

        [Benchmark]
        public object ToVector()
        {
            return P1.ToVector();
        }

        [Benchmark]
        public new string ToString()
        {
            return P1.ToString();
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
