using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "PolyLine2D")]
    public class PolyLine2DSurrogate
    {
        [DataMember]
        public List<Point2D> Points;
    }

    internal class PolyLine2DSerializer : ISerializationSurrogate
    {

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            PolyLine2D line = (PolyLine2D)obj;
            List<Point2D> linepoints = new List<Point2D>(line);
            info.AddValue("Points", linepoints);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            List<Point2D> points = (List<Point2D>)info.GetValue("Points", typeof(List<Point2D>));
            return new PolyLine2D(points);
        }

        public static PolyLine2DSurrogate TranslateToSurrogate(PolyLine2D source)
        {
            return new PolyLine2DSurrogate { Points = new List<Point2D>(source) };
        }

        public static PolyLine2D TranslateToSource(PolyLine2DSurrogate surrogate)
        {
            return new PolyLine2D(surrogate.Points);
        }
    }
}
