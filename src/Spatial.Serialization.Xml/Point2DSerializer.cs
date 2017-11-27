using ExtendedXmlSerializer.ExtensionModel.Xml;
using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using MathNet.Spatial;

namespace MathNet.Spatial.Serialization.Xml
{
    public class Point2DSerializer : IExtendedXmlCustomSerializer<Point2D>
    {
        public Point2D Deserialize(XElement xElement)
        {
            var x = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("X"));
            var y = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("Y"));
            return new Point2D(x, y);
        }

        public void Serializer(XmlWriter xmlWriter, Point2D obj)
        {
            xmlWriter.WriteAttributeString("X", obj.X.ToString("R", CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("Y", obj.Y.ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
