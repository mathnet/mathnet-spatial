namespace MathNet.Spatial.Internals
{
    using System;
    using System.Linq;

    internal class QuadrantSpecific3 : Quadrant
    {
        internal QuadrantSpecific3(MutablePoint[] listOfPoint, Func<MutablePoint, MutablePoint, int> comparer)
            : base(listOfPoint, new QComparer(comparer))
        {
        }

        protected override void SetQuadrantLimits()
        {
            MutablePoint firstPoint = this.ListOfPoint.First();

            double leftX = firstPoint.X;
            double leftY = firstPoint.Y;

            double bottomX = leftX;
            double bottomY = leftY;

            foreach (var point in ListOfPoint)
            {
                if (point.X <= leftX)
                {
                    if (point.X == leftX)
                    {
                        if (point.Y < leftY)
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

                if (point.Y <= bottomY)
                {
                    if (point.Y == bottomY)
                    {
                        if (point.X < bottomX)
                        {
                            bottomX = point.X;
                        }
                    }
                    else
                    {
                        bottomX = point.X;
                        bottomY = point.Y;
                    }
                }
            }

            FirstPoint = new MutablePoint(leftX, leftY);
            LastPoint = new MutablePoint(bottomX, bottomY);
            RootPoint = new MutablePoint(bottomX, leftY);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool IsGoodQuadrantForPoint(MutablePoint pt)
        {
            if (pt.X < this.RootPoint.X && pt.Y < this.RootPoint.Y)
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
            CurrentNode = Root;
            AvlNode<MutablePoint> currentPrevious = null;
            AvlNode<MutablePoint> currentNext = null;

            while (CurrentNode != null)
            {
                //if (CanQuickReject(point, CurrentNode.Item))
                //{
                //	return false;
                //}

                var insertionSide = Side.Unknown;
                if (point.X > CurrentNode.Item.X)
                {
                    if (CurrentNode.Right != null)
                    {
                        CurrentNode = CurrentNode.Right;
                        continue;
                    }
                    currentNext = CurrentNode.GetNextNode();
                    if (CanQuickReject(ref point, ref currentNext.Item))
                    {
                        return;
                    }

                    if (!IsPointToTheRightOfOthers(CurrentNode.Item, currentNext.Item, point))
                    {
                        return;
                    }

                    if (CurrentNode.Item == point) // Ensure to have no duplicate
                    {
                        continue;
                    }

                    insertionSide = Side.Right;
                }
                else if (point.X < CurrentNode.Item.X)
                {
                    if (CurrentNode.Left != null)
                    {
                        CurrentNode = CurrentNode.Left;
                        continue;
                    }

                    currentPrevious = CurrentNode.GetPreviousNode();
                    if (CanQuickReject(ref point, ref currentPrevious.Item))
                    {
                        return;
                    }

                    if (!IsPointToTheRightOfOthers(currentPrevious.Item, CurrentNode.Item, point))
                    {
                        return;
                    }

                    if (CurrentNode.Item == point) // Ensure to have no duplicate
                    {
                        return;
                    }

                    insertionSide = Side.Left;
                }
                else
                {
                    if (point.Y >= CurrentNode.Item.Y)
                    {
                        return; // invalid point
                    }

                    // Replace CurrentNode point with point
                    CurrentNode.Item = point;
                    InvalidateNeighbors(CurrentNode.GetPreviousNode(), CurrentNode, CurrentNode.GetNextNode());
                    return;
                }

                //We should insert the point

                // Try to optimize and verify if can replace a node instead insertion to minimize tree balancing
                if (insertionSide == Side.Right)
                {
                    currentPrevious = CurrentNode.GetPreviousNode();
                    if (currentPrevious != null && !IsPointToTheRightOfOthers(currentPrevious.Item, point, CurrentNode.Item))
                    {
                        CurrentNode.Item = point;
                        InvalidateNeighbors(currentPrevious, CurrentNode, currentNext);
                        return;
                    }

                    var nextNext = currentNext.GetNextNode();
                    if (nextNext != null && !IsPointToTheRightOfOthers(point, nextNext.Item, currentNext.Item))
                    {
                        currentNext.Item = point;
                        InvalidateNeighbors(null, currentNext, nextNext);
                        return;
                    }
                }
                else // Left
                {
                    currentNext = CurrentNode.GetNextNode();
                    if (currentNext != null && !IsPointToTheRightOfOthers(point, currentNext.Item, CurrentNode.Item))
                    {
                        CurrentNode.Item = point;
                        InvalidateNeighbors(currentPrevious, CurrentNode, currentNext);
                        return;
                    }

                    var previousPrevious = currentPrevious.GetPreviousNode();
                    if (previousPrevious != null && !IsPointToTheRightOfOthers(previousPrevious.Item, point, currentPrevious.Item))
                    {
                        currentPrevious.Item = point;
                        InvalidateNeighbors(previousPrevious, currentPrevious, null);
                        return;
                    }
                }

                // Should insert but no invalidation is required. (That's why we need to insert... can't replace an adjacent neightbor)
                AvlNode<MutablePoint> newNode = new AvlNode<MutablePoint>();
                if (insertionSide == Side.Right)
                {
                    newNode.Parent = CurrentNode;
                    newNode.Item = point;
                    CurrentNode.Right = newNode;
                    this.AddBalance(newNode.Parent, -1);
                }
                else // Left
                {
                    newNode.Parent = CurrentNode;
                    newNode.Item = point;
                    CurrentNode.Left = newNode;
                    this.AddBalance(newNode.Parent, 1);
                }

                return;
            }

            return;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool CanQuickReject(ref MutablePoint pt, ref MutablePoint ptHull)
        {
            return pt.X >= ptHull.X && pt.Y >= ptHull.Y;
        }
    }
}
