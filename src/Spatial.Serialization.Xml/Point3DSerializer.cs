using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Point3D")]
    [Serializable]
    public class Point3DSurrogate
    {
        [DataMember(Order = 1)]
        public double X;
        [DataMember(Order = 2)]
        public double Y;
        [DataMember(Order = 3)]
        public double Z;

        public static implicit operator Point3DSurrogate(Point3D point) => new Point3DSurrogate { X = point.X, Y = point.Y, Z = point.Z };
        public static implicit operator Point3D(Point3DSurrogate point) => new Point3D(point.X, point.Y, point.Z);
    }

    internal class Point3DSerializer : ISerializationSurrogate
    {
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
    }
}
