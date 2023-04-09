using Gu.Roslyn.Asserts;
using NUnit.Framework;
using SpatialAnalyzers;

namespace Spatial.Roslyn.Tests
{
    public class UnitVector3DTests
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ExpectedDiagnostic CS0618 = ExpectedDiagnostic.Create("CS0618");

        [Test]
        public void ReplaceCtorWithCreate()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var uv = new Direction(1, 2, 3);
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
            var uv = Direction.Create(1, 2, 3);
        }
    }
}";
            RoslynAssert.CodeFix(new UpdateCodeFix(), CS0618, testCode, fixedCode);
        }

        [Test]
        public void ReplaceRotateWithDegreesWithRotateWithAngleFromDegrees()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    class Foo
    {
        public Foo()
        {
            var uv = Direction.Create(1, 0, 0);
            var about = Direction.Create(0, 1, 0);
            var rotated = uv.Rotate(about, 1.2, AngleUnit.Degrees);
        }
    }
}";
            var fixedCode = @"
namespace RoslynSandbox
{
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    class Foo
    {
        public Foo()
        {
            var uv = Direction.Create(1, 0, 0);
            var about = Direction.Create(0, 1, 0);
            var rotated = uv.Rotate(about, Angle.FromDegrees(1.2));
        }
    }
}";
            RoslynAssert.CodeFix(new UpdateCodeFix(), CS0618, testCode, fixedCode);
        }

        [Test]
        public void ReplaceRotateWithRadiansWithRotateWithAngleFromRadians()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    class Foo
    {
        public Foo()
        {
            var uv = Direction.Create(1, 0, 0);
            var about = Direction.Create(0, 1, 0);
            var rotated = uv.Rotate(about, 1.2, AngleUnit.Radians);
        }
    }
}";
            var fixedCode = @"
namespace RoslynSandbox
{
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    class Foo
    {
        public Foo()
        {
            var uv = Direction.Create(1, 0, 0);
            var about = Direction.Create(0, 1, 0);
            var rotated = uv.Rotate(about, Angle.FromRadians(1.2));
        }
    }
}";
            RoslynAssert.CodeFix(new UpdateCodeFix(), CS0618, testCode, fixedCode);
        }
    }
}
