using System;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Line2DTests
    {

        [Test]
        public void Constructor()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(1, 1);
            var line = new Line2D(p1, p2);

            AssertGeometry.AreEqual(p1, line.StartPoint);
            AssertGeometry.AreEqual(p2, line.EndPoint);
        }

        [Test]
        public void ConstructorThrowsErrorOnSamePoint()
        {
            var p1 = new Point2D(1, -1);
            var p2 = new Point2D(1, -1);
            Assert.Throws<ArgumentException>(() => new Line2D(p1, p2));
        }

        [TestCase("0,0", "1,0", 1)]
        [TestCase("0,0", "0,1", 1)]
        [TestCase("0,0", "-1,0", 1)]
        [TestCase("0,-1", "0,1", 2)]
        [TestCase("-1,-1", "2,2", 4.24264068711)]
        public void LineLength(string p1s, string p2s, double expected)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            var line = new Line2D(p1, p2);
            double len = line.Length;

            Assert.AreEqual(expected, len, 1e-7);
        }

        [TestCase("0,0", "4,0", "1,0")]
        [TestCase("3,0", "0,0", "-1,0")]
        [TestCase("2.7,-2.7", "0,0", "-0.707106781,0.707106781")]
        [TestCase("11,-1", "11,1", "0,1")]
        public void LineDirection(string p1s, string p2s, string exs)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            var ex = Vector2D.Parse(exs);
            var line = new Line2D(p1, p2);
            
            AssertGeometry.AreEqual(ex, line.Direction);
        }

        [TestCase("0,0", "10,10", "0,0", "10,10", true)]
        [TestCase("0,0", "10,10", "0,0", "10,11", false)]
        public void EqualityOperator(string p1s, string p2s, string p3s, string p4s, bool expected)
        {
            var l1 = Line2D.Parse(p1s, p2s);
            var l2 = Line2D.Parse(p3s, p4s);
            
            Assert.AreEqual(expected, l1 == l2);
        }

        [TestCase("0,0", "10,10", "0,0", "10,10", false)]
        [TestCase("0,0", "10,10", "0,0", "10,11", true)]
        public void InequalityOperator(string p1s, string p2s, string p3s, string p4s, bool expected)
        {
            var l1 = new Line2D(Point2D.Parse(p1s), Point2D.Parse(p2s));
            var l2 = new Line2D(Point2D.Parse(p3s), Point2D.Parse(p4s));
         
            Assert.AreEqual(expected, l1 != l2);
        }

        [Test]
        public void EqualityComparisonFalseAgainstNull()
        {
            var line = new Line2D(new Point2D(), new Point2D(1,1) );
            Assert.IsFalse(line.Equals(null));
        }

        [Test]
        public void AdditionOperator()
        {
            var l1 = Line2D.Parse("0,0", "1,1");
            var ex = Line2D.Parse("-1,-1", "0,0");

            Assert.AreEqual(ex, l1 + new Vector2D(-1, -1));
        }

        [Test]
        public void SubtractionOperator()
        {
            var l1 = Line2D.Parse("0,0", "1,1");
            var ex = Line2D.Parse("-1,-1", "0,0");

            Assert.AreEqual(ex, l1 - new Vector2D(1, 1));
        }

        [TestCase("0,0", "1,-1")]   // Check start point
        [TestCase("1,0", "1,-1")]
        [TestCase("1,-2", "1,-1")]
        [TestCase("4,0", "3,-1")]   // Check end point
        [TestCase("3,0", "3,-1")]
        [TestCase("3,-3", "3,-1")]
        [TestCase("1.5,0", "1.5,-1")]   // Check near middle
        [TestCase("1.5,-2", "1.5,-1")]
        public void LineToBetweenEndPoints(string ptest, string exs)
        {
            var line = Line2D.Parse("1,-1", "3,-1");
            var point = Point2D.Parse(ptest);
            var expPoint = Point2D.Parse(exs);
            var expLine = new Line2D(expPoint, point);

            Assert.AreEqual(expLine, line.LineTo(point, true));
        }

        [TestCase("0,0", "0,-1")]   // Check start point
        [TestCase("1,0", "1,-1")]
        [TestCase("1,-2", "1,-1")]
        [TestCase("4,0", "4,-1")]   // Check end point
        [TestCase("3,0", "3,-1")]
        [TestCase("3,-3", "3,-1")]
        [TestCase("1.5,0", "1.5,-1")]   // Check near middle
        [TestCase("1.5,-2", "1.5,-1")]
        public void LineToIgnoreEndPoints(string ptest, string exs)
        {
            var line = Line2D.Parse("1,-1", "3,-1");
            var point = Point2D.Parse(ptest);
            var expPoint = Point2D.Parse(exs);
            var expLine = new Line2D(expPoint, point);

            Assert.AreEqual(expLine, line.LineTo(point, false));
        }

        [TestCase("0,0", "1,0", "0,0", "0,0")]
        [TestCase("0,0", "1,0", "1,0", "1,0")]
        [TestCase("0,0", "1,0", ".25,1", ".25,0")]
        [TestCase("0,0", "1,0", "-1,0", "0,0")]
        [TestCase("0,0", "1,0", "3,0", "1,0")]
        public void ClosestPointToWithinSegment(string start, string end, string point, string expected)
        {
            var line = Line2D.Parse(start, end);
            var p = Point2D.Parse(point);
            var e = Point2D.Parse(expected);

            Assert.AreEqual(e, line.ClosestPointTo(p, true));
        }

        [TestCase("0,0", "1,0", "0,0", "0,0")]
        [TestCase("0,0", "1,0", "1,0", "1,0")]
        [TestCase("0,0", "1,0", ".25,1", ".25,0")]
        [TestCase("0,0", "1,0", "-1,1", "-1,0")]
        [TestCase("0,0", "1,0", "3,0", "3,0")]
        public void ClosestPointToOutsideSegment(string start, string end, string point, string expected)
        {
            var line = Line2D.Parse(start, end);
            var p = Point2D.Parse(point);
            var e = Point2D.Parse(expected);

            Assert.AreEqual(e, line.ClosestPointTo(p, false));
        }

        [TestCase("0,0", "2,2", "1,0", "1,2", "1,1")]
        [TestCase("0,0", "2,2", "0,1", "2,1", "1,1")]
        [TestCase("0,0", "2,2", "-1,-5", "-1,0", "-1,-1")]
        [TestCase("0,0", "2,2", "0,1", "1,2", null)]
        public void IntersectWithTest(string s1, string e1, string s2, string e2, string expected)
        {
            var line1 = Line2D.Parse(s1, e1);
            var line2 = Line2D.Parse(s2, e2);
            Point2D? e = string.IsNullOrEmpty(expected) ? (Point2D?)null : Point2D.Parse(expected);

            Assert.AreEqual(e, line1.IntersectWith(line2));
        }

        [Test]
        public void ToStringCheck()
        {
            string check = Line2D.Parse("0,0", "1,1").ToString();

            Assert.AreEqual("StartPoint: (0, 0), EndPoint: (1, 1)", check);
        }
    }
}