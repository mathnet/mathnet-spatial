namespace MathNet.Spatial.Internals
{
    using System;
    using System.Linq;

    internal class QuadrantSpecific2 : Quadrant
    {
        internal QuadrantSpecific2(MutablePoint[] listOfPoint, Func<MutablePoint, MutablePoint, int> comparer)
            : base(listOfPoint, new QComparer(comparer))
        {
        }

        protected override void SetQuadrantLimits()
        {
            MutablePoint firstPoint = this.ListOfPoint.First();

            double leftX = firstPoint.X;
            double leftY = firstPoint.Y;

            double topX = leftX;
            double topY = leftY;

            foreach (var point in this.ListOfPoint)
            {

                if (point.X <= leftX)
                {
                    if (point.X == leftX)
                    {
                        if (point.Y > leftY)
                        {
                            leftY = point.Y;
                        }
                    }
                    else
                    {
                        leftX = point.X;
                        leftY = point.Y;
                    }
                }

                if (point.Y >= topY)
                {
                    if (point.Y == topY)
                    {
                        if (point.X < topX)
                        {
                            topX = point.X;
                        }
                    }
                    else
                    {
                        topX = point.X;
                        topY = point.Y;
                    }
                }
            }

            this.FirstPoint = new MutablePoint(topX, topY);
            this.LastPoint = new MutablePoint(leftX, leftY);
            this.RootPoint = new MutablePoint(topX, leftY);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool IsGoodQuadrantForPoint(MutablePoint pt)
        {
            if (pt.X < this.RootPoint.X && pt.Y > this.RootPoint.Y)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Iterate over each points to see if we can add it has a ConvexHull point.
        /// It is specific by Quadrant to improve efficiency.
        /// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void ProcessPoint(ref MutablePoint point)
        {
            this.CurrentNode = this.Root;
            AvlNode<MutablePoint> currentPrevious = null;
            AvlNode<MutablePoint> currentNext = null;

            while (this.CurrentNode != null)
            {
                //if (CanQuickReject(point, CurrentNode.Item))
                //{
                //	return false;
                //}

                var insertionSide = Side.Unknown;
                if (point.X > this.CurrentNode.Item.X)
                {
                    if (this.CurrentNode.Left != null)
                    {
                        this.CurrentNode = this.CurrentNode.Left;
                        continue;
                    }

                    currentPrevious = this.CurrentNode.GetPreviousNode();
                    if (CanQuickReject(ref point, ref currentPrevious.Item))
                    {
                        return;
                    }

                    if (!this.IsPointToTheRightOfOthers(currentPrevious.Item, this.CurrentNode.Item, point))
                    {
                        return;
                    }

                    if (this.CurrentNode.Item == point) // Ensure to have no duplicate
                    {
                        return;
                    }

                    insertionSide = Side.Left;
                }
                else if (point.X < this.CurrentNode.Item.X)
                {
                    if (this.CurrentNode.Right != null)
                    {
                        this.CurrentNode = this.CurrentNode.Right;
                        continue;
                    }

                    currentNext = this.CurrentNode.GetNextNode();
                    if (CanQuickReject(ref point, ref currentNext.Item))
                    {
                        return;
                    }

                    if (!this.IsPointToTheRightOfOthers(this.CurrentNode.Item, currentNext.Item, point))
                    {
                        return;
                    }

                    if (this.CurrentNode.Item == point) // Ensure to have no duplicate
                    {
                        continue;
                    }

                    insertionSide = Side.Right;
                }
                else
                {
                    if (point.Y <= this.CurrentNode.Item.Y)
                    {
                        return; // invalid point
                    }

                    // Replace CurrentNode point with point
                    this.CurrentNode.Item = point;
                    this.InvalidateNeighbors(this.CurrentNode.GetPreviousNode(), this.CurrentNode, this.CurrentNode.GetNextNode());
                    return;
                }

                //We should insert the point

                // Try to optimize and verify if can replace a node instead insertion to minimize tree balancing
                if (insertionSide == Side.Right)
                {
                    currentPrevious = this.CurrentNode.GetPreviousNode();
                    if (currentPrevious != null && !this.IsPointToTheRightOfOthers(currentPrevious.Item, point, this.CurrentNode.Item))
                    {
                        this.CurrentNode.Item = point;
                        this.InvalidateNeighbors(currentPrevious, this.CurrentNode, currentNext);
                        return;
                    }

                    var nextNext = currentNext.GetNextNode();
                    if (nextNext != null && !this.IsPointToTheRightOfOthers(point, nextNext.Item, currentNext.Item))
                    {
                        currentNext.Item = point;
                        this.InvalidateNeighbors(null, currentNext, nextNext);
                        return;
                    }
                }
                else // Left
                {
                    currentNext = this.CurrentNode.GetNextNode();
                    if (currentNext != null && !this.IsPointToTheRightOfOthers(point, currentNext.Item, this.CurrentNode.Item))
                    {
                        this.CurrentNode.Item = point;
                        this.InvalidateNeighbors(currentPrevious, this.CurrentNode, currentNext);
                        return;
                    }

                    var previousPrevious = currentPrevious.GetPreviousNode();
                    if (previousPrevious != null && !this.IsPointToTheRightOfOthers(previousPrevious.Item, point, currentPrevious.Item))
                    {
                        currentPrevious.Item = point;
                        this.InvalidateNeighbors(previousPrevious, currentPrevious, null);
                        return;
                    }
                }

                // Should insert but no invalidation is required. (That's why we need to insert... can't replace an adjacent neightbor)
                AvlNode<MutablePoint> newNode = new AvlNode<MutablePoint>();
                if (insertionSide == Side.Right)
                {
                    newNode.Parent = this.CurrentNode;
                    newNode.Item = point;
                    this.CurrentNode.Right = newNode;
                    this.AddBalance(newNode.Parent, -1);
                }
                else // Left
                {
                    newNode.Parent = this.CurrentNode;
                    newNode.Item = point;
                    this.CurrentNode.Left = newNode;
                    this.AddBalance(newNode.Parent, 1);
                }

                return;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool CanQuickReject(ref MutablePoint pt, ref MutablePoint ptHull)
        {
            return pt.X >= ptHull.X && pt.Y <= ptHull.Y;
        }
    }
}
