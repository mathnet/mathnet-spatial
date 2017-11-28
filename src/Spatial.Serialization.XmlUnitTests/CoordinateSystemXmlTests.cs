using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class CoordinateSystemXmlTests
    {
        [Test]
        public void CoordinateSystemXml()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            const string xml = @"
<CoordinateSystem>
    <Origin X=""1"" Y=""-2"" Z=""3"" />
    <XAxis X=""0"" Y=""1"" Z=""0"" />
    <YAxis X=""0"" Y=""0"" Z=""1"" />
    <ZAxis X=""1"" Y=""0"" Z=""0"" />
</CoordinateSystem>";
            var result = AssertXml.XmlSerializerRoundTrip(cs, xml);
            AssertGeometry.AreEqual(cs, result);
        }

        [Test]
        public void CoordinateSystemDataContract()
        {
            var cs = new CoordinateSystem(new Point3D(1, -2, 3), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0));
            const string xml = @"
<CoordinateSystem>
    <Origin X=""1"" Y=""-2"" Z=""3"" />
    <XAxis X=""0"" Y=""1"" Z=""0"" />
    <YAxis X=""0"" Y=""0"" Z=""1"" />
    <ZAxis X=""1"" Y=""0"" Z=""0"" />
</CoordinateSystem>";
            var result = AssertXml.DataContractRoundTrip(cs, xml);
            AssertGeometry.AreEqual(cs, result);
        }
    }
}
