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
        [DataMember(Order = 1)]
        public double X;
        [DataMember(Order = 2)]
        public double Y;

        public static implicit operator Vector2DSurrogate(Vector2D vector) => new Vector2DSurrogate { X = vector.X, Y = vector.Y };
        public static implicit operator Vector2D(Vector2DSurrogate vector) => new Vector2D(vector.X, vector.Y);
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
    }
}
