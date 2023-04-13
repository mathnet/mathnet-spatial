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
        public void CircleCenterRadius(string p1S, double radius)
        {
            var center = Point3D.Parse(p1S);
            var circle = new Circle(center, radius);
            Assert.AreEqual(2 * radius, circle.Diameter, double.Epsilon);
            Assert.AreEqual(2 * Math.PI * radius, circle.Circumference, double.Epsilon);
            Assert.AreEqual(Math.PI * radius * radius, circle.Area, double.Epsilon);
        }

        [TestCase("0, 0", 1)]
        [TestCase("2, -4", 4.7)]
        public void CircleEquality(string center, double radius)
        {
            var cp = Point3D.Parse(center);
            var c = new Circle(cp, radius);
            var c2 = new Circle(cp, radius);
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
        public void CircleFromThreePoints(string p1S, string p2S, string p3S, string centers, double radius)
        {
            var p1 = Point3D.Parse(p1S);
            var p2 = Point3D.Parse(p2S);
            var p3 = Point3D.Parse(p3S);
            var center = Point3D.Parse(centers);

            var circle = Circle.FromPoints(p1, p2, p3);

            AssertGeometry.AreEqual(center, circle.Center);
            Assert.AreEqual(radius, circle.Radius, 1e-6);
        }

        [Test]
        public void CircleFromThreePointsArgumentException()
        {
            var p1 = new Point3D(0, 0);
            var p2 = new Point3D(-1, 0);
            var p3 = new Point3D(1, 0);

            Assert.Throws<ArgumentException>(() => { Circle.FromPoints(p1, p2, p3); });
        }
    }
}
