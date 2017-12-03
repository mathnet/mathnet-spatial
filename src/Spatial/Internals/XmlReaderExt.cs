namespace MathNet.Spatial.Internals
{
    using System.Xml;

    internal static class XmlReaderExt
    {
        internal static bool TryReadAttributeAsDouble(this XmlReader reader, string localName, out double value)
        {
            if (reader.MoveToContent() == XmlNodeType.Element &&
                reader.HasAttributes &&
                reader.MoveToAttribute(localName))
            {
                value = XmlConvert.ToDouble(reader.Value);
                return true;
            }

            value = 0;
            return false;
        }
    }
}
