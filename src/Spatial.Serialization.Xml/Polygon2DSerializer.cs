using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Polygon2D")]
    public class Polygon2DSurrogate
    {
        [DataMember]
        public List<Point2D> Points;
    }

    internal class Polygon2DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Polygon2D line = (Polygon2D)obj;
            List<Point2D> linepoints = new List<Point2D>(line);
            info.AddValue("Points", linepoints);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            List<Point2D> points = (List<Point2D>)info.GetValue("Points", typeof(List<Point2D>));
            return new Polygon2D(points);
        }

        public static Polygon2DSurrogate TranslateToSurrogate(Polygon2D source)
        {
            return new Polygon2DSurrogate { Points = new List<Point2D>(source) };
        }

        public static Polygon2D TranslateToSource(Polygon2DSurrogate surrogate)
        {
            return new Polygon2D(surrogate.Points);
        }
    }
}
