namespace MathNet.Spatial.Units
{
    using System;
    using System.ComponentModel;

    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Radians : IAngleUnit
    {
        internal const string Name = "rad";

        /// <inheritdoc />
        public double Conversionfactor => 1.0;

        /// <inheritdoc />
        public double ConversionFactor => 1.0;

        /// <inheritdoc />
        public string ShortName => Name;

        [Obsolete("This operator will be removed, use factory method FromDegrees or FromRadians. Made obsolete 2017-12-04.")]
        public static Angle operator *(double left, Radians right)
        {
            return Angle.FromRadians(left);
        }
    }
}
