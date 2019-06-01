using System;
using System.ComponentModel;

namespace MathNet.Spatial.Units
{
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Radians : IAngleUnit
    {
        /// <summary>
        /// internal name
        /// </summary>
        internal const string Name = "rad";

        /// <inheritdoc />
        public double ConversionFactor => 1.0;

        /// <inheritdoc />
        public string ShortName => Name;
    }
}
