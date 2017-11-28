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
            var result = AssertXml.XmlSerializerRoundTrip(ray, xml);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
        }

        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><Direction><X>-0.2672612419124244</X><Y>0.53452248382484879</Y><Z>0.80178372573727319</Z></Direction><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint></Ray3D>")]
        public void Ray3DDataContract(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = AssertXml.DataContractRoundTrip(ray, xml);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><StartPoint X=""1"" Y=""2"" Z=""3"" /><EndPoint X=""4"" Y=""5"" Z=""6"" /></Line3D>")]
        public void Line3DXml(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><EndPoint><X>4</X><Y>5</Y><Z>6</Z></EndPoint><StartPoint><X>1</X><Y>2</Y><Z>3</Z></StartPoint></Line3D>")]
        public void Line3DDataContract(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            var result = AssertXml.DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Test]
        public void Vector2DXml()
        {
            var v = new Vector2D(1, 2);
            const string Xml = @"<Vector2D X=""1"" Y=""2"" />";
            var result = AssertXml.XmlSerializerRoundTrip(v, Xml);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector2DDataContract()
        {
            var v = new Vector2D(1, 2);
            const string ElementXml = @"<Vector2D><X>1</X><Y>2</Y></Vector2D>";
            var result = AssertXml.DataContractRoundTrip(v, ElementXml);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector3DXml()
        {
            var v = new Vector3D(1, -2, 3);
            const string Xml = @"<Vector3D X=""1"" Y=""-2"" Z=""3"" />";
            var result = AssertXml.XmlSerializerRoundTrip(v, Xml);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector3DDataContract()
        {
            var v = new Vector3D(1, -2, 3);
            const string ElementXml = @"<Vector3D><X>1</X><Y>-2</Y><Z>3</Z></Vector3D>";
            var result = AssertXml.DataContractRoundTrip(v, ElementXml);
            Assert.AreEqual(v, result);
        }
    }
}
