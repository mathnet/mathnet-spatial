using ExtendedXmlSerializer.ExtensionModel.Xml;
using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Ray3D")]
    public class Ray3DSurrogate
    {
        [DataMember]
        public Point3D ThroughPoint;
        [DataMember]
        public UnitVector3D Direction;
    }

    internal class Ray3DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Ray3D line = (Ray3D)obj;
            info.AddValue("ThroughPoint", line.ThroughPoint);
            info.AddValue("Direction", line.Direction);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Point3D throughPoint = (Point3D)info.GetValue("ThroughPoint", typeof(Point3D));
            UnitVector3D direction = (UnitVector3D)info.GetValue("Direction", typeof(UnitVector3D));
            return new Ray3D(throughPoint, direction);
        }

        public static Ray3DSurrogate TranslateToSurrogate(Ray3D source)
        {
            return new Ray3DSurrogate { ThroughPoint = source.ThroughPoint, Direction = source.Direction };
        }

        public static Ray3D TranslateToSource(Ray3DSurrogate surrogate)
        {
            return new Ray3D(surrogate.ThroughPoint, surrogate.Direction);
        }
    }
}
