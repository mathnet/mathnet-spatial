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
        [DataMember]
        public double X;
        [DataMember]
        public double Y;
        [DataMember]
        public double Z;
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

        public static UnitVector3DSurrogate TranslateToSurrogate(UnitVector3D source)
        {
            return new UnitVector3DSurrogate { X = source.X, Y = source.Y, Z = source.Z };
        }

        public static UnitVector3D TranslateToSource(UnitVector3DSurrogate surrogate)
        {
            return new UnitVector3D(surrogate.X, surrogate.Y, surrogate.Z);
        }
    }
}
