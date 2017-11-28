using ExtendedXmlSerializer.ExtensionModel.Xml;
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
        [DataMember]
        public Point3D StartPoint;
        [DataMember]
        public Point3D EndPoint;
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

        public static Line3DSurrogate TranslateToSurrogate(Line3D source)
        {
            return new Line3DSurrogate { StartPoint = source.StartPoint, EndPoint = source.EndPoint };
        }

        public static Line3D TranslateToSource(Line3DSurrogate surrogate)
        {
            return new Line3D(surrogate.StartPoint, surrogate.EndPoint);
        }
    }
}
