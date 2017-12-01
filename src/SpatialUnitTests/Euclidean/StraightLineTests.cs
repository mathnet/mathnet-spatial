namespace MathNet.Spatial.UnitTests.Euclidean
{
    using System;
    using System.Linq;
    using MathNet.Spatial.Euclidean;
    using NUnit.Framework;

    [TestFixture]
    public class StraightLineTests
    {       
        [Test]
        public void ConstructorThrowsErrorOnSamePoint()
        {
            var p1 = new Point2D(1, -1);
            var p2 = new Point2D(1, -1);
            Assert.Throws<ArgumentException>(() => new StraightLine(p1, p2));
        }

        [Test]
        public void EqualityComparisonFalseAgainstNull()
        {
            var line = new StraightLine(new Point2D(), new Point2D(1, 1));
            Assert.IsFalse(line.Equals(null));
        }

        [TestCase("0,0", "1,1", 1, 0)]
        [TestCase("1,2", "3,1", -0.5, 2.5)]
        [TestCase("1,0", "1,1", double.PositiveInfinity, double.NaN)]
        [TestCase("0,1", "1,1", 0, 1)]
        public void CreateFromPoints(string p1, string p2, double gradient, double yintercept)
        {
            var first = Point2D.Parse(p1);
            var second = Point2D.Parse(p2);
            StraightLine l = new StraightLine(first, second);
            Assert.AreEqual(gradient, l.Gradient);
            Assert.AreEqual(yintercept, l.XIntercept().First().Y);    
        }

        [TestCase("-1,3", 2, 5)]
        [TestCase("2,2", -2, 6)]
        [TestCase("0,0", 1, 0)]
        public void CreateFromPointGradient(string point, double gradient, double yintercept)
        {
            var p1 = Point2D.Parse(point);
            StraightLine l = new StraightLine(p1, gradient);
            Assert.AreEqual(gradient, l.Gradient);
            Assert.AreEqual(yintercept, l.XIntercept().First().Y);
        }

        [TestCase("0,0;1,1", "1,0;2,1", true)]
        public void IsParallel(string line1, string line2, bool expected)
        {
            var pointpair1 = line1.Split(';').Select(t => Point2D.Parse(t));
            var pointpair2 = line2.Split(';').Select(t => Point2D.Parse(t));
            StraightLine l1 = new StraightLine(pointpair1);
            StraightLine l2 = new StraightLine(pointpair2);
            Assert.AreEqual(expected, l1.IsParallel(l2));
        }
       
        [TestCase("0,0", "1,0", "0,0", "0,0")]
        [TestCase("0,0", "1,0", "1,0", "1,0")]
        [TestCase("0,0", "1,0", ".25,1", ".25,0")]
        [TestCase("0,0", "1,0", "-1,0", "-1,0")]
        [TestCase("0,0", "1,0", "3,0", "3,0")]
        [TestCase("0,0", "1,0", "0,0", "0,0")]
        [TestCase("0,0", "1,0", "1,0", "1,0")]
        [TestCase("0,0", "1,0", ".25,1", ".25,0")]
        [TestCase("0,0", "1,0", "-1,1", "-1,0")]
        [TestCase("0,0", "1,0", "3,0", "3,0")]
        public void ClosestPointTo(string start, string end, string point, string expected)
        {
            var line = new StraightLine(Point2D.Parse(start), Point2D.Parse(end));
            var p = Point2D.Parse(point);
            var e = Point2D.Parse(expected);

            Assert.AreEqual(e, line.ClosestPointTo(p));
        }

        [TestCase("0,0", "2,2", "1,0", "1,2", "1,1")]
        [TestCase("0,0", "2,2", "0,1", "2,1", "1,1")]
        [TestCase("0,0", "2,2", "-1,-5", "-1,0", "-1,-1")]
        [TestCase("0,0", "2,2", "0,1", "1,2", null)]
        public void IntersectWithTest(string s1, string e1, string s2, string e2, string expected)
        {
            var line1 = new StraightLine(Point2D.Parse(s1), Point2D.Parse(e1));
            var line2 = new StraightLine(Point2D.Parse(s2), Point2D.Parse(e2));
            Point2D? e = string.IsNullOrEmpty(expected) ? (Point2D?)null : Point2D.Parse(expected);
            Point2D? intersection = line1.Intersection(line2);

            Assert.AreEqual(e, intersection);
        }        
    }
}
