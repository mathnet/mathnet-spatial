namespace Spatial.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    public class Point3DBenchmarks
    {
        private static readonly Point3D P1 = new Point3D(1, 2, 3);
        private static readonly Point3D P2 = new Point3D(1, 2, 3);
        private static readonly Vector3D V = new Vector3D(1, 2, 3);
        private static readonly Plane Plane = new Plane(UnitVector3D.Create(0, 0, 1), Point3D.Origin);

        [Benchmark]
        public Point3D OperatorAddition()
        {
            return P1 + V;
        }

        [Benchmark]
        public Vector3D OperatorSubtraction()
        {
            return P1 - P2;
        }

        [Benchmark]
        public bool OperatorEquality()
        {
            return P1 == P2;
        }

        [Benchmark]
        public Point3D MirrorAbout()
        {
            return P1.MirrorAbout(Plane);
        }

        [Benchmark]
        public Point3D ProjectOn()
        {
            return P1.ProjectOn(Plane);
        }

        [Benchmark]
        public Point3D Rotate()
        {
            return P1.Rotate(V, Angle.FromRadians(1));
        }

        [Benchmark]
        public Vector3D VectorTo()
        {
            return P1.VectorTo(P2);
        }

        [Benchmark]
        public double DistanceTo()
        {
            return P1.DistanceTo(P2);
        }

        [Benchmark]
        public Vector3D ToVector3D()
        {
            return P1.ToVector3D();
        }

        [Benchmark]
        public Vector<double> ToVector()
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
