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
        [DataMember(Order = 1)]
        public double X;
        [DataMember(Order = 2)]
        public double Y;
        [DataMember(Order = 3)]
        public double Z;

        public static implicit operator Vector3DSurrogate(Vector3D vector) => new Vector3DSurrogate { X = vector.X, Y = vector.Y, Z = vector.Z };
        public static implicit operator Vector3D(Vector3DSurrogate vector) => new Vector3D(vector.X, vector.Y, vector.Z);
    }

    internal class Vector3DSerializer : ISerializationSurrogate
    {
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
    }
}
