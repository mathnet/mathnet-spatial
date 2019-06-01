using Gu.Roslyn.Asserts;
using NUnit.Framework;
using SpatialAnalyzers;

namespace Spatial.Roslyn.Tests
{
    public class Vector3DTests
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ExpectedDiagnostic CS0618 = ExpectedDiagnostic.Create("CS0618");

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
            var v = new Vector3D(1, 0, 0);
            var about = UnitVector3D.Create(0, 1, 0);
            var rotated = v.Rotate(about, 1.2, AngleUnit.Degrees);
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
            var v = new Vector3D(1, 0, 0);
            var about = UnitVector3D.Create(0, 1, 0);
            var rotated = v.Rotate(about, Angle.FromDegrees(1.2));
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
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
            var v = new Vector3D(1, 0, 0);
            var about = UnitVector3D.Create(0, 1, 0);
            var rotated = v.Rotate(about, 1.2, AngleUnit.Radians);
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
            var v = new Vector3D(1, 0, 0);
            var about = UnitVector3D.Create(0, 1, 0);
            var rotated = v.Rotate(about, Angle.FromRadians(1.2));
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }
    }
}
