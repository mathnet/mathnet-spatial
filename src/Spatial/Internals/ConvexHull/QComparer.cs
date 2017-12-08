namespace MathNet.Spatial.Internals
{
    using System;
    using System.Collections.Generic;

    internal class QComparer : IComparer<MutablePoint>
    {
        private Func<MutablePoint, MutablePoint, int> comparer;

        public QComparer(Func<MutablePoint, MutablePoint, int> comparer)
        {
            this.comparer = comparer;
        }

        public int Compare(MutablePoint pt1, MutablePoint pt2)
        {
            return this.comparer.Invoke(pt1, pt2);
        }
    }
}
