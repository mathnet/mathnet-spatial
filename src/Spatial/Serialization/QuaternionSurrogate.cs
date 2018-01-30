namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "Quaternion")]
    [Serializable]
    public class QuaternionSurrogate
    {
        [DataMember(Order = 1)]
        [XmlAttribute]
        public double W;
        [DataMember(Order = 2)]
        [XmlAttribute]
        public double X;
        [DataMember(Order = 3)]
        [XmlAttribute]
        public double Y;
        [DataMember(Order = 4)]
        [XmlAttribute]
        public double Z;

        public static implicit operator QuaternionSurrogate(Quaternion quat) => new QuaternionSurrogate { W = quat.Real, X = quat.ImagX, Y = quat.ImagY, Z = quat.ImagZ };

        public static implicit operator Quaternion(QuaternionSurrogate quat) => new Quaternion(quat.W, quat.X, quat.Y, quat.Z);
    }
}
