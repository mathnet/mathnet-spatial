namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    using System.Linq;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;
    using MathNet.Spatial.UnitTests;
    using NUnit.Framework;

    public class XmlSerializaerTests
    {
        private const double Tolerance = 1e-6;

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><ThroughPoint X=""1"" Y=""2"" Z=""3"" /><Direction X=""-0.2672612419124244"" Y=""0.53452248382484879"" Z=""0.80178372573727319"" /></Ray3D>")]
        public void Ray3DXml(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = AssertXml.XmlSerializerRoundTrip(ray, xml);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, Tolerance);
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

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5", @"<Line2D><StartPoint X=""1"" Y=""2"" /><EndPoint X=""4"" Y=""5"" /></Line2D>")]
        public void Line2DXml(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6", @"<LineSegement3D><StartPoint X=""1"" Y=""2"" Z=""3"" /><EndPoint X=""4"" Y=""5"" Z=""6"" /></LineSegment3D>")]
        public void LineSegment3DXml(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new LineSegment3D(p1, p2);
            var result = AssertXml.XmlSerializerRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5", @"<LineSegment2D><StartPoint X=""1"" Y=""2"" /><EndPoint X=""4"" Y=""5"" /></LineSegment2D>")]
        public void LineSegement2DXml(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new LineSegment2D(p1, p2);
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

        [Explicit("fix later")]
        [TestCase("1, 1", 3, @"<Circle2D><CenterPoint X=""1"" Y=""1"" /><Radius>3</Radius></Circle2D>")]
        public void Circle2DXml(string point, double radius, string xml)
        {
            var center = Point2D.Parse(point);
            var c = new Circle2D(center, radius);
            var result = AssertXml.XmlSerializerRoundTrip(c, xml);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", 2.5, @"<Circle3D><CenterPoint X=""0"" Y=""0"" Z=""0"" /><Axis X=""0"" Y=""0"" Z=""1"" /><Radius>2.5</Radius></Circle3D>")]
        public void Circle3DXml(string point, double radius, string xml)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            var result = AssertXml.XmlSerializerRoundTrip(c, xml);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Polygon2DXml()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            const string Xml = @"<Polygon2D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></Polygon2D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine2DXml()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            const string Xml = @"<PolyLine2D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></PolyLine2D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine3DXml()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            const string Xml = @"<PolyLine3D><Points><Point X=""0.25"" Y=""0"" /><Point X=""0.5"" Y=""1"" /><Point X=""1"" Y=""-1"" /></Points></PolyLine3D>";
            var result = AssertXml.XmlSerializerRoundTrip(p, Xml);
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

        [Explicit("fix later")]
        [Test]
        public void QuaternionXml()
        {
            var q = new Quaternion(1, 2, 3, 4);
            const string Xml = @"<Quaternion W=""1"" X=""2"" Y=""3"" Z=""4"" />";
            var result = AssertXml.XmlSerializerRoundTrip(q, Xml);
            Assert.AreEqual(q, result);
        }

        [Explicit("fix later")]
        [Test]
        public void EulerAnglesXml()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            const string Xml = @"<EulerAngles><Alpha Value=""0""></Alpha><Beta Value=""0""></Beta><Gamma Value=""0""></Gamma></EulerAngles>";
            var result = AssertXml.XmlSerializerRoundTrip(eulerAngles, Xml);
            Assert.AreEqual(eulerAngles, result);
        }

        [TestCase("15 °", @"<Angle Value=""0.26179938779914941"" />")]
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
