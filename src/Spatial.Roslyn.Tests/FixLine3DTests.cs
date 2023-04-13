using Gu.Roslyn.Asserts;
using NUnit.Framework;
using SpatialAnalyzers;

namespace Spatial.Roslyn.Tests
{
    public class FixLine3DTests
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ExpectedDiagnostic CS0618 = ExpectedDiagnostic.Create("CS0618");

        [Test]
        public void ReplaceCtor()
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
            var p2 = new Point3D(3, 4, 0);
            var line = new Line3D(p1, p2);
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
            var p2 = new Point3D(3, 4, 0);
            var line = new LineSegment(p1, p2);
        }
    }
}";
            RoslynAssert.CodeFix(new UpdateCodeFix(), CS0618, testCode, fixedCode);
        }
    }
}
