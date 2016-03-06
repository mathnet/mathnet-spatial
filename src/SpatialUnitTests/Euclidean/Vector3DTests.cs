using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Vector3DTests
    {
        const string X = "1; 0 ; 0";
        const string Y = "0; 1; 0";
        const string Z = "0; 0; 1";
        const string NegativeX = "-1; 0; 0";
        const string NegativeY = "0; -1; 0";
        const string NegativeZ = "0; 0; -1";

        [Test]
        public void Ctor()
        {
            var actuals = new[]
            {
                new Vector3D(1, 2, 3),
                new Vector3D(new[] {1, 2, 3.0}),
            };
            foreach (var actual in actuals)
            {
                Assert.AreEqual(1, actual.X, 1e-6);
                Assert.AreEqual(2, actual.Y, 1e-6);
                Assert.AreEqual(3, actual.Z, 1e-6);
            }
            Assert.Throws<ArgumentException>(() => new Vector3D(new[] { 1.0, 2, 3, 4 }));
        }

        [Test]
        public void ToDenseVector()
        {
            var v = new Vector3D(1, 2, 3);
            var denseVector = v.ToVector();
            Assert.AreEqual(3, denseVector.Count);
            Assert.AreEqual(1, denseVector[0], 1e-6);
            Assert.AreEqual(2, denseVector[1], 1e-6);
            Assert.AreEqual(3, denseVector[2], 1e-6);
        }

        [TestCase("1, 2, 3", "1, 2, 3", 1e-4, true)]
        [TestCase("1, 2, 3", "4, 5, 6", 1e-4, false)]
        public void Equals(string p1s, string p2s, double tol, bool expected)
        {
            var v1 = Vector3D.Parse(p1s);
            var v2 = Vector3D.Parse(p2s);
            Assert.AreEqual(expected, v1 == v2);
            Assert.AreEqual(expected, v1.Equals(v2));
            Assert.AreEqual(expected, v1.Equals((object)v2));
            Assert.AreEqual(expected, Equals(v1, v2));
            Assert.AreEqual(expected, v1.Equals(v2, tol));
            Assert.AreNotEqual(expected, v1 != v2);
        }

        [TestCase("1; 0 ; 0")]
        [TestCase("1; 1 ; 0")]
        [TestCase("1; -1 ; 0")]
        public void Orthogonal(string vs)
        {
            Vector3D v = Vector3D.Parse(vs);
            UnitVector3D orthogonal = v.Orthogonal;
            Assert.IsTrue(orthogonal.DotProduct(v) < 1e-6);
        }

        [TestCase("0; 0 ; 0")]
        public void Orthogonal_BadArgument(string vs)
        {
            Vector3D v = Vector3D.Parse(vs);
            Assert.Throws<ArgumentException>(() => { UnitVector3D orthogonal = v.Orthogonal; });
        }

        [TestCase("-2, 0, 1e-4", null, "(-2, 0, 0.0001)", 1e-4)]
        [TestCase("-2, 0, 1e-4", "F2", "(-2.00, 0.00, 0.00)", 1e-4)]
        public void ToString(string vs, string format, string expected, double tolerance)
        {
            var v = Vector3D.Parse(vs);
            string actual = v.ToString(format);
            Assert.AreEqual(expected, actual);
            AssertGeometry.AreEqual(v, Vector3D.Parse(actual), tolerance);
        }

        [TestCase(X, Y, Z)]
        [TestCase(X, "1, 1, 0", Z)]
        [TestCase(X, NegativeY, NegativeZ)]
        [TestCase(Y, Z, X)]
        [TestCase(Y, "0.1, 0.1, 1", "1, 0, -0.1", Description = "Nästan Z")]
        [TestCase(Y, "-0.1, -0.1, 1", "1, 0, 0.1", Description = "Nästan Z men minus")]
        public void CrossProductTest(string v1s, string v2s, string ves)
        {
            var vector1 = Vector3D.Parse(v1s);
            var vector2 = Vector3D.Parse(v2s);
            var expected = Vector3D.Parse(ves);
            Vector3D crossProduct = vector1.CrossProduct(vector2);
            AssertGeometry.AreEqual(expected, crossProduct, 1E-6);
        }

        [TestCase(X, Y, Z, 90)]
        [TestCase(X, X, Z, 0)]
        [TestCase(X, NegativeY, Z, -90)]
        [TestCase(X, NegativeX, Z, 180)]
        public void SignedAngleToTest(string fromString, string toString, string axisString, double degreeAngle)
        {
            var fromVector = Vector3D.Parse(fromString);
            var toVector = Vector3D.Parse(toString);
            var aboutVector = Vector3D.Parse(axisString);
            Assert.AreEqual(degreeAngle, fromVector.SignedAngleTo(toVector, aboutVector.Normalize()).Degrees, 1E-6);
        }

        [TestCase("1; 0; 1", Y, "-1; 0; 1", "90°")]
        public void SignedAngleToArbitraryVector(string fromString, string toString, string axisString, string @as)
        {
            var fromVector = Vector3D.Parse(fromString);
            var toVector = Vector3D.Parse(toString);
            var aboutVector = Vector3D.Parse(axisString);
            var angle = Angle.Parse(@as);
            Assert.AreEqual(angle.Degrees, fromVector.SignedAngleTo(toVector.Normalize(), aboutVector.Normalize()).Degrees, 1E-6);
        }

        [TestCase(X, 5)]
        [TestCase(Y, 5)]
        [TestCase("1; 1; 0", 5)]
        [TestCase("1; 0; 1", 5)]
        [TestCase("0; 1; 1", 5)]
        [TestCase("1; 1; 1", 5)]
        [TestCase(X, 90)]
        [TestCase(Y, 90)]
        [TestCase("1; 1; 0", 90)]
        [TestCase("1; 0; 1", 90)]
        [TestCase("0; 1; 1", 90)]
        [TestCase("1; 1; 1", 90)]
        [TestCase("1; 0; 1", -90)]
        [TestCase("1; 0; 1", 180)]
        [TestCase("1; 0; 1", 0)]
        public void SignedAngleTo_RotationAroundZ(string vectorDoubles, double rotationInDegrees)
        {
            var vector = Vector3D.Parse(vectorDoubles);
            Angle angle = Angle.FromDegrees(rotationInDegrees);
            Vector3D rotated = new Vector3D(Matrix3D.RotationAroundZAxis(angle).Multiply(vector.ToVector()));
            var actual = vector.SignedAngleTo(rotated, Vector3D.Parse(Z).Normalize());
            Assert.AreEqual(rotationInDegrees, actual.Degrees, 1E-6);
        }

        [TestCase(X, Z, 90, Y)]
        public void RotateTest(string vs, string avs, double deg, string evs)
        {
            var v = Vector3D.Parse(vs);
            var aboutvector = Vector3D.Parse(avs);
            var rotated = v.Rotate(aboutvector, Angle.FromDegrees(deg));
            var expected = Vector3D.Parse(evs);
            AssertGeometry.AreEqual(expected, rotated, 1E-6);
        }

        [TestCase("X", X)]
        [TestCase("Y", Y)]
        [TestCase("Z", Z)]
        public void SignedAngleTo_Itself(string axisDummy, string aboutDoubles)
        {
            Vector3D vector = new Vector3D(1, 1, 1);
            Vector3D aboutVector = Vector3D.Parse(aboutDoubles);
            var angle = vector.SignedAngleTo(vector, aboutVector.Normalize());
            Assert.AreEqual(0, angle.Degrees, 1E-6);
        }

        [Test]
        public void SignedAngleToBug()
        {
            var ninetyDegAngle = new Vector3D(0, 1, 0);
        }

        [TestCase(X, Y, "90°")]
        [TestCase(Y, X, "90°")]
        [TestCase(X, Z, "90°")]
        [TestCase(Z, X, "90°")]
        [TestCase(Y, Z, "90°")]
        [TestCase(Z, Y, "90°")]
        [TestCase(X, X, "0°")]
        [TestCase(Y, Y, "0°")]
        [TestCase(Z, Z, "0°")]
        [TestCase(X, NegativeY, "90°")]
        [TestCase(Y, NegativeY, "180°")]
        [TestCase(Z, NegativeZ, "180°")]
        [TestCase("1; 1; 0", X, "45°")]
        [TestCase("1; 1; 0", Y, "45°")]
        [TestCase("1; 1; 0", Z, "90°")]
        [TestCase("2; 2; 0", "0; 0; 2", "90°")]
        [TestCase("1; 1; 1", X, "54.74°")]
        [TestCase("1; 1; 1", Y, "54.74°")]
        [TestCase("1; 1; 1", Z, "54.74°")]
        [TestCase("1; 0; 0", "1; 0; 0", "0°")]
        [TestCase("-1; -1; 1", "-1; -1; 1", "0°")]
        [TestCase("1; 1; 1", "-1; -1; -1", "180°")]
        public void AngleToTest(string v1s, string v2s, string ea)
        {
            var v1 = Vector3D.Parse(v1s);
            var v2 = Vector3D.Parse(v2s);
            var angles = new[]
                {
                    v1.AngleTo(v2),
                    v2.AngleTo(v1)
                };
            var expected = Angle.Parse(ea);
            foreach (var angle in angles)
            {
                Assert.AreEqual(expected.Radians, angle.Radians, 1E-2);
            }
        }

        [TestCase("5; 0; 0", "1; 0 ; 0")]
        [TestCase("-5; 0; 0", "-1; 0 ; 0")]
        [TestCase("0; 5; 0", "0; 1 ; 0")]
        [TestCase("0; -5; 0", "0; -1 ; 0")]
        [TestCase("0; 0; 5", "0; 0 ; 1")]
        [TestCase("0; 0; -5", "0; 0 ; -1")]
        [TestCase("2; 2; 2", "0,577350269189626; 0,577350269189626; 0,577350269189626")]
        [TestCase("-2; 15; 2", "-0,131024356416084; 0,982682673120628; 0,131024356416084")]
        public void Normalize(string vs, string evs)
        {
            var vector = Vector3D.Parse(vs);
            var uv = vector.Normalize();
            var expected = UnitVector3D.Parse(evs);
            AssertGeometry.AreEqual(expected, uv, 1E-6);
        }

        [TestCase("0; 0; 0", "0; 0 ; 0")]
        public void Normalize_BadArgument(string vs, string evs)
        {
            var vector = Vector3D.Parse(vs);
            Assert.Throws<ArgumentException>(() => vector.Normalize());
        }

        [TestCase("1, -1, 10", 5, "5, -5, 50")]
        public void Scale(string vs, double s, string evs)
        {
            var v = Vector3D.Parse(vs);
            Vector3D actual = v.ScaleBy(s);
            AssertGeometry.AreEqual(Vector3D.Parse(evs), actual, 1e-6);
        }

        [TestCase("5;0;0", 5)]
        [TestCase("-5;0;0", 5)]
        [TestCase("-3;0;4", 5)]
        public void Length(string vectorString, double length)
        {
            var vector = Vector3D.Parse(vectorString);
            Assert.AreEqual(length, vector.Length);
        }

        [TestCase("1.0 , 2.5,3.3", new double[] { 1, 2.5, 3.3 })]
        [TestCase("1,0 ; 2,5;3,3", new double[] { 1, 2.5, 3.3 })]
        [TestCase("1.0 ; 2.5;3.3", new double[] { 1, 2.5, 3.3 })]
        [TestCase("1.0,2.5,-3.3", new double[] { 1, 2.5, -3.3 })]
        [TestCase("1;2;3", new double[] { 1, 2, 3 })]
        public void ParseTest(string vs, double[] ep)
        {
            Vector3D point3D = Vector3D.Parse(vs);
            Vector3D expected = new Vector3D(ep);
            AssertGeometry.AreEqual(point3D, expected, 1e-9);
        }

        [TestCase(X, X, true)]
        [TestCase(X, NegativeX, true)]
        [TestCase(Y, Y, true)]
        [TestCase(Y, NegativeY, true)]
        [TestCase(Z, NegativeZ, true)]
        [TestCase(Z, Z, true)]
        [TestCase("1;-8;7", "1;-8;7", true)]
        [TestCase(X, "1;-8;7", false)]
        [TestCase("1;-1.2;0", Z, false)]
        public void IsParallelToTest(string vector1, string vector2, bool isParalell)
        {
            var firstVector = Vector3D.Parse(vector1);
            var secondVector = Vector3D.Parse(vector2);
            Assert.AreEqual(isParalell, firstVector.IsParallelTo(secondVector, 1E-6));
        }

        [TestCase(X, X, false)]
        [TestCase(NegativeX, X, false)]
        [TestCase("-11;0;0", X, false)]
        [TestCase("1;1;0", X, false)]
        [TestCase(X, Y, true)]
        [TestCase(X, Z, true)]
        [TestCase(Y, X, true)]
        [TestCase(Y, Z, true)]
        [TestCase(Z, Y, true)]
        [TestCase(Z, X, true)]
        public void IsPerpendicilarToTest(string v1s, string v2s, bool expected)
        {
            var v1 = Vector3D.Parse(v1s);
            var v2 = Vector3D.Parse(v2s);
            Assert.AreEqual(expected, v1.IsPerpendicularTo(v2));
        }

        [TestCase("1,2,-3", 3, "3,6,-9")]
        public void Multiply(string vectorAsString, double mulitplier, string expected)
        {
            var vector = Vector3D.Parse(vectorAsString);
            AssertGeometry.AreEqual(Vector3D.Parse(expected), mulitplier * vector, 1e-6);
            AssertGeometry.AreEqual(Vector3D.Parse(expected), mulitplier * vector, 1e-6);
        }

        [Test]
        public void SerializeDeserialize()
        {
            var v = new Vector3D(1, -2, 3);
            const string Xml = @"<Vector3D X=""1"" Y=""-2"" Z=""3"" />";
            const string ElementXml = @"<Vector3D><X>1</X><Y>-2</Y><Z>3</Z></Vector3D>";
            var roundTrip = AssertXml.XmlSerializerRoundTrip(v, Xml);
            AssertGeometry.AreEqual(v, roundTrip);

            var serializer = new XmlSerializer(typeof(Vector3D));

            var actuals = new[]
                          {
                              Vector3D.ReadFrom(XmlReader.Create(new StringReader(Xml))),
                              Vector3D.ReadFrom(XmlReader.Create(new StringReader(ElementXml))),
                              (Vector3D)serializer.Deserialize(new StringReader(Xml)),
                              (Vector3D)serializer.Deserialize(new StringReader(ElementXml))
                          };
            foreach (var actual in actuals)
            {
                AssertGeometry.AreEqual(v, actual);
            }
        }
    }
}
