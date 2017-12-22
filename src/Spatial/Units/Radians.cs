namespace MathNet.Spatial.Units
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Obsolete - Use methods on Angle directly
    /// </summary>
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("No longer required, use Angle methods directly; Obsolete since 2017-12-18")]
    public struct Radians : IAngleUnit
    {
        /// <summary>
        /// internal name
        /// </summary>
        internal const string Name = "rad";

#pragma warning disable CS3005 // Identifier differing only in case is not CLS-compliant
        /// <inheritdoc />
        double IUnit.Conversionfactor => 1.0;

                              /// <inheritdoc />
        public double ConversionFactor => 1.0;
#pragma warning restore CS3005 // Identifier differing only in case is not CLS-compliant

        /// <inheritdoc />
        public string ShortName => Name;

        [Obsolete("This operator will be removed, use factory method FromDegrees or FromRadians. Made obsolete 2017-12-04.")]
        public static Angle operator *(double left, Radians right)
        {
            return Angle.FromRadians(left);
        }
    }
}
