using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class LinesXmlTests
    {
        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><Direction><X>-0.2672612419124244</X><Y>0.53452248382484879</Y><Z>0.80178372573727319</Z></Direction><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint></Ray3D>")]
        public void Ray3DDataContract(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = AssertXml.DataContractRoundTrip(ray, xml);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
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

        [TestCase("1, 2", "4, 5", @"<Line2D><EndPoint><X>4</X><Y>5</Y></EndPoint><StartPoint><X>1</X><Y>2</Y></StartPoint></Line2D>")]
        public void Line2DDataContract(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = AssertXml.DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
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
        public void Vector3DDataContract()
        {
            var v = new Vector3D(1, -2, 3);
            const string ElementXml = @"<Vector3D><X>1</X><Y>-2</Y><Z>3</Z></Vector3D>";
            var result = AssertXml.DataContractRoundTrip(v, ElementXml);
            Assert.AreEqual(v, result);
        }

        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DDataContract(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            const string ElementXml = @"<Circle3D><Axis><X>0</X><Y>0</Y><Z>1</Z></Axis><CenterPoint><X>0</X><Y>0</Y><Z>0</Z></CenterPoint><Radius>2.5</Radius></Circle3D>";
            var result = AssertXml.DataContractRoundTrip(c, ElementXml);
            Assert.AreEqual(c, result);
        }

        [Test]
        public void Polygon2DDataContract()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            const string ElementXml = @"<Polygon2D><Points><Point2D><X>0.25</X><Y>0</Y></Point2D><Point2D><X>0.5</X><Y>1</Y></Point2D><Point2D><X>1</X><Y>-1</Y></Point2D></Points></Polygon2D>";
            var result = AssertXml.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine2DDataContract()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            const string ElementXml = @"<PolyLine2D><Points><Point2D><X>0.25</X><Y>0</Y></Point2D><Point2D><X>0.5</X><Y>1</Y></Point2D><Point2D><X>1</X><Y>-1</Y></Point2D></Points></PolyLine2D>";
            var result = AssertXml.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine3DDataContract()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            const string ElementXml = @"<PolyLine3D><Points><Point3D><X>0</X><Y>-1.5</Y><Z>0</Z></Point3D><Point3D><X>0</X><Y>1</Y><Z>0</Z></Point3D><Point3D><X>1</X><Y>1</Y><Z>0</Z></Point3D></Points></PolyLine3D>";
            var result = AssertXml.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        /*
        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><ThroughPoint X=""1"" Y=""2"" Z=""3"" /><Direction X=""-0.2672612419124244"" Y=""0.53452248382484879"" Z=""0.80178372573727319"" /></Ray3D>")]
        public void Ray3DXml(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = AssertXml.XmlSerializerRoundTrip(ray, xml);
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

        [TestCase("1, 2", "4, 5", @"<Line2D><StartPoint X=""1"" Y=""2"" /><EndPoint X=""4"" Y=""5"" /></Line2D>")]
        public void Line2DXml(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
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
        public void Vector3DXml()
        {
            var v = new Vector3D(1, -2, 3);
            const string Xml = @"<Vector3D X=""1"" Y=""-2"" Z=""3"" />";
            var result = AssertXml.XmlSerializerRoundTrip(v, Xml);
            Assert.AreEqual(v, result);
        }

        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DXml(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            const string Xml = @"<Circle3D><CenterPoint X=""0"" Y=""0"" Z=""0"" /><Axis X=""0"" Y=""0"" Z=""1"" /><Radius>2.5</Radius></Circle3D>";
            var result = AssertXml.XmlSerializerRoundTrip(c, Xml);
            Assert.AreEqual(c, result);
        }

        [Test]
        public void Polygon2DXml()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            const string Xml = @"<Polygon2D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></Polygon2D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine2DXml()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            const string Xml = @"<PolyLine2D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></PolyLine2D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        
        [Test]
        public void PolyLine3DXml()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            const string Xml = @"<PolyLine3D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></PolyLine3D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }
        */
    }
}
