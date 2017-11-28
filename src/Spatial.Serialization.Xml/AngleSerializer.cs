using ExtendedXmlSerializer.ExtensionModel.Xml;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Angle")]
    public class AngleSurrogate
    {
        [DataMember]
        public double Value;
    }

    internal class AngleSerializer : IExtendedXmlCustomSerializer<Angle>, ISerializationSurrogate
    {
        public Angle Deserialize(XElement xElement)
        {
            var rad = XmlConvert.ToDouble(xElement.ReadAttributeOrElementOrDefault("Value"));
            return new Angle(rad, AngleUnit.Radians);
        }

        public void Serializer(XmlWriter xmlWriter, Angle obj)
        {
            xmlWriter.WriteAttributeString("Value", obj.Radians.ToString("R", CultureInfo.InvariantCulture));
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Angle point = (Angle)obj;
            info.AddValue("Value", point.Radians);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            double rad = info.GetDouble("Value");
            return new Angle(rad, AngleUnit.Radians);
        }

        public static AngleSurrogate TranslateToSurrogate(Angle source)
        {
            return new AngleSurrogate { Value = source.Radians };
        }

        public static Angle TranslateToSource(AngleSurrogate surrogate)
        {
            return new Angle(surrogate.Value, AngleUnit.Radians);
        }
    }
}
