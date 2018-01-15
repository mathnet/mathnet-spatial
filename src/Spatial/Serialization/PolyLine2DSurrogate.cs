namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "PolyLine2D")]
    [Serializable]
    public class PolyLine2DSurrogate
    {
        [DataMember(Order = 1)]
        public Point2D[] Points;

        public static implicit operator PolyLine2DSurrogate(PolyLine2D polyline) => new PolyLine2DSurrogate { Points = polyline.Vertices.ToArray() };

        public static implicit operator PolyLine2D(PolyLine2DSurrogate polyline) => new PolyLine2D(polyline.Points);
    }
}
