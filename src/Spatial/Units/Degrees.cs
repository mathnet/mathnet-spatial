using System;
using System.ComponentModel;

namespace MathNet.Spatial.Units
{
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
        public double ConversionFactor => DegToRad;

        /// <inheritdoc />
        public string ShortName => Name;
    }
}
