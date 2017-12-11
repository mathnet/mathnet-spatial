namespace MathNet.Spatial.Internals
{
    using System;
    using System.Collections.Generic;

    internal class QComparer : IComparer<MutablePoint>
    {
        private readonly Func<MutablePoint, MutablePoint, int> comparer;

        public QComparer(Func<MutablePoint, MutablePoint, int> comparer)
        {
            this.comparer = comparer;
        }

        public int Compare(MutablePoint pt1, MutablePoint pt2)
        {
            return this.comparer(pt1, pt2);
        }
    }
}
