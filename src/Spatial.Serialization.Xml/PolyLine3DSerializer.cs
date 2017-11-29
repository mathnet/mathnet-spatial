using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "PolyLine3D")]
    public class PolyLine3DSurrogate
    {
        [DataMember]
        public List<Point3D> Points;
    }

    internal class PolyLine3DSerializer : ISerializationSurrogate
    {

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            PolyLine3D line = (PolyLine3D)obj;
            List<Point3D> linepoints = new List<Point3D>(line);
            info.AddValue("Points", linepoints);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            List<Point3D> points = (List<Point3D>)info.GetValue("Points", typeof(List<Point3D>));
            return new PolyLine3D(points);
        }

        public static PolyLine3DSurrogate TranslateToSurrogate(PolyLine3D source)
        {
            return new PolyLine3DSurrogate { Points = new List<Point3D>(source) };
        }

        public static PolyLine3D TranslateToSource(PolyLine3DSurrogate surrogate)
        {
            return new PolyLine3D(surrogate.Points);
        }
    }
}
