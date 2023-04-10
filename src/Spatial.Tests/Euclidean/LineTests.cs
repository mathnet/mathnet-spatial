// ReSharper disable InconsistentNaming

using System.Runtime.InteropServices;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Euclidean
{
    [TestFixture]
    public class LineTests
    {
        [TestCase("1, 2, 3", "0, 0, 1", "1, 2, 3", "0, 0, 1")]
        public void Parse(string rootPoint, string unitVector, string eps, string evs)
        {
            var line = new Line(Point3D.Parse(rootPoint), Direction.Parse(unitVector));
            AssertGeometry.AreEqual(Point3D.Parse(eps), line.ThroughPoint);
            AssertGeometry.AreEqual(Vector3D.Parse(evs), line.Direction);
        }

        [TestCase("0, 0, 0", "0, 0, 1", "0, 0, 0", "0, 1, 0", "0, 0, 0", "-1, 0, 0")]
        [TestCase("0, 0, 2", "0, 0, 1", "0, 0, 0", "0, 1, 0", "0, 0, 2", "-1, 0, 0")]
        public void IntersectionOf(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2, string eps, string evs)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), Direction.Parse(unitVector1));
            var plane2 = new Plane(Point3D.Parse(rootPoint2), Direction.Parse(unitVector2));
            var actual = Line.IntersectionOf(plane1, plane2);
            var expected = Line.Parse(eps, evs);
            AssertGeometry.AreEqual(expected, actual);
        }

        [TestCase("0, 0, 0", "1, 0, 0", "5, 2, 0", 2)]
        [TestCase("0, 0, 0", "1, 0, 0", "-1, 3, 4", 5)]
        [TestCase("1, 0, 0", "0, 1, 0", "5, 2, 0", 4)]
        [TestCase("1, -2, 3", "-4, 5, -6", "10, 11, -20", 20.415807473749229)]
        public void DistanceTest(string rootPointString, string directionString, string pointString, double expected)
        {
            var line = new Line(Point3D.Parse(rootPointString), Vector3D.Parse(directionString));
            var point = Point3D.Parse(pointString);
            Assert.AreEqual(expected, line.DistanceTo(point), 1e-12);
        }

        [Test]
        public void LineToTest()
        {
            var line = new Line(new Point3D(0, 0), Direction.ZAxis);
            var point3D = new Point3D(1, 0);
            var segment = line.ShortestLineSegmentTo(point3D);
            AssertGeometry.AreEqual(new Point3D(0, 0), segment.StartPoint);
            AssertGeometry.AreEqual(point3D, segment.EndPoint, float.Epsilon);
        }

        [TestCase("0, 0, 0", "1, -1, 1", "0, 0, 0", "1, -1, 1", true)]
        [TestCase("0, 0, 2", "1, -1, 1", "0, 0, 0", "1, -1, 1", false)]
        [TestCase("0, 0, 0", "1, -1, 1", "0, 0, 0", "2, -1, 1", false)]
        public void Equals(string p1s, string v1s, string p2s, string v2s, bool expected)
        {
            var line1 = new Line(Point3D.Parse(p1s), Direction.Parse(v1s, tolerance: 2));
            var line2 = new Line(Point3D.Parse(p2s), Direction.Parse(v2s, tolerance: 2));
            Assert.AreEqual(expected, line1.Equals(line2));
            Assert.AreEqual(expected, line1 == line2);
            Assert.AreEqual(!expected, line1 != line2);
        }

        [TestCase("1, 2, 3", "-0.26726124191242445, 0.53452248382484879, 0.80178372573727319", false, "<Line><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint><Direction><X>-0.26726124191242445</X><Y>0.53452248382484879</Y><Z>0.80178372573727319</Z></Direction></Line>")]
        public void XmlTests(string ps, string vs, bool asElements, string xml)
        {
            var line = new Line(Point3D.Parse(ps), Direction.Parse(vs));
            AssertXml.XmlRoundTrips(line, xml, (e, a) => AssertGeometry.AreEqual(e, a));
        }
    }
}
