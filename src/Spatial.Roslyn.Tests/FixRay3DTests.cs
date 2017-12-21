namespace Spatial.Roslyn.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;
    using SpatialAnalyzers;

    public class FixRay3DTests
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ExpectedDiagnostic CS0618 = ExpectedDiagnostic.Create("CS0618");

        [Test]
        public void ReplaceLineTo()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var p1 = new Point3D(1, 2, 0);
            var p2 = new Point3D(3, 4, 5);
            var u = new UnitVector3D(0, 1, 0);
            var ray = new Ray3D(p1, u);
            var answer = ray.LineTo(p2);
        }
    }
}";
            var fixedCode = @"
namespace RoslynSandbox
{
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var p1 = new Point3D(1, 2, 0);
            var p2 = new Point3D(3, 4, 5);
            var u = new UnitVector3D(0, 1, 0);
            var ray = new Ray3D(p1, u);
            var answer = ray.ShortestLineTo(p2);
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }
    }
}
