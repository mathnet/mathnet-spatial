namespace Spatial.Roslyn.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;
    using SpatialAnalyzers;

    public class FixLine2DTests
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
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(3, 4);
            var line = new Line2D(p1, p2);
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
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(3, 4);
            var line = new LineSegment2D(p1, p2);
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }
    }
}
