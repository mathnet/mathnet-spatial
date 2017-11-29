using MathNet.Spatial.Euclidean;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "Vector2D")]
    public class Vector2DSurrogate
    {
        [DataMember]
        public double X;
        [DataMember]
        public double Y;
    }

    internal class Vector2DSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector2D point = (Vector2D)obj;
            info.AddValue("x", point.X);
            info.AddValue("y", point.Y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            double x = info.GetDouble("x");
            double y = info.GetDouble("y");
            return new Vector2D(x, y);
        }

        public static Vector2DSurrogate TranslateToSurrogate(Vector2D source)
        {
            return new Vector2DSurrogate { X = source.X, Y = source.Y };
        }

        public static Vector2D TranslateToSource(Vector2DSurrogate surrogate)
        {
            return new Vector2D(surrogate.X, surrogate.Y);
        }
    }
}
