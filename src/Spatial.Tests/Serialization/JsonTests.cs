using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Serialization
{
    public class JsonTests
    {
        private const double Tolerance = 1e-6;

        [Explicit("fix later")]
        [TestCase("15 °")]
        public void AngleJson(string vs)
        {
            var angle = Angle.Parse(vs);
            var roundTrip = JsonRoundTrip(angle);
            Assert.AreEqual(angle.Radians, roundTrip.Radians, Tolerance);
        }

        [Explicit("fix later")]
        [Test]
        public void Point2DJson()
        {
            var p = new Point2D(1, 2);
            var result = JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Point3DJson()
        {
            var p = new Point3D(1, -2, 3);
            var result = JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void QuaternionJson()
        {
            var q = new Quaternion(1, 2, 3, 4);
            var result = JsonRoundTrip(q);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesJson()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            var result = JsonRoundTrip(eulerAngles);
            Assert.AreEqual(eulerAngles, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", "0, 0, 1")]
        public void PlaneJson(string rootPoint, string unitVector)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), Direction.Parse(unitVector));
            var result = JsonRoundTrip(plane);
            Assert.AreEqual(plane, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "-1, 2, 3", false)]
        public void LineJson(string ps, string vs, bool asElements)
        {
            var line = new Line(Point3D.Parse(ps), Direction.Parse(vs));
            var result = JsonRoundTrip(line);
            Assert.AreEqual(line, result);
            AssertGeometry.AreEqual(line, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6")]
        public void LineSegment3DJson(string p1S, string p2S)
        {
            Point3D p1 = Point3D.Parse(p1S);
            Point3D p2 = Point3D.Parse(p2S);
            var l = new LineSegment3D(p1, p2);
            var result = JsonRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5")]
        public void LineSegment2DJson(string p1S, string p2S)
        {
            Point2D p1 = Point2D.Parse(p1S);
            Point2D p2 = Point2D.Parse(p2S);
            var l = new LineSegment2D(p1, p2);
            var result = JsonRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector2DJson()
        {
            var v = new Vector2D(1, 2);
            var result = JsonRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector3DJson()
        {
            var v = new Vector3D(1, -2, 3);
            var result = JsonRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [TestCase("0, 0", 3)]
        public void Circle2DJson(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle(center, radius);
            var result = JsonRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DJson(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle(center, Direction.ZAxis, radius);
            var result = JsonRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Polygon2DJson()
        {
            var points = from x in new[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            var result = JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine2DJson()
        {
            var points = from x in new[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            var result = JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine3DJson()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            var result = JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void CoordinateSystemJson()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1), new Vector3D(0, 0, 1));
            var result = JsonRoundTrip(cs);
            AssertGeometry.AreEqual(cs, result);
        }

        private static T JsonRoundTrip<T>(T test)
        {
            var output = JsonConvert.SerializeObject(test);
            return JsonConvert.DeserializeObject<T>(output);
        }
    }
}
