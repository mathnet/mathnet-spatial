using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "UnitVector3D")]
    public class UnitVector3DSurrogate
    {
        [DataMember(Order = 1)]
        public double X;
        [DataMember(Order = 2)]
        public double Y;
        [DataMember(Order = 3)]
        public double Z;

        public static implicit operator UnitVector3DSurrogate(UnitVector3D vector) => new UnitVector3DSurrogate { X = vector.X, Y = vector.Y, Z = vector.Z };
        public static implicit operator UnitVector3D(UnitVector3DSurrogate vector) => new UnitVector3D(vector.X, vector.Y, vector.Z);

    }

    internal class UnitVector3DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            UnitVector3D point = (UnitVector3D)obj;
            info.AddValue("x", point.X);
            info.AddValue("y", point.Y);
            info.AddValue("z", point.Z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            double x = info.GetDouble("x");
            double y = info.GetDouble("y");
            double z = info.GetDouble("z");
            return new UnitVector3D(x, y, z);
        }
    }
}
