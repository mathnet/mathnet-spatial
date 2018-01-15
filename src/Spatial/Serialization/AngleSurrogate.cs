namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Units;

    /// <summary>
    /// Surrogate for Angle
    /// </summary>
    [DataContract(Name = "Angle")]
    [Serializable]
    public class AngleSurrogate
    {
        [DataMember(Order = 1)]
        public double Value;

        public static implicit operator AngleSurrogate(Angle angle) => new AngleSurrogate { Value = angle.Radians };

        public static implicit operator Angle(AngleSurrogate angle) => Angle.FromRadians(angle.Value);
    }
}
