using System;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class QuaternionTests
    {
        [TestCase(1.0, 1.0, 1.0, 1.0, new[] { Numerics.Constants.PiOver2, 0.0, Numerics.Constants.PiOver2 })]
        [TestCase(0.0, 1.0, 0.0, 0.0, new[] { Numerics.Constants.Pi, 0.0, 0.0 })]
        [TestCase(0.0, 1.0, 0.5, 0.0, new[] { Numerics.Constants.Pi, 0.0, 0.92729522 })]
        [TestCase(0.0, 0.0, 0.0, 0.0, new[] { 0.0, 0.0, 0.0 })]
        [TestCase(0.0, 1.0, 0.5, 0.5, new[] { 2.67794504, -Numerics.Constants.PiOver2, 1.10714872 })]
        public void ToEulerAnglesTest(double real, double x, double y, double z, double[] expectedAsArray)
        {
            var quat = new Quaternion(real, x, y, z);
            var eulerAngles = quat.ToEulerAngles();
            var expected = new EulerAngles(Angle.FromRadians(expectedAsArray[0]), Angle.FromRadians(expectedAsArray[1]), Angle.FromRadians(expectedAsArray[2]));
            Assert.AreEqual(expected,eulerAngles);
        }
    }
}
