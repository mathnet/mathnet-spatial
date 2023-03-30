using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Serialization
{
    public class XmlSerializerTests
    {
        private const double Tolerance = 1e-6;

        [TestCase("1, 2, 3", "-0.267261241912424, 0.534522483824849, 0.801783725737273", "<Ray3D><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint><Direction><X>-0.26726124191242406</X><Y>0.53452248382484913</Y><Z>0.80178372573727308</Z></Direction></Ray3D>")]
        public void Ray3DXml(string ps, string vs, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = AssertXml.XmlSerializerRoundTrip(ray, xml);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><StartPoint X=""1"" Y=""2"" Z=""3"" /><EndPoint X=""4"" Y=""5"" Z=""6"" /></Line3D>")]
        public void Line3DXml(string p1S, string p2S, string xml)
        {
            Point3D p1 = Point3D.Parse(p1S);
            Point3D p2 = Point3D.Parse(p2S);
            var l = new Line3D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [TestCase("1, 2", "4, 5", @"<Line2D><StartPoint X=""1"" Y=""2"" /><EndPoint X=""4"" Y=""5"" /></Line2D>")]
        public void Line2DXml(string p1S, string p2S, string xml)
        {
            Point2D p1 = Point2D.Parse(p1S);
            Point2D p2 = Point2D.Parse(p2S);
            var l = new Line2D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<LineSegment3D><StartPoint X=""1"" Y=""2"" Z=""3"" /><EndPoint X=""4"" Y=""5"" Z=""6"" /></LineSegment3D>")]
        public void LineSegment3DXml(string p1S, string p2S, string xml)
        {
            Point3D p1 = Point3D.Parse(p1S);
            Point3D p2 = Point3D.Parse(p2S);
            var l = new LineSegment3D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [TestCase("1, 2", "4, 5", @"<LineSegment2D><StartPoint X=""1"" Y=""2"" /><EndPoint X=""4"" Y=""5"" /></LineSegment2D>")]
        public void LineSegment2DXml(string p1S, string p2S, string xml)
        {
            Point2D p1 = Point2D.Parse(p1S);
            Point2D p2 = Point2D.Parse(p2S);
            var l = new LineSegment2D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Test]
        public void Vector2DXml()
        {
            var v = new Vector2D(1, 2);
            const string xml = "<Vector2D><X>1</X><Y>2</Y></Vector2D>";
            var result = AssertXml.XmlSerializerRoundTrip(v, xml);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector3DXml()
        {
            var v = new Vector3D(1, -2, 3);
            const string xml = "<Vector3D><X>1</X><Y>-2</Y><Z>3</Z></Vector3D>";
            var result = AssertXml.XmlSerializerRoundTrip(v, xml);
            Assert.AreEqual(v, result);
        }

        [TestCase("1, 1", 3, "<Circle2D><Center><X>1</X><Y>1</Y></Center><Radius>3</Radius></Circle2D>")]
        public void Circle2DXml(string point, double radius, string xml)
        {
            var center = Point2D.Parse(point);
            var c = new Circle2D(center, radius);
            var result = AssertXml.XmlSerializerRoundTrip(c, xml);
            Assert.AreEqual(c, result);
        }

        [TestCase("0, 0, 0", 2.5, @"<Circle3D><CenterPoint X=""0"" Y=""0"" Z=""0"" /><Axis X=""0"" Y=""0"" Z=""1"" /><Radius>2.5</Radius></Circle3D>")]
        public void Circle3DXml(string point, double radius, string xml)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            var result = AssertXml.XmlSerializerRoundTrip(c, xml);
            Assert.AreEqual(c, result);
        }

        [Test]
        public void Polygon2DXml()
        {
            var points = from x in new[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            const string xml = @"<Polygon2D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></Polygon2D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine2DXml()
        {
            var points = from x in new[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            const string xml = @"<PolyLine2D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></PolyLine2D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine3DXml()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            const string xml = @"<PolyLine3D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></PolyLine3D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, xml);
            Assert.AreEqual(p, result);
        }

        [TestCase("0, 0, 0", "0, 0, 1", @"<Plane><RootPoint X=""0"" Y=""0"" Z=""0"" /><Normal X=""0"" Y=""0"" Z=""1"" /></Plane>")]
        public void PlaneXml(string rootPoint, string unitVector, string xml)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), UnitVector3D.Parse(unitVector));
            var result = AssertXml.XmlSerializerRoundTrip(plane, xml);
            Assert.AreEqual(plane, result);
        }

        [Test]
        public void Point3DXml()
        {
            var p = new Point3D(1, -2, 3);
            const string xml = "<Point3D><X>1</X><Y>-2</Y><Z>3</Z></Point3D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point2DXml()
        {
            var p = new Point2D(1, 2);
            const string xml = "<Point2D><X>1</X><Y>2</Y></Point2D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, xml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void QuaternionXml()
        {
            var q = new Quaternion(1, 2, 3, 4);
            const string xml = "<Quaternion><W>1</W><X>2</X><Y>3</Y><Z>4</Z></Quaternion>";
            var result = AssertXml.XmlSerializerRoundTrip(q, xml);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesXml()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            const string xml = @"<EulerAngles><Alpha Value=""0""></Alpha><Beta Value=""0""></Beta><Gamma Value=""0""></Gamma></EulerAngles>";
            var result = AssertXml.XmlSerializerRoundTrip(eulerAngles, xml);
            Assert.AreEqual(eulerAngles, result);
        }

        [TestCase("15 °", "<Angle><Value>0.26179938779914941</Value></Angle>")]
        public void AngleXml(string vs, string xml)
        {
            var angle = Angle.Parse(vs);
            var result = AssertXml.XmlSerializerRoundTrip(angle, xml);
            Assert.AreEqual(angle.Radians, result.Radians, Tolerance);
        }

        [Test]
        public void CoordinateSystemXml()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            string xml = @"
<CoordinateSystem>
    <Origin X=""1"" Y=""-2"" Z=""3"" />
    <XAxis X=""0"" Y=""1"" Z=""0"" />
    <YAxis X=""0"" Y=""0"" Z=""1"" />
    <ZAxis X=""1"" Y=""0"" Z=""0"" />
</CoordinateSystem>";
            var result = AssertXml.XmlSerializerRoundTrip(cs, xml);
            AssertGeometry.AreEqual(cs, result);
        }
    }
}
