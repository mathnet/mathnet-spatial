using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class AssertXmlTests2
    {
        /*
        [Test]
        public void XmlSerializerRoundTripTest()
        {
            var dummy = new XmlSerializableDummy("Meh", 14);
            var roundTrip = AssertXml.XmlSerializerRoundTrip(dummy, @"<XmlSerializableDummy><Name>Meh</Name><Age>14</Age></XmlSerializableDummy>");
            Assert.AreEqual(dummy.Name, roundTrip.Name);
            Assert.AreEqual(dummy.Age, roundTrip.Age);
        }
        */

        [Test]
        public void DataContractRoundTripTest()
        {
            var dummy = new XmlSerializableDummy("Meh", 14);
            var roundTrip = AssertXml.DataContractRoundTrip(dummy, @"<XmlSerializableDummy><Age>14</Age><Name>Meh</Name></XmlSerializableDummy>");
            Assert.AreEqual(dummy.Name, roundTrip.Name);
            Assert.AreEqual(dummy.Age, roundTrip.Age);
        }

    }
}
