using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class LinesXmlTests
    {
        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><ThroughPoint X=""1"" Y=""2"" Z=""3"" /><Direction X=""-0.2672612419124244"" Y=""0.53452248382484879"" Z=""0.80178372573727319"" /></Ray3D>")]
        public void Ray3DXml(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            AssertXml.XmlRoundTrips(ray, xml, (e, a) => AssertGeometry.AreEqual(e, a, 1e-6));
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><StartPoint X=""1"" Y=""2"" Z=""3"" /><EndPoint X=""4"" Y=""5"" Z=""6"" /></Line3D>")]
        public void Line3DXml(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            AssertXml.XmlRoundTrips(l, xml, (e, a) => AssertGeometry.AreEqual(e, a));
        }

        [Test]
        public void Vector2DXml()
        {
            const string Xml = @"<Vector2D X=""1"" Y=""2"" />";
            const string ElementXml = @"<Vector2D><X>1</X><Y>2</Y></Vector2D>";
            var v = new Vector2D(1, 2);

            AssertXml.XmlRoundTrips(v, Xml, (e, a) => AssertGeometry.AreEqual(e, a));

            var serializer = new XmlSerializer(typeof(Vector2D));


            var actuals = new[]
                          {
                              Vector2D.ReadFrom(XmlReader.Create(new StringReader(Xml))),
                              Vector2D.ReadFrom(XmlReader.Create(new StringReader(ElementXml))),
                              (Vector2D)serializer.Deserialize(new StringReader(Xml)),
                              (Vector2D)serializer.Deserialize(new StringReader(ElementXml))
                          };
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(v, actual);
            }
        }

        [Test]
        public void Vector3DXml ()
        {
            var v = new Vector3D(1, -2, 3);
            const string Xml = @"<Vector3D X=""1"" Y=""-2"" Z=""3"" />";
            const string ElementXml = @"<Vector3D><X>1</X><Y>-2</Y><Z>3</Z></Vector3D>";
            var roundTrip = AssertXml.XmlSerializerRoundTrip(v, Xml);
            AssertGeometry.AreEqual(v, roundTrip);

            var serializer = new XmlSerializer(typeof(Vector3D));

            var actuals = new[]
                          {
                              Vector3D.ReadFrom(XmlReader.Create(new StringReader(Xml))),
                              Vector3D.ReadFrom(XmlReader.Create(new StringReader(ElementXml))),
                              (Vector3D)serializer.Deserialize(new StringReader(Xml)),
                              (Vector3D)serializer.Deserialize(new StringReader(ElementXml))
                          };
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(v, actual);
            }
        }
    }
}
