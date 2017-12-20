namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    using System.Linq;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;
    using MathNet.Spatial.UnitTests;
    using Newtonsoft.Json;
    using NUnit.Framework;

    public class JsonTests
    {
        private const double Tolerance = 1e-6;

        [Explicit("fix later")]
        [TestCase("15 °")]
        public void AngleJson(string vs)
        {
            var angle = Angle.Parse(vs);
            var roundTrip = this.JsonRoundTrip(angle);
            Assert.AreEqual(angle.Radians, roundTrip.Radians, Tolerance);
        }

        [Explicit("fix later")]
        [Test]
        public void Point2DJson()
        {
            var p = new Point2D(1, 2);
            var result = this.JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Point3DJson()
        {
            var p = new Point3D(1, -2, 3);
            var result = this.JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void QuaternionJson()
        {
            var q = new Quaternion(1, 2, 3, 4);
            var result = this.JsonRoundTrip(q);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesJson()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            var result = this.JsonRoundTrip(eulerAngles);
            Assert.AreEqual(eulerAngles, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", "0, 0, 1")]
        public void PlaneJson(string rootPoint, string unitVector)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), UnitVector3D.Parse(unitVector));
            var result = this.JsonRoundTrip(plane);
            Assert.AreEqual(plane, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "-1, 2, 3", false)]
        public void Ray3DJson(string ps, string vs, bool asElements)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = this.JsonRoundTrip(ray);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6")]
        public void Line3DJson(string p1s, string p2s)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            var result = this.JsonRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5")]
        public void Line2DJson(string p1s, string p2s)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = this.JsonRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6")]
        public void LineSegment3DJson(string p1s, string p2s)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new LineSegment3D(p1, p2);
            var result = this.JsonRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5")]
        public void LineSegment2DJson(string p1s, string p2s)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new LineSegment2D(p1, p2);
            var result = this.JsonRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector2DJson()
        {
            var v = new Vector2D(1, 2);
            var result = this.JsonRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Vector3DJson()
        {
            var v = new Vector3D(1, -2, 3);
            var result = this.JsonRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [TestCase("0, 0", 3)]
        public void Circle2DJson(string point, double radius)
        {
            var center = Point2D.Parse(point);
            var c = new Circle2D(center, radius);
            var result = this.JsonRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DJson(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            var result = this.JsonRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Polygon2DJson()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            var result = this.JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine2DJson()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            var result = this.JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine3DJson()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            var result = this.JsonRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void CoordinateSystemJson()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            var result = this.JsonRoundTrip(cs);
            AssertGeometry.AreEqual(cs, result);
        }

        private T JsonRoundTrip<T>(T test)
        {
            string output = JsonConvert.SerializeObject(test);
            return JsonConvert.DeserializeObject<T>(output);
        }
    }
}
