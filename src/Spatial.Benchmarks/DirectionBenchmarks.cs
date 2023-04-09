using System.Globalization;
using BenchmarkDotNet.Attributes;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Spatial.Benchmarks
{
    public class DirectionBenchmarks
    {
        private static readonly Direction Direction1 = Direction.Create(1, 0, 0);
        private static readonly Direction Direction2 = Direction.Create(0, 1, 0);
        private static readonly Vector3D Vector3D = new Vector3D(1, 2, 3);
        private static readonly Vector<double> Vector = Direction.Create(1, 0, 0).ToVector();
        private static readonly Plane Plane = new Plane(Direction.Create(0, 0, 1), Point3D.Origin);

        [Benchmark]
        public Direction Orthogonal()
        {
            return Direction1.Orthogonal;
        }

        [Benchmark]
        public double Length()
        {
            return Direction1.Length;
        }

        [Benchmark]
        public bool OperatorEqualityUnitVector3DUnitVector3D()
        {
            return Direction1 == Direction2;
        }

        [Benchmark]
        public bool OperatorEqualityUnitVector3DVector3D()
        {
            return Direction1 == Vector3D;
        }

        [Benchmark]
        public Vector3D OperatorAdditionUnitVector3DUnitVector3D()
        {
            return Direction1 + Direction2;
        }

        [Benchmark]
        public Vector3D OperatorAdditionUnitVector3DVector3D()
        {
            return Direction1 + Vector3D;
        }

        [Benchmark]
        public Vector3D OperatorSubtraction()
        {
            return Direction1 - Direction2;
        }

        [Benchmark]
        public Vector3D OperatorSubtractionUnitVector3DUnitVector3D()
        {
            return Direction1 - Direction2;
        }

        [Benchmark]
        public Vector3D OperatorSubtractionUnitVector3DVector3D()
        {
            return Direction1 - Vector3D;
        }

        [Benchmark]
        public Vector3D OperatorUnaryNegation()
        {
            return -Direction1;
        }

        [Benchmark]
        public double OperatorMultiply()
        {
            return Direction1 * Direction2;
        }

        [Benchmark]
        public Vector3D OperatorDivision()
        {
            return Direction1 / 2;
        }

        [Benchmark]
        public double OperatorMultiplyUnitVector3DUnitVector3D()
        {
            return Direction1 * Direction2;
        }

        [Benchmark]
        public Direction Create()
        {
            return Direction.Create(1, 0, 0, 2);
        }

        [Benchmark]
        public Direction OfVector()
        {
            return Direction.OfVector(Vector);
        }

        [Benchmark]
        public Direction Parse()
        {
            return Direction.Parse("1; 0; 0", CultureInfo.InvariantCulture, 2);
        }

        [Benchmark]
        public bool EqualsVector3D()
        {
            return Direction1.Equals(Vector3D);
        }

        [Benchmark]
        public bool EqualsUnitVector3D()
        {
            return Direction1.Equals(Direction2);
        }

        [Benchmark]
        public bool EqualsUnitVector3DDoubleTolerance()
        {
            return Direction1.Equals(Direction2, 2);
        }

        [Benchmark]
        public bool EqualsVector3DDoubleTolerance()
        {
            return Direction1.Equals(Vector3D, 2);
        }

        [Benchmark]
        public Vector3D ScaleBy()
        {
            return Direction1.ScaleBy(2);
        }

        [Benchmark]
        public Ray3D ProjectOnPlane()
        {
            return Direction1.ProjectOn(Plane);
        }

        [Benchmark]
        public Vector3D ProjectOnUnitVector3D()
        {
            return Direction1.ProjectOn(Direction2);
        }

        [Benchmark]
        public bool IsParallelToVector3DDoubleTolerance()
        {
            return Direction1.IsParallelTo(Vector3D, 2);
        }

        [Benchmark]
        public bool IsParallelToUnitVector3DDoubleTolerance()
        {
            return Direction1.IsParallelTo(Direction2, 2);
        }

        [Benchmark]
        public bool IsParallelToUnitVector3DAngleTolerance()
        {
            return Direction1.IsParallelTo(Direction2, Angle.FromRadians(1));
        }

        [Benchmark]
        public bool IsParallelToVector3D()
        {
            return Direction1.IsParallelTo(Vector3D, Angle.FromRadians(1));
        }

        [Benchmark]
        public bool IsPerpendicularToVector3D()
        {
            return Direction1.IsPerpendicularTo(Vector3D, 2);
        }

        [Benchmark]
        public bool IsPerpendicularTo()
        {
            return Direction1.IsPerpendicularTo(Direction2, 2);
        }

        [Benchmark]
        public Direction Negate()
        {
            return Direction1.Negate();
        }

        [Benchmark]
        public double DotProductVector3D()
        {
            return Direction1.DotProduct(Vector3D);
        }

        [Benchmark]
        public double DotProductUnitVector3D()
        {
            return Direction1.DotProduct(Direction2);
        }

        [Benchmark]
        public Vector3D Subtract()
        {
            return Direction1 - Direction2;
        }

        [Benchmark]
        public Vector3D Add()
        {
            return Direction1 + Direction2;
        }

        [Benchmark]
        public Direction CrossProduct()
        {
            return Direction1.CrossProduct(Direction2);
        }

        [Benchmark]
        public Angle SignedAngleTo()
        {
            return Direction1.SignedAngleTo(Vector3D, Direction2);
        }

        [Benchmark]
        public Angle AngleToVector3D()
        {
            return Direction1.AngleTo(Vector3D);
        }

        [Benchmark]
        public Angle AngleToUnitVector3D()
        {
            return Direction1.AngleTo(Direction2);
        }

        [Benchmark]
        public Direction Rotate()
        {
            return Direction1.Rotate(Direction2, Angle.FromRadians(1));
        }

        [Benchmark]
        public Point3D ToPoint3D()
        {
            return Direction1.ToPoint3D();
        }

        [Benchmark]
        public Vector3D ToVector3D()
        {
            return Direction1.ToVector3D();
        }

        [Benchmark]
        public Vector<double> ToVector()
        {
            return Direction1.ToVector();
        }
    }
}
