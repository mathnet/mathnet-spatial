using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "CoordinateSystem")]
    public class CoordinateSystemSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D Origin;
        [DataMember(Order = 2)]
        public Vector3D XAxis;
        [DataMember(Order = 3)]
        public Vector3D YAxis;
        [DataMember(Order = 4)]
        public Vector3D ZAxis;

    //    public static implicit operator CoordinateSystemSurrogate(CoordinateSystem cs) => new CoordinateSystemSurrogate { Origin = cs.Origin, XAxis = cs.XAxis, YAxis = cs.YAxis, ZAxis = cs.ZAxis };
    //   public static implicit operator CoordinateSystem(CoordinateSystemSurrogate cs) => new CoordinateSystem(cs.Origin, cs.XAxis, cs.YAxis, cs.ZAxis);
    }

    internal class CoordinateSystemSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            CoordinateSystem c = (CoordinateSystem)obj;
            info.AddValue("Origin", c.Origin);
            info.AddValue("XAxis", c.XAxis);
            info.AddValue("YAxis", c.YAxis);
            info.AddValue("ZAxis", c.ZAxis);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Point3D origin = (Point3D)info.GetValue("Origin", typeof(Point3D));
            Vector3D x = (Vector3D)info.GetValue("XAxis", typeof(Vector3D));
            Vector3D y = (Vector3D)info.GetValue("YAxis", typeof(Vector3D));
            Vector3D z = (Vector3D)info.GetValue("ZAxis", typeof(Vector3D));
            return new CoordinateSystem(origin, x, y, z);
        }
    }
}
