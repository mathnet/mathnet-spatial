using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Point2D")]
    public class Point2DSurrogate
    {
        [DataMember]
        public double X;
        [DataMember]
        public double Y;
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

        public static Point2DSurrogate TranslateToSurrogate(Point2D source)
        {
            return new Point2DSurrogate { X = source.X, Y = source.Y };
        }

        public static Point2D TranslateToSource(Point2DSurrogate surrogate)
        {
            return new Point2D(surrogate.X, surrogate.Y);
        }
    }
}
