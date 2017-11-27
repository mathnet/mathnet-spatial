using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using ExtendedXmlSerializer.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
 
    public partial class PointsXmlTests
    {
        [Test]
        public void Point2DXml()
        {
            var p = new Point2D(1, 2);
            const string Xml = @"<Point2D X=""1"" Y=""2"" />";
            const string ElementXml = @"<Point2D><X>1</X><Y>2</Y></Point2D>";
            
            AssertXml.XmlRoundTrips(p, Xml, (e, a) => AssertGeometry.AreEqual(e, a));
            var serializer = new XmlSerializer(typeof(Point2D));

            var actuals = new[]
                          {
                              Point2D.ReadFrom(XmlReader.Create(new StringReader(Xml))),
                              Point2D.ReadFrom(XmlReader.Create(new StringReader(ElementXml))),
                              (Point2D)serializer.Deserialize(new StringReader(Xml)),
                              (Point2D)serializer.Deserialize(new StringReader(ElementXml))
                          };
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(p, actual);
            }
            
        }

        [Test]
        public void Point3DXml()
        {
            var p = new Point3D(1, -2, 3);
            const string Xml = @"<Point3D X=""1"" Y=""-2"" Z=""3"" />";
            const string ElementXml = @"<Point3D><X>1</X><Y>-2</Y><Z>3</Z></Point3D>";
            AssertXml.XmlRoundTrips(p, Xml, (expected, actual) => AssertGeometry.AreEqual(expected, actual));
            var serializer = new XmlSerializer(typeof(Point3D));

            var actuals = new[]
                          {
                              Point3D.ReadFrom(XmlReader.Create(new StringReader(Xml))),
                              Point3D.ReadFrom(XmlReader.Create(new StringReader(ElementXml))),
                              (Point3D)serializer.Deserialize(new StringReader(Xml)),
                              (Point3D)serializer.Deserialize(new StringReader(ElementXml))
                          };
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(p, actual);
            }
        }
    }
}
