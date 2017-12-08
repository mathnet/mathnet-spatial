namespace MathNet.Spatial.Internals
{
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class Quadrant : AvlTreeSet<MutablePoint>
    {
        internal enum Side
        {
            Unknown = 0,
            Left = 1,
            Right = 2
        }

        public MutablePoint FirstPoint;
        public MutablePoint LastPoint;
        public MutablePoint RootPoint;

        protected AvlNode<MutablePoint> CurrentNode = null;

        protected MutablePoint[] ListOfPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadrant"/> class.
        /// </summary>
        /// <param name="listOfPoint"></param>
        /// <param name="comparer">Comparer is only used to add the second point (the last point, which is compared against the first one).</param>
        internal Quadrant(MutablePoint[] listOfPoint, IComparer<MutablePoint> comparer)
            : base(comparer)
        {
            ListOfPoint = listOfPoint;
        }

        /// <summary>
        /// Initialize every values needed to extract values that are parts of the convex hull.
        /// This is where the first pass of all values is done the get maximum in every directions (x and y).
        /// </summary>
        protected abstract void SetQuadrantLimits();

        public void Prepare()
        {
            if (!ListOfPoint.Any())
            {
                // There is no points at all. Hey don't try to crash me.
                return;
            }

            // Begin : General Init
            Add(FirstPoint);
            if (FirstPoint.Equals(LastPoint))
            {
                return; // Case where for weird distribution like triangle or diagonal. This quadrant will have no point
            }

            Add(LastPoint);
        }

        /// <summary>
        /// To know if to the right. It is meaninful when p1 is first and p2 is next.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="ptToCheck"></param>
        /// <returns>Equivalent of tracing a line from p1 to p2 and tell if ptToCheck
        ///  is to the right or left of that line taking p1 as reference point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsPointToTheRightOfOthers(MutablePoint p1, MutablePoint p2, MutablePoint ptToCheck)
        {
            return ((p2.X - p1.X) * (ptToCheck.Y - p1.Y)) - ((p2.Y - p1.Y) * (ptToCheck.X - p1.X)) < 0;
        }

        /// <summary>
        /// Tell if should try to add and where. -1 ==> Should not add.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal abstract void ProcessPoint(ref MutablePoint point);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool IsGoodQuadrantForPoint(MutablePoint pt);

        // protected abstract bool CanQuickReject(Point pt, Point ptHull);
        
        /// <summary>
        /// Called after insertion in order to see if the newly added point invalidate one 
        /// or more neighbors and if so, remove it/them from the tree.
        /// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void InvalidateNeighbors(AvlNode<MutablePoint> pointPrevious, AvlNode<MutablePoint> pointNew, AvlNode<MutablePoint> pointNext)
        {
            bool invalidPoint;

            if (pointPrevious != null)
            {
                AvlNode<MutablePoint> previousPrevious = pointPrevious.GetPreviousNode();
                for (;;)
                {
                    if (previousPrevious == null)
                    {
                        break;
                    }

                    invalidPoint = !IsPointToTheRightOfOthers(previousPrevious.Item, pointNew.Item, pointPrevious.Item);
                    if (!invalidPoint)
                    {
                        break;
                    }

                    MutablePoint ptPrevPrev = previousPrevious.Item;
                    RemoveNode(pointPrevious);
                    pointPrevious = this.GetNode(ptPrevPrev);
                    previousPrevious = pointPrevious.GetPreviousNode();
                }
            }

            // Invalidate next(s)
            if (pointNext != null)
            {
                AvlNode<MutablePoint> nextNext = pointNext.GetNextNode();
                for (; ;)
                {
                    if (nextNext == null)
                    {
                        break;
                    }

                    invalidPoint = !IsPointToTheRightOfOthers(pointNew.Item, nextNext.Item, pointNext.Item);
                    if (!invalidPoint)
                    {
                        break;
                    }

                    MutablePoint ptNextNext = nextNext.Item;
                    RemoveNode(pointNext);
                    pointNext = GetNode(ptNextNext);
                    nextNext = pointNext.GetNextNode();
                }
            }
        }
    }
}
