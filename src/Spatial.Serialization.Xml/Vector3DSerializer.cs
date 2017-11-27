using ExtendedXmlSerializer.ExtensionModel.Xml;
using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using MathNet.Spatial;

namespace MathNet.Spatial.Serialization.Xml
{
    public class Vector3DSerializer : IExtendedXmlCustomSerializer<Vector3D>
    {
        public Vector3D Deserialize(XElement xElement)
        {
            var x = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("X"));
            var y = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("Y"));
            var z = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("Z"));
            return new Vector3D(x, y, z);
        }

        public void Serializer(XmlWriter xmlWriter, Vector3D obj)
        {
            xmlWriter.WriteAttributeString("X", obj.X.ToString("R", CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("Y", obj.Y.ToString("R", CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("Z", obj.Z.ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
