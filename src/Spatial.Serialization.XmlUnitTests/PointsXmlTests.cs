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
        public void Point2DDataContract()
        {
            var p = new Point2D(1, 2);
            const string ElementXml = @"<Point2D><X>1</X><Y>2</Y></Point2D>";
            var result = AssertXml.DataContractRoundTrip(p, ElementXml);
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

        [Test]
        public void QuaternionDataContract()
        {
            var q = new Quaternion(1, 2, 3, 4);
            const string ElementXml = @"<Quaternion><W>1</W><X>2</X><Y>3</Y><Z>4</Z></Quaternion>";
            var result = AssertXml.DataContractRoundTrip(q, ElementXml);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesDataContract()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            const string ElementXml = @"<EulerAngles><Alpha><Value>0</Value></Alpha><Beta><Value>0</Value></Beta><Gamma><Value>0</Value></Gamma></EulerAngles>";
            var result = AssertXml.DataContractRoundTrip(eulerAngles, ElementXml);
            Assert.AreEqual(eulerAngles, result);
        }

        /*
        [Test]
        public void Point3DXml()
        {
            var p = new Point3D(1, -2, 3);
            const string Xml = @"<Point3D X=""1"" Y=""-2"" Z=""3"" />";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point2DXml()
        {
            var p = new Point2D(1, 2);
            const string Xml = @"<Point2D X=""1"" Y=""2"" />";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void QuaternionXml()
        {
            var q = new Quaternion(1, 2, 3, 4);
            const string Xml = @"<Quaternion W=""1"" X=""2"" Y=""3"" Z=""4"" />";
            var result = AssertXml.XmlSerializerRoundTrip(q, Xml);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesXml()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            const string Xml = @"<EulerAngles><Alpha Value=""0""></Alpha><Beta Value=""0""></Beta><Gamma Value=""0""></Gamma></EulerAngles>";
            var result = AssertXml.XmlSerializerRoundTrip(eulerAngles, Xml);
            Assert.AreEqual(eulerAngles, result);
        }
        */
    }
}
