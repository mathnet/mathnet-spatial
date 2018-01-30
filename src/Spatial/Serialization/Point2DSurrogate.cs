namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Point2D")]
    [Serializable]
    public class Point2DSurrogate
    {
        [DataMember(Order=1)]
        [XmlAttribute]
        public double X;
        [DataMember(Order=2)]
        [XmlAttribute]
        public double Y;

        public static implicit operator Point2DSurrogate(Point2D point) => new Point2DSurrogate { X = point.X, Y = point.Y };

        public static implicit operator Point2D(Point2DSurrogate point) => new Point2D(point.X, point.Y);
    }
}
