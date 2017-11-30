using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Polygon2D")]
    public class Polygon2DSurrogate
    {
        [DataMember(Order = 1)]
        public List<Point2D> Points;

        public static implicit operator Polygon2DSurrogate(Polygon2D polygon) => new Polygon2DSurrogate { Points = new List<Point2D>(polygon) };
        public static implicit operator Polygon2D(Polygon2DSurrogate polygon) => new Polygon2D(polygon.Points);
    }

    internal class Polygon2DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Polygon2D line = (Polygon2D)obj;

            //unfortunately the legacy formatters do not handled nested objects in lists when the object has readonly properties
            List<Point2DSurrogate> linepoints = line.Select(x => (Point2DSurrogate)x).ToList();
            info.AddValue("Points", linepoints);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            List<Point2DSurrogate> points = (List<Point2DSurrogate>)info.GetValue("Points", typeof(List<Point2DSurrogate>));
            List<Point2D> linepoints = points.Select(x => (Point2D)x).ToList();
            return new Polygon2D(linepoints);
        }
    }
}
