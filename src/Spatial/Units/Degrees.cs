namespace MathNet.Spatial.Units
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A degree or degree of arc typically denoted by °.  It is defined such that a full rotation is 360 degrees.
    /// </summary>
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Degrees : IAngleUnit
    {
        /// <summary>
        /// Internal name
        /// </summary>
        internal const string Name = "\u00B0";

        /// <summary>
        /// Degree to radians conversion factor
        /// </summary>
        private const double DegToRad = Math.PI / 180.0;

        /// <inheritdoc />
        double IUnit.Conversionfactor => this.ConversionFactor;

        /// <inheritdoc />
        public double ConversionFactor => DegToRad;

        /// <inheritdoc />
        public string ShortName => Name;

        [Obsolete("This operator will be removed, use factory method FromDegrees or FromRadians. Made obsolete 2017-12-04.")]
        public static Angle operator *(double left, Degrees right)
        {
            return Angle.FromDegrees(left);
        }
    }
}
