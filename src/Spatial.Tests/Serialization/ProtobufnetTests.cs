using System.IO;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;

namespace MathNet.Spatial.Tests.Serialization
{
    public class ProtobufNetTests
    {
        private const double Tolerance = 1e-6;

        public ProtobufNetTests()
        {
            // ReSharper disable once UnusedVariable
            var model = RuntimeTypeModel.Default;
        }

        [Explicit("fix later")]
        [TestCase("15 °")]
        public void AngleProtoBuf(string vs)
        {
            var angle = Angle.Parse(vs);
            var roundTrip = ProtobufRoundTrip(angle);
            Assert.AreEqual(angle.Radians, roundTrip.Radians, Tolerance);
        }

        [Test]
        public void Point2DProtoBuf()
        {
            var p = new Point3D(1, 2);
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point3DProtoBuf()
        {
            var p = new Point3D(1, -2, 3);
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void QuaternionProtoBuf()
        {
            var q = new Quaternion(1, 2, 3, 4);
            var result = ProtobufRoundTrip(q);
            Assert.AreEqual(q, result);
        }

        [Explicit("fix later")]
        [Test]
        public void EulerAnglesProtoBuf()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            var result = ProtobufRoundTrip(eulerAngles);
            Assert.AreEqual(eulerAngles, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", "0, 0, 1")]
        public void PlaneProtoBuf(string rootPoint, string unitVector)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), Direction.Parse(unitVector));
            var result = ProtobufRoundTrip(plane);
            Assert.AreEqual(plane, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "-1, 2, 3", false)]
        public void LineProtoBuf(string ps, string vs, bool asElements)
        {
            var line = new Line(Point3D.Parse(ps), Direction.Parse(vs));
            var result = ProtobufRoundTrip(line);
            Assert.AreEqual(line, result);
            AssertGeometry.AreEqual(line, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6")]
        public void LineSegment3DProtoBuf(string p1s, string p2s)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new LineSegment(p1, p2);
            var result = ProtobufRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5")]
        public void LineSegment2DProtoBuf(string p1s, string p2s)
        {
            var p1 = Point3D.Parse(p1s);
            var p2 = Point3D.Parse(p2s);
            var l = new LineSegment(p1, p2);
            var result = ProtobufRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector2DProtoBuf()
        {
            var v = new Vector3D(1, 2);
            var result = ProtobufRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector3DProtoBuf()
        {
            var v = new Vector3D(1, -2, 3);
            var result = ProtobufRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0", 3)]
        public void Circle2DProtoBuf(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle(center, radius);
            var result = ProtobufRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DProtoBuf(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle(center, Direction.ZAxis, radius);
            var result = ProtobufRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Polygon2DProtoBuf()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point3D.Parse(x);
            var p = new Polygon2D(points);
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine2DProtoBuf()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point3D.Parse(x);
            var p = new PolyLine2D(points);
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine3DProtoBuf()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void CoordinateSystemProtoBuf()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1), new Vector3D(0, 0, 1));
            var result = ProtobufRoundTrip(cs);
            AssertGeometry.AreEqual(cs, result);
        }

        private static T ProtobufRoundTrip<T>(T test)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, test);

                ms.Flush();
                ms.Position = 0;
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
