namespace MathNet.Spatial.Units
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A degree or degree of arc typically denoted by �.  It is defined such that a full rotation is 360 degrees.
    /// </summary>
    [Serializable, EditorBrowsable(EditorBrowsableState.Never)]
    public struct Degrees : IAngleUnit
    {
        private const double Conv = Math.PI / 180.0;
        internal const string Name = "\u00B0";

        public double Conversionfactor
        {
            get
            {
                return Conv;
            }
        }

        public string ShortName
        {
            get
            {
                return Name;
            }
        }

        public static Angle operator *(double left, Degrees right)
        {
            return new Angle(left, right);
        }
    }
}
