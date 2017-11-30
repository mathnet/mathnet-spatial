using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Line3D")]
    public class Line3DSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D StartPoint;
        [DataMember(Order = 2)]
        public Point3D EndPoint;

        public static implicit operator Line3DSurrogate(Line3D line) => new Line3DSurrogate { StartPoint = line.StartPoint, EndPoint = line.EndPoint };
        public static implicit operator Line3D(Line3DSurrogate line) => new Line3D(line.StartPoint, line.EndPoint);
    }

    internal class Line3DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Line3D line = (Line3D)obj;
            info.AddValue("StartPoint", line.StartPoint);
            info.AddValue("EndPoint", line.EndPoint);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Point3D start = (Point3D)info.GetValue("StartPoint", typeof(Point3D));
            Point3D end = (Point3D)info.GetValue("EndPoint", typeof(Point3D));
            return new Line3D(start, end);
        }
    }
}
