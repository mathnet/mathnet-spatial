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
    [DataContract(Name = "PolyLine3D")]
    public class PolyLine3DSurrogate
    {
        [DataMember(Order = 1)]
        public List<Point3D> Points;

        public static implicit operator PolyLine3DSurrogate(PolyLine3D polyline) => new PolyLine3DSurrogate { Points = new List<Point3D>(polyline) };
        public static implicit operator PolyLine3D(PolyLine3DSurrogate polyline) => new PolyLine3D(polyline.Points);
    }

    internal class PolyLine3DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            PolyLine3D line = (PolyLine3D)obj;

            //unfortunately the legacy formatters do not handled nested objects in lists when the object has readonly properties
            List<Point3DSurrogate> linepoints = line.Select(x => (Point3DSurrogate)x).ToList(); 
            info.AddValue("Points", linepoints);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            List<Point3DSurrogate> points = (List<Point3DSurrogate>)info.GetValue("Points", typeof(List<Point3DSurrogate>));
            List<Point3D> linepoints = points.Select(x => (Point3D)x).ToList();
            return new PolyLine3D(linepoints);
        }
    }
}
