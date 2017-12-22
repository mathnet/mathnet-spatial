namespace Spatial.Benchmarks
{
    using System.Globalization;
    using BenchmarkDotNet.Attributes;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    public class UnitVector3DBenchmarks
    {
        private static readonly UnitVector3D UnitVector3D1 = UnitVector3D.Create(1, 0, 0);
        private static readonly UnitVector3D UnitVector3D2 = UnitVector3D.Create(0, 1, 0);
        private static readonly Vector3D Vector3D = new Vector3D(1, 2, 3);
        private static readonly Vector<double> Vector = UnitVector3D.Create(1, 0, 0).ToVector();
        private static readonly Plane Plane = new Plane(UnitVector3D.Create(0, 0, 1), Point3D.Origin);

        [Benchmark]
        public UnitVector3D Orthogonal()
        {
            return UnitVector3D1.Orthogonal;
        }

        [Benchmark]
        public double Length()
        {
            return UnitVector3D1.Length;
        }

        [Benchmark]
        public bool OperatorEqualityUnitVector3DUnitVector3D()
        {
            return UnitVector3D1 == UnitVector3D2;
        }

        [Benchmark]
        public bool OperatorEqualityUnitVector3DVector3D()
        {
            return UnitVector3D1 == Vector3D;
        }

        [Benchmark]
        public Vector3D OperatorAdditionUnitVector3DUnitVector3D()
        {
            return UnitVector3D1 + UnitVector3D2;
        }

        [Benchmark]
        public Vector3D OperatorAdditionUnitVector3DVector3D()
        {
            return UnitVector3D1 + Vector3D;
        }

        [Benchmark]
        public Vector3D OperatorSubtraction()
        {
            return UnitVector3D1 - UnitVector3D2;
        }

        [Benchmark]
        public Vector3D OperatorSubtractionUnitVector3DUnitVector3D()
        {
            return UnitVector3D1 - UnitVector3D2;
        }

        [Benchmark]
        public Vector3D OperatorSubtractionUnitVector3DVector3D()
        {
            return UnitVector3D1 - Vector3D;
        }

        [Benchmark]
        public Vector3D OperatorUnaryNegation()
        {
            return -UnitVector3D1;
        }

        [Benchmark]
        public double OperatorMultiply()
        {
            return UnitVector3D1 * UnitVector3D2;
        }

        [Benchmark]
        public Vector3D OperatorDivision()
        {
            return UnitVector3D1 / 2;
        }

        [Benchmark]
        public double OperatorMultiplyUnitVector3DUnitVector3D()
        {
            return UnitVector3D1 * UnitVector3D2;
        }

        [Benchmark]
        public UnitVector3D Create()
        {
            return UnitVector3D.Create(1, 0, 0, 2);
        }

        [Benchmark]
        public UnitVector3D OfVector()
        {
            return UnitVector3D.OfVector(Vector);
        }

        [Benchmark]
        public UnitVector3D Parse()
        {
            return UnitVector3D.Parse("1; 0; 0", CultureInfo.InvariantCulture, 2);
        }

        [Benchmark]
        public bool EqualsVector3D()
        {
            return UnitVector3D1.Equals(Vector3D);
        }

        [Benchmark]
        public bool EqualsUnitVector3D()
        {
            return UnitVector3D1.Equals(UnitVector3D2);
        }

        [Benchmark]
        public bool EqualsUnitVector3DDoubleTolerance()
        {
            return UnitVector3D1.Equals(UnitVector3D2, 2);
        }

        [Benchmark]
        public bool EqualsVector3DDoubleTolerance()
        {
            return UnitVector3D1.Equals(Vector3D, 2);
        }

        [Benchmark]
        public Vector3D ScaleBy()
        {
            return UnitVector3D1.ScaleBy(2);
        }

        [Benchmark]
        public Ray3D ProjectOnPlane()
        {
            return UnitVector3D1.ProjectOn(Plane);
        }

        [Benchmark]
        public Vector3D ProjectOnUnitVector3D()
        {
            return UnitVector3D1.ProjectOn(UnitVector3D2);
        }

        [Benchmark]
        public bool IsParallelToVector3DDoubleTolerance()
        {
            return UnitVector3D1.IsParallelTo(Vector3D, 2);
        }

        [Benchmark]
        public bool IsParallelToUnitVector3DDoubleTolerance()
        {
            return UnitVector3D1.IsParallelTo(UnitVector3D2, 2);
        }

        [Benchmark]
        public bool IsParallelToUnitVector3DAngleTolerance()
        {
            return UnitVector3D1.IsParallelTo(UnitVector3D2, Angle.FromRadians(1));
        }

        [Benchmark]
        public bool IsParallelToVector3D()
        {
            return UnitVector3D1.IsParallelTo(Vector3D, Angle.FromRadians(1));
        }

        [Benchmark]
        public bool IsPerpendicularToVector3D()
        {
            return UnitVector3D1.IsPerpendicularTo(Vector3D, 2);
        }

        [Benchmark]
        public bool IsPerpendicularTo()
        {
            return UnitVector3D1.IsPerpendicularTo(UnitVector3D2, 2);
        }

        [Benchmark]
        public UnitVector3D Negate()
        {
            return UnitVector3D1.Negate();
        }

        [Benchmark]
        public double DotProductVector3D()
        {
            return UnitVector3D1.DotProduct(Vector3D);
        }

        [Benchmark]
        public double DotProductUnitVector3D()
        {
            return UnitVector3D1.DotProduct(UnitVector3D2);
        }

        [Benchmark]
        public Vector3D Subtract()
        {
            return UnitVector3D1 - UnitVector3D2;
        }

        [Benchmark]
        public Vector3D Add()
        {
            return UnitVector3D1 + UnitVector3D2;
        }

        [Benchmark]
        public UnitVector3D CrossProduct()
        {
            return UnitVector3D1.CrossProduct(UnitVector3D2);
        }

        [Benchmark]
        public Angle SignedAngleTo()
        {
            return UnitVector3D1.SignedAngleTo(Vector3D, UnitVector3D2);
        }

        [Benchmark]
        public Angle AngleToVector3D()
        {
            return UnitVector3D1.AngleTo(Vector3D);
        }

        [Benchmark]
        public Angle AngleToUnitVector3D()
        {
            return UnitVector3D1.AngleTo(UnitVector3D2);
        }

        [Benchmark]
        public UnitVector3D Rotate()
        {
            return UnitVector3D1.Rotate(UnitVector3D2, Angle.FromRadians(1));
        }

        [Benchmark]
        public Point3D ToPoint3D()
        {
            return UnitVector3D1.ToPoint3D();
        }

        [Benchmark]
        public Vector3D ToVector3D()
        {
            return UnitVector3D1.ToVector3D();
        }

        [Benchmark]
        public Vector<double> ToVector()
        {
            return UnitVector3D1.ToVector();
        }
    }
}
