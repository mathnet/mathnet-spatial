using System;
using NUnit.Framework;
using MathNet.Spatial.Euclidean;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class PlaneXmlTests
    {
        /*
        [TestCase("p:{0, 0, 0} v:{0, 0, 1}", @"<Plane><RootPoint X=""0"" Y=""0"" Z=""0"" /><Normal X=""0"" Y=""0"" Z=""1"" /></Plane>")]
        public void PlaneXml(string p1s, string xml)
        {
            var plane = Plane.Parse(p1s);
            var result = AssertXml.XmlSerializerRoundTrip(plane, xml);
            Assert.AreEqual(plane, result);
        }
        */

        [TestCase("p:{0, 0, 0} v:{0, 0, 1}", @"<Plane><Normal><X>0</X><Y>0</Y><Z>1</Z></Normal><RootPoint><X>0</X><Y>0</Y><Z>0</Z></RootPoint></Plane>")]
        public void PlaneDataContract(string p1s, string xml)
        {
            var plane = Plane.Parse(p1s);
            var result = AssertXml.DataContractRoundTrip(plane, xml);
            Assert.AreEqual(plane, result);
        }
    }
}
