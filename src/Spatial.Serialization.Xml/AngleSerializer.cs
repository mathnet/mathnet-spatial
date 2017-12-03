using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    /// <summary>
    /// Surrogate for Angle
    /// </summary>
    [DataContract(Name = "Angle")]
    public class AngleSurrogate
    {
        [DataMember(Order = 1)]
        public double Value;

        public static implicit operator AngleSurrogate(Angle angle) => new AngleSurrogate { Value = angle.Radians };
        public static implicit operator Angle(AngleSurrogate angle) => new Angle(angle.Value, AngleUnit.Radians);
    }

    internal class AngleSerializer : ISerializationSurrogate
    {
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
    }
}
