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
        [DataMember(Order = 1)]
        public double W;
        [DataMember(Order = 2)]
        public double X;
        [DataMember(Order = 3)]
        public double Y;
        [DataMember(Order = 4)]
        public double Z;

        public static implicit operator QuaternionSurrogate(Quaternion quat) => new QuaternionSurrogate { W = quat.Real, X = quat.ImagX, Y = quat.ImagY, Z = quat.ImagZ };
        public static implicit operator Quaternion(QuaternionSurrogate quat) => new Quaternion(quat.W, quat.X, quat.Y, quat.Z);

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

    }
}
