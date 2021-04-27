
using System;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Sphere3DTests
    {
        [TestCase("2.3,4,5", 1)]
        [TestCase("-1,4,2", 1)]
        [TestCase("3,-2,1", 1)]
        [TestCase("2,4,-4", 1)]
        [TestCase("0,0,0", 1)]
        [TestCase("0,0,0", 2)]
        public void SphereCenterRadius(string p1s, double radius)
        {
            var center = Point3D.Parse(p1s);
            var sphere = new Sphere3D(center, radius);
            Assert.AreEqual(2 * radius, sphere.Diameter, double.Epsilon);
            Assert.AreEqual(2 * Math.PI * radius, sphere.Circumference, double.Epsilon);
            Assert.AreEqual(Math.PI * radius * radius * 4, sphere.Area, double.Epsilon);
        }

        [TestCase("-1,0,0", "1,0,0","0,0,0", 1)]
        [TestCase("-1,0,0", "5,0,0","2,0,0", 3)]
        [TestCase("0,0,0", "0,2,0", "0,1,0", 1)]
        [TestCase("2,4,0", "2,2,2", "2,3,1", 1.4142135623)]
        [TestCase("1,2,0", "0,0,0", "0.5,1,0", 1.11803398874)]
        [TestCase("2,3,4", "4,5,2", "3,4,3", 1.73205080)]
        public void SphereFromTwoPoints(string p1s, string p2s, string p2centers, double p2radius)
        {
            var p2p1 = Point3D.Parse(p1s);
            var p2p2 = Point3D.Parse(p2s);
            var p2center = Point3D.Parse(p2centers);
            var p2sphere = Sphere3D.FromTwoPoints(p2p1, p2p2);

            AssertGeometry.AreEqual(p2center, p2sphere.CenterPoint);
            Assert.AreEqual(p2radius, p2sphere.Radius, 1e-6);
        }


        [TestCase("3,2,1", "1,-2,-3", "2,1,3", "-1,1,2", "1.2631578, -0.8421052, 0.2105263", 3.4230763)]
        [TestCase("1,2,1", "0,3,2", "1,1,0", "-1,0,-2", "-1.5,2.5,-0.5", 2.9580398)]
        [TestCase("2,1,1", "5,2,2", "2,3,3", "3,6,2", "3.1666666,3.5833333,0.4166666", 2.8939592)]
        [TestCase("2,1,1", "4,2,2", "2,1,3", "1,3,2", "2.4,2.2,2", 1.612451549)]
        [TestCase("4,2,1", "2,-2,-3", "6,1,3", "-1,1,2", "2.5405405,-2.986486486,2.21621621621", 5.336126992333)]
        public void SphereFromFourPoints(string p1s, string p2s, string p3s, string p4s, string centers, double radius)
        {
            var p1 = Point3D.Parse(p1s);
            var p2 = Point3D.Parse(p2s);
            var p3 = Point3D.Parse(p3s);
            var p4 = Point3D.Parse(p4s);
            var center = Point3D.Parse(centers);
            var sphere = Sphere3D.FromFourPoints(p1, p2, p3, p4);

            AssertGeometry.AreEqual(center, sphere.CenterPoint);
            Assert.AreEqual(radius, sphere.Radius, 1e-6);
        }

        [Test]
        public void SphereFromFourPointsArgumentException()
        {
            var p1 = new Point3D(0, 0, 0);
            var p2 = new Point3D(1, 2, 4);
            var p3 = new Point3D(2, 4, 8);
            var p4 = new Point3D(-1, 1, 2);

            Assert.Throws<ArgumentException>(() => Sphere3D.FromFourPoints(p1, p2, p3, p4));
        }

        [Test]
        public void SphereFromTwoPointsArgumentException()
        {
            var p1 = new Point3D(1, 4, 1);
            var p2 = new Point3D(1, 4, 1);

            Assert.Throws<ArgumentException>(() => Sphere3D.FromTwoPoints(p1, p2));
        }
    }
}


