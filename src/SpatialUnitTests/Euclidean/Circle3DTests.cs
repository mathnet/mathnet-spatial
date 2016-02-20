using System;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Circle3DTests
    {
        [TestCase("0, 0, 0", 2.5)]
        [TestCase("2, -4, 0", 4.7)]
        public void CircleCenterRadius(string p1s, double radius)
        {
            var center = Point3D.Parse(p1s);
            var cicle3D = new Circle3D(center, UnitVector3D.ZAxis, radius);
            Assert.AreEqual(2 * radius, cicle3D.Diameter, double.Epsilon);
            Assert.AreEqual(2 * Math.PI * radius, cicle3D.Circumference, double.Epsilon);
            Assert.AreEqual( Math.PI * radius * radius, cicle3D.Area, double.Epsilon);
        }

        [TestCase("0,0,0", "5,0,0", "2.5,0,0", 2.5)]
        [TestCase("23.56,15.241,0", "62.15,-12.984,0", "42.8550,1.1285,0", 23.90522289)]
        public void Circle2Points(string p1s, string p2s, string centers, double radius)
        {
            var p1 = Point3D.Parse(p1s);
            var p2 = Point3D.Parse(p2s);
            var circle3D = new Circle3D(p1, p2, UnitVector3D.ZAxis);
            AssertGeometry.AreEqual(circle3D.CenterPoint, Point3D.Parse(centers));
            Assert.AreEqual(circle3D.Radius, radius, 1e-6);
        }
    }
}
