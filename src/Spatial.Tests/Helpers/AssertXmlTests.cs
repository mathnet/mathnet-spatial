namespace MathNet.Spatial.UnitTests
{
#if NETCOREAPP1_1 == false

    using System.Globalization;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using NUnit.Framework;

    public class AssertXmlTests
    {
        [Test]
        public void XmlSerializerRoundTripTest()
        {
            var dummy = new XmlSerializableDummy("Meh", 14);
            var roundTrip = AssertXml.XmlSerializerRoundTrip(dummy, @"<XmlSerializableDummy Age=""14""><Name>Meh</Name></XmlSerializableDummy>");
            Assert.AreEqual(dummy.Name, roundTrip.Name);
            Assert.AreEqual(dummy.Age, roundTrip.Age);
        }

        public class XmlSerializableDummy : IXmlSerializable
        {
            private readonly string name;

            public XmlSerializableDummy(string name, int age)
            {
                this.Age = age;
                this.name = name;
            }

            // ReSharper disable once UnusedMember.Local
            private XmlSerializableDummy()
            {
            }

            public string Name => this.name;

            public int Age { get; set; }

            public XmlSchema GetSchema() => null;

            public void ReadXml(XmlReader reader)
            {
                var e = (XElement)XNode.ReadFrom(reader);
                this.Age = XmlConvert.ToInt32(e.Attribute("Age").Value);
                var name = e.ReadAttributeOrElement("Name");
                XmlExt.WriteValueToReadonlyField(this, name, () => this.name);
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteAttributeString("Age", this.Age.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Name", this.Name);
            }
        }
    }

#endif

}
