namespace MathNet.Spatial.UnitTests
{
#if NETCOREAPP1_1 == false

    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;
    using NUnit.Framework;

    public class BinaryFormatterTests
    {
        private const double Tolerance = 1e-6;

        [TestCase("15 °")]
        public void AngleBinaryFormatter(string vs)
        {
            var angle = Angle.Parse(vs);
            var roundTrip = this.BinaryFormmaterRoundTrip(angle);
            Assert.AreEqual(angle.Radians, roundTrip.Radians, Tolerance);
        }

        [Test]
        public void Point2DBinaryFormatter()
        {
            var p = new Point2D(1, 2);
            var result = this.BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point3DBinaryFormatter()
        {
            var p = new Point3D(1, -2, 3);
            var result = this.BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void QuaternionBinaryFormatter()
        {
            var q = new Quaternion(1, 2, 3, 4);
            var result = this.BinaryFormmaterRoundTrip(q);
            Assert.AreEqual(q, result);
        }

        [Explicit("fix later")]
        [Test]
        public void EulerAnglesBinaryFormatter()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            var result = this.BinaryFormmaterRoundTrip(eulerAngles);
            Assert.AreEqual(eulerAngles, result);
        }

        [TestCase("0, 0, 0", "0, 0, 1")]
        public void PlaneBinaryFormatter(string rootPoint, string unitVector)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), UnitVector3D.Parse(unitVector));
            var result = this.BinaryFormmaterRoundTrip(plane);
            Assert.AreEqual(plane, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "-1, 2, 3", false)]
        public void Ray3DBinaryFormatter(string ps, string vs, bool asElements)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = this.BinaryFormmaterRoundTrip(ray);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
        }

        [TestCase("1, 2, 3", "4, 5, 6")]
        public void Line3DBinaryFormatter(string p1s, string p2s)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            var result = this.BinaryFormmaterRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2, 3", "4, 5, 6")]
        public void LineSegment3DBinaryFormatter(string p1s, string p2s)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new LineSegment3D(p1, p2);
            var result = this.BinaryFormmaterRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5")]
        public void Line2DBinaryFormatter(string p1s, string p2s)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = this.BinaryFormmaterRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Explicit("fix later")]
        [TestCase("1, 2", "4, 5")]
        public void LineSegment2DBinaryFormatter(string p1s, string p2s)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new LineSegment2D(p1, p2);
            var result = this.BinaryFormmaterRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Test]
        public void Vector2DBinaryFormatter()
        {
            var v = new Vector2D(1, 2);
            var result = this.BinaryFormmaterRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector3DBinaryFormatter()
        {
            var v = new Vector3D(1, -2, 3);
            var result = this.BinaryFormmaterRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Explicit("fix later")]
        [TestCase("0, 0", 3)]
        public void Circle2DBinaryFormatter(string point, double radius)
        {
            var center = Point2D.Parse(point);
            var c = new Circle2D(center, radius);
            var result = this.BinaryFormmaterRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DBinaryFormatter(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            var result = this.BinaryFormmaterRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Explicit("fix later")]
        [Test]
        public void Polygon2DBinaryFormatter()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            var result = this.BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine2DBinaryFormatter()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            var result = this.BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void PolyLine3DBinaryFormatter()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            var result = this.BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Explicit("fix later")]
        [Test]
        public void CoordinateSystemBinaryFormatter()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            var result = this.BinaryFormmaterRoundTrip(cs);
            AssertGeometry.AreEqual(cs, result);
        }

        private T BinaryFormmaterRoundTrip<T>(T test)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                // formatter.SurrogateSelector = SerializerFactory.CreateSurrogateSelector();
                formatter.Serialize(ms, test);
                ms.Flush();
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }

#endif

}
