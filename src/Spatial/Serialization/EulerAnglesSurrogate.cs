namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    [DataContract(Name = "EulerAngles")]
    [Serializable]
    public class EulerAnglesSurrogate
    {
        [DataMember(Order = 1)]
        public Angle Alpha;
        [DataMember(Order = 2)]
        public Angle Beta;
        [DataMember(Order = 3)]
        public Angle Gamma;

        public static implicit operator EulerAnglesSurrogate(EulerAngles angles) => new EulerAnglesSurrogate { Alpha = angles.Alpha, Beta = angles.Beta, Gamma = angles.Gamma };

        public static implicit operator EulerAngles(EulerAnglesSurrogate angles) => new EulerAngles(angles.Alpha, angles.Beta, angles.Gamma);
    }
}
