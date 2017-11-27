using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class CircleTests
    {
        [TestCase("0,0", 3, 6 * Math.PI)]
        [TestCase("5,1", 2, 4 * Math.PI)]
        public void CircumferenceTest(string origin, double radius, double expected)
        {
            var center = Point2D.Parse(origin);
            Circle c = new Circle(center, radius);
            Assert.AreEqual(expected, c.Circumference);
        }

        [TestCase("0,0", 3, 9 * Math.PI)]
        [TestCase("5,1", 5, 25 * Math.PI)]
        public void AreaTest(string origin, double radius, double expected)
        {
            var center = Point2D.Parse(origin);
            Circle c = new Circle(center, radius);
            Assert.AreEqual(expected, c.Area);
        }
    }
}
