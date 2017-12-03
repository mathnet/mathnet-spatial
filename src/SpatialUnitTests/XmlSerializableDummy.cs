namespace MathNet.Spatial.UnitTests
{
    using System.Globalization;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    public class XmlSerializableDummy : IXmlSerializable
    {
        private readonly string name;
        private XmlSerializableDummy()
        {
        }

        public XmlSerializableDummy(string name, int age)
        {
            this.Age = age;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int Age { get; set; }

        public XmlSchema GetSchema() { return null; }

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
