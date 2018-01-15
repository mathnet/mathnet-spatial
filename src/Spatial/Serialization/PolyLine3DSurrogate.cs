namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "PolyLine3D")]
    [Serializable]
    public class PolyLine3DSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D[] Points;

        public static implicit operator PolyLine3DSurrogate(PolyLine3D polyline) => new PolyLine3DSurrogate { Points = polyline.Vertices.ToArray() };

        public static implicit operator PolyLine3D(PolyLine3DSurrogate polyline) => new PolyLine3D(polyline.Points);
    }
}
