using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using ExtendedXmlSerializer.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class BinaryFormatterTests
    {
        private const double Tolerance = 1e-6;

        private T BinaryFormmaterRoundTrip<T>(T test)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.SurrogateSelector = SerializerFactory.CreateSurrogateSelector();
                formatter.Serialize(ms, test);
                ms.Flush();
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        [TestCase("15 °", @"<Angle><Value>0.26179938779914941</Value></Angle>")]
        public void AngleBinaryFormatter(string vs, string xml)
        {
            var angle = Angle.Parse(vs);
            var roundTrip = BinaryFormmaterRoundTrip(angle);
            Assert.AreEqual(angle.Radians, roundTrip.Radians, Tolerance);
        }

        [Test]
        public void Point2DBinaryFormatter()
        {
            var p = new Point2D(1, 2);
            var result = BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void Point3DBinaryFormatter()
        {
            var p = new Point3D(1, -2, 3);
            var result = BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void QuaternionBinaryFormatter()
        {
            var q = new Quaternion(1, 2, 3, 4);
            var result = BinaryFormmaterRoundTrip(q);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesBinaryFormatter()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            var result = BinaryFormmaterRoundTrip(eulerAngles);
            Assert.AreEqual(eulerAngles, result);
        }

        [TestCase("p:{0, 0, 0} v:{0, 0, 1}", @"<Plane><Normal><X>0</X><Y>0</Y><Z>1</Z></Normal><RootPoint><X>0</X><Y>0</Y><Z>0</Z></RootPoint></Plane>")]
        public void PlaneBinaryFormatter(string p1s, string xml)
        {
            var plane = Plane.Parse(p1s);
            var result = BinaryFormmaterRoundTrip(plane);
            Assert.AreEqual(plane, result);
        }

        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><Direction><X>-0.2672612419124244</X><Y>0.53452248382484879</Y><Z>0.80178372573727319</Z></Direction><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint></Ray3D>")]
        public void Ray3DBinaryFormatter(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = BinaryFormmaterRoundTrip(ray);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><EndPoint><X>4</X><Y>5</Y><Z>6</Z></EndPoint><StartPoint><X>1</X><Y>2</Y><Z>3</Z></StartPoint></Line3D>")]
        public void Line3DBinaryFormatter(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            var result = BinaryFormmaterRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [TestCase("1, 2", "4, 5", @"<Line2D><EndPoint><X>4</X><Y>5</Y></EndPoint><StartPoint><X>1</X><Y>2</Y></StartPoint></Line2D>")]
        public void Line2DBinaryFormatter(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = BinaryFormmaterRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Test]
        public void Vector2DBinaryFormatter()
        {
            var v = new Vector2D(1, 2);
            var result = BinaryFormmaterRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector3DBinaryFormatter()
        {
            var v = new Vector3D(1, -2, 3);
            var result = BinaryFormmaterRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DBinaryFormatter(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            var result = BinaryFormmaterRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        [Test]
        public void Polygon2DBinaryFormatter()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            var result = BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine2DBinaryFormatter()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            var result = BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void PolyLine3DBinaryFormatter()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            var result = BinaryFormmaterRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        [Test]
        public void CoordinateSystemBinaryFormatter()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            var result = BinaryFormmaterRoundTrip(cs);
            AssertGeometry.AreEqual(cs, result);
        }
    }
}
