namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Vector2D")]
    [Serializable]
    public class Vector2DSurrogate
    {
        [DataMember(Order = 1)]
        public double X;
        [DataMember(Order = 2)]
        public double Y;

        public static implicit operator Vector2DSurrogate(Vector2D vector) => new Vector2DSurrogate { X = vector.X, Y = vector.Y };

        public static implicit operator Vector2D(Vector2DSurrogate vector) => new Vector2D(vector.X, vector.Y);
    }
}
