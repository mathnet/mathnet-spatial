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
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point2DDataContract()
        {
            var p = new Point2D(1, 2);
            const string ElementXml = @"<Point2D><X>1</X><Y>2</Y></Point2D>";
            var result = AssertXml.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point3DXml()
        {
            var p = new Point3D(1, -2, 3);
            const string Xml = @"<Point3D X=""1"" Y=""-2"" Z=""3"" />";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point3DDataContract()
        {
            var p = new Point3D(1, -2, 3);
            const string ElementXml = @"<Point3D><X>1</X><Y>-2</Y><Z>3</Z></Point3D>";
            var result = AssertXml.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }
    }
}
