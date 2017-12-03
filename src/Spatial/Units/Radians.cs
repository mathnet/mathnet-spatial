namespace MathNet.Spatial.Units
{
    using System;
    using System.ComponentModel;

    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Radians : IAngleUnit
    {
        internal const string Name = "rad";

        public double Conversionfactor => 1.0;

        public string ShortName => Name;

        public static Angle operator *(double left, Radians right)
        {
            return new Angle(left, right);
        }
    }
}
