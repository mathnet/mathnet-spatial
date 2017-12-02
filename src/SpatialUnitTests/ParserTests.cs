namespace MathNet.Spatial.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class ParserTests
    {
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
    }
}
