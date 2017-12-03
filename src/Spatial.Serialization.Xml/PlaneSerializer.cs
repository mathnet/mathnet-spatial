using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Plane")]
    public class PlaneSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D RootPoint;
        [DataMember(Order = 2)]
        public UnitVector3D Normal;

        public static implicit operator PlaneSurrogate(Plane plane) => new PlaneSurrogate { RootPoint = plane.RootPoint, Normal = plane.Normal };
        public static implicit operator Plane(PlaneSurrogate plane) => new Plane(plane.RootPoint, plane.Normal);
    }

    internal class PlaneSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Plane p = (Plane)obj;
            info.AddValue("RootPoint", p.RootPoint);
            info.AddValue("Normal", p.Normal);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Point3D throughPoint = (Point3D)info.GetValue("RootPoint", typeof(Point3D));
            UnitVector3D direction = (UnitVector3D)info.GetValue("Normal", typeof(UnitVector3D));
            return new Plane(throughPoint, direction);
        }
    }
}
