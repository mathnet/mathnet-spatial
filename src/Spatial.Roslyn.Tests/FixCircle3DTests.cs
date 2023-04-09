using Gu.Roslyn.Asserts;
using NUnit.Framework;
using SpatialAnalyzers;

namespace Spatial.Roslyn.Tests
{
    public class FixCircle3DTests
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ExpectedDiagnostic CS0618 = ExpectedDiagnostic.Create("CS0618");

        [Test]
        public void ReplacePointAndAxisCtor()
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
            var u1 = Direction.Create(0, 1, 0);
            var c = new Circle3D(p1, p2, u1);
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
            var u1 = Direction.Create(0, 1, 0);
            var c = Circle3D.FromPointsAndAxis(p1, p2, u1);
        }
    }
}";
            RoslynAssert.CodeFix(new UpdateCodeFix(), CS0618, testCode, fixedCode);
        }

        [Test]
        public void ReplaceThreePointCtor()
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
            var p2 = new Point3D(3, 5, 0);
            var p3 = new Point3D(-1, 0, 0);
            var c = new Circle3D(p1, p2, p3);
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
            var p2 = new Point3D(3, 5, 0);
            var p3 = new Point3D(-1, 0, 0);
            var c = Circle3D.FromPoints(p1, p2, p3);
        }
    }
}";
            RoslynAssert.CodeFix(new UpdateCodeFix(), CS0618, testCode, fixedCode);
        }
    }
}
