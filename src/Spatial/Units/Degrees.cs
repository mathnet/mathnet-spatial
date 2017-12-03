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
        internal const string Name = "\u00B0";
        private const double DegToRad = Math.PI / 180.0;

        public double Conversionfactor => DegToRad;

        public string ShortName => Name;

        public static Angle operator *(double left, Degrees right)
        {
            return new Angle(left, right);
        }
    }
}
