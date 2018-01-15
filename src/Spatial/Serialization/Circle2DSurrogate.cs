namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Circle2D")]
    [Serializable]
    public class Circle2DSurrogate
    {
        [DataMember(Order = 1)]
        public Point2D Center;
        [DataMember(Order = 2)]
        public double Radius;

        public static implicit operator Circle2DSurrogate(Circle2D circle) => new Circle2DSurrogate { Center = circle.Center, Radius = circle.Radius };

        public static implicit operator Circle2D(Circle2DSurrogate circle) => new Circle2D(circle.Center, circle.Radius);
    }
}
