// ReSharper disable InconsistentNaming
namespace MathNet.Spatial.UnitTests.Euclidean
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;
    using NUnit.Framework;

    public class Point2DTests
    {
        [TestCase(5, "90 °", "0, 5")]
        [TestCase(3, "-90 °", "0, -3")]
        [TestCase(1, "45 °", "0.71, 0.71")]
        [TestCase(1, "-45 °", "0.71, -0.71")]
        [TestCase(1, "0 °", "1, 0")]
        [TestCase(1, "180 °", "-1, 0")]
        public void PolarCtorTest(int r, string avs, string eps)
        {
            var av = Angle.Parse(avs);
            var p = new Point2D(r, av);
            var ep = Point2D.Parse(eps);
            AssertGeometry.AreEqual(ep, p, 1e-2);
        }

        [Test]
        public void Ctor()
        {
            var p = new Point2D(1, 2);
            Assert.AreEqual(1, p.X);
            Assert.AreEqual(2, p.Y);
        }

        [TestCase("-1, -2", "1, 2", "0, 0")]
        public void OperatorAddVector2D(string ps, string vs, string eps)
        {
            var p = Point2D.Parse(ps);
            var v = Vector2D.Parse(vs);
            var actual = p + v;
            var expected = Point2D.Parse(eps);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("-1, -2", "1, 2", "-2, -4")]
        public void OperatorSubtractVector2D(string ps, string vs, string eps)
        {
            var p = Point2D.Parse(ps);
            var v = Vector2D.Parse(vs);
            var actual = p - v;
            var expected = Point2D.Parse(eps);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("-1, -2", "1, 2", "-2, -4")]
        public void OperatorSubtractPoint2D(string p1s, string p2s, string eps)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            var actual = p1 - p2;
            var expected = Vector2D.Parse(eps);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OfVector()
        {
            var p = Point2D.OfVector(DenseVector.OfArray(new[] { 1, 2.0 }));
            Assert.AreEqual(1, p.X);
            Assert.AreEqual(2, p.Y);

            Assert.Throws<ArgumentException>(() => Point2D.OfVector(DenseVector.OfArray(new[] { 1, 2, 3.0 })));
        }

        [TestCase("1.2; 3.4", 1.2, 3.4)]
        [TestCase("1,2; 3,4", 1.2, 3.4)]
        [TestCase("1.2, 3.4", 1.2, 3.4)]
        [TestCase("1.2 3.4", 1.2, 3.4)]
        [TestCase("(1.2, 3.4)", 1.2, 3.4)]
        [TestCase("(.1, 2.3e-4)", 0.1, 2.3E4)]
        public void Parse(string text, double expectedX, double expectedY)
        {
            var p = Point2D.Parse(text);
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
        }

        [TestCase("1.2")]
        public void ParseThrows(string text)
        {
            Assert.Throws<FormatException>(() => Point2D.Parse(text));
        }

        [TestCase("1, 2", "1, 2", 1e-4, true)]
        [TestCase("-1, 2", "-1, 2", 1e-4, true)]
        [TestCase("1, 2", "3, 4", 1e-4, false)]
        public void Equals(string p1s, string p2s, double tol, bool expected)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            Assert.AreEqual(expected, p1 == p2);
            Assert.AreEqual(expected, p2 == p1);
            Assert.AreEqual(!expected, p1 != p2);
            Assert.AreEqual(!expected, p2 != p1);

            Assert.AreEqual(expected, p1.Equals(p2));
            Assert.AreEqual(expected, p1.Equals((object)p2));
            Assert.AreEqual(expected, Equals(p1, p2));
            Assert.AreEqual(expected, p1.Equals(p2, tol));
            Assert.AreNotEqual(expected, p1 != p2);
        }

        [Test]
        public void ToDenseVector()
        {
            var p = new Point2D(1, 2);
            var v = p.ToVector();
            Assert.AreEqual(2, v.Count);
            Assert.AreEqual(1, v[0]);
            Assert.AreEqual(2, v[1]);
        }

        [TestCase("0, 0", "1, 0", 1.0)]
        [TestCase("2, 0", "1, 0", 1.0)]
        [TestCase("2, 0", "-1, 0", 3.0)]
        [TestCase("0, 2", "0, -1", 3.0)]
        public void DistanceTo(string p1s, string p2s, double expected)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            Assert.AreEqual(expected, p1.DistanceTo(p2));
            Assert.AreEqual(expected, p2.DistanceTo(p1));
        }

        [TestCase("0, 0", "1, 2", "0.5, 1")]
        [TestCase("-1, -2", "1, 2", "0, 0")]
        public void MidPoint(string p1s, string p2s, string eps)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            var centroids = new[]
                           {
                               Point2D.Centroid(p1, p2),
                               Point2D.MidPoint(p1, p2),
                           };
            var expected = Point2D.Parse(eps);
            foreach (var centroid in centroids)
            {
                AssertGeometry.AreEqual(expected, centroid);
            }
        }

        [TestCase("1, 0", "90 °", "0, 1")]
        [TestCase("1, 0", "-90 °", "0, -1")]
        [TestCase("1, 0", "45 °", "0.71, 0.71")]
        [TestCase("1, 0", "-45 °", "0.71, -0.71")]
        [TestCase("1, 0", "30 °", "0,87, 0.5")]
        [TestCase("-5, 0", "30 °", "-4.33, -2.5")]
        public void RotateTest(string ps, string avs, string eps)
        {
            var p = Point2D.Parse(ps);
            var av = Angle.Parse(avs);
            var expected = Point2D.Parse(eps);
            var rm = Matrix2D.Rotation(av);
            var actual = p.TransformBy(rm);
            AssertGeometry.AreEqual(expected, actual, 1e-2);
        }

        [TestCase("0, 0", "1, 2", "1, 2")]
        public void VectorTo(string p1s, string p2s, string evs)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            var actual = p1.VectorTo(p2);
            var expected = Vector2D.Parse(evs);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("1, 2", "1, 2")]
        public void ToVector(string ps, string evs)
        {
            var p1 = Point2D.Parse(ps);
            var actual = p1.ToVector2D();
            var expected = Vector2D.Parse(evs);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void XmlRoundtrip()
        {
            var p = new Point2D(1, 2);
            const string Xml = @"<Point2D X=""1"" Y=""2"" />";
            const string ElementXml = @"<Point2D><X>1</X><Y>2</Y></Point2D>";

            AssertXml.XmlRoundTrips(p, Xml, (e, a) => AssertGeometry.AreEqual(e, a));
            var serializer = new XmlSerializer(typeof(Point2D));

            var actuals = new[]
                          {
                              Point2D.ReadFrom(XmlReader.Create(new StringReader(Xml))),
                              Point2D.ReadFrom(XmlReader.Create(new StringReader(ElementXml))),
                              (Point2D)serializer.Deserialize(new StringReader(Xml)),
                              (Point2D)serializer.Deserialize(new StringReader(ElementXml))
                          };
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(p, actual);
            }
        }
    }
}
