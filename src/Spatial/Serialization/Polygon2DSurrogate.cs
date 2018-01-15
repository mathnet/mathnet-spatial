namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Polygon2D")]
    [Serializable]
    public class Polygon2DSurrogate
    {
        [DataMember(Order = 1)]
        public Point2D[] Points;

        public static implicit operator Polygon2DSurrogate(Polygon2D polygon) => new Polygon2DSurrogate { Points = polygon.Vertices.ToArray() };

        public static implicit operator Polygon2D(Polygon2DSurrogate polygon) => new Polygon2D(polygon.Points);
    }
}
