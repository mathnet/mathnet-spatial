using NUnit.Framework;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class AssertXmlTests2
    {
        [Test]
        public void XmlSerializerRoundTripTest()
        {
            var dummy = new XmlSerializableDummy("Meh", 14);
            var roundTrip = AssertXml.XmlSerializerRoundTrip(dummy, @"<?xml version=""1.0"" encoding=""utf-8""?><XmlSerializableDummy Age=""14""><Name>Meh</Name></XmlSerializableDummy>");
            Assert.AreEqual(dummy.Name, roundTrip.Name);
            Assert.AreEqual(dummy.Age, roundTrip.Age);
        }
        [Test]
        public void DataContractRoundTripTest()
        {
            var dummy = new XmlSerializableDummy("Meh", 14);
            var roundTrip = AssertXml.DataContractRoundTrip(dummy, @"<?xml version=""1.0"" encoding=""utf-8""?><XmlSerializableDummy Age=""14""><Name>Meh</Name></XmlSerializableDummy>");
            Assert.AreEqual(dummy.Name, roundTrip.Name);
            Assert.AreEqual(dummy.Age, roundTrip.Age);
        }
    }
}
