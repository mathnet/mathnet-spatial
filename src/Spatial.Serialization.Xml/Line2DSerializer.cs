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
        [DataMember(Order = 1)]
        public Point2D StartPoint;
        [DataMember(Order = 2)]
        public Point2D EndPoint;

        public static implicit operator Line2DSurrogate(Line2D line) => new Line2DSurrogate { StartPoint = line.StartPoint, EndPoint = line.EndPoint };
        public static implicit operator Line2D(Line2DSurrogate line) => new Line2D(line.StartPoint, line.EndPoint);
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
    }
}
