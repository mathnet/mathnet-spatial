namespace MathNet.Spatial.UnitTests.Euclidean
{
    using System;
    using MathNet.Spatial.Euclidean;
    using NUnit.Framework;

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
            Circle2D c = new Circle2D(cp, radius);
            Circle2D c2 = new Circle2D(cp, radius);
            Assert.True(c == c2);
            Assert.True(c.Equals(c2));
        }

    }
}
