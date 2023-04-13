using System;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Euclidean
{
    [TestFixture]
    public class LineSegmentTests
    {
        [Test]
        public void Constructor()
        {
            var p1 = new Point3D(0, 0);
            var p2 = new Point3D(1, 1);
            var line = new LineSegment(p1, p2);

            AssertGeometry.AreEqual(p1, line.StartPoint);
            AssertGeometry.AreEqual(p2, line.EndPoint);
        }

        [Test]
        public void ConstructorThrowsErrorOnSamePoint()
        {
            var p1 = new Point3D(1, -1);
            var p2 = new Point3D(1, -1);
            Assert.Throws<ArgumentException>(() => new LineSegment(p1, p2));
        }

        [TestCase("0,0", "1,0", 1)]
        [TestCase("0,0", "0,1", 1)]
        [TestCase("0,0", "-1,0", 1)]
        [TestCase("0,-1", "0,1", 2)]
        [TestCase("-1,-1", "2,2", 4.24264068711)]
        public void LineLength(string p1S, string p2S, double expected)
        {
            var p1 = Point3D.Parse(p1S);
            var p2 = Point3D.Parse(p2S);
            var line = new LineSegment(p1, p2);
            var len = line.Length;

            Assert.AreEqual(expected, len, 1e-7);
        }

        [TestCase("0,0", "4,0", "1,0")]
        [TestCase("3,0", "0,0", "-1,0")]
        [TestCase("2.7,-2.7", "0,0", "-0.707106781,0.707106781")]
        [TestCase("11,-1", "11,1", "0,1")]
        public void LineDirection(string p1S, string p2S, string exs)
        {
            var p1 = Point3D.Parse(p1S);
            var p2 = Point3D.Parse(p2S);
            var ex = Vector3D.Parse(exs);
            var line = new LineSegment(p1, p2);

            AssertGeometry.AreEqual(ex, line.Direction);
        }

        [TestCase("0,0", "10,10", "0,0", "10,10", true)]
        [TestCase("0,0", "10,10", "0,0", "10,11", false)]
        public void EqualityOperator(string p1S, string p2S, string p3S, string p4S, bool expected)
        {
            var l1 = LineSegment.Parse(p1S, p2S);
            var l2 = LineSegment.Parse(p3S, p4S);

            Assert.AreEqual(expected, l1 == l2);
        }

        [TestCase("0,0", "10,10", "0,0", "10,10", false)]
        [TestCase("0,0", "10,10", "0,0", "10,11", true)]
        public void InequalityOperator(string p1S, string p2S, string p3S, string p4S, bool expected)
        {
            var l1 = new LineSegment(Point3D.Parse(p1S), Point3D.Parse(p2S));
            var l2 = new LineSegment(Point3D.Parse(p3S), Point3D.Parse(p4S));

            Assert.AreEqual(expected, l1 != l2);
        }

        [Test]
        public void EqualityComparisonFalseAgainstNull()
        {
            var line = new LineSegment(default(Point3D), new Point3D(1, 1));
            Assert.IsFalse(line.Equals(null));
        }

        [TestCase("0,0", "1,-1", Description = "Check start point")]
        [TestCase("1,0", "1,-1")]
        [TestCase("1,-2", "1,-1")]
        [TestCase("4,0", "3,-1", Description = "Check end point")]
        [TestCase("3,0", "3,-1")]
        [TestCase("3,-3", "3,-1")]
        [TestCase("1.5,0", "1.5,-1", Description = "Check near middle")]
        [TestCase("1.5,-2", "1.5,-1")]
        public void LineToBetweenEndPoints(string ptest, string exs)
        {
            var line = LineSegment.Parse("1,-1", "3,-1");
            var point = Point3D.Parse(ptest);
            var expPoint = Point3D.Parse(exs);
            var expLine = new LineSegment(expPoint, point);

            Assert.AreEqual(expLine, line.LineTo(point));
        }

        [TestCase("1,1", "3,1", "1,1", "2,2", "4,2")]
        [TestCase("1,1", "3,1", "-1,-1", "0,0", "2,0")]
        public void TranslateBy(string spoint1, string spoint2, string svector, string spoint3, string spoint4)
        {
            var line = LineSegment.Parse(spoint1, spoint2);
            var expected = LineSegment.Parse(spoint3, spoint4);
            var vector = Vector3D.Parse(svector);
            Assert.AreEqual(expected.Length, line.Length);
            Assert.AreEqual(expected, line.TranslateBy(vector));
        }

        [TestCase("0,0", "1,0", "0,0", "0,0")]
        [TestCase("0,0", "1,0", "1,0", "1,0")]
        [TestCase("0,0", "1,0", ".25,1", ".25,0")]
        [TestCase("0,0", "1,0", "-1,0", "0,0")]
        [TestCase("0,0", "1,0", "3,0", "1,0")]
        public void ClosestPointToWithinSegment(string start, string end, string point, string expected)
        {
            var line = LineSegment.Parse(start, end);
            var p = Point3D.Parse(point);
            var e = Point3D.Parse(expected);

            Assert.AreEqual(e, line.ClosestPointTo(p));
        }

        [TestCase("0,0", "2,2", "1,0", "1,2", "1,1")]
        [TestCase("0,0", "2,2", "0,1", "2,1", "1,1")]
        [TestCase("-1,-1", "2,2", "-1,-5", "-1,0", "-1,-1")]
        public void IntersectWithTest(string s1, string e1, string s2, string e2, string expected)
        {
            var line1 = LineSegment.Parse(s1, e1);
            var line2 = LineSegment.Parse(s2, e2);
            var e = string.IsNullOrEmpty(expected) ? (Point3D?)null : Point3D.Parse(expected);
            bool success = line1.TryIntersect(line2, out var result, Angle.FromRadians(0.001));
            Assert.IsTrue(success);
            Assert.AreEqual(e, result);
        }

        [TestCase("0,0", "-2,-2", "1,0", "1,2")]
        [TestCase("0,0", "-2,-2", "0,1", "2,1")]
        [TestCase("0,0", "2,2", "-1,-5", "-1,0")]
        public void IntersectWithTest2(string s1, string e1, string s2, string e2)
        {
            var line1 = LineSegment.Parse(s1, e1);
            var line2 = LineSegment.Parse(s2, e2);
            bool success = line1.TryIntersect(line2, out var result, Angle.FromRadians(0.001));
            Assert.IsFalse(success);
            Assert.AreEqual(null, result);
        }

        [TestCase("0,0", "0,1", "1,1", "1,2", 0.0001, true)]
        [TestCase("0,0", "0,-1", "1,1", "1,2", 0.0001, true)]
        [TestCase("0,0", "0.5,-1", "1,1", "1,2", 0.0001, false)]
        [TestCase("0,0", "0.00001,-1.0000", "1,1", "1,2", 0.0001, false)]
        [TestCase("0,0", "0,1", "1,1", "1,2", 0.01, true)]
        [TestCase("0,0", "0,-1", "1,1", "1,2", 0.01, true)]
        [TestCase("0,0", "0.5,-1", "1,1", "1,2", 0.01, false)]
        [TestCase("0,0", "0.001,-1.0000", "1,1", "1,2", 0.05, false)]
        [TestCase("0,0", "0.001,-1.0000", "1,1", "1,2", 0.06, true)]
        public void IsParallelToWithinAngleTol(string s1, string e1, string s2, string e2, double degreesTol, bool expected)
        {
            var line1 = LineSegment.Parse(s1, e1);
            var line2 = LineSegment.Parse(s2, e2);

            Assert.AreEqual(expected, line1.IsParallelTo(line2, Angle.FromDegrees(degreesTol)));
        }

        [Test]
        public void ToStringCheck()
        {
            var check = LineSegment.Parse("0,0", "1,1").ToString();

            Assert.AreEqual("StartPoint: (0, 0, 0), EndPoint: (1, 1, 0)", check);
        }
    }
}
