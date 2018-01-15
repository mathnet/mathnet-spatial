namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "UnitVector3D")]
    [Serializable]
    public class UnitVector3DSurrogate
    {
        [DataMember(Order = 1)]
        public double X;
        [DataMember(Order = 2)]
        public double Y;
        [DataMember(Order = 3)]
        public double Z;

        public static implicit operator UnitVector3DSurrogate(UnitVector3D vector) => new UnitVector3DSurrogate { X = vector.X, Y = vector.Y, Z = vector.Z };

        public static implicit operator UnitVector3D(UnitVector3DSurrogate vector) => UnitVector3D.Create(vector.X, vector.Y, vector.Z);
    }
}
