namespace Spatial.Benchmarks
{
    using System.Globalization;
    using BenchmarkDotNet.Attributes;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    public class Vector2DBenchmarks
    {
        private static readonly Vector2D P1 = new Vector2D(1, 2);
        private static readonly Vector2D P2 = new Vector2D(1, 2);
        private static readonly Vector<double> Vector = P1.ToVector();

        [Benchmark]
        public double Length()
        {
            return P1.Length;
        }

        [Benchmark]
        public bool OperatorEquality()
        {
            return P1 == P2;
        }

        [Benchmark]
        public Vector2D OperatorAddition()
        {
            return P1 + P2;
        }

        [Benchmark]
        public Vector2D OperatorSubtraction()
        {
            return P1 - P2;
        }

        [Benchmark]
        public Vector2D OperatorUnaryNegation()
        {
            return -P1;
        }

        [Benchmark]
        public Vector2D OperatorMultiply()
        {
            return 2 * P2;
        }

        [Benchmark]
        public Vector2D OperatorDivision()
        {
            return P1 / 2;
        }

        [Benchmark]
        public Vector2D FromPolar()
        {
            return Vector2D.FromPolar(2, Angle.FromRadians(1));
        }

        [Benchmark]
        public Vector2D Parse()
        {
            return Vector2D.Parse("1; 2", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public Vector2D OfVector()
        {
            return Vector2D.OfVector(Vector);
        }

        [Benchmark]
        public bool IsParallelToDoubleTolerance()
        {
            return P1.IsParallelTo(P2, 2);
        }

        [Benchmark]
        public bool IsParallelToAngleTolerance()
        {
            return P1.IsParallelTo(P2, Angle.FromRadians(1));
        }

        [Benchmark]
        public bool IsPerpendicularToDoubleTolerance()
        {
            return P1.IsPerpendicularTo(P2, 2);
        }

        [Benchmark]
        public bool IsPerpendicularToAngleTolerance()
        {
            return P1.IsPerpendicularTo(P2, Angle.FromRadians(1));
        }

        [Benchmark]
        public Angle SignedAngleTo()
        {
            return P1.SignedAngleTo(P2, true, true);
        }

        [Benchmark]
        public Angle AngleTo()
        {
            return P1.AngleTo(P2);
        }

        [Benchmark]
        public Vector2D Rotate()
        {
            return P1.Rotate(Angle.FromRadians(2));
        }

        [Benchmark]
        public double DotProduct()
        {
            return P1.DotProduct(P2);
        }

        [Benchmark]
        public double CrossProduct()
        {
            return P1.CrossProduct(P2);
        }

        [Benchmark]
        public Vector2D ProjectOn()
        {
            return P1.ProjectOn(P2);
        }

        [Benchmark]
        public Vector2D Normalize()
        {
            return P1.Normalize();
        }

        [Benchmark]
        public Vector2D ScaleBy()
        {
            return P1.ScaleBy(2);
        }

        [Benchmark]
        public Vector2D Negate()
        {
            return P1.Negate();
        }

        [Benchmark]
        public Vector2D Subtract()
        {
            return P1.Subtract(P2);
        }

        [Benchmark]
        public Vector2D Add()
        {
            return P1.Add(P2);
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
            return P1.Equals(P2, 2);
        }
    }
}
