using ExtendedXmlSerializer.ExtensionModel.Xml;
using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Vector3D")]
    public class Vector3DSurrogate
    {
        [DataMember]
        public double X;
        [DataMember]
        public double Y;
        [DataMember]
        public double Z;
    }

    internal class Vector3DSerializer : IExtendedXmlCustomSerializer<Vector3D>, ISerializationSurrogate
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

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3D point = (Vector3D)obj;
            info.AddValue("x", point.X);
            info.AddValue("y", point.Y);
            info.AddValue("z", point.Z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            double x = info.GetDouble("x");
            double y = info.GetDouble("y");
            double z = info.GetDouble("z");
            return new Vector3D(x, y, z);
        }

        public static Vector3DSurrogate TranslateToSurrogate(Vector3D source)
        {
            return new Vector3DSurrogate { X = source.X, Y = source.Y, Z = source.Z };
        }

        public static Vector3D TranslateToSource(Vector3DSurrogate surrogate)
        {
            return new Vector3D(surrogate.X, surrogate.Y, surrogate.Z);
        }
    }
}
