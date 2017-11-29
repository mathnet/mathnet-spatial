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
        [DataMember]
        public Point3D CenterPoint;
        [DataMember]
        public UnitVector3D Axis;
        [DataMember]
        public double Radius;
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

        public static Circle3DSurrogate TranslateToSurrogate(Circle3D source)
        {
            return new Circle3DSurrogate { CenterPoint = source.CenterPoint, Axis = source.Axis, Radius = source.Radius };
        }

        public static Circle3D TranslateToSource(Circle3DSurrogate surrogate)
        {
            return new Circle3D(surrogate.CenterPoint, surrogate.Axis, surrogate.Radius);
        }
    }
}
