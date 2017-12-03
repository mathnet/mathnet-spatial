namespace MathNet.Spatial.Internals
{
    using System.Globalization;
    using System.Xml;
    using System.Xml.Serialization;

    internal static class XmlWriterExt
    {
        internal static void WriteElement(this XmlWriter writer, string name, IXmlSerializable value)
        {
            writer.WriteStartElement(name);
            value.WriteXml(writer);
            writer.WriteEndElement();
        }

        internal static void WriteElement(this XmlWriter writer, string name, double value)
        {
            writer.WriteStartElement(name);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        internal static void WriteElement(this XmlWriter writer, string name, double value, string format)
        {
            writer.WriteElementString(name, value.ToString(format, CultureInfo.InvariantCulture));
        }
    }
}
