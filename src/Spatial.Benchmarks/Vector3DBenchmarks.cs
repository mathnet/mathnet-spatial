using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Spatial.Benchmarks
{
    public class Vector3DBenchmarks
    {
        private static readonly Vector3D V1 = new Vector3D(1, 2, 3);
        private static readonly Vector3D V2 = new Vector3D(3, 4, 5);

        [Benchmark]
        public double Length()
        {
            return V1.Length;
        }

        [Benchmark]
        public Vector3D Parse()
        {
            // Probably not going to be in any hot path but adding a benchmark any way because it is fun.
            return Vector3D.Parse("1; 2; 3");
        }

        [Benchmark]
        public Direction Normalize()
        {
            return V1.Normalize();
        }

        [Benchmark]
        public double DotProduct()
        {
            return V1.DotProduct(V2);
        }

        [Benchmark]
        public double OperatorMultiplyVector()
        {
            return V1 * V2;
        }

        [Benchmark]
        public Vector3D OperatorAdd()
        {
            return V1 + V2;
        }

        [Benchmark]
        public Vector3D ScaleBy()
        {
            return V1.ScaleBy(2);
        }

        [Benchmark]
        public Vector3D OperatorMultiplyDouble()
        {
            return 2 * V1;
        }

        [Benchmark]
        public bool IsParallelToAngle()
        {
            return V1.IsParallelTo(V2, Angle.FromRadians(0));
        }

        [Benchmark]
        public bool IsParallelToDouble()
        {
            return V1.IsParallelTo(V2, 0);
        }

        [Benchmark]
        public Vector3D Rotate()
        {
            return V1.Rotate(Direction.XAxis, Angle.FromRadians(1));
        }

        [Benchmark]
        public Angle AngleTo()
        {
            return V1.AngleTo(V2);
        }

        [Benchmark]
        public Angle SignedAngleTo()
        {
            return V1.SignedAngleTo(V2, Direction.XAxis);
        }

        [Benchmark]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public bool OperatorEquals()
        {
            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            return V1 == V1;
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [Benchmark]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public bool Equals()
        {
            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            return V1.Equals(V1);
#pragma warning restore CS1718 // Comparison made to same variable
        }
    }
}
