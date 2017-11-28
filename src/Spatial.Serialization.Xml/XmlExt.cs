namespace MathNet.Spatial.Serialization.Xml
{
    using System;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public static class XmlExt
    {
        public static string ReadAttributeOrElement(this XElement e, string localName)
        {
            XAttribute xattribute = e.Attributes()
                                     .SingleOrDefault(x => x.Name.LocalName == localName);
            if (xattribute != null)
            {
                return xattribute.Value;
            }

            XElement xelement = e.Elements()
                                 .SingleOrDefault(x => x.Name.LocalName == localName);
            if (xelement != null)
            {
                return xelement.Value;
            }

            throw new XmlException(string.Format("Attribute or element {0} not found", localName));
        }

        public static T ReadAttributeOrElementEnum<T>(this XElement e, string localName) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), e.ReadAttributeOrElement(localName));
        }

        public static string ReadAttributeOrElementOrDefault(this XElement e, string localName)
        {
            XAttribute xattribute = e.Attributes()
                                     .SingleOrDefault(x => x.Name.LocalName == localName);
            if (xattribute != null)
            {
                return xattribute.Value;
            }

            XElement xelement = e.Elements()
                                 .SingleOrDefault(x => x.Name.LocalName == localName);
            if (xelement != null)
            {
                return xelement.Value;
            }

            return null;
        }
    }
}
