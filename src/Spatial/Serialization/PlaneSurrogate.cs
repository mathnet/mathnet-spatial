namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Plane")]
    [Serializable]
    public class PlaneSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D RootPoint;
        [DataMember(Order = 2)]
        public UnitVector3D Normal;

        public static implicit operator PlaneSurrogate(Plane plane) => new PlaneSurrogate { RootPoint = plane.RootPoint, Normal = plane.Normal };

        public static implicit operator Plane(PlaneSurrogate plane) => new Plane(plane.RootPoint, plane.Normal);
    }
}
