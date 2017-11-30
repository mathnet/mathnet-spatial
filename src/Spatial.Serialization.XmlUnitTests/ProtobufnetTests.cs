using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using ExtendedXmlSerializer.Configuration;
using System.IO;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Meta;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class ProtobufNetTests
    {
        public ProtobufNetTests()
        {
            var model = ProtoBuf.Meta.RuntimeTypeModel.Default;
            var surrogates = SerializerFactory.KnownSurrogates();
            surrogates.ForEach(t => { model.Add(t.Item2, true); model.Add(t.Item1, false).SetSurrogate(t.Item2); });
        }
        private const double Tolerance = 1e-6;

        private T ProtobufRoundTrip<T>(T test)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, test);

                ms.Flush();
                var xml = ms.ToString();
                ms.Position = 0;
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        [TestCase("15 °", @"<Angle><Value>0.26179938779914941</Value></Angle>")]
        public void AngleProtoBuf(string vs, string xml)
        {
            var angle = Angle.Parse(vs);
            var roundTrip = ProtobufRoundTrip(angle);
            Assert.AreEqual(angle.Radians, roundTrip.Radians, Tolerance);
        }

        [Test]
        public void Point2DProtoBuf()
        {
            var p = new Point2D(1, 2);
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

        [Test]
        public void QuaternionProtoBuf()
        {
            var q = new Quaternion(1, 2, 3, 4);
            var result = ProtobufRoundTrip(q);
            Assert.AreEqual(q, result);
        }

        [Test]
        public void EulerAnglesProtoBuf()
        {
            var q = new Quaternion(0, 0, 0, 0);
            var eulerAngles = q.ToEulerAngles();
            var result = ProtobufRoundTrip(eulerAngles);
            Assert.AreEqual(eulerAngles, result);
        }

        [TestCase("p:{0, 0, 0} v:{0, 0, 1}", @"<Plane><Normal><X>0</X><Y>0</Y><Z>1</Z></Normal><RootPoint><X>0</X><Y>0</Y><Z>0</Z></RootPoint></Plane>")]
        public void PlaneProtoBuf(string p1s, string xml)
        {
            var plane = Plane.Parse(p1s);
            var result = ProtobufRoundTrip(plane);
            Assert.AreEqual(plane, result);
        }

        [TestCase("1, 2, 3", "-1, 2, 3", false, @"<Ray3D><Direction><X>-0.2672612419124244</X><Y>0.53452248382484879</Y><Z>0.80178372573727319</Z></Direction><ThroughPoint><X>1</X><Y>2</Y><Z>3</Z></ThroughPoint></Ray3D>")]
        public void Ray3DProtoBuf(string ps, string vs, bool asElements, string xml)
        {
            var ray = new Ray3D(Point3D.Parse(ps), UnitVector3D.Parse(vs));
            var result = ProtobufRoundTrip(ray);
            Assert.AreEqual(ray, result);
            AssertGeometry.AreEqual(ray, result, 1e-6);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><EndPoint><X>4</X><Y>5</Y><Z>6</Z></EndPoint><StartPoint><X>1</X><Y>2</Y><Z>3</Z></StartPoint></Line3D>")]
        public void Line3DProtoBuf(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            var result = ProtobufRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [TestCase("1, 2", "4, 5", @"<Line2D><EndPoint><X>4</X><Y>5</Y></EndPoint><StartPoint><X>1</X><Y>2</Y></StartPoint></Line2D>")]
        public void Line2DProtoBuf(string p1s, string p2s, string xml)
        {
            Point2D p1 = Point2D.Parse(p1s);
            Point2D p2 = Point2D.Parse(p2s);
            var l = new Line2D(p1, p2);
            var result = ProtobufRoundTrip(l);
            Assert.AreEqual(l, result);
        }

        [Test]
        public void Vector2DProtoBuf()
        {
            var v = new Vector2D(1, 2);
            var result = ProtobufRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [Test]
        public void Vector3DProtoBuf()
        {
            var v = new Vector3D(1, -2, 3);
            var result = ProtobufRoundTrip(v);
            Assert.AreEqual(v, result);
        }

        [TestCase("0, 0, 0", 2.5)]
        public void Circle3DProtoBuf(string point, double radius)
        {
            var center = Point3D.Parse(point);
            var c = new Circle3D(center, UnitVector3D.ZAxis, radius);
            var result = ProtobufRoundTrip(c);
            Assert.AreEqual(c, result);
        }

        // Not Supported in current iteration
        /*
        [Test]
        public void Polygon2DProtoBuf()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new Polygon2D(points);
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        // Not Supported in current iteration
        [Test]
        public void PolyLine2DProtoBuf()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            var p = new PolyLine2D(points);
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        // Not Supported in current iteration
        [Test]
        public void PolyLine3DProtoBuf()
        {
            var points = "0, -1.5, 0; 0,1,0; 1,1,0";
            var p = new PolyLine3D(from x in points.Split(';') select Point3D.Parse(x));
            var result = ProtobufRoundTrip(p);
            Assert.AreEqual(p, result);
        }

        // Not Supported in current iteration        
        [Test]
        public void CoordinateSystemProtoBuf()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            var result = ProtobufRoundTrip(cs);
            AssertGeometry.AreEqual(cs, result);
        }
        */
    }
}
