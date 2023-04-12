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

        //[TestCase("", "", "")]
        //public void AroundX(string pointString, string rotationAngle, string expectedString)
        //{
        //    var point = Point3D.Parse(pointString);
        //    var angle = Angle.Parse(rotationAngle);
        //    var expected = Point3D.Parse(expectedString);
        //    var actual = RotationMatrix.AroundX(angle);
        //}
    }
}
