using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Euclidean
{
    [TestFixture]
    public class RotationMatrixTests
    {
        [Test]
        public void Identity()
        {
            AssertGeometry.AreEqual(new Point3D(1, -2, 3), RotationMatrix.Identity * new Point3D(1, -2, 3));
            AssertGeometry.AreEqual(new Vector3D(1, -2, 3), RotationMatrix.Identity * new Vector3D(1, -2, 3));
            AssertGeometry.AreEqual(Direction.Create(1, -2, 3), RotationMatrix.Identity * Direction.Create(1, -2, 3));
        }

        [TestCase("10, -3, 7", 34.6, "10, -6.4443153209750346, 4.0584233445817883")]
        [TestCase("5, 1, 2", 0, "5, 1, 2")]
        [TestCase("5, 1, 2", 90, "5, -2, 1")]
        [TestCase("5, 1, 2", 180, "5, -1, -2")]
        public void AroundX(string cartesian, double degrees, string expectedString)
        {
            var angle = Angle.FromDegrees(degrees);
            var rotation = RotationMatrix.AroundX(angle);
            var inverse = rotation.Inverse();

            var point = Point3D.Parse(cartesian);
            var expectedPoint = Point3D.Parse(expectedString);
            var actualPoint = rotation * point;
            AssertGeometry.AreEqual(expectedPoint, actualPoint);
            AssertGeometry.AreEqual(point, inverse * actualPoint);

            var vector = Vector3D.Parse(cartesian);
            var expectedVector = Vector3D.Parse(expectedString);
            var actualVector = rotation * vector;
            AssertGeometry.AreEqual(expectedVector, actualVector);
            AssertGeometry.AreEqual(vector, inverse * actualVector);

            var expectedDirection = expectedVector.Normalize();
            var actualDirection = actualVector.Normalize();
            AssertGeometry.AreEqual(expectedDirection, actualDirection);
            AssertGeometry.AreEqual(vector.Normalize(), inverse * actualDirection);
        }

        [TestCase("10, -3, 7", 34.6, "12.206269900716126, -3, 0.083517129210079588")]
        [TestCase("5, 1, 2", 0, "5, 1, 2")]
        [TestCase("5, 1, 2", 90, "2, 1, -5")]
        [TestCase("5, 1, 2", 180, "-5, 1, -2")]
        public void AroundY(string cartesian, double degrees, string expectedString)
        {
            var angle = Angle.FromDegrees(degrees);
            var rotation = RotationMatrix.AroundY(angle);
            var inverse = rotation.Inverse();

            var point = Point3D.Parse(cartesian);
            var expectedPoint = Point3D.Parse(expectedString);
            var actualPoint = rotation * point;
            AssertGeometry.AreEqual(expectedPoint, actualPoint);
            AssertGeometry.AreEqual(point, inverse * actualPoint);

            var vector = Vector3D.Parse(cartesian);
            var expectedVector = Vector3D.Parse(expectedString);
            var actualVector = rotation * vector;
            AssertGeometry.AreEqual(expectedVector, actualVector);
            AssertGeometry.AreEqual(vector, inverse * actualVector);

            var expectedDirection = expectedVector.Normalize();
            var actualDirection = actualVector.Normalize();
            AssertGeometry.AreEqual(expectedDirection, actualDirection);
            AssertGeometry.AreEqual(vector.Normalize(), inverse * actualDirection);
        }

        [TestCase("10, -3, 7", 34.6, "9.9348949205037211, 3.2090283449276877, 7")]
        [TestCase("5, 1, 2", 0, "5, 1, 2")]
        [TestCase("5, 1, 2", 90, "-1, 5, 2")]
        [TestCase("5, 1, 2", 180, "-5, -1, 2")]
        [TestCase("1, 0, 0", 30, "0.86602540378443864676372317075294, 0.5, 0")]
        public void AroundZ(string cartesian, double degrees, string expectedString)
        {
            var angle = Angle.FromDegrees(degrees);
            var rotation = RotationMatrix.AroundZ(angle);
            var inverse = rotation.Inverse();

            var point = Point3D.Parse(cartesian);
            var expectedPoint = Point3D.Parse(expectedString);
            var actualPoint = rotation * point;
            AssertGeometry.AreEqual(expectedPoint, actualPoint);
            AssertGeometry.AreEqual(point, inverse * actualPoint);

            var vector = Vector3D.Parse(cartesian);
            var expectedVector = Vector3D.Parse(expectedString);
            var actualVector = rotation * vector;
            AssertGeometry.AreEqual(expectedVector, actualVector);
            AssertGeometry.AreEqual(vector, inverse * actualVector);

            var expectedDirection = expectedVector.Normalize();
            var actualDirection = actualVector.Normalize();
            AssertGeometry.AreEqual(expectedDirection, actualDirection);
            AssertGeometry.AreEqual(vector.Normalize(), inverse * actualDirection);
        }

        [TestCase("10, -3, 7", "0, 1, 2", 0, "10, -3, 7")]
        [TestCase("10, -3, 7", "0, 1, 2", 90, "5.8137767414994546, 11.14427190999916, -0.072135954999579255")]
        [TestCase("10, -3, 7", "0, 1, 2", 180, "-10, 7.4, 1.8")]
        [TestCase("10, -3, 7", "-2, 5, -1", 40, "12.070925574564843, -3.4663965295105625, 0.5261662033175023")]
        public void AroundAxis(string cartesian, string axis, double degrees, string expectedString)
        {
            var angle = Angle.FromDegrees(degrees);
            var rotationAxis = Vector3D.Parse(axis).Normalize();
            var rotation = RotationMatrix.AroundAxis(rotationAxis, angle);
            var inverse = rotation.Inverse();

            var point = Point3D.Parse(cartesian);
            var expectedPoint = Point3D.Parse(expectedString);
            var actualPoint = rotation * point;
            AssertGeometry.AreEqual(expectedPoint, actualPoint);
            AssertGeometry.AreEqual(point, inverse * actualPoint);

            var vector = Vector3D.Parse(cartesian);
            var expectedVector = Vector3D.Parse(expectedString);
            var actualVector = rotation * vector;
            AssertGeometry.AreEqual(expectedVector, actualVector);
            AssertGeometry.AreEqual(vector, inverse * actualVector);

            var expectedDirection = expectedVector.Normalize();
            var actualDirection = actualVector.Normalize();
            AssertGeometry.AreEqual(expectedDirection, actualDirection);
            AssertGeometry.AreEqual(vector.Normalize(), inverse * actualDirection);
        }

        [TestCase("0, 1, 2", "-2, 5, 7", "10, 0, 0", "9.738637232617128, 1.1695570275133884, 1.9470699039524673", null)]
        [TestCase("0, 1, 2", "4, 1, 6", "-8, 1, 5", "-4.0550340058827921, 0.79721118232671273, 8.5393883587706334", "10, -3, 7")]
        public void RotationTo(string fromVector, string toVector, string pS, string exp, string axisString)
        {
            var from = Vector3D.Parse(fromVector);
            var to = Vector3D.Parse(toVector);
            var matrix = axisString == null
                ? RotationMatrix.RotationTo(from, to)
                : RotationMatrix.RotationTo(from, to, Vector3D.Parse(axisString).Normalize());
            var point = Point3D.Parse(pS);
            var actual = matrix * point;
            var expected = Point3D.Parse(exp);
            AssertGeometry.AreEqual(expected, actual);
        }

        [TestCase("0, 1, 2", 0, "0, 1, 2", 0, true)]
        [TestCase("0, 1, 2", 0, "0, 1, 2", 10, false)]
        [TestCase("0, 1, 2", 90, "0, 1, 2", 90, true)]
        [TestCase("0, 1, 2", 90, "0, 2, 2", 90, false)]
        [TestCase("0, 1, 2", 180, "0, 1, 2", 180, true)]
        [TestCase("-2, 5, -1", 40, "-2, 5, -1", 40, true)]
        public void Equality(string axis1, double degrees1, string axis2, double degrees2, bool expected)
        {
            var angle1 = Angle.FromDegrees(degrees1);
            var rotationAxis1 = Vector3D.Parse(axis1).Normalize();
            var matrix1 = RotationMatrix.AroundAxis(rotationAxis1, angle1);

            var angle2 = Angle.FromDegrees(degrees2);
            var rotationAxis2 = Vector3D.Parse(axis2).Normalize();
            var matrix2 = RotationMatrix.AroundAxis(rotationAxis2, angle2);

            Assert.AreEqual(expected, matrix1.Equals(matrix2));
            Assert.AreEqual(expected, matrix1 == matrix2);
            Assert.AreEqual(!expected, matrix1 != matrix2);
        }
    }
}
