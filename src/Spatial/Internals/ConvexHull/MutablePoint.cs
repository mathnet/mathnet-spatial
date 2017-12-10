namespace MathNet.Spatial.Internals
{
    using System;

    internal struct MutablePoint : IEquatable<MutablePoint>
    {
        internal MutablePoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        internal double X { get; set; }

        internal double Y { get; set; }

        public static bool operator ==(MutablePoint p1, MutablePoint p2) => p1.Equals(p2);

        public static bool operator !=(MutablePoint p1, MutablePoint p2) => !p1.Equals(p2);

        public bool Equals(MutablePoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
    }
}
