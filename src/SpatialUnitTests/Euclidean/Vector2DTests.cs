namespace MathNet.Spatial.UnitTests.Euclidean
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml;
    using System.Xml.Serialization;
    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;
    using NUnit.Framework;

    public class Vector2DTests
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
            var p = new Vector2D(r, av);
            var ep = Vector2D.Parse(eps);
            AssertGeometry.AreEqual(ep, p, 1e-2);
        }

        [Test]
        public void Ctor()
        {
            var points = new[]
                           {
                               new Vector2D(1, 2),
                               new Vector2D(new[] { 1, 2.0 }),
                               new Vector2D(DenseVector.OfArray(new[] { 1, 2.0 })),
                           };
            foreach (var p in points)
            {
                Assert.AreEqual(1, p.X);
                Assert.AreEqual(2, p.Y);
            }

            Assert.Throws<ArgumentException>(() => new Vector2D(new[] { 1, 2, 3.0 }));
            Assert.Throws<ArgumentException>(() => new Vector2D(DenseVector.OfArray(new[] { 1, 2, 3.0 })));
        }

        [TestCase("1, 0", "1, 0", 1e-4, true)]
        [TestCase("-1, 1", "-1, 1", 1e-4, true)]
        [TestCase("1, 0", "1, 1", 1e-4, false)]
        public void Equals(string v1s, string v2s, double tol, bool expected)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            Assert.AreEqual(expected, v1 == v2);
            Assert.AreEqual(expected, v1.Equals(v2));
            Assert.AreEqual(expected, v1.Equals((object)v2));
            Assert.AreEqual(expected, Equals(v1, v2));
            Assert.AreEqual(expected, v1.Equals(v2, tol));
            Assert.AreNotEqual(expected, v1 != v2);
        }

        [TestCase("-1, -2", "1, 2", "0, 0")]
        public void Add(string v1s, string v2s, string evs)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            var actuals = new[]
                            {
                                v1 + v2,
                                v2 + v1,
                                v1.Add(v2),
                                v2.Add(v1)
                            };
            var expected = Vector2D.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("-1, -2", "1, 2", "-2, -4")]
        public void Subtract(string v1s, string v2s, string evs)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            var actuals = new[]
                          {
                              v1 - v2,
                              v1.Subtract(v2)
                          };
            var expected = Vector2D.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("-1, -2", "1, 2")]
        public void Negate(string vs, string evs)
        {
            var v = Vector2D.Parse(vs);
            var actuals = new[]
                            {
                                -v,
                                v.Negate()
                            };
            var expected = Vector2D.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("-1, -2", 2, "-2, -4")]
        public void MultiplyAndScaleBy(string vs, double d, string evs)
        {
            var v = Vector2D.Parse(vs);
            var actuals = new[]
                          {
                              d * v,
                              v.ScaleBy(d)
                          };
            var expected = Vector2D.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("-1, -2", 2, "-2, -4")]
        public void FlippedMultiplyAndScaleBy(string vs, double d, string evs)
        {
            var v = Vector2D.Parse(vs);
            var actuals = new[]
                          {
                              v * d,
                              v.ScaleBy(d)
                          };
            var expected = Vector2D.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("-1, -2", 2, "-0.5, -1")]
        public void Divide(string vs, double d, string evs)
        {
            var v = Vector2D.Parse(vs);
            var actual = v / d;
            var expected = Vector2D.Parse(evs);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("1.2; 3.4", 1.2, 3.4)]
        [TestCase("1.2;3.4", 1.2, 3.4)]
        [TestCase("1,2; 3,4", 1.2, 3.4)]
        [TestCase("1.2, 3.4", 1.2, 3.4)]
        [TestCase("1.2 3.4", 1.2, 3.4)]
        [TestCase("(1.2, 3.4)", 1.2, 3.4)]
        [TestCase("(.1, 2.3e-4)", 0.1, 0.00023000000000000001)]
        public void Parse(string text, double expectedX, double expectedY)
        {
            Assert.AreEqual(true, Vector2D.TryParse(text, out var p));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);

            p = Vector2D.Parse(text);
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);

            p = Vector2D.Parse(p.ToString());
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
        }

        [TestCase("1,2; 3,4", 1.2, 3.4)]
        [TestCase("1,2;3,4", 1.2, 3.4)]
        [TestCase("1,2 3,4", 1.2, 3.4)]
        [TestCase("(1,2, 3,4)", 1.2, 3.4)]
        [TestCase("(,1 2,3e-4)", 0.1, 0.00023000000000000001)]
        public void ParseSwedish(string text, double expectedX, double expectedY)
        {
            var culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual(true, Vector2D.TryParse(text, culture, out var p));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);

            p = Vector2D.Parse(text, culture);
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);

            p = Vector2D.Parse(p.ToString(culture));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
        }

        [TestCase("1.2")]
        [TestCase("1; 2; 3")]
        public void ParseFails(string text)
        {
            Assert.AreEqual(false, Vector2D.TryParse(text, out _));
            Assert.Throws<FormatException>(() => Vector2D.Parse(text));
        }

        [TestCase("2, 0", 2)]
        [TestCase("-2, 0", 2)]
        [TestCase("0, 2", 2)]
        public void Length(string vs, double expected)
        {
            var v = Vector2D.Parse(vs);
            Assert.AreEqual(expected, v.Length, 1e-6);
        }

        [TestCase("-2, 0", null, "(-2, 0)")]
        [TestCase("-2, 0", "N2", "(-2.00, 0.00)")]
        public void ToString(string vs, string format, string expected)
        {
            var v = Vector2D.Parse(vs);
            string actual = v.ToString(format);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(v, Vector2D.Parse(actual));
        }

        [Test]
        public void ToDenseVector()
        {
            var v = new Vector2D(1, 2);
            var actual = v.ToVector();
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(1, actual[0]);
            Assert.AreEqual(2, actual[1]);
        }

        [TestCase("1, 0", "1, 0", 1e-4, false)]
        [TestCase("1, 0", "0, -1", 1e-4, true)]
        [TestCase("1, 0", "0, 1", 1e-4, true)]
        [TestCase("0, 1", "1, 0", 1e-4, true)]
        [TestCase("0, 1", "0, 1", 1e-4, false)]
        public void IsPerpendicularTo(string v1s, string v2s, double tol, bool expected)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            Assert.AreEqual(expected, v1.IsPerpendicularTo(v2, tol));
            Assert.AreEqual(expected, v2.IsPerpendicularTo(v1, tol));
        }

        [TestCase("1, 0", "1, 0", 1e-10, true)]
        [TestCase("1, 0", "-1, 0", 1e-10, true)]
        [TestCase("1, 0", "1, 1", 1e-10, false)]
        [TestCase("1, 1", "1, 1", 1e-10, true)]
        [TestCase("1, -1", "-1, 1", 1e-10, true)]
        [TestCase("1, 0.5", "-1, -0.5", 1e-10, true)]
        [TestCase("1, 0.5", "1, 0.5001", 1e-10, false)]
        [TestCase("1, 0.5", "1, 0.5001", 1e-8, true)] // Demonstration of the effect of tolerance
        public void IsParallelToByDoubleTolerance(string v1s, string v2s, double tol, bool expected)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            Assert.AreEqual(expected, v1.IsParallelTo(v2, tol));
            Assert.AreEqual(expected, v2.IsParallelTo(v1, tol));
        }

        [TestCase("1, 0", "1, 0", 1e-4, true)]
        [TestCase("1, 0", "-1, 0", 1e-4, true)]
        [TestCase("1, 0", "1, 1", 1e-4, false)]
        [TestCase("1, 1", "1, 1", 1e-4, true)]
        [TestCase("1, -1", "-1, 1", 1e-4, true)]
        [TestCase("1, 0", "1, 0.001", 0.06, true)]
        [TestCase("1, 0", "1, -0.001", 0.06, true)]
        [TestCase("-1, 0", "1, 0.001", 0.06, true)]
        [TestCase("-1, 0", "1, -0.001", 0.06, true)]
        [TestCase("1, 0", "1, 0.001", 0.05, false)]
        [TestCase("1, 0", "1, -0.001", 0.05, false)]
        [TestCase("-1, 0", "1, 0.001", 0.05, false)]
        [TestCase("-1, 0", "1, -0.001", 0.05, false)]
        [TestCase("1, 0.5", "-1, -0.5", 1e-4, true)]
        public void IsParallelToByAngleTolerance(string v1s, string v2s, double degreesTolerance, bool expected)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            Assert.AreEqual(expected, v1.IsParallelTo(v2, Angle.FromDegrees(degreesTolerance)));
            Assert.AreEqual(expected, v2.IsParallelTo(v1, Angle.FromDegrees(degreesTolerance)));
        }

        [TestCase("1, 0", "0, 1", false, 90)]
        [TestCase("1, 0", "0, 1", true, 270)]
        [TestCase("1, 0", "0, -1", true, 90)]
        [TestCase("1, 0", "0, -1", false, 270)]
        [TestCase("1, 0", "-1, 0", true, 180)]
        [TestCase("1, 0", "-1, 0", false, 180)]
        [TestCase("1, 0", "1, 0", false, 0)]
        [TestCase("0, 1", "1, 0", true, 90)]
        public void SignedAngleToTest(string v1s, string v2s, bool clockWise, float expected)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            Angle av = v1.SignedAngleTo(v2, clockWise);
            Assert.AreEqual(expected, av.Degrees, 0.1);
        }

        [TestCase("1, 0", "90°", "0, 1")]
        [TestCase("1, 0", "-270°", "0, 1")]
        [TestCase("1, 0", "-90°", "0, -1")]
        [TestCase("1, 0", "270°", "0, -1")]
        [TestCase("1, 0", "180°", "-1, 0")]
        [TestCase("1, 0", "0°", "1, 0")]
        [TestCase("0, 1", "-90°", "1, 0")]
        public void Rotate(string vs, string @as, string evs)
        {
            var v = Vector2D.Parse(vs);
            var angle = Angle.Parse(@as);
            var actuals = new[]
                          {
                              v.Rotate(angle),
                              v.Rotate(angle.Degrees, AngleUnit.Degrees)
                          };
            var expected = Vector2D.Parse(evs);
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(expected, actual, 0.01);
            }
        }

        [TestCase("1, 2", "3, 4", 11)]
        public void DotProduct(string vs, string evs, double expected)
        {
            var v1 = Vector2D.Parse(vs);
            var v2 = Vector2D.Parse(evs);
            Assert.AreEqual(expected, v1.DotProduct(v2));
        }

        [TestCase("2, 3", "0.55470019, 0.83205029")]
        public void Normalize(string vs, string evs)
        {
            var v1 = Vector2D.Parse(vs);
            var expected = Vector2D.Parse(evs);
            AssertGeometry.AreEqual(expected, v1.Normalize());
        }

        [TestCase("1,0", "0,1", "270°", "-90°")]
        [TestCase("0,1", "1,0", "90°", "90°")]
        [TestCase("-0.99985, 0.01745", "-1, 0", "359°", "-1°")]
        [TestCase("-0.99985, -0.01745", "-1, 0", "1°", "1°")]
        [TestCase("0.99985, 0.01745", "1, 0", "1°", "1°")]
        [TestCase("0.99985, -0.01745", "1, 0", "359°", "-1°")]
        public void SignedAngleTo(string v1s, string v2s, string expectedClockWise, string expectedNegative)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            var cw = v1.SignedAngleTo(v2, true);
            var expected = Angle.Parse(expectedClockWise);
            Assert.AreEqual(expected.Degrees, cw.Degrees, 1e-3);
            var cwNeg = v1.SignedAngleTo(v2, true, true);
            Assert.AreEqual(Angle.Parse(expectedNegative).Degrees, cwNeg.Degrees, 1e-3);

            var ccw = v1.SignedAngleTo(v2, false);
            Assert.AreEqual(360 - expected.Degrees, ccw.Degrees, 1e-3);
        }

        [TestCase(@"<Vector2D X=""1"" Y=""2"" />")]
        [TestCase(@"<Vector2D><X>1</X><Y>2</Y></Vector2D>")]
        public void ReadFrom(string xml)
        {
            var v = new Vector2D(1, 2);
            AssertGeometry.AreEqual(v, Vector2D.ReadFrom(XmlReader.Create(new StringReader(xml))));
        }

        [Test]
        public void PolarConstructorThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => { new Vector2D(-1.0, new Angle(0, default(Radians))); });
        }

        [Test]
        public void CheckCachedXAxis()
        {
            AssertGeometry.AreEqual(new Vector2D(1, 0), Vector2D.XAxis);
        }

        [Test]
        public void CheckCachedYAxis()
        {
            AssertGeometry.AreEqual(new Vector2D(0, 1), Vector2D.YAxis);
        }

        [Test]
        public void EqualityComparerThrowsExceptionOnNegativeTolerance()
        {
            var v1 = new Vector2D(0, 0);
            var v2 = new Vector2D(1, 1);
            Assert.Throws<ArgumentException>(() => v1.Equals(v2, -0.01));
        }

        [Test]
        public void EqualityComparerReturnsFalseOnNullReference()
        {
            Assert.IsFalse(default(Vector2D).Equals(null));
        }

        [TestCase("1,0", "0,1", "90°")]
        [TestCase("0,1", "1,0", "90°")]
        [TestCase("-0.99985, 0.01745", "-1, 0", "1°")]
        [TestCase("-0.99985, -0.01745", "-1, 0", "1°")]
        [TestCase("0.99985, 0.01745", "1, 0", "1°")]
        [TestCase("0.99985, -0.01745", "1, 0", "1°")]
        [TestCase("-0.99985, -0.01745", "1, 0", "179°")]
        [TestCase("-0.99985, 0.01745", "1, 0", "179°")]
        public void UnSignedAngleTo(string v1s, string v2s, string expectedAngle)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            var expected = Angle.Parse(expectedAngle);

            var angle = v1.AngleTo(v2);

            Assert.AreEqual(expected.Degrees, angle.Degrees, 1e-3);
        }

        [TestCase("1,0", "0,1", 1)]
        [TestCase("-1,0", "0,1", -1)]
        [TestCase("0.5003,-0.7066", "0.0739,0.7981", 0.452)]
        [TestCase("0.7097,0.6059", "0.0142,-0.7630", -0.550)]
        [TestCase("-0.6864,0.7036", "-0.8541,-0.1124", 0.678)]
        [TestCase("-0.2738,0.6783", "0.1695,0.9110", -0.364)]
        public void CrossProducts(string v1s, string v2s, double expected)
        {
            // Generated test data from http://calculator.tutorvista.com/math/8/cross-product-calculator.html
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);

            double cross = v1.CrossProduct(v2);

            Assert.AreEqual(expected, cross, 1e-3);
        }

        [TestCase("1,0", "2,0", "1,0")]
        [TestCase("1,1", "2,0", "1,0")]
        [TestCase("1,0", "-2,0", "1,0")]
        [TestCase("-1,1", "2,0", "-1,0")]
        [TestCase("-0.0563,-0.2904", "-0.3671,-0.7945", "-0.120,-0.261")]
        [TestCase("0.4610,0.9067", "-0.7948,-0.7748", "0.690,0.672")]
        [TestCase("0.3916,-0.9644", "-0.0873,0.0978", "0.653,-0.731")]
        public void VectorProjection(string v1s, string v2s, string exs)
        {
            var v1 = Vector2D.Parse(v1s);
            var v2 = Vector2D.Parse(v2s);
            var ex = Vector2D.Parse(exs);

            AssertGeometry.AreEqual(ex, v1.ProjectOn(v2), 1e-3);
        }


        [Test]
        public void XmlRoundtrip()
        {
            var v = new Vector2D(1, 2);
            AssertXml.XmlRoundTrips(v, @"<Vector2D X=""1"" Y=""2"" />", (e, a) => AssertGeometry.AreEqual(e, a));
        }

        [Test]
        public void XmlContainerRoundtrip()
        {
            var container = new AssertXml.Container<Vector2D>
            {
                Value1 = new Vector2D(1, 2),
                Value2 = new Vector2D(3, 4)
            };
            var expected = "<ContainerOfVector2D>\r\n" +
                           "  <Value1 X=\"1\" Y=\"2\"></Value1>\r\n" +
                           "  <Value2 X=\"3\" Y=\"4\"></Value2>\r\n" +
                           "</ContainerOfVector2D>";
            var roundTrip = AssertXml.XmlSerializerRoundTrip(container, expected);
            AssertGeometry.AreEqual(container.Value1, roundTrip.Value1);
            AssertGeometry.AreEqual(container.Value2, roundTrip.Value2);
        }

        [Test]
        public void XmlElements()
        {
            var v = new Vector2D(1, 2);
            var serializer = new XmlSerializer(typeof(Vector2D));
            AssertGeometry.AreEqual(v, (Vector2D)serializer.Deserialize(new StringReader(@"<Vector2D><X>1</X><Y>2</Y></Vector2D>")));
        }

        [Test]
        public void XmlContainerElements()
        {
            var container = new AssertXml.Container<Vector2D>
                            {
                                Value1 = new Vector2D(1, 2),
                                Value2 = new Vector2D(3, 4)
                            };
            var xml = "<ContainerOfVector2D>\r\n" +
                      "  <Value1><X>1</X><Y>2</Y></Value1>\r\n" +
                      "  <Value2><X>3</X><Y>4</Y></Value2>\r\n" +
                      "</ContainerOfVector2D>";
            var serializer = new XmlSerializer(typeof(AssertXml.Container<Vector2D>));
            var deserialized = (AssertXml.Container<Vector2D>)serializer.Deserialize(new StringReader(xml));
            AssertGeometry.AreEqual(container.Value1, deserialized.Value1);
            AssertGeometry.AreEqual(container.Value2, deserialized.Value2);
        }

        [Test]
        public void BinaryRountrip()
        {
            var v = new Vector2D(1, 2);
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, v);
                ms.Flush();
                ms.Position = 0;
                var roundTrip = (Vector2D)formatter.Deserialize(ms);
                AssertGeometry.AreEqual(v, roundTrip);
            }
        }
    }
}
