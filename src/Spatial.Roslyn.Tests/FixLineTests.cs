using Gu.Roslyn.Asserts;
using NUnit.Framework;
using SpatialAnalyzers;

namespace Spatial.Roslyn.Tests
{
    public class FixLineTests
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
            var u = new Direction(0, 1, 0);
            var line = new Line(p1, u);
            var answer = line.LineTo(p2);
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
            var u = new Direction(0, 1, 0);
            var line = new Line(p1, u);
            var answer = line.ShortestLineSegmentTo(p2);
        }
    }
}";
            RoslynAssert.CodeFix(new UpdateCodeFix(), CS0618, testCode, fixedCode);
        }
    }
}
