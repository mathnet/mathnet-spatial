using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Quaternion")]
    public class QuaternionSurrogate
    {
        [DataMember]
        public double W;
        [DataMember]
        public double X;
        [DataMember]
        public double Y;
        [DataMember]
        public double Z;
    }

    internal class QuaternionSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Quaternion point = (Quaternion)obj;
            info.AddValue("w", point.Real);
            info.AddValue("x", point.ImagX);
            info.AddValue("y", point.ImagY);
            info.AddValue("z", point.ImagZ);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            double w = info.GetDouble("w");
            double x = info.GetDouble("x");
            double y = info.GetDouble("y");
            double z = info.GetDouble("z");
            return new Quaternion(w, x, y, z);
        }

        public static QuaternionSurrogate TranslateToSurrogate(Quaternion source)
        {
            return new QuaternionSurrogate { W = source.Real, X = source.ImagX, Y = source.ImagY, Z = source.ImagZ };
        }

        public static Quaternion TranslateToSource(QuaternionSurrogate surrogate)
        {
            return new Quaternion(surrogate.W, surrogate.X, surrogate.Y, surrogate.Z);
        }
    }
}
