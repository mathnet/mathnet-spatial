using System.Runtime.InteropServices;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Euclidean
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class CoordinateSystemTest
    {
        private const string X = "1; 0 ; 0";
        private const string Y = "0; 1; 0";
        private const string Z = "0; 0; 1";
        private const string NegativeX = "-1; 0; 0";
        private const string NegativeY = "0; -1; 0";
        private const string NegativeZ = "0; 0; -1";
        private const string TestX = "4, 5, 6";
        private const string TestY = "0.732992355020098, 0.19546462800535944, -0.65154876001786488";

        [TestCase("1, 2, 3", TestX, TestY)]
        public void ConstructorTest(string ps, string xs, string ys)
        {
            var origin = Point3D.Parse(ps);
            var xAxis = Vector3D.Parse(xs);
            var yAxis = Vector3D.Parse(ys);
            var css = new[]
            {
                new CoordinateSystem(origin, xAxis, yAxis),
                new CoordinateSystem(xAxis, yAxis, origin)
            };
            foreach (var cs in css)
            {
                AssertGeometry.AreEqual(origin, cs.Origin);
                AssertGeometry.AreEqual(xAxis.Normalize(), cs.XAxis);
                AssertGeometry.AreEqual(yAxis.Normalize(), cs.YAxis);
            }
        }

        [TestCase("o:{1, 2e-6, -3} x:{4, 5, 6} y:{0.732992355020098, 0.19546462800535944, -0.65154876001786488} z:{4, 4, 4}", "1, 2e-6, -3", TestX, TestY, "-0.5049059315257477, 0.79819687704438047, -0.32856010985315204")]
        public void ParseTests(string s, string ops, string xs, string ys, string zs)
        {
            var cs = CoordinateSystem.Parse(s);
            AssertGeometry.AreEqual(Point3D.Parse(ops), cs.Origin);
            AssertGeometry.AreEqual(Vector3D.Parse(xs).Normalize(), cs.XAxis);
            AssertGeometry.AreEqual(Vector3D.Parse(ys).Normalize(), cs.YAxis);
            AssertGeometry.AreEqual(Vector3D.Parse(zs).Normalize(), cs.ZAxis);
        }

        [TestCase("1, 2, 3", "90°", "0, 0, 1", "-2, 1, 3")]
        [TestCase("1, 2, 3", "90°", "0, 0, -1", "2, -1, 3")]
        [TestCase("1, 2, 3", "-90°", "0, 0, 1", "2, -1, 3")]
        [TestCase("1, 2, 3", "180°", "0, 0, 1", "-1, -2, 3")]
        [TestCase("1, 2, 3", "270°", "0, 0, 1", "2, -1, 3")]
        [TestCase("1, 2, 3", "90°", "1, 0, 0", "1, -3, 2")]
        [TestCase("1, 2, 3", "-90°", "1, 0, 0", "1, 3, -2")]
        [TestCase("1, 2, 3", "90°", "-1, 0, 0", "1, 3, -2")]
        [TestCase("1, 2, 3", "90°", "0, 1, 0", "3, 2, -1")]
        [TestCase("1, 2, 3", "-90°", "0, 1, 0", "-3, 2, 1")]
        public void RotationAroundVector(string ps, string @as, string vs, string eps)
        {
            var p = Point3D.Parse(ps);
            var angle = Angle.Parse(@as);
            var coordinateSystems = new[]
            {
                CoordinateSystem.Rotation(angle, Direction.Parse(vs)),
                CoordinateSystem.Rotation(angle, Vector3D.Parse(vs)),
            };
            var expected = Point3D.Parse(eps);
            foreach (var coordinateSystem in coordinateSystems)
            {
                var rotatedPoint = coordinateSystem.Transform(p);
                AssertGeometry.AreEqual(expected, rotatedPoint);
            }
        }

        [TestCase("0°", "0°", "0°", "1, 2, 3", "1, 2, 3")]
        [TestCase("90°", "0°", "0°", "1, 2, 3", "-2, 1, 3")]
        [TestCase("-90°", "0°", "0°", "1, 2, 3", "2, -1, 3")]
        [TestCase("0°", "90°", "0°", "1, 2, 3", "3, 2, -1")]
        [TestCase("0°", "-90°", "0°", "1, 2, 3", "-3, 2, 1")]
        [TestCase("0°", "0°", "90°", "1, 2, 3", "1, -3, 2")]
        [TestCase("0°", "0°", "-90°", "1, 2, 3", "1, 3, -2")]
        [TestCase("90°", "90°", "90°", "1, 2, 3", "3, 2, -1")]
        [TestCase("90°", "0°", "90°", "1, 2, 3", "3, 1, 2")]
        [TestCase("180°", "0°", "90°", "1, 2, 3", "-1, 3, 2")]
        [TestCase("180°", "-90°", "0°", "1, 2, 3", "3, -2, 1")]
        [TestCase("90°", "10°", "0°", "1, 2, 3", "-2, 1.506, 2.781")]
        [TestCase("90°", "10°", "30°", "1, 2, 3", "-0.232, 1.609, 3.370")]
        [TestCase("15°", "-23°", "48°", "1, 2, 3", "-0.199, -0.976, 3.607")]
        public void RotationYawPitchRoll(string yaws, string pitchs, string rolls, string ps, string eps)
        {
            var p = Point3D.Parse(ps);
            var yaw = Angle.Parse(yaws);
            var pitch = Angle.Parse(pitchs);
            var roll = Angle.Parse(rolls);
            var coordinateSystem = CoordinateSystem.Rotation(yaw, pitch, roll);
            var expected = Point3D.Parse(eps);
            var rotatedPoint = coordinateSystem.Transform(p);
            AssertGeometry.AreEqual(expected, rotatedPoint, 1e-3);
        }

        [TestCase("1, 2, 3", "0, 0, 1", "1, 2, 4")]
        [TestCase("1, 2, 3", "0, 0, -1", "1, 2, 2")]
        [TestCase("1, 2, 3", "0, 0, 0", "1, 2, 3")]
        [TestCase("1, 2, 3", "0, 1, 0", "1, 3, 3")]
        [TestCase("1, 2, 3", "0, -1, 0", "1, 1, 3")]
        [TestCase("1, 2, 3", "1, 0, 0", "2, 2, 3")]
        [TestCase("1, 2, 3", "-1, 0, 0", "0, 2, 3")]
        public void Translation(string ps, string vs, string eps)
        {
            var p = Point3D.Parse(ps);
            var cs = CoordinateSystem.Translation(Vector3D.Parse(vs));
            var tp = cs.Transform(p);
            AssertGeometry.AreEqual(Point3D.Parse(eps), tp);
        }

        [TestCase(X, X, null)]
        [TestCase(X, X, X)]
        [TestCase(X, X, Y)]
        [TestCase(X, X, Z)]
        [TestCase(X, NegativeX, null)]
        [TestCase(X, NegativeX, Z)]
        [TestCase(X, NegativeX, Y)]
        [TestCase(X, Y, null)]
        [TestCase(X, Z, null)]
        [TestCase(Y, Y, null)]
        [TestCase(Y, Y, X)]
        [TestCase(Y, NegativeY, null)]
        [TestCase(Y, NegativeY, X)]
        [TestCase(Y, NegativeY, Z)]
        [TestCase(Z, NegativeZ, null)]
        [TestCase(Z, NegativeZ, X)]
        [TestCase(Z, NegativeZ, Y)]
        [TestCase("1, 2, 3", "-1, 0, -1", null)]
        public void RotateToTest(string v1s, string v2s, string @as)
        {
            var axis = string.IsNullOrEmpty(@as) ? (Direction?)null : Vector3D.Parse(@as).Normalize();
            var v1 = Vector3D.Parse(v1s).Normalize();
            var v2 = Vector3D.Parse(v2s).Normalize();
            var actual = CoordinateSystem.RotateTo(v1, v2, axis);
            var rv = actual.Transform(v1);
            AssertGeometry.AreEqual(v2, rv);
            actual = CoordinateSystem.RotateTo(v2, v1, axis);
            rv = actual.Transform(v2);
            AssertGeometry.AreEqual(v1, rv);
        }

        [Test]
        public void InvertTest()
        {
            var cs = new CoordinateSystem(new Point3D(1, 2), Direction.Create(3, 1, 0), Direction.Create(-1, 3, 0));
            var pointLocal = new Point3D(1, 2, 3);
            var pointGlobal = cs.Transform(pointLocal);
            var actualPointLocal = cs.Invert().Transform(pointGlobal);
            AssertGeometry.AreEqual(pointLocal, actualPointLocal);
        }

        [Test]
        public void EqualityNullOperator()
        {
            var test = "o:{1, 2e-6, -3} x:{" + TestX + "} y:{" + TestY + "} z:{4, 4, 4}";
            var cs = CoordinateSystem.Parse(test);

            Assert.IsFalse(cs == null);
        }

        [Test]
        public void EqualityNullOperatorTrue()
        {
            CoordinateSystem cs = null;

            Assert.IsTrue(cs == null);
        }

        [Test]
        public void EqualityNotNullOperator()
        {
            var test = "o:{1, 2e-6, -3} x:{" + TestX + "} y:{" + TestY +"} z:{4, 4, 4}";
            var cs = CoordinateSystem.Parse(test);

            Assert.IsTrue(cs != null);
        }

        [Test]
        public void EqualityNotNullOperatorFalse()
        {
            CoordinateSystem cs = null;

            Assert.IsFalse(cs != null);
        }

        [Test]
        public void EqualityNull()
        {
            var test = "o:{1, 2e-6, -3} x:{" + TestX + "} y:{" + TestY + "} z:{4, 4, 4}";
            var cs = CoordinateSystem.Parse(test);

            Assert.IsFalse(cs.Equals(null));
        }

        [TestCase("1; -5; 3", "1; -5; 3", "o:{0, 0, 0} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        public void TransformPoint(string ps, string eps, string css)
        {
            var p = Point3D.Parse(ps);
            var cs = CoordinateSystem.Parse(css);
            var actual = p.TransformBy(cs);
            var expected = Point3D.Parse(eps);
            AssertGeometry.AreEqual(expected, actual, float.Epsilon);
        }

        [TestCase("1; 2; 3", "1; 2; 3", "o:{0, 0, 0} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        [TestCase("1; 2; 3", "1; 2; 3", "o:{3, 4, 5} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        public void TransformVector(string vs, string evs, string css)
        {
            var v = Vector3D.Parse(vs);
            var cs = CoordinateSystem.Parse(css);
            var actual = cs.Transform(v);
            var expected = Vector3D.Parse(evs);
            AssertGeometry.AreEqual(expected, actual);
        }

        [Test]
        public void TransformUnitVector()
        {
            var cs = CoordinateSystem.Rotation(Angle.FromDegrees(90), Direction.ZAxis);
            var uv = Direction.XAxis;
            var actual = cs.Transform(uv);
            AssertGeometry.AreEqual(Direction.YAxis, actual);
        }

        [TestCase("o:{0, 0, 0} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}", "o:{0, 0, 0} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        [TestCase("o:{0, 0, 0} x:{10, 0, 0} y:{0, 1, 0} z:{0, 0, 1}", "o:{1, 0, 0} x:{0.1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        [TestCase("o:{0, 0, 0} x:{10, 0, 0} y:{0, 1, 0} z:{0, 0, 1}", "o:{1, 0, 0} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        [TestCase("o:{1, 2, -7} x:{10, 0, 0} y:{0, 1, 0} z:{0, 0, 1}", "o:{0, 0, 0} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        [TestCase("o:{1, 2, -7} x:{10, 0.1, 0} y:{-0.1, 10, 0} z:{0.1, 0, 1}", "o:{2, 5, 1} x:{0.1, 2, 0} y:{1.6035674514745464, -0.080178372573727327, -0.48107023544236394} z:{0, 0.4, 1}")]
        public void SetToAlignCoordinateSystemsTest(string fcss, string tcss)
        {
            var fcs = CoordinateSystem.Parse(fcss);
            var tcs = CoordinateSystem.Parse(tcss);

            var css = new[]
            {
                CoordinateSystem.SetToAlignCoordinateSystems(fcs.Origin, fcs.XAxis, fcs.YAxis, tcs.Origin, tcs.XAxis, tcs.YAxis),
                CoordinateSystem.CreateMappingCoordinateSystem(fcs, tcs)
            };
            foreach (var cs in css)
            {
                var aligned = cs.Transform(fcs);
                AssertGeometry.AreEqual(tcs.Origin, aligned.Origin);
                AssertGeometry.AreEqual(tcs.XAxis, aligned.XAxis);
                AssertGeometry.AreEqual(tcs.YAxis, aligned.YAxis);
                AssertGeometry.AreEqual(tcs.ZAxis, aligned.ZAxis);
            }
        }

        [TestCase(X, Y, Z)]
        [TestCase(NegativeX, Y, Z)]
        [TestCase(NegativeX, Y, null)]
        [TestCase(X, Y, null)]
        [TestCase(X, Y, "0,0,1")]
        [TestCase("1,-1, 1", "0, 1, 1", null)]
        [TestCase(X, Z, Y)]
        public void SetToRotateToTest(string vs, string vts, string axisString)
        {
            var v = Direction.Parse(vs, tolerance: 1);
            var vt = Direction.Parse(vts, tolerance: 1);
            Direction? axis = null;
            if (axisString != null)
            {
                axis = Direction.Parse(axisString);
            }

            var cs = CoordinateSystem.RotateTo(v, vt, axis);
            var rv = cs.Transform(v);
            AssertGeometry.AreEqual(vt, rv);

            var invert = cs.Invert();
            var rotateBack = invert.Transform(rv);
            AssertGeometry.AreEqual(v, rotateBack);

            cs = CoordinateSystem.RotateTo(vt, v, axis);
            rotateBack = cs.Transform(rv);
            AssertGeometry.AreEqual(v, rotateBack);
        }

        [TestCase("o:{1, 2, -7} x:{10, 0, 0} y:{0, 1, 0} z:{0, 0, 1}", "o:{0, 0, 0} x:{1, 0, 0} y:{0, 1, 0} z:{0, 0, 1}")]
        public void Transform(string cs1s, string cs2s)
        {
            var cs1 = CoordinateSystem.Parse(cs1s);
            var cs2 = CoordinateSystem.Parse(cs2s);
            var actual = cs1.Transform(cs2);
            var matrix = ToMatrix(cs1).Inverse().Multiply(ToMatrix(cs2));
            var x = Direction.Create(matrix[0, 0], matrix[1, 0], matrix[2, 0]);
            var y = Direction.Create(matrix[0, 1], matrix[1, 1], matrix[2, 1]);
            var origin = new Point3D(matrix[0, 3], matrix[1, 3], matrix[2, 3]);
            var expected = new CoordinateSystem(origin, x, y);
            AssertGeometry.AreEqual(expected, actual);
        }

        [Test]
        public void XmlRoundTrips()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1), new Vector3D(0, 0, 1));
            const string expected = "<CoordinateSystem><Origin><X>1</X><Y>-2</Y><Z>3</Z></Origin><XAxis><X>0</X><Y>1</Y><Z>0</Z></XAxis><YAxis><X>0</X><Y>0</Y><Z>1</Z></YAxis></CoordinateSystem>";
            AssertXml.XmlRoundTrips(cs, expected, (e, a) => AssertGeometry.AreEqual(e, a));
        }

        private static Matrix<double> ToMatrix(CoordinateSystem cs)
        {
            var m = new DenseMatrix(4)
            {
                [0, 0] = cs.OrientationMatrix.XAxis.X,
                [1, 0] = cs.OrientationMatrix.XAxis.Y,
                [2, 0] = cs.OrientationMatrix.XAxis.Z,

                [0, 1] = cs.OrientationMatrix.YAxis.X,
                [1, 1] = cs.OrientationMatrix.YAxis.Y,
                [2, 1] = cs.OrientationMatrix.YAxis.Z,

                [0, 2] = cs.OrientationMatrix.ZAxis.X,
                [1, 2] = cs.OrientationMatrix.ZAxis.Y,
                [2, 2] = cs.OrientationMatrix.ZAxis.Z,

                [0, 3] = cs.Origin.X,
                [1, 3] = cs.Origin.Y,
                [2, 3] = cs.Origin.Z,

                [3, 3] = 1.0
            };

            return m;
        }
    }
}
