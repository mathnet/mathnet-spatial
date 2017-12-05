namespace Spatial.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using MathNet.Spatial.Euclidean;

    public class Vector3DBenchmarks
    {
        private static readonly Vector3D V1 = new Vector3D(1, 2, 3);
        private static readonly Vector3D V2 = new Vector3D(3, 4, 5);

        [Benchmark]
        public UnitVector3D Normalize()
        {
            return V1.Normalize();
        }

        [Benchmark]
        public double DotProduct()
        {
            return V1.DotProduct(V2);
        }
    }
}
