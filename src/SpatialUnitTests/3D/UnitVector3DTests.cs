using System.IO;
using System.Xml;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests
{
    using System;
    using System.Xml.Serialization;

    [TestFixture]
    public class UnitVector3DTests
    {
        [Test]
        public void Ctor()
        {
            var l = Math.Sqrt(1 * 1 + 2 * 2 + 3 * 3);
            var actuals = new[]
            {
                new UnitVector3D(1/l, 2/l, 3/l),
                new UnitVector3D(1, 2, 3), // Ctor normalizes
                new UnitVector3D(new[] {1, 2, 3.0}),
                new UnitVector3D(new[] {1/l, 2/l, 3.0/l}),
            };
            foreach (var actual in actuals)
            {
                Assert.AreEqual(1 / l, actual.X, 1e-6);
                Assert.AreEqual(2 / l, actual.Y, 1e-6);
                Assert.AreEqual(3 / l, actual.Z, 1e-6);
            }
            Assert.Throws<ArgumentException>(() => new UnitVector3D(new[] { 1.0, 2, 3, 4 }));
        }

        [Test]
        public void ToDenseVector()
        {
            var l = Math.Sqrt(1 * 1 + 2 * 2 + 3 * 3);

            var uv = new UnitVector3D(1 / l, 2 / l, 3 / l);
            var denseVector = uv.ToVector();
            Assert.AreEqual(3, denseVector.Count);
            Assert.AreEqual(1 / l, denseVector[0], 1e-6);
            Assert.AreEqual(2 / l, denseVector[1], 1e-6);
            Assert.AreEqual(3 / l, denseVector[2], 1e-6);
        }

        [TestCase("1, 2, 3", "1, 2, 3", 1e-4, true)]
        [TestCase("1, 2, 3", "4, 5, 6", 1e-4, false)]
        public void Equals(string p1s, string p2s, double tol, bool expected)
        {
            var v1 = UnitVector3D.Parse(p1s);
            var v2 = UnitVector3D.Parse(p2s);
            var vector3D = v1.ToVector3D();
            Assert.AreEqual(expected, v1 == v2);
            Assert.IsTrue(v1 == vector3D);
            Assert.IsTrue(vector3D == v1);

            Assert.AreEqual(expected, v1.Equals(v2));
            Assert.IsTrue(v1.Equals(vector3D));
            Assert.IsTrue(vector3D.Equals(v1));
            Assert.AreEqual(expected, v1.Equals(v2.ToVector3D()));
            Assert.AreEqual(expected, v2.ToVector3D().Equals(v1));

            Assert.AreEqual(expected, v1.Equals((object)v2));
            Assert.AreEqual(expected, Equals(v1, v2));

            Assert.AreEqual(expected, v1.Equals(v2, tol));
            Assert.AreNotEqual(expected, v1 != v2);
            Assert.AreNotEqual(expected, v1 != v2.ToVector3D());
            Assert.AreNotEqual(expected, v2.ToVector3D() != v1);
        }

        [TestCase("1; 0; 0", 5, "5; 0; 0")]
        [TestCase("1; 0; 0", -5, "-5; 0; 0")]
        [TestCase("-1; 0; 0", 5, "-5; 0; 0")]
        [TestCase("-1; 0; 0", -5, "5; 0; 0")]
        [TestCase("0; 1; 0", 5, "0; 5; 0")]
        [TestCase("0; 0; 1", 5, "0; 0; 5")]
        public void Scale(string ivs, double s, string exs)
        {
            var uv = UnitVector3D.Parse(ivs);
            var v = uv.ScaleBy(s);
            AssertGeometry.AreEqual(Vector3D.Parse(exs), v, float.Epsilon);
        }

        [TestCase("-1, 0, 0", null, "(-1, 0, 0)", 1e-4)]
        [TestCase("-1, 0, 1e-4", "F2", "(-1.00, 0.00, 0.00)", 1e-3)]
        public void ToString(string vs, string format, string expected, double tolerance)
        {
            var v = UnitVector3D.Parse(vs);
            string actual = v.ToString(format);
            Assert.AreEqual(expected, actual);
            AssertGeometry.AreEqual(v, UnitVector3D.Parse(actual), tolerance);
        }

        [Test]
        public void XmlRoundTrips()
        {
            var uv = new UnitVector3D(0.2672612419124244, -0.53452248382484879, 0.80178372573727319);
            var xml = @"<UnitVector3D X=""0.2672612419124244"" Y=""-0.53452248382484879"" Z=""0.80178372573727319"" />";
            var elementXml = @"<UnitVector3D><X>0.2672612419124244</X><Y>-0.53452248382484879</Y><Z>0.80178372573727319</Z></UnitVector3D>";

            AssertXml.XmlRoundTrips(uv, xml, (e, a) => AssertGeometry.AreEqual(e, a));
            var serializer = new XmlSerializer(typeof (UnitVector3D));
            var actuals = new[]
                                {
                                    UnitVector3D.ReadFrom(XmlReader.Create(new StringReader(xml))),
                                    (UnitVector3D)serializer.Deserialize(new StringReader(xml)),
                                    (UnitVector3D)serializer.Deserialize(new StringReader(elementXml))
                                };
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(uv, actual);
            }
        }

        [Test]
        public void CastTest()
        {
            Assert.Inconclusive("Is it possible to implement this in a good way?");
            //Vector<double> vector = new DenseVector(new[] { 1.0, 1.0, 0.0 });
            //UnitVector3D casted = (UnitVector3D)vector;
            //Assert.AreEqual(1, casted.Length, 0.00001);
            //Vector<double> castback = casted;

            //Vector<double> vectorTooLong = new DenseVector(new[] { 1.0, 0.0, 0.0, 0.0 });
            //UnitVector3D vectorFail;
            //Assert.Throws<InvalidCastException>(() => vectorFail = (UnitVector3D)vectorTooLong);
        }

        [TestCase("1,0,0", 3, "3,0,0")]
        public void MultiplyTest(string unitVectorAsString, double multiplier, string expected)
        {
            UnitVector3D unitVector3D = UnitVector3D.Parse(unitVectorAsString);
            Assert.AreEqual(Vector3D.Parse(expected), multiplier * unitVector3D);
        }
    }
}
