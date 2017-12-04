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

        internal static bool TryReadElementsAsDoubles(this XmlReader reader, string xName, string yName, out double x, out double y)
        {
            x = 0;
            y = 0;
            bool foundX = false;
            bool foundY = false;

            if (reader.MoveToContent() == XmlNodeType.Element &&
                !reader.IsEmptyElement &&
                !reader.HasValue)
            {
                var subtree = reader.ReadSubtree();
                while (subtree.Read())
                {
                    if (subtree.LocalName == xName)
                    {
                        foundX = true;
                        x = subtree.ReadElementContentAsDouble();
                    }

                    if (subtree.LocalName == yName)
                    {
                        foundY = true;
                        y = subtree.ReadElementContentAsDouble();
                    }
                }

                return foundX && foundY;
            }

            return false;
        }
    }
}
