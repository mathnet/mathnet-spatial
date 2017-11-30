using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Circle3D")]
    public class Circle3DSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D CenterPoint;
        [DataMember(Order = 2)]
        public UnitVector3D Axis;
        [DataMember(Order = 3)]
        public double Radius;

        public static implicit operator Circle3DSurrogate(Circle3D circle) => new Circle3DSurrogate { CenterPoint = circle.CenterPoint, Axis = circle.Axis, Radius = circle.Radius };
        public static implicit operator Circle3D(Circle3DSurrogate circle) => new Circle3D(circle.CenterPoint, circle.Axis, circle.Radius);
    }

    internal class Circle3DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Circle3D circle = (Circle3D)obj;
            info.AddValue("CenterPoint", circle.CenterPoint);
            info.AddValue("Axis", circle.Axis);
            info.AddValue("Radius", circle.Radius);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Point3D center = (Point3D)info.GetValue("CenterPoint", typeof(Point3D));
            UnitVector3D axis = (UnitVector3D)info.GetValue("Axis", typeof(UnitVector3D));
            double radius = info.GetDouble("Radius");
            return new Circle3D(center, axis, radius);
        }
    }
}
