using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Ray3D")]
    public class Ray3DSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D ThroughPoint;
        [DataMember(Order = 2)]
        public UnitVector3D Direction;

        public static implicit operator Ray3DSurrogate(Ray3D ray) => new Ray3DSurrogate { ThroughPoint = ray.ThroughPoint, Direction = ray.Direction };
        public static implicit operator Ray3D(Ray3DSurrogate ray) => new Ray3D(ray.ThroughPoint, ray.Direction);
    }

    internal class Ray3DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Ray3D line = (Ray3D)obj;
            info.AddValue("ThroughPoint", line.ThroughPoint);
            info.AddValue("Direction", line.Direction);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Point3D throughPoint = (Point3D)info.GetValue("ThroughPoint", typeof(Point3D));
            UnitVector3D direction = (UnitVector3D)info.GetValue("Direction", typeof(UnitVector3D));
            return new Ray3D(throughPoint, direction);
        }
    }
}
