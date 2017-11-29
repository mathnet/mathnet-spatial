using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using ExtendedXmlSerializer.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
 
    public partial class UnitsXmlTests
    {
        private const double Tolerance = 1e-6;

        /*
        [TestCase("15 °", @"<Angle Value=""0.26179938779914941"" />")]
        public void AngleXml(string vs, string xml)
        {
            var angle = Angle.Parse(vs);
            var result = AssertXml.XmlSerializerRoundTrip(angle, xml);
            Assert.AreEqual(angle.Radians, result.Radians, Tolerance);
        }
        */

        [TestCase("15 °", @"<Angle><Value>0.26179938779914941</Value></Angle>")]
        public void AngleDataContract(string vs, string xml)
        {
            var angle = Angle.Parse(vs);
            var result = AssertXml.DataContractRoundTrip(angle, xml);
            Assert.AreEqual(angle.Radians, result.Radians, Tolerance);
        }
    }
}
