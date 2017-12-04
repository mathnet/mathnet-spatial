namespace MathNet.Spatial.Internals
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    internal static class XmlReaderExt
    {
        /// <summary>
        /// Creates a default(T) and calls ReadXml(reader) on it.
        /// </summary>
        /// <typeparam name="T">The type of the instance to read from the current position of the reader.</typeparam>
        /// <param name="reader">A <see cref="XmlReader"/></param>
        /// <returns> A new instance of {T} with values from <paramref name="reader"/></returns>
        internal static T ReadFrom<T>(this XmlReader reader)
            where T : struct, IXmlSerializable
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var instance = default(T);
            instance.ReadXml(reader);
            return instance;
        }

        /// <summary>
        /// Reads the attribute named <paramref name="localName"/> if it exists on the current element.
        /// This is not a proper try method as it checks if <paramref name="reader"/> is null and throws.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/></param>
        /// <param name="localName">The name of the attribute to read the value of.</param>
        /// <param name="value">The value read from from <paramref name="reader"/></param>
        /// <returns>True if the attribute was found.</returns>
        internal static bool TryReadAttributeAsDouble(this XmlReader reader, string localName, out double value)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

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

        /// <summary>
        /// Reads the values oif the elements named <paramref name="localName"/> and <paramref name="localName"/> if they exist on the current element.
        /// This is not a proper try method as it checks if <paramref name="reader"/> is null and throws.
        /// </summary>
        /// <remarks>
        /// Calling this method has side effects as it changes the position of the reader.
        /// </remarks>
        /// <param name="reader">A <see cref="XmlReader"/></param>
        /// <param name="localName">The local name of the element to read value from.</param>
        /// <param name="value">The value read from from <paramref name="reader"/></param>
        /// <returns>True if both elements were found.</returns>
        internal static bool TryReadElementContentAsDouble(this XmlReader reader, string localName, out double value)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.MoveToContent() == XmlNodeType.Element &&
                !reader.IsEmptyElement &&
                reader.LocalName == localName)
            {
                value = reader.ReadElementContentAsDouble();
                return true;
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// Reads the values oif the elements named <paramref name="xName"/> and <paramref name="xName"/> if they exist on the current element.
        /// This is not a proper try method as it checks if <paramref name="reader"/> is null and throws.
        /// </summary>
        /// <remarks>
        /// Calling this method has side effects as it changes the position of the reader.
        /// </remarks>
        /// <param name="reader">A <see cref="XmlReader"/></param>
        /// <param name="xName">The local name of the x element.</param>
        /// <param name="yName">The local name of the y element.</param>
        /// <param name="x">The x value read from from <paramref name="reader"/></param>
        /// <param name="y">The y value read from from <paramref name="reader"/></param>
        /// <returns>True if both elements were found.</returns>
        internal static bool TryReadElementsAsDoubles(this XmlReader reader, string xName, string yName, out double x, out double y)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            x = 0;
            y = 0;
            if (reader.MoveToContent() == XmlNodeType.Element &&
                !reader.IsEmptyElement &&
                !reader.HasValue)
            {
                var subtree = reader.ReadSubtree();
                if (subtree.ReadToFirstDescendant())
                {
                    if (subtree.TryReadElementContentAsDouble(xName, out x) &&
                        subtree.TryReadElementContentAsDouble(yName, out y))
                    {
                        subtree.Skip();
                        return true;
                    }

                    if (subtree.TryReadElementContentAsDouble(yName, out y) &&
                        subtree.TryReadElementContentAsDouble(xName, out x))
                    {
                        subtree.Skip();
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool ReadToFirstDescendant(this XmlReader reader)
        {
            var depth = reader.Depth;
            if (reader.MoveToContent() != XmlNodeType.Element)
            {
                return reader.Depth > depth &&
                       reader.NodeType == XmlNodeType.Element;
            }

            while (reader.Read() &&
                   reader.Depth <= depth)
            {
            }

            return reader.Depth > depth &&
                   reader.NodeType == XmlNodeType.Element;
        }
    }
}
