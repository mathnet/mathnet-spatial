using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Serialization
{
    public class DataContractTests
    {
        private const double Tolerance = 1e-6;

        [TestCase("15 °", @"<Angle><Value>0.26179938779914941</Value></Angle>")]
        public void AngleDataContract(string vs, string xml)
        {
            var angle = Angle.Parse(vs);
            var result = DataContractRoundTrip(angle, xml);
            Assert.AreEqual(angle.Radians, result.Radians, Tolerance);
        }

        [Test]
        public void Point2DDataContract()
        {
            var p = new Point2D(1, 2);
            const string elementXml = @"<Point2D><X>1</X><Y>2</Y></Point2D>";
            var result = DataContractRoundTrip(p, elementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point3DDataContract()
        {
            var p = new Point3D(1, -2, 3);
            const string elementXml = @"<Point3D><X>1</X><Y>-2</Y><Z>3</Z></Point3D>";
            var result = DataContractRoundTrip(p, elementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void QuaternionDataContract()
        {
            var q = new Quaternion(1, 2, 3, 4);
            const string elementXml = @"<Quaternion><W>1</W><X>2</X><Y>3</Y><Z>4</Z></Quaternion>";
            var result = DataContractRoundTrip(q, elementXml);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesDataContract()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            const string elementXml = @"<EulerAngles><Alpha><Value>0</Value></Alpha><Beta><Value>0</Value></Beta><Gamma><Value>0</Value></Gamma></EulerAngles>";
            var result = DataContractRoundTrip(eulerAngles, elementXml);
            Assert.AreEqual(eulerAngles, result);
        }

        [TestCase("0, 0, 0", "0, 0, 1", @"<Plane><RootPoint><X>0</X><Y>0</Y><Z>0</Z></RootPoint><Normal><X>0</X><Y>0</Y><Z>1</Z></Normal></Plane>")]
        public void PlaneDataContract(string rootPoint, string unitVector, string xml)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), Direction.Parse(unitVector));
            var result = DataContractRoundTrip(plane, xml);
            Assert.AreEqual(plane, result);
        }

        [TestCase("1, 2, 3", "-0.2672612419124244, 0.53452248382484879, 0.80178372573727319", false, @"<Line><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint><Direction><X>-0.2672612419124244</X><Y>0.53452248382484879</Y><Z>0.80178372573727319</Z></Direction></Line>")]
        public void LineDataContract(string ps, string vs, bool asElements, string xml)
        {
            var line = new Line(Point3D.Parse(ps), Direction.Parse(vs));
            var result = DataContractRoundTrip(line, xml);
            Assert.AreEqual(line, result);
            AssertGeometry.AreEqual(line, result);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<LineSegment3D><StartPoint><X>1</X><Y>2</Y><Z>3</Z></StartPoint><EndPoint><X>4</X><Y>5</Y><Z>6</Z></EndPoint></LineSegment3D>")]
        public void LineSegment3DDataContract(string p1S, string p2S, string xml)
        {
            Point3D p1 = Point3D.Parse(p1S);
            Point3D p2 = Point3D.Parse(p2S);
            var l = new LineSegment3D(p1, p2);
            var result = DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [TestCase("1, 2", "4, 5", @"<LineSegment2D><StartPoint><X>1</X><Y>2</Y></StartPoint><EndPoint><X>4</X><Y>5</Y></EndPoint></LineSegment2D>")]
        public void LineSegment2DDataContract(string p1S, string p2S, string xml)
        {
            Point2D p1 = Point2D.Parse(p1S);
            Point2D p2 = Point2D.Parse(p2S);
            var l = new LineSegment2D(p1, p2);
            var result = DataContractRoundTrip(l, xml);
            Assert.AreEqual(l, result);
        }

        [Test]
        public void Vector2DDataContract()
        {
            var v = new Vector2D(1, 2);
            const string elementXml = @"<Vector2D><X>1</X><Y>2</Y></Vector2D>";
            var result = DataContractRoundTrip(v, elementXml);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector3DDataContract()
        {
            var v = new Vector3D(1, -2, 3);
            const string elementXml = @"<Vector3D><X>1</X><Y>-2</Y><Z>3</Z></Vector3D>";
            var result = DataContractRoundTrip(v, elementXml);
            Assert.AreEqual(v, result);
        }

        [TestCase("0, 0", 3)]
        public void Circle2DDataContract(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle(center, radius);
            const string elementXml = @"<Circle><Center><X>0</X><Y>0</Y><Z>0</Z></Center><Axis><X>0</X><Y>0</Y><Z>1</Z></Axis><Radius>3</Radius></Circle>";
            var result = DataContractRoundTrip(c, elementXml);
            Assert.AreEqual(c, result);
        }

        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DDataContract(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle(center, Direction.ZAxis, radius);
            const string elementXml = @"<Circle><Center><X>0</X><Y>0</Y><Z>0</Z></Center><Axis><X>0</X><Y>0</Y><Z>1</Z></Axis><Radius>2.5</Radius></Circle>";
            var result = DataContractRoundTrip(c, elementXml);
            Assert.AreEqual(c, result);
        }

        [Test]
        public void Polygon2DDataContract()
        {
            var points = from x in new[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            const string elementXml = @"<Polygon2D><Points><Point><X>0.25</X><Y>0</Y></Point><Point><X>0.5</X><Y>1</Y></Point><Point><X>1</X><Y>-1</Y></Point></Points></Polygon2D>";
            var result = DataContractRoundTrip(p, elementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine2DDataContract()
        {
            var points = from x in new[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            const string elementXml = @"<PolyLine2D><Points><Point><X>0.25</X><Y>0</Y></Point><Point><X>0.5</X><Y>1</Y></Point><Point><X>1</X><Y>-1</Y></Point></Points></PolyLine2D>";
            var result = DataContractRoundTrip(p, elementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine3DDataContract()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            const string elementXml = @"<PolyLine3D><Points><Point><X>0</X><Y>-1.5</Y><Z>0</Z></Point><Point><X>0</X><Y>1</Y><Z>0</Z></Point><Point><X>1</X><Y>1</Y><Z>0</Z></Point></Points></PolyLine3D>";
            var result = DataContractRoundTrip(p, elementXml);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void CoordinateSystemDataContract()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1), new Vector3D(0, 0, 1));
            const string xml = "<CoordinateSystem><Origin><X>1</X><Y>-2</Y><Z>3</Z></Origin><XAxis><X>0</X><Y>1</Y><Z>0</Z></XAxis><YAxis><X>0</X><Y>0</Y><Z>1</Z></YAxis></CoordinateSystem>";
            var result = DataContractRoundTrip(cs, xml);
            AssertGeometry.AreEqual(cs, result);
        }

        [Test]
        public void DataContractRoundTripTest()
        {
            var dummy = new AssertXmlTests.XmlSerializableDummy("Meh", 14);
            var roundTrip = DataContractRoundTrip(dummy, @"<AssertXmlTests.XmlSerializableDummy Age=""14""><Name>Meh</Name></AssertXmlTests.XmlSerializableDummy>");
            Assert.AreEqual(dummy.Name, roundTrip.Name);
            Assert.AreEqual(dummy.Age, roundTrip.Age);
        }

        private static T DataContractRoundTrip<T>(T item, string expected)
        {
            var serializer = new DataContractSerializer(item.GetType());
            string xml;
            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw, AssertXml.Settings))
            {
                serializer.WriteObject(writer, item);
                writer.Flush();
                xml = sw.ToString();
                AssertXml.AreEqual(expected, xml);
            }

            using (var stringReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stringReader))
            {
                return (T)serializer.ReadObject(reader);
            }
        }
    }
}
