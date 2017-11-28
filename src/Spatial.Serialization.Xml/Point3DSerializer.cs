using ExtendedXmlSerializer.ExtensionModel.Xml;
using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Point3D")]
    public class Point3DSurrogate
    {
        [DataMember]
        public double X;
        [DataMember]
        public double Y;
        [DataMember]
        public double Z;
    }

    internal class Point3DSerializer : IExtendedXmlCustomSerializer<Point3D>, ISerializationSurrogate
    {
        public Point3D Deserialize(XElement xElement)
        {
            var x = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("X"));
            var y = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("Y"));
            var z = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("Z"));
            return new Point3D(x, y, z);
        }

        public void Serializer(XmlWriter xmlWriter, Point3D obj)
        {
            xmlWriter.WriteAttributeString("X", obj.X.ToString("R", CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("Y", obj.Y.ToString("R", CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("Z", obj.Z.ToString("R", CultureInfo.InvariantCulture));
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Point3D point = (Point3D)obj;
            info.AddValue("x", point.X);
            info.AddValue("y", point.Y);
            info.AddValue("z", point.Z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            double x = info.GetDouble("x");
            double y = info.GetDouble("y");
            double z = info.GetDouble("z");
            return new Point3D(x, y, z);
        }

        public static Point3DSurrogate TranslateToSurrogate(Point3D source)
        {
            return new Point3DSurrogate { X = source.X, Y = source.Y, Z = source.Z };
        }

        public static Point3D TranslateToSource(Point3DSurrogate surrogate)
        {
            return new Point3D(surrogate.X, surrogate.Y, surrogate.Z);
        }
    }
}
