using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Point2D")]
    [Serializable]
    public class Point2DSurrogate
    {
        [DataMember(Order=1)]
        public double X;
        [DataMember(Order=2)]
        public double Y;

        public static implicit operator Point2DSurrogate(Point2D point) => new Point2DSurrogate { X = point.X, Y = point.Y };
        public static implicit operator Point2D(Point2DSurrogate point) => new Point2D(point.X, point.Y);
    }

    internal class Point2DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Point2D point = (Point2D)obj;
            info.AddValue("x", point.X);
            info.AddValue("y", point.Y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            double x = info.GetDouble("x");
            double y = info.GetDouble("y");
            return new Point2D(x, y);
        }
    }
}
