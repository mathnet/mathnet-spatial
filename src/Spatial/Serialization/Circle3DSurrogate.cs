namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Circle3D")]
    [Serializable]
    public class Circle3DSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D CenterPoint;
        [DataMember(Order = 2)]
        public UnitVector3D Axis;
        [DataMember(Order = 3)]
        public double Radius;

        public static implicit operator Circle3DSurrogate(Circle3D circle) => new Circle3DSurrogate { CenterPoint = circle.CenterPoint, Axis = circle.Axis, Radius = circle.Radius };

        public static implicit operator Circle3D(Circle3DSurrogate circle) => new Circle3D(circle.CenterPoint, circle.Axis, circle.Radius);
    }
}
