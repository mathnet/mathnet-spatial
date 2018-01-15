namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Ray3D")]
    [Serializable]
    public class Ray3DSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D ThroughPoint;
        [DataMember(Order = 2)]
        public UnitVector3D Direction;

        public static implicit operator Ray3DSurrogate(Ray3D ray) => new Ray3DSurrogate { ThroughPoint = ray.ThroughPoint, Direction = ray.Direction };

        public static implicit operator Ray3D(Ray3DSurrogate ray) => new Ray3D(ray.ThroughPoint, ray.Direction);
    }
}
