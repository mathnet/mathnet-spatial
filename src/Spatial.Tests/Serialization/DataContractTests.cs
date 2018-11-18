namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
#if NETCOREAPP1_1 == false

    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;
    using MathNet.Spatial.UnitTests;
    using NUnit.Framework;

    public class DataContractTests
    {
        private const double Tolerance = 1e-6;

        [Explicit("fix later")]
        [TestCase("15 °", @"<Angle><Value>0.26179938779914941</Value></Angle>")]
        public void AngleDataContract(string vs, string xml)
        {
            var angle = Angle.Parse(vs);
            var result = this.DataContractRoundTrip(angle, xml);
            Assert.AreEqual(angle.Radians, result.Radians, Tolerance);
        }

        [Explicit("fix later")]
        [Test]
        public void Point2DDataContract()
        {
            var p = new Point2D(1, 2);
            const string ElementXml = @"<Point2D><X>1</X><Y>2</Y></Point2D>";
            var result = this.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Point3DDataContract()
        {
            var p = new Point3D(1, -2, 3);
            const string ElementXml = @"<Point3D><X>1</X><Y>-2</Y><Z>3</Z></Point3D>";
            var result = this.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void QuaternionDataContract()
        {
            var q = new Quaternion(1, 2, 3, 4);
            const string ElementXml = @"<Quaternion><W>1</W><X>2</X><Y>3</Y><Z>4</Z></Quaternion>";
            var result = this.DataContractRoundTrip(q, ElementXml);
            Assert.AreEqual(q, result);
        }

        [Explicit("fix later")]
        [Test]
        public void EulerAnglesDataContract()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            const string ElementXml = @"<EulerAngles><Alpha><Value>0</Value></Alpha><Beta><Value>0</Value></Beta><Gamma><Value>0</Value></Gamma></EulerAngles>";
            var result = this.DataContractRoundTrip(eulerAngles, ElementXml);
            Assert.AreEqual(eulerAngles, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", "0, 0, 1", @"<Plane><Normal><X>0</X><Y>0</Y><Z>1</Z></Normal><RootPoint><X>0</X><Y>0</Y><Z>0</Z></RootPoint></Plane>")]
        public void PlaneDataContract(string rootPoint, string unitVector, string xml)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), UnitVector3D.Parse(unitVector));
            var result = this.DataContractRoundTrip(plane, xml);
            Assert.AreEqual(plane, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><Direction><X>-0.2672612419124244</X><Y>0.53452248382484879</Y><Z>0.80178372573727319</Z></Direction><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint></Ray3D>")]
        public void Ray3DDataContract(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = this.DataContractRoundTrip(ray, xml);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><EndPoint><X>4</X><Y>5</Y><Z>6</Z></EndPoint><StartPoint><X>1</X><Y>2</Y><Z>3</Z></StartPoint></Line3D>")]
        public void Line3DDataContract(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            var result = this.DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6", @"<LineSegment3D><EndPoint><X>4</X><Y>5</Y><Z>6</Z></EndPoint><StartPoint><X>1</X><Y>2</Y><Z>3</Z></StartPoint></LineSegment3D>")]
        public void LineSegment3DDataContract(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new LineSegment3D(p1, p2);
            var result = this.DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5", @"<Line2D><EndPoint><X>4</X><Y>5</Y></EndPoint><StartPoint><X>1</X><Y>2</Y></StartPoint></Line2D>")]
        public void Line2DDataContract(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = this.DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5", @"<LineSegement2D><EndPoint><X>4</X><Y>5</Y></EndPoint><StartPoint><X>1</X><Y>2</Y></StartPoint></LineSegement2D>")]
        public void LineSegment2DDataContract(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new LineSegment2D(p1, p2);
            var result = this.DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector2DDataContract()
        {
            var v = new Vector2D(1, 2);
            const string ElementXml = @"<Vector2D><X>1</X><Y>2</Y></Vector2D>";
            var result = this.DataContractRoundTrip(v, ElementXml);
            Assert.AreEqual(v, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector3DDataContract()
        {
            var v = new Vector3D(1, -2, 3);
            const string ElementXml = @"<Vector3D><X>1</X><Y>-2</Y><Z>3</Z></Vector3D>";
            var result = this.DataContractRoundTrip(v, ElementXml);
            Assert.AreEqual(v, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0", 3)]
        public void Circle2DDataContract(string point, double radius)
        {
            var center = Point2D.Parse(point);
            var c = new Circle2D(center, radius);
            const string ElementXml = @"<Circle2D><CenterPoint><X>0</X><Y>0</Y></CenterPoint><Radius>3</Radius></Circle2D>";
            var result = this.DataContractRoundTrip(c, ElementXml);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DDataContract(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            const string ElementXml = @"<Circle3D><Axis><X>0</X><Y>0</Y><Z>1</Z></Axis><CenterPoint><X>0</X><Y>0</Y><Z>0</Z></CenterPoint><Radius>2.5</Radius></Circle3D>";
            var result = this.DataContractRoundTrip(c, ElementXml);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Polygon2DDataContract()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            const string ElementXml = @"<Polygon2D><Points><Point2D><X>0.25</X><Y>0</Y></Point2D><Point2D><X>0.5</X><Y>1</Y></Point2D><Point2D><X>1</X><Y>-1</Y></Point2D></Points></Polygon2D>";
            var result = this.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine2DDataContract()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            const string ElementXml = @"<PolyLine2D><Points><Point2D><X>0.25</X><Y>0</Y></Point2D><Point2D><X>0.5</X><Y>1</Y></Point2D><Point2D><X>1</X><Y>-1</Y></Point2D></Points></PolyLine2D>";
            var result = this.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine3DDataContract()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            const string ElementXml = @"<PolyLine3D><Points><Point3D><X>0</X><Y>-1.5</Y><Z>0</Z></Point3D><Point3D><X>0</X><Y>1</Y><Z>0</Z></Point3D><Point3D><X>1</X><Y>1</Y><Z>0</Z></Point3D></Points></PolyLine3D>";
            var result = this.DataContractRoundTrip(p, ElementXml);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void CoordinateSystemDataContract()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            const string xml = @"
<CoordinateSystem>
    <Origin><X>1</X><Y>-2</Y><Z>3</Z><Origin>
    <XAxis><X>0</X><Y>1</Y><Z>0</Z></XAxis>
    <YAxis><X>0</X><Y>0</Y><Z>1</Z></YAxis>
    <ZAxis><X>1</X><Y>0</Y><Z>0</Z><ZAxis>
</CoordinateSystem>";
            var result = this.DataContractRoundTrip(cs, xml);
            AssertGeometry.AreEqual(cs, result);
        }

        [Test]
        public void DataContractRoundTripTest()
        {
            var dummy = new AssertXmlTests.XmlSerializableDummy("Meh", 14);
            var roundTrip = this.DataContractRoundTrip(dummy, @"<AssertXmlTests.XmlSerializableDummy Age=""14""><Name>Meh</Name></AssertXmlTests.XmlSerializableDummy>");
            Assert.AreEqual(dummy.Name, roundTrip.Name);
            Assert.AreEqual(dummy.Age, roundTrip.Age);
        }

        private T DataContractRoundTrip<T>(T item, string expected)
        {
            var serializer = new DataContractSerializer(item.GetType());
            string xml;
            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw, AssertXml.Settings))
            {
                serializer.WriteObject(writer, item);
                writer.Flush();
                xml = sw.ToString();
                Debug.WriteLine("DataContractSerializer");
                Debug.Write(xml);
                Debug.WriteLine(string.Empty);
                AssertXml.AreEqual(expected, xml);
            }

            using (var stringReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stringReader))
            {
                return (T)serializer.ReadObject(reader);
            }
        }
    }

#endif
}
