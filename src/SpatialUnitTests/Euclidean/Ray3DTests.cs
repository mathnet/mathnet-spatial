// ReSharper disable InconsistentNaming
namespace MathNet.Spatial.UnitTests.Euclidean
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using MathNet.Spatial.Euclidean;
    using NUnit.Framework;

    [TestFixture]
    public class Ray3DTests
    {
        [TestCase("p:{1, 2, 3} v:{0, 0, 1}", "1, 2, 3", "0, 0, 1")]
        public void Parse(string rs, string eps, string evs)
        {
            var ray = Ray3D.Parse(rs);
            AssertGeometry.AreEqual(Point3D.Parse(eps), ray.ThroughPoint);
            AssertGeometry.AreEqual(Vector3D.Parse(evs), ray.Direction);
        }

        [TestCase("p:{0, 0, 0} v:{0, 0, 1}", "p:{0, 0, 0} v:{0, 1, 0}", "0, 0, 0", "-1, 0, 0")]
        [TestCase("p:{0, 0, 2} v:{0, 0, 1}", "p:{0, 0, 0} v:{0, 1, 0}", "0, 0, 2", "-1, 0, 0")]
        public void IntersectionOf(string pl1s, string pl2s, string eps, string evs)
        {
            var plane1 = Plane.Parse(pl1s);
            var plane2 = Plane.Parse(pl2s);
            var actual = Ray3D.IntersectionOf(plane1, plane2);
            var expected = Ray3D.Parse(eps, evs);
            AssertGeometry.AreEqual(expected, actual);
        }

        [Test]
        public void LineToTest()
        {
            var ray = new Ray3D(new Point3D(0, 0, 0), UnitVector3D.ZAxis);
            var point3D = new Point3D(1, 0, 0);
            var line3DTo = ray.ShortestLineTo(point3D);
            AssertGeometry.AreEqual(new Point3D(0, 0, 0), line3DTo.StartPoint);
            AssertGeometry.AreEqual(point3D, line3DTo.EndPoint, float.Epsilon);
        }

        [TestCase("0, 0, 0", "1, -1, 1", "0, 0, 0", "1, -1, 1", true)]
        [TestCase("0, 0, 2", "1, -1, 1", "0, 0, 0", "1, -1, 1", false)]
        [TestCase("0, 0, 0", "1, -1, 1", "0, 0, 0", "2, -1, 1", false)]
        public void Equals(string p1s, string v1s, string p2s, string v2s, bool expected)
        {
            var ray1 = new Ray3D(Point3D.Parse(p1s), UnitVector3D.Parse(v1s, tolerance: 2));
            var ray2 = new Ray3D(Point3D.Parse(p2s), UnitVector3D.Parse(v2s, tolerance: 2));
            Assert.AreEqual(expected, ray1.Equals(ray2));
            Assert.AreEqual(expected, ray1 == ray2);
            Assert.AreEqual(!expected, ray1 != ray2);
        }

        [TestCase("1, 2, 3", "-0.2672612419124244, 0.53452248382484879, 0.80178372573727319", false, @"<Ray3D><ThroughPoint X=""1"" Y=""2"" Z=""3"" /><Direction X=""-0.2672612419124244"" Y=""0.53452248382484879"" Z=""0.80178372573727319"" /></Ray3D>")]
        public void XmlTests(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            AssertXml.XmlRoundTrips(ray, xml, (e, a) => AssertGeometry.AreEqual(e, a, 1e-6));
        }

        [Test]
        public void BinaryRoundtrip()
        {
            var v = new Ray3D(new Point3D(1, 2, -3), UnitVector3D.Create(1, 2, 3));
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, v);
                ms.Flush();
                ms.Position = 0;
                var roundTrip = (Ray3D)formatter.Deserialize(ms);
                AssertGeometry.AreEqual(v, roundTrip);
            }
        }
    }
}
