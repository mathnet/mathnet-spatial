namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Point3D")]
    [Serializable]
    public class Point3DSurrogate
    {
        [DataMember(Order = 1)]
        [XmlAttribute]
        public double X;
        [DataMember(Order = 2)]
        [XmlAttribute]
        public double Y;
        [DataMember(Order = 3)]
        [XmlAttribute]
        public double Z;

        public static implicit operator Point3DSurrogate(Point3D point) => new Point3DSurrogate { X = point.X, Y = point.Y, Z = point.Z };

        public static implicit operator Point3D(Point3DSurrogate point) => new Point3D(point.X, point.Y, point.Z);
    }
}
