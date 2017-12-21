namespace Spatial.Roslyn.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;
    using SpatialAnalyzers;

    public class FixPolygon2DTests
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ExpectedDiagnostic CS0618 = ExpectedDiagnostic.Create("CS0618");

        [Test]
        public void ReplaceIsPointInPolygonStatic()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var p1 = new Point2D(1, 2);
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var answer = Polygon2D.IsPointInPolygon(p1, poly);
        }
    }
}";
            var fixedCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var p1 = new Point2D(1, 2);
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var answer = poly.EnclosesPoint(p1);
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }

        [Test]
        public void ReplacePolygonVectorAddition()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var v1 = new Vector2D(1, 2);
            var answer = poly + v1;
        }
    }
}";
            var fixedCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var v1 = new Vector2D(1, 2);
            var answer = poly.TranslateBy(v1);
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }

        [Test]
        public void ReplaceVectorPolygonAddition()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var v1 = new Vector2D(1, 2);
            var answer = v1 + poly;
        }
    }
}";
            var fixedCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var v1 = new Vector2D(1, 2);
            var answer = poly.TranslateBy(v1);
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }

        [Test]
        public void ReplaceCount()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var answer = poly.Count;
        }
    }
}";
            var fixedCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            var answer = poly.VertexCount;
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }

        [Test]
        public void ReplaceEnumerator()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;
    using MathNet.Spatial.Euclidean;

    class Foo
    {
        public Foo()
        {
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            foreach (var vertex in poly)
            {
                //do something
            }
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
            var pointlist = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            var poly = new Polygon2D(pointlist); 
            foreach (var vertex in poly.Vertices)
            {
                //do something
            }
        }
    }
}";
            AnalyzerAssert.CodeFix<UpdateCodeFix>(CS0618, testCode, fixedCode);
        }
    }
}
