namespace MathNet.Spatial.UnitTests
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;
    using NUnit.Framework;

    public class ParserTests
    {
        private const double Tolerance = 1e-6;
        private const double DegToRad = Math.PI / 180;

        [TestCase("1", 1)]
        [TestCase(".1", .1)]
        [TestCase("1.2", 1.2)]
        [TestCase("1.2E+3", 1.2E+3)]
        [TestCase("1.2e+3", 1.2E+3)]
        [TestCase("1.2E3", 1.2E3)]
        [TestCase("1.2e3", 1.2E3)]
        [TestCase("1.2E-3", 1.2E-3)]
        [TestCase("1.2e-3", 1.2E-3)]
        public void DoublePattern(string s, double expected)
        {
            Assert.IsTrue(Regex.IsMatch(s, Parser.DoublePattern));
            Assert.AreEqual(expected, double.Parse(s, CultureInfo.InvariantCulture));
        }

#if NET45 == true

        // All the swedish tests seem to have a problem on Travis
        [TestCase("5 °", 5 * DegToRad)]
        [TestCase("5°", 5 * DegToRad)]
        [TestCase("-5,34 rad", -5.34)]
        [TestCase("-5,34 rad", -5.34)]
        [TestCase("1e-4 rad", 0.0001)]
        [TestCase("1e-4 °", 0.0001 * DegToRad)]
        public void ParseSwedishAngle(string s, double expected)
        {
            var culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual(true, Angle.TryParse(s, culture, out var angle));
            Assert.AreEqual(expected, angle.Radians, Tolerance);

            angle = Angle.Parse(s, culture);
            Assert.AreEqual(expected, angle.Radians, Tolerance);
            Assert.IsInstanceOf<Angle>(angle);

            angle = Angle.Parse(s.ToString(culture), culture);
            Assert.AreEqual(expected, angle.Radians, Tolerance);
            Assert.IsInstanceOf<Angle>(angle);
        }

        [TestCase("1,2; 3,4; 5,6", 1.2, 3.4, 5.6)]
        [TestCase("1,2;3,4;5,6", 1.2, 3.4, 5.6)]
        [TestCase("1,2 3,4 5,6", 1.2, 3.4, 5.6)]
        [TestCase("(,1 2,3e-4 1)", 0.1, 0.00023000000000000001, 1)]
        public void ParseSwedishVector3D(string text, double expectedX, double expectedY, double expectedZ)
        {
            var culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual(true, Vector3D.TryParse(text, culture, out var p));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);

            p = Vector3D.Parse(text, culture);
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);

            p = Vector3D.Parse(p.ToString(culture));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);
        }

        [TestCase("1,2; 3,4", 1.2, 3.4)]
        [TestCase("1,2;3,4", 1.2, 3.4)]
        [TestCase("1,2 3,4", 1.2, 3.4)]
        [TestCase("(,1 2,3e-4)", 0.1, 0.00023000000000000001)]
        public void ParseSwedishVector2D(string text, double expectedX, double expectedY)
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

        [TestCase("1,2; 3,4; 5,6", 1.2, 3.4, 5.6)]
        [TestCase("1,2;3,4;5,6", 1.2, 3.4, 5.6)]
        [TestCase("1,2 3,4 5,6", 1.2, 3.4, 5.6)]
        [TestCase("(,1 2,3e-4 1)", 0.1, 0.00023000000000000001, 1)]
        public void ParseSwedishPoint3D(string text, double expectedX, double expectedY, double expectedZ)
        {
            var culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual(true, Point3D.TryParse(text, culture, out var p));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);

            p = Point3D.Parse(text, culture);
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);

            p = Point3D.Parse(p.ToString(culture));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);
        }

        [TestCase("1,2; 3,4", 1.2, 3.4)]
        [TestCase("1,2;3,4", 1.2, 3.4)]
        [TestCase("1,2 3,4", 1.2, 3.4)]
        [TestCase("1,2\u00A03,4", 1.2, 3.4)]
        [TestCase("(,1 2,3e-4)", 0.1, 0.00023000000000000001)]
        public void ParseSwedishPoint2D(string text, double expectedX, double expectedY)
        {
            var culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual(true, Point2D.TryParse(text, culture, out var p));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);

            p = Point2D.Parse(text, culture);
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);

            p = Point2D.Parse(p.ToString(culture));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
        }

        [TestCase("1,0; 0; 0,0", 1, 0, 0)]
        [TestCase("0; 1,0; 0,0", 0, 1, 0)]
        [TestCase("0; 0,0; 1,0", 0, 0, 1)]
        public void ParseSwedishUnitVector(string text, double expectedX, double expectedY, double expectedZ)
        {
            var culture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual(true, UnitVector3D.TryParse(text, culture, out var p));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);

            p = UnitVector3D.Parse(text, culture);
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);

            p = UnitVector3D.Parse(p.ToString(culture));
            Assert.AreEqual(expectedX, p.X);
            Assert.AreEqual(expectedY, p.Y);
            Assert.AreEqual(expectedZ, p.Z);
        }

#endif

    }
}
