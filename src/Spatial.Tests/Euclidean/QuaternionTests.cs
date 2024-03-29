﻿using System;
using System.Collections;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Euclidean
{
    /// <summary>
    /// Complex32 tests.
    /// </summary>
    /// <remarks>
    /// Based on http://web.cs.iastate.edu/~cs577/handouts/quaternion.pdf and my own calculations
    /// </remarks>
    [TestFixture]
    public class QuaternionTests
    {
        private const double TestTolerance = 0.000001;

        [Test]
        public void CanConstructQuaternion()
        {
            // From algebraic notation :
            var a1 = new Quaternion(1, 2, 3, 4);
            Assert.AreEqual(1, a1.Real);
            Assert.AreEqual(2, a1.ImagX);
            Assert.AreEqual(3, a1.ImagY);
            Assert.AreEqual(4, a1.ImagZ);

            // From DenseVector :
            var v1 = new Quaternion(new DenseVector(new double[] { 1, 2, 3, 4 }));
            Assert.AreEqual(1, v1.Real);
            Assert.AreEqual(2, v1.ImagX);
            Assert.AreEqual(3, v1.ImagY);
            Assert.AreEqual(4, v1.ImagZ);
        }

        [Test]
        public void CanGetProporties()
        {
            var q = new Quaternion(1, 2, 3, 4);
            Assert.AreEqual(1, q.Real);
            Assert.AreEqual(2, q.ImagX);
            Assert.AreEqual(3, q.ImagY);
            Assert.AreEqual(4, q.ImagZ);
            var norm = Math.Sqrt(1 + (2 * 2) + (3 * 3) + (4 * 4));
            Assert.AreEqual(norm, q.Norm);
            Assert.AreEqual(norm * norm, q.NormSquared);
            Assert.AreEqual(new Quaternion(0, 2, 3, 4), q.Vector);
            Assert.AreEqual(new Quaternion(1, 0, 0, 0), q.Scalar);
            var norm2 = Math.Sqrt((2 * 2) + (3 * 3) + (4 * 4));
            Assert.AreEqual(new Quaternion(1 / norm, 2 / norm, 3 / norm, 4 / norm), q.Normalized);
            Assert.AreEqual(new Quaternion(0, 2 / norm2, 3 / norm2, 4 / norm2), q.NormalizedVector);
            var arg = Math.Acos(q.Real / q.Norm);
            Assert.AreEqual(arg, q.Arg);
            Assert.IsTrue(new Quaternion(0.5, 0.5, 0.5, 0.5).IsUnitQuaternion);
        }

        /// <summary>
        /// Can add a quaternions using operator.
        /// </summary>
        [Test]
        public void CanAddQuaternionsAndFloatUsingOperator()
        {
            Assert.AreEqual(new Quaternion(2, 4, 6, 8), new Quaternion(1, 2, 3, 4) + new Quaternion(1, 2, 3, 4));
            Assert.AreEqual(new Quaternion(1, 2, 3, 4), new Quaternion(0, 0, 0, 0) + new Quaternion(1, 2, 3, 4));
            Assert.AreEqual(new Quaternion(2, 2, 3, 4), 1 + new Quaternion(1, 2, 3, 4));
            Assert.AreEqual(new Quaternion(2, 2, 3, 4), new Quaternion(1, 2, 3, 4) + 1);
            Assert.AreEqual(Quaternion.Zero, new Quaternion(0, 0, 0, 0) + Quaternion.Zero);
            Assert.AreEqual(Quaternion.One, new Quaternion(0, 0, 0, 0) + Quaternion.One);
        }

        /// <summary>
        /// Can substract quaternions using operator.
        /// </summary>
        [Test]
        public void CanSubtractQuaternionsAndFloatUsingOperator()
        {
            Assert.AreEqual(Quaternion.Zero, new Quaternion(1, 2, 3, 4) - new Quaternion(1, 2, 3, 4));
            Assert.AreEqual(new Quaternion(-1, -2, -3, -4), new Quaternion(0, 0, 0, 0) - new Quaternion(1, 2, 3, 4));
            Assert.AreEqual(Quaternion.Zero, new Quaternion(0, 0, 0, 0) - Quaternion.Zero);
            Assert.AreEqual(-Quaternion.One, new Quaternion(0, 0, 0, 0) - Quaternion.One);
            Assert.AreEqual(new Quaternion(0, 2, 3, 4), 1 - new Quaternion(1, 2, 3, 4));
            Assert.AreEqual(new Quaternion(0, 2, 3, 4), new Quaternion(1, 2, 3, 4) - 1);
        }

        [Test]
        public void CanMultiplyQuaternionsAndFloatUsingOperator()
        {
            Assert.AreEqual(new Quaternion(8, -9, -2, 11), new Quaternion(3, 1, -2, 1) * new Quaternion(2, -1, 2, 3));
            Assert.AreEqual(new Quaternion(3, 1, -2, 1), new Quaternion(3, 1, -2, 1) * 1);
            Assert.AreEqual(new Quaternion(6, 2, -4, 2), 2 * new Quaternion(3, 1, -2, 1));
            Assert.AreEqual(new Quaternion(3, 1, -2, 1) * 2, 2 * new Quaternion(3, 1, -2, 1));
            Assert.AreEqual(new Quaternion(1.5d, 0.5d, -1, 0.5d), new Quaternion(3, 1, -2, 1) * 0.5d);
        }

        [Test]
        public void CanDivideQuaternionsAndFloatUsingOperator()
        {
            Assert.AreEqual(new Quaternion(1, 1, 1, 1), new Quaternion(2, 2, 2, 2) / 2);
            Assert.AreEqual(new Quaternion(0, 0, 0, 0), new Quaternion(0, 0, 0, 0) / 2);
        }

        [Test]
        public void CanNegate()
        {
            Assert.AreEqual(new Quaternion(-1, -2, -3, -4), -new Quaternion(1, 2, 3, 4));
            Assert.AreEqual(new Quaternion(0, 0, 0, 0), -new Quaternion(0, 0, 0, 0));
            Assert.AreEqual(Quaternion.Zero, new Quaternion(5, 6, 7, 8) + (-new Quaternion(5, 6, 7, 8)));
        }

        [Test]
        public void CanEqalAndNotEqualQuaternionsAndFloatsUsingOperator()
        {
            Assert.IsTrue(new Quaternion(1, 0, 0, 0) == 1);
            Assert.IsTrue(new Quaternion(2, 0, 0, 0) == 2);
            Assert.IsTrue(1 == new Quaternion(1, 0, 0, 0));
            Assert.IsTrue(2 == new Quaternion(2, 0, 0, 0));
            Assert.IsTrue(new Quaternion(1, 0, 0, 1) != 1);
            Assert.IsTrue(new Quaternion(1, 0, 0, 1) != 99);
            Assert.IsTrue(new Quaternion(1, 0, 0, 1) != 99);
            Assert.IsTrue(99 != new Quaternion(1, 0, 0, 1));
            Assert.IsTrue(99 != new Quaternion(1, 0, 0, 0));
            Assert.IsTrue(1 != new Quaternion(1, 0, 0, 1));
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(new Quaternion(1, 0, 0, 1) == new Quaternion(1, 0, 0, 1));
            Assert.IsTrue(new Quaternion(1, 1, 1, 1) == new Quaternion(2, 2, 2, 2) / 2);
            Assert.IsTrue(new Quaternion(1, 1, 1, 1) != new Quaternion(0, 2, 2, 2) / 2);
        }

        [TestCase(1.0, 1.0, 1.0, 1.0, new[] { Constants.PiOver2, 0.0, Constants.PiOver2 })]
        [TestCase(0.0, 1.0, 0.0, 0.0, new[] { Constants.Pi, 0.0, 0.0 })]
        [TestCase(0.0, 1.0, 0.5, 0.0, new[] { Constants.Pi, 0.0, 0.92729522 })]
        [TestCase(0.0, 0.0, 0.0, 0.0, new[] { 0.0, 0.0, 0.0 })]
        [TestCase(0.0, 1.0, 0.5, 0.5, new[] { 2.67794504, -Constants.PiOver2, 1.10714872 })]
        public void ToEulerAnglesTest(double real, double x, double y, double z, double[] expectedAsArray)
        {
            var quat = new Quaternion(real, x, y, z);
            var eulerAngles = quat.ToEulerAngles();
            var expected = new EulerAngles(Angle.FromRadians(expectedAsArray[0]), Angle.FromRadians(expectedAsArray[1]), Angle.FromRadians(expectedAsArray[2]));
            Assert.IsTrue(eulerAngles.Equals(expected, TestTolerance));
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "LogTests")]
        public void CanCalculateLog(Quaternion quat, Quaternion expected)
        {
            var log = quat.Log();
            Assert.IsTrue(log.Equals(expected, TestTolerance));
        }

        [TestCaseSource(typeof(QuaternionCalculationTestClass), "NormTests")]
        public double CanCalculateNorm(double real, double x, double y, double z)
        {
            return new Quaternion(real, x, y, z).Norm;
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "DivisionTests")]
        public void CanDivideQuaternionsUsingOperator(Quaternion q1, Quaternion q2, Quaternion expected)
        {
            var result = q1 / q2;
            Assert.IsTrue(result.Equals(expected, TestTolerance));
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "CanNormalizeTests")]
        public Quaternion CanNormalize(double a, double b, double c, double d)
        {
            return new Quaternion(a, b, c, d).Normalized;
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "CanInverseTests")]
        public Quaternion CanInverse(double a, double b, double c, double d)
        {
            return new Quaternion(a, b, c, d).Inversed;
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "CanCalculateDistance")]
        public double CaculateDistance(Quaternion q1, Quaternion q2)
        {
            return Quaternion.Distance(q1, q2);
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "CanCalculatePower")]
        public void CanCalculatePower(Quaternion q1, double k, Quaternion expected)
        {
            var result = q1.Pow(k);
            Assert.IsTrue(result.Equals(expected, TestTolerance));
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "CanCalculatePowerInt")]
        public Quaternion CanCalculatePowerInt(Quaternion q1, int k)
        {
            return q1.Pow(k);
        }

        [Test]
        [TestCaseSource(typeof(QuaternionCalculationTestClass), "CanCalculatePower")]
        public void CanCalculatePowerUsingOperator(Quaternion q1, double k, Quaternion expected)
        {
            var result = q1 ^ k;
            Assert.IsTrue(result.Equals(expected, TestTolerance));
        }

        [Test]
        public void CanEqualsEqualityMethod()
        {
            Assert.IsTrue(Quaternion.Zero.Equals(Quaternion.Zero));
            Assert.IsTrue(Quaternion.One.Equals(Quaternion.One));
            Assert.IsFalse(Quaternion.One.Equals(Quaternion.Zero));
            Assert.IsTrue(new Quaternion(1, 1, 1, 1).Equals(new Quaternion(1, 1, 1, 1)));
            Assert.IsTrue(new Quaternion(0, 1, 1, 1).Equals(new Quaternion(0, 1, 1, 1)));
            Assert.IsTrue(new Quaternion(1, 0, 1, 1).Equals(new Quaternion(1, 0, 1, 1)));
            Assert.IsTrue(new Quaternion(1, 0, 0, 1).Equals(new Quaternion(1, 0, 0, 1)));
            Assert.IsTrue(new Quaternion(1, 0, 0, 0).Equals(new Quaternion(1, 0, 0, 0)));
            Assert.IsFalse(new Quaternion(1, 1, 1, 1).Equals(null));
            object q = new Quaternion(0, 0, 0, 0);
            Assert.IsTrue(Quaternion.Zero.Equals(q));
        }

        [TestCase(1, 1, 1, 1, "1+1i+1j+1k")]
        [TestCase(1, -1, -1, -1, "1-1i-1j-1k")]
        [TestCase(-2, -3, -4, -5, "-2-3i-4j-5k")]
        public void CanToString(double a, double b, double c, double d, string propperoutput)
        {
            Assert.AreEqual(propperoutput, new Quaternion(a, b, c, d).ToString());
        }

        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 3, 4)]
        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 0, 0, 0)]
        [TestCase(0, 1, 0, 0)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(0, 0, 0, 1)]
        public void CanRotateQuaternion(double a, double b, double c, double d)
        {
            var quat = new Quaternion(a, b, c, d);
            var quat180 = new Quaternion(0, 0, 1, 0);
            var nonRotation = new Quaternion(0, 0, 0, 0);
            var oneRotation = Quaternion.One;

            Assert.AreEqual(quat180 * quat, quat.RotateRotationQuaternion(quat180));
            Assert.AreEqual(oneRotation * quat, quat.RotateRotationQuaternion(oneRotation));
            if (quat.IsUnitQuaternion)
            {
                Assert.AreEqual(quat * quat180 * quat.Conjugate(), quat.RotateUnitQuaternion(quat180));
                Assert.AreEqual(quat * oneRotation * quat.Conjugate(), quat.RotateUnitQuaternion(oneRotation));
                Assert.Throws<ArgumentException>(delegate { quat.RotateUnitQuaternion(nonRotation); });
                Assert.Throws<ArgumentException>(delegate { quat.RotateRotationQuaternion(nonRotation); });
            }
            else
            {
                Assert.Throws<InvalidOperationException>(delegate { quat.RotateUnitQuaternion(quat180); });
                Assert.Throws<InvalidOperationException>(delegate { quat.RotateUnitQuaternion(oneRotation); });
                Assert.Throws<InvalidOperationException>(delegate { quat.RotateUnitQuaternion(nonRotation); });
            }
        }

        public class QuaternionCalculationTestClass
        {
            private static readonly double smallNumber = float.Epsilon;
            private static readonly Random random = new Random();
            private static readonly Quaternion posInf = new Quaternion(double.PositiveInfinity, 0, 0, 0);
            private static readonly Quaternion nanQuaternion = new Quaternion(double.NaN, 0, 0, 0);

            public static IEnumerable NormTests
            {
                get
                {
                    // yield return new TestCaseData(bigNumber, bigNumber, bigNumber, bigNumber).Returns(2*bigNumber); // This test is supposted to work after making Norm more robust
                    yield return new TestCaseData(0, 0, 0, 0).Returns(0.0);
                    yield return new TestCaseData(1, 1, 1, 1).Returns(2.0);
                    yield return new TestCaseData(1, 1, 1, 0).Returns(Math.Sqrt(3));
                    yield return new TestCaseData(1, 2, 3, 4).Returns(Math.Sqrt(30));
                    yield return new TestCaseData(5, 6, 7, 8).Returns(Math.Sqrt(174));
                    yield return new TestCaseData(smallNumber, smallNumber, smallNumber, smallNumber).Returns(2 * smallNumber);
                }
            }

            public static IEnumerable DivisionTests
            {
                get
                {
                    var qexpected = new Quaternion(1, 0, 0, 0);
                    for (var i = 0; i < 10; ++i)
                    {
                        var a = new[] { random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble() };
                        var test1 = new Quaternion(a[0], a[1], a[2], a[3]);
                        var test2 = new Quaternion(a[0], a[1], a[2], a[3]);
                        yield return new TestCaseData(test1, test2, qexpected);
                    }

                    yield return new TestCaseData(new Quaternion(0, 0, 0, 0), new Quaternion(1, 1, 1, 1), new Quaternion(0, 0, 0, 0));
                    yield return new TestCaseData(new Quaternion(4, 4, 4, 4), new Quaternion(2, 2, 2, 2), new Quaternion(2, 0, 0, 0));
                    yield return new TestCaseData(new Quaternion(9, 9, 9, 9), new Quaternion(3, 3, 3, 3), new Quaternion(3, 0, 0, 0));
                    yield return new TestCaseData(new Quaternion(1, 0, 0, 0), new Quaternion(1, 2, 3, 4), new Quaternion(1d / 30, -1d / 15, -1d / 10, -2d / 15));
                    yield return new TestCaseData(new Quaternion(4, 6, 9, 18), new Quaternion(2, 3, 3, 9), new Quaternion(215d / 103, 27d / 103, 6d / 103, -9d / 103));
                    yield return new TestCaseData(new Quaternion(1, 2, 3, 4), new Quaternion(5, 6, 7, 8), new Quaternion(70d / 174, 0d, 16d / 174, 8d / 174));
                    yield return new TestCaseData(new Quaternion(1, 1, 1, 1), new Quaternion(0, 0, 0, 0), posInf);
                    yield return new TestCaseData(new Quaternion(0, 0, 0, 0), new Quaternion(0, 0, 0, 0), nanQuaternion);
                    yield return new TestCaseData(new Quaternion(-1, -1, -1, -1), new Quaternion(0, 0, 0, 0), posInf);
                }
            }

            public static IEnumerable CanNormalizeTests
            {
                get
                {
                    yield return new TestCaseData(1, 1, 1, 1).Returns(new Quaternion(1d / 2, 1d / 2, 1d / 2, 1d / 2));
                    var norm = Math.Sqrt(30);
                    yield return new TestCaseData(1, 2, 3, 4).Returns(new Quaternion(1d / norm, 2d / norm, 3d / norm, 4d / norm));
                    yield return new TestCaseData(0, 0, 0, 0).Returns(Quaternion.Zero);
                }
            }

            public static IEnumerable CanInverseTests
            {
                get
                {
                    yield return new TestCaseData(0, 0, 0, 0).Returns(Quaternion.Zero);
                    yield return new TestCaseData(1, 2, 3, 4).Returns(new Quaternion(1d / 30, -2d / 30, -3d / 30, -4d / 30));
                    yield return new TestCaseData(1, 1, 0, 0).Returns(new Quaternion(0.5, -0.5, 0, 0));
                }
            }

            public static IEnumerable CanCalculateDistance
            {
                get
                {
                    var q1 = new Quaternion(0, 0, 0, 1);
                    var q2 = new Quaternion(1, 1, 1, 0);
                    var q3 = Quaternion.Zero;
                    var q4 = new Quaternion(9, 9, 9, 1);
                    var q5 = new Quaternion(2, 3, 4, 5);
                    yield return new TestCaseData(q1, q2).Returns(2);
                    yield return new TestCaseData(q2, q1).Returns(2);
                    yield return new TestCaseData(q3, q1).Returns(q1.Norm);
                    yield return new TestCaseData(q3, q2).Returns(q2.Norm);
                    yield return new TestCaseData(q4, q2).Returns(Math.Sqrt((8 * 8) + (8 * 8) + (8 * 8) + 1));
                    yield return new TestCaseData(q5, q2).Returns(Math.Sqrt(1 + (2 * 2) + (3 * 3) + (5 * 5)));
                }
            }

            public static IEnumerable CanCalculatePower
            {
                get
                {
                    var q0 = new Quaternion(0, 0, 0, 0);
                    var q00 = new Quaternion(0, 0, 0, 0);
                    yield return new TestCaseData(q0, 3.981, q00);

                    var q1 = new Quaternion(1, 0, 0, 0);
                    var q11 = new Quaternion(1, 0, 0, 0);
                    yield return new TestCaseData(q1, 3.981, q11);

                    var q2 = new Quaternion(1, 1, 1, 1).Normalized;
                    var q29 = q2 * q2 * q2 * q2 * q2 * q2 * q2 * q2 * q2;
                    var q23 = q2 * q2 * q2;
                    yield return new TestCaseData(q2, 9.0, q29);
                    yield return new TestCaseData(q2, 3.0, q23);

                    var q3 = new Quaternion(2, 2, 2, 2).Normalized;
                    var q39 = q3 * q3 * q3 * q3 * q3 * q3 * q3 * q3 * q3;
                    yield return new TestCaseData(q3, 9.0, q39);
                }
            }

            public static IEnumerable CanCalculatePowerInt
            {
                get
                {
                    var q0 = new Quaternion(0, 0, 0, 0);
                    var q1 = new Quaternion(1, 0, 0, 0);
                    var q2 = new Quaternion(1, 1, 1, 1);
                    var q3 = new Quaternion(2, 2, 2, 2);
                    var q4 = new Quaternion(1, 2, 3, 4);
                    var q5 = new Quaternion(3, 2, 1, 0);

                    yield return new TestCaseData(q0, 9).Returns(q0);
                    yield return new TestCaseData(q1, 9).Returns(q1);

                    yield return new TestCaseData(q2, 9).Returns(q2 * q2 * q2 * q2 * q2 * q2 * q2 * q2 * q2);
                    yield return new TestCaseData(q2, 3).Returns(q2 * q2 * q2);
                    yield return new TestCaseData(q3, 9).Returns(q3 * q3 * q3 * q3 * q3 * q3 * q3 * q3 * q3);
                    yield return new TestCaseData(q4, 9).Returns(q4 * q4 * q4 * q4 * q4 * q4 * q4 * q4 * q4);
                    yield return new TestCaseData(q5, 9).Returns(q5 * q5 * q5 * q5 * q5 * q5 * q5 * q5 * q5);
                }
            }

            public static object[] LogTests
            {
                get
                {
                    var q0 = new Quaternion(1, 0, 0, 0);
                    var q00 = new Quaternion(1, 0, 0, 0);
                    var q1 = new Quaternion(0, 0, 0, 1);
                    var q10 = new Quaternion(0, 0, 0, Math.PI / 2);
                    var q2 = new Quaternion(1, 1, 1, 1);
                    var q20 = new Quaternion(
                        Math.Log(2),
                        1 / Math.Sqrt(3) * Math.PI / 3,
                        1 / Math.Sqrt(3) * Math.PI / 3,
                        1 / Math.Sqrt(3) * Math.PI / 3);

                    var tests = new object[]
                    {
                        new object[] { q0, q00 },
                        new object[] { q1, q10 },
                        new object[] { q2, q20 }
                    };
                    return tests;
                }
            }
        }
    }
}
