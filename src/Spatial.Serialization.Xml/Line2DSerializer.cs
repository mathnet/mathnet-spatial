using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Line2D")]
    public class Line2DSurrogate
    {
        [DataMember]
        public Point2D StartPoint;
        [DataMember]
        public Point2D EndPoint;
    }

    internal class Line2DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Line2D line = (Line2D)obj;
            info.AddValue("StartPoint", line.StartPoint);
            info.AddValue("EndPoint", line.EndPoint);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Point2D start = (Point2D)info.GetValue("StartPoint", typeof(Point2D));
            Point2D end = (Point2D)info.GetValue("EndPoint", typeof(Point2D));
            return new Line2D(start, end);
        }

        public static Line2DSurrogate TranslateToSurrogate(Line2D source)
        {
            return new Line2DSurrogate { StartPoint = source.StartPoint, EndPoint = source.EndPoint };
        }

        public static Line2D TranslateToSource(Line2DSurrogate surrogate)
        {
            return new Line2D(surrogate.StartPoint, surrogate.EndPoint);
        }
    }
}
