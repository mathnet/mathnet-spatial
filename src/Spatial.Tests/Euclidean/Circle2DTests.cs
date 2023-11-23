using System;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Euclidean
{
    [TestFixture]
    public class Circle2DTests
    {
        [TestCase("0, 0", 2.5)]
        [TestCase("2, -4", 4.7)]
        public void CircleCenterRadius(string p1s, double radius)
        {
            var center = Point2D.Parse(p1s);
            var circle = new Circle2D(center, radius);
            Assert.AreEqual(2 * radius, circle.Diameter, double.Epsilon);
            Assert.AreEqual(2 * Math.PI * radius, circle.Circumference, double.Epsilon);
            Assert.AreEqual(Math.PI * radius * radius, circle.Area, double.Epsilon);
        }

        [TestCase("0, 0", 1)]
        [TestCase("2, -4", 4.7)]
        public void CircleEquality(string center, double radius)
        {
            var cp = Point2D.Parse(center);
            var c = new Circle2D(cp, radius);
            var c2 = new Circle2D(cp, radius);
            Assert.True(c == c2);
            Assert.True(c.Equals(c2));
        }

        [TestCase("-7,4", "-4,5", "0,3", "-4,0", 5)]
        [TestCase("1,1", "2,4", "5,3", "3,2", 2.2360679775)]
        [TestCase("-1,0", "0,1", "1,0", "0,0", 1)]
        [TestCase("0,0", "0,1", "1,0", "0.5,0.5", 0.70710678118654752440)] // radius = sqrt(2)
        [TestCase("0,0", "0,1", "1,1", "0.5,0.5", 0.70710678118654752440)]
        [TestCase("0,0", "1,0", "0,1", "0.5,0.5", 0.70710678118654752440)]
        [TestCase("0,0", "1,1", "1,0", "0.5,0.5", 0.70710678118654752440)]
        public void CircleFromThreePoints(string p1s, string p2s, string p3s, string centers, double radius)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            var p3 = Point2D.Parse(p3s);
            var center = Point2D.Parse(centers);

            var circle = Circle2D.FromPoints(p1, p2, p3);

            AssertGeometry.AreEqual(center, circle.Center);
            Assert.AreEqual(radius, circle.Radius, 1e-6);
        }

        [Test]
        public void CircleFromThreePointsArgumentException()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(-1, 0);
            var p3 = new Point2D(1, 0);

            Assert.Throws<ArgumentException>(() => { Circle2D.FromPoints(p1, p2, p3); });
        }

        [TestCase("0,0", 1.41421356/*=sqrt(2)*/, "-1,-1", "+1,+1", "-1,-1", "+1,+1")]
        [TestCase("0,0", 1, "-1,0", "+1,0", "-1,0", "+1,0")]
        [TestCase("0,0", 1, "-1,0", "+1,0", "-1,0", "+1,0")]
        public void CircleIntersectWithLine2D(string sc, double radius, string sps, string spe, string esp0, string esp1)
        {
            var circle = new Circle2D(Point2D.Parse(sc), radius);
            var line = new Line2D(Point2D.Parse(sps), Point2D.Parse(spe));
            var actual = circle.IntersectWith(line);
            Assert.That(actual.Length, Is.EqualTo(2));

            var expected = new [] { Point2D.Parse(esp0), Point2D.Parse(esp1) };
            AssertGeometry.AreEqual(actual[0], expected[0]);
            AssertGeometry.AreEqual(actual[1], expected[1]);
        }
    }
}
