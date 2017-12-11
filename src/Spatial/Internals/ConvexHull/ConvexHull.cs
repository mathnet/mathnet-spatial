/// <summary>
/// Based upon the work of Lui, Chen and Ouellet - https://www.codeproject.com/Articles/1210225/Fast-and-improved-D-Convex-Hull-algorithm-and-its
/// </summary>
namespace MathNet.Spatial.Internals
{
    using MathNet.Spatial.Euclidean;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class ConvexHull
    {
        private struct Limit
        {
            internal MutablePoint Q1Top, Q2Top, Q2Left, Q3Left, Q3Bottom, Q4Bottom, Q4Right, Q1Right;

            internal Limit(MutablePoint pt)
            {
                this.Q1Top = pt;
                this.Q2Top = pt;
                this.Q2Left = pt;
                this.Q3Left = pt;
                this.Q3Bottom = pt;
                this.Q4Bottom = pt;
                this.Q4Right = pt;
                this.Q1Right = pt;
            }

            internal Limit Copy()
            {
                Limit limit = new Limit
                {
                    Q1Top = this.Q1Top,
                    Q2Top = this.Q2Top,
                    Q2Left = this.Q2Left,
                    Q3Left = this.Q3Left,
                    Q3Bottom = this.Q3Bottom,
                    Q4Bottom = this.Q4Bottom,
                    Q4Right = this.Q4Right,
                    Q1Right = this.Q1Right
                };

                return limit;
            }
        }


        // Quadrant: Q2 | Q1
        //	         -------
        //           Q3 | Q4

        private Quadrant _q1;
        private Quadrant _q2;
        private Quadrant _q3;
        private Quadrant _q4;

        private MutablePoint[] _listOfPoint;
        private bool _shouldCloseTheGraph;

        // ******************************************************************
        public ConvexHull(IEnumerable<Point2D> listOfPoint, bool shouldCloseTheGraph = true, int initialResultGuessSize = 0)
        {
            List<MutablePoint> l = new List<MutablePoint>();
            foreach (var point in listOfPoint)
            {
                MutablePoint p = new MutablePoint(point.X, point.Y);
                l.Add(p);
            }

            this.Init(l.ToArray(), shouldCloseTheGraph);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsPointToTheRightOfOthers(MutablePoint p1, MutablePoint p2, MutablePoint ptToCheck)
        {
            return ((p2.X - p1.X) * (ptToCheck.Y - p1.Y)) - ((p2.Y - p1.Y) * (ptToCheck.X - p1.X)) < 0;
        }

        private void Init(MutablePoint[] listOfPoint, bool shouldCloseTheGraph)
        {
            this._listOfPoint = listOfPoint;
            this._shouldCloseTheGraph = shouldCloseTheGraph;

            this._q1 = new QuadrantSpecific1(this._listOfPoint, (a, b) => (a.X > b.X) ? -1 : (a.X < b.X) ? 1 : 0);
            this._q2 = new QuadrantSpecific2(this._listOfPoint, (a, b) => (a.X > b.X) ? -1 : (a.X < b.X) ? 1 : 0);
            this._q3 = new QuadrantSpecific3(this._listOfPoint, (a, b) => (a.X < b.X) ? -1 : (a.X > b.X) ? 1 : 0);
            this._q4 = new QuadrantSpecific4(this._listOfPoint, (a, b) => (a.X < b.X) ? -1 : (a.X > b.X) ? 1 : 0);
        }

        private bool IsQuadrantAreDisjoint()
        {
            if (IsPointToTheRightOfOthers(this._q1.FirstPoint, this._q1.LastPoint, this._q3.RootPoint))
            {
                return false;
            }

            if (IsPointToTheRightOfOthers(this._q2.FirstPoint, this._q2.LastPoint, this._q4.RootPoint))
            {
                return false;
            }

            if (IsPointToTheRightOfOthers(this._q3.FirstPoint, this._q3.LastPoint, this._q1.RootPoint))
            {
                return false;
            }

            if (IsPointToTheRightOfOthers(this._q4.FirstPoint, this._q4.LastPoint, this._q2.RootPoint))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadUsage">Using ConvexHullThreadUsage.All will only use all thread for the first pass (se quadrant limits) then use only 4 threads for pass 2 (which is the actual limit).</param>
        public void CalcConvexHull()
        {
            if (this.IsZeroData())
            {
                return;
            }

            this.SetQuadrantLimitsOneThread();

            this._q1.Prepare();
            this._q2.Prepare();
            this._q3.Prepare();
            this._q4.Prepare();

            MutablePoint q1Root = this._q1.RootPoint;
            MutablePoint q2Root = this._q2.RootPoint;
            MutablePoint q3Root = this._q3.RootPoint;
            MutablePoint q4Root = this._q4.RootPoint;

            // Main Loop to extract ConvexHullPoints
            MutablePoint[] points = this._listOfPoint;
            int index = 0;
            int pointCount = points.Length;

            if (points != null)
            {
                MutablePoint point;

                if (this.IsQuadrantAreDisjoint())
                {
                    Q1First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            this._q1.ProcessPoint(ref point);
                            goto Q1First;
                        }

                        if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            this._q2.ProcessPoint(ref point);
                            goto Q2First;
                        }

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            this._q3.ProcessPoint(ref point);
                            goto Q3First;
                        }

                        if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            this._q4.ProcessPoint(ref point);
                            goto Q4First;
                        }

                        goto Q1First;
                    }
                    else
                    {
                        goto End;
                    }

                    Q2First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            this._q2.ProcessPoint(ref point);
                            goto Q2First;
                        }

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            this._q3.ProcessPoint(ref point);
                            goto Q3First;
                        }

                        if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            this._q4.ProcessPoint(ref point);
                            goto Q4First;
                        }

                        if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            this._q1.ProcessPoint(ref point);
                            goto Q1First;
                        }

                        goto Q2First;
                    }
                    else
                    {
                        goto End;
                    }

                    Q3First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            this._q3.ProcessPoint(ref point);
                            goto Q3First;
                        }

                        if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            this._q4.ProcessPoint(ref point);
                            goto Q4First;
                        }

                        if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            this._q1.ProcessPoint(ref point);
                            goto Q1First;
                        }

                        if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            this._q2.ProcessPoint(ref point);
                            goto Q2First;
                        }

                        goto Q3First;
                    }
                    else
                    {
                        goto End;
                    }

                    Q4First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            this._q4.ProcessPoint(ref point);
                            goto Q4First;
                        }

                        if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            this._q1.ProcessPoint(ref point);
                            goto Q1First;
                        }

                        if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            this._q2.ProcessPoint(ref point);
                            goto Q2First;
                        }

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            this._q3.ProcessPoint(ref point);
                            goto Q3First;
                        }

                        goto Q4First;
                    }
                    else
                    {
                        goto End;
                    }
                }
                else  // Not disjoint ***********************************************************************************
                {
                    Q1First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q1.FirstPoint, this._q1.LastPoint, point))
                            {
                                this._q1.ProcessPoint(ref point);
                                goto Q1First;
                            }

                            if (point.X < q3Root.X && point.Y < q3Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q3.FirstPoint, this._q3.LastPoint, point))
                                {
                                    this._q3.ProcessPoint(ref point);
                                }

                                goto Q3First;
                            }

                            goto Q1First;
                        }

                        if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q2.FirstPoint, this._q2.LastPoint, point))
                            {
                                this._q2.ProcessPoint(ref point);
                                goto Q2First;
                            }

                            if (point.X > q4Root.X && point.Y < q4Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q4.FirstPoint, this._q4.LastPoint, point))
                                {
                                    this._q4.ProcessPoint(ref point);
                                }

                                goto Q4First;
                            }

                            goto Q2First;
                        }

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q3.FirstPoint, this._q3.LastPoint, point))
                            {
                                this._q3.ProcessPoint(ref point);
                            }

                            goto Q3First;
                        }
                        else if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q4.FirstPoint, this._q4.LastPoint, point))
                            {
                                this._q4.ProcessPoint(ref point);
                            }

                            goto Q4First;
                        }

                        goto Q1First;
                    }
                    else
                    {
                        goto End;
                    }

                    Q2First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q2.FirstPoint, this._q2.LastPoint, point))
                            {
                                this._q2.ProcessPoint(ref point);
                                goto Q2First;
                            }

                            if (point.X > q4Root.X && point.Y < q4Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q4.FirstPoint, this._q4.LastPoint, point))
                                {
                                    this._q4.ProcessPoint(ref point);
                                }

                                goto Q4First;
                            }

                            goto Q2First;
                        }

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q3.FirstPoint, this._q3.LastPoint, point))
                            {
                                this._q3.ProcessPoint(ref point);
                                goto Q3First;
                            }

                            if (point.X > q1Root.X && point.Y > q1Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q1.FirstPoint, this._q1.LastPoint, point))
                                {
                                    this._q1.ProcessPoint(ref point);
                                }

                                goto Q1First;
                            }

                            goto Q3First;
                        }

                        if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q4.FirstPoint, this._q4.LastPoint, point))
                            {
                                this._q4.ProcessPoint(ref point);
                            }

                            goto Q4First;
                        }
                        else if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q1.FirstPoint, this._q1.LastPoint, point))
                            {
                                this._q1.ProcessPoint(ref point);
                            }

                            goto Q1First;
                        }

                        goto Q2First;
                    }
                    else
                    {
                        goto End;
                    }

                    Q3First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q3.FirstPoint, this._q3.LastPoint, point))
                            {
                                this._q3.ProcessPoint(ref point);
                                goto Q3First;
                            }

                            if (point.X > q1Root.X && point.Y > q1Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q1.FirstPoint, this._q1.LastPoint, point))
                                {
                                    this._q1.ProcessPoint(ref point);
                                }

                                goto Q1First;
                            }

                            goto Q3First;
                        }

                        if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q4.FirstPoint, this._q4.LastPoint, point))
                            {
                                this._q4.ProcessPoint(ref point);
                                goto Q4First;
                            }

                            if (point.X < q2Root.X && point.Y > q2Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q2.FirstPoint, this._q2.LastPoint, point))
                                {
                                    this._q2.ProcessPoint(ref point);
                                }

                                goto Q2First;
                            }

                            goto Q4First;
                        }

                        if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q1.FirstPoint, this._q1.LastPoint, point))
                            {
                                this._q1.ProcessPoint(ref point);
                                goto Q1First;
                            }
                        }
                        else if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q2.FirstPoint, this._q2.LastPoint, point))
                            {
                                this._q2.ProcessPoint(ref point);
                                goto Q2First;
                            }
                        }

                        goto Q3First;
                    }
                    else
                    {
                        goto End;
                    }

                    Q4First:
                    if (index < pointCount)
                    {
                        point = points[index++];

                        if (point.X > q4Root.X && point.Y < q4Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q4.FirstPoint, this._q4.LastPoint, point))
                            {
                                this._q4.ProcessPoint(ref point);
                                goto Q4First;
                            }

                            if (point.X < q2Root.X && point.Y > q2Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q2.FirstPoint, this._q2.LastPoint, point))
                                {
                                    this._q2.ProcessPoint(ref point);
                                }

                                goto Q2First;
                            }

                            goto Q4First;
                        }

                        if (point.X > q1Root.X && point.Y > q1Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q1.FirstPoint, this._q1.LastPoint, point))
                            {
                                this._q1.ProcessPoint(ref point);
                                goto Q1First;
                            }

                            if (point.X < q3Root.X && point.Y < q3Root.Y)
                            {
                                if (IsPointToTheRightOfOthers(this._q3.FirstPoint, this._q3.LastPoint, point))
                                {
                                    this._q3.ProcessPoint(ref point);
                                }

                                goto Q3First;
                            }

                            goto Q1First;
                        }

                        if (point.X < q3Root.X && point.Y < q3Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q3.FirstPoint, this._q3.LastPoint, point))
                            {
                                this._q3.ProcessPoint(ref point);
                                goto Q3First;
                            }
                        }

                        if (point.X < q2Root.X && point.Y > q2Root.Y)
                        {
                            if (IsPointToTheRightOfOthers(this._q2.FirstPoint, this._q2.LastPoint, point))
                            {
                                this._q2.ProcessPoint(ref point);
                                goto Q2First;
                            }
                        }

                        goto Q4First;
                    }
                    else
                    {
                        goto End;
                    }
                }

                End:
                { }
            }
        }

        private void SetQuadrantLimitsOneThread()
        {
            MutablePoint ptFirst = this._listOfPoint.First();

            // Find the quadrant limits (maximum x and y)

            double right, topLeft, topRight, left, bottomLeft, bottomRight;
            right = topLeft = topRight = left = bottomLeft = bottomRight = ptFirst.X;

            double top, rightTop, rightBottom, bottom, leftTop, leftBottom;
            top = rightTop = rightBottom = bottom = leftTop = leftBottom = ptFirst.Y;

            foreach (MutablePoint pt in this._listOfPoint)
            {
                if (pt.X >= right)
                {
                    if (pt.X == right)
                    {
                        if (pt.Y > rightTop)
                        {
                            rightTop = pt.Y;
                        }
                        else
                        {
                            if (pt.Y < rightBottom)
                            {
                                rightBottom = pt.Y;
                            }
                        }
                    }
                    else
                    {
                        right = pt.X;
                        rightTop = rightBottom = pt.Y;
                    }
                }

                if (pt.X <= left)
                {
                    if (pt.X == left)
                    {
                        if (pt.Y > leftTop)
                        {
                            leftTop = pt.Y;
                        }
                        else
                        {
                            if (pt.Y < leftBottom)
                            {
                                leftBottom = pt.Y;
                            }
                        }
                    }
                    else
                    {
                        left = pt.X;
                        leftBottom = leftTop = pt.Y;
                    }
                }

                if (pt.Y >= top)
                {
                    if (pt.Y == top)
                    {
                        if (pt.X < topLeft)
                        {
                            topLeft = pt.X;
                        }
                        else
                        {
                            if (pt.X > topRight)
                            {
                                topRight = pt.X;
                            }
                        }
                    }
                    else
                    {
                        top = pt.Y;
                        topLeft = topRight = pt.X;
                    }
                }

                if (pt.Y <= bottom)
                {
                    if (pt.Y == bottom)
                    {
                        if (pt.X < bottomLeft)
                        {
                            bottomLeft = pt.X;
                        }
                        else
                        {
                            if (pt.X > bottomRight)
                            {
                                bottomRight = pt.X;
                            }
                        }
                    }
                    else
                    {
                        bottom = pt.Y;
                        bottomRight = bottomLeft = pt.X;
                    }
                }

                this._q1.FirstPoint = new MutablePoint(right, rightTop);
                this._q1.LastPoint = new MutablePoint(topRight, top);
                this._q1.RootPoint = new MutablePoint(topRight, rightTop);

                this._q2.FirstPoint = new MutablePoint(topLeft, top);
                this._q2.LastPoint = new MutablePoint(left, leftTop);
                this._q2.RootPoint = new MutablePoint(topLeft, leftTop);

                this._q3.FirstPoint = new MutablePoint(left, leftBottom);
                this._q3.LastPoint = new MutablePoint(bottomLeft, bottom);
                this._q3.RootPoint = new MutablePoint(bottomLeft, leftBottom);

                this._q4.FirstPoint = new MutablePoint(bottomRight, bottom);
                this._q4.LastPoint = new MutablePoint(right, rightBottom);
                this._q4.RootPoint = new MutablePoint(bottomRight, rightBottom);

            }
        }

        private Limit _limit = default(Limit);

        /*
        // ******************************************************************
        // For usage of Parallel func, I highly suggest: Stephen Toub: Patterns of parallel programming ==> Just Awsome !!!
        // But its only my own fault if I'm not using it at its full potential...
        private void SetQuadrantLimitsUsingAllThreads()
        {
            MutablePoint pt = this._listOfPoint.First();
            _limit = new Limit(pt);

            int coreCount = Environment.ProcessorCount;

            Task[] tasks = new Task[coreCount];
            for (int n = 0; n < tasks.Length; n++)
            {
                int nLocal = n; // Prevent Lambda internal closure error.
                tasks[n] = Task.Factory.StartNew(() =>
                {
                    Limit limit = _limit.Copy();
                    FindLimits(_listOfPoint, nLocal, coreCount, limit);
                    AggregateLimits(limit);
                });
            }
            Task.WaitAll(tasks);

            _q1.FirstPoint = _limit.Q1Right;
            _q1.LastPoint = _limit.Q1Top;
            _q2.FirstPoint = _limit.Q2Top;
            _q2.LastPoint = _limit.Q2Left;
            _q3.FirstPoint = _limit.Q3Left;
            _q3.LastPoint = _limit.Q3Bottom;
            _q4.FirstPoint = _limit.Q4Bottom;
            _q4.LastPoint = _limit.Q4Right;

            _q1.RootPoint = new MutablePoint(_q1.LastPoint.X, _q1.FirstPoint.Y);
            _q2.RootPoint = new MutablePoint(_q2.FirstPoint.X, _q2.LastPoint.Y);
            _q3.RootPoint = new MutablePoint(_q3.LastPoint.X, _q3.FirstPoint.Y);
            _q4.RootPoint = new MutablePoint(_q4.FirstPoint.X, _q4.LastPoint.Y);
        }
        */

        private Limit FindLimits(MutablePoint[] listOfPoint, int start, int offset, Limit limit)
        {
            for (int index = start; index < listOfPoint.Length; index += offset)
            {
                MutablePoint pt = listOfPoint[index];

                double x = pt.X;
                double y = pt.Y;

                // Top
                if (y >= limit.Q2Top.Y)
                {
                    if (y == limit.Q2Top.Y) // Special
                    {
                        if (y == limit.Q1Top.Y)
                        {
                            if (x < limit.Q2Top.X)
                            {
                                limit.Q2Top.X = x;
                            }
                            else if (x > limit.Q1Top.X)
                            {
                                limit.Q1Top.X = x;
                            }
                        }
                        else
                        {
                            if (x < limit.Q2Top.X)
                            {
                                limit.Q1Top.X = limit.Q2Top.X;
                                limit.Q1Top.Y = limit.Q2Top.Y;

                                limit.Q2Top.X = x;
                            }
                            else if (x > limit.Q1Top.X)
                            {
                                limit.Q1Top.X = x;
                                limit.Q1Top.Y = y;
                            }
                        }
                    }
                    else
                    {
                        limit.Q2Top.X = x;
                        limit.Q2Top.Y = y;
                    }
                }

                // Bottom
                if (y <= limit.Q3Bottom.Y)
                {
                    if (y == limit.Q3Bottom.Y) // Special
                    {
                        if (y == limit.Q4Bottom.Y)
                        {
                            if (x < limit.Q3Bottom.X)
                            {
                                limit.Q3Bottom.X = x;
                            }
                            else if (x > limit.Q4Bottom.X)
                            {
                                limit.Q4Bottom.X = x;
                            }
                        }
                        else
                        {
                            if (x < limit.Q3Bottom.X)
                            {
                                limit.Q4Bottom.X = limit.Q3Bottom.X;
                                limit.Q4Bottom.Y = limit.Q3Bottom.Y;

                                limit.Q3Bottom.X = x;
                            }
                            else if (x > limit.Q3Bottom.X)
                            {
                                limit.Q4Bottom.X = x;
                                limit.Q4Bottom.Y = y;
                            }
                        }
                    }
                    else
                    {
                        limit.Q3Bottom.X = x;
                        limit.Q3Bottom.Y = y;
                    }
                }

                // Right
                if (x >= limit.Q4Right.X)
                {
                    if (x == limit.Q4Right.X) // Special
                    {
                        if (x == limit.Q1Right.X)
                        {
                            if (y < limit.Q4Right.Y)
                            {
                                limit.Q4Right.Y = y;
                            }
                            else if (y > limit.Q1Right.Y)
                            {
                                limit.Q1Right.Y = y;
                            }
                        }
                        else
                        {
                            if (y < limit.Q4Right.Y)
                            {
                                limit.Q1Right.X = limit.Q4Right.X;
                                limit.Q1Right.Y = limit.Q4Right.Y;

                                limit.Q4Right.Y = y;
                            }
                            else if (y > limit.Q1Right.Y)
                            {
                                limit.Q1Right.X = x;
                                limit.Q1Right.Y = y;
                            }
                        }
                    }
                    else
                    {
                        limit.Q4Right.X = x;
                        limit.Q4Right.Y = y;
                    }
                }

                // Left
                if (x <= limit.Q3Left.X)
                {
                    if (x == limit.Q3Left.X) // Special
                    {
                        if (x == limit.Q2Left.X)
                        {
                            if (y < limit.Q3Left.Y)
                            {
                                limit.Q3Left.Y = y;
                            }
                            else if (y > limit.Q2Left.Y)
                            {
                                limit.Q2Left.Y = y;
                            }
                        }
                        else
                        {
                            if (y < limit.Q3Left.Y)
                            {
                                limit.Q2Left.X = limit.Q3Left.X;
                                limit.Q2Left.Y = limit.Q3Left.Y;

                                limit.Q3Left.Y = y;
                            }
                            else if (y > limit.Q2Left.Y)
                            {
                                limit.Q2Left.X = x;
                                limit.Q2Left.Y = y;
                            }
                        }
                    }
                    else
                    {
                        limit.Q3Left.X = x;
                        limit.Q3Left.Y = y;
                    }
                }

                if (limit.Q2Left.X != limit.Q3Left.X)
                {
                    limit.Q2Left.X = limit.Q3Left.X;
                    limit.Q2Left.Y = limit.Q3Left.Y;
                }

                if (limit.Q1Right.X != limit.Q4Right.X)
                {
                    limit.Q1Right.X = limit.Q4Right.X;
                    limit.Q1Right.Y = limit.Q4Right.Y;
                }

                if (limit.Q1Top.Y != limit.Q2Top.Y)
                {
                    limit.Q1Top.X = limit.Q2Top.X;
                    limit.Q1Top.Y = limit.Q2Top.Y;
                }

                if (limit.Q4Bottom.Y != limit.Q3Bottom.Y)
                {
                    limit.Q4Bottom.X = limit.Q3Bottom.X;
                    limit.Q4Bottom.Y = limit.Q3Bottom.Y;
                }
            }

            return limit;
        }

        private Limit FindLimits(MutablePoint pt, ParallelLoopState state, Limit limit)
        {
            double x = pt.X;
            double y = pt.Y;

            // Top
            if (y >= limit.Q2Top.Y)
            {
                if (y == limit.Q2Top.Y) // Special
                {
                    if (y == limit.Q1Top.Y)
                    {
                        if (x < limit.Q2Top.X)
                        {
                            limit.Q2Top.X = x;
                        }
                        else if (x > limit.Q1Top.X)
                        {
                            limit.Q1Top.X = x;
                        }
                    }
                    else
                    {
                        if (x < limit.Q2Top.X)
                        {
                            limit.Q1Top.X = limit.Q2Top.X;
                            limit.Q1Top.Y = limit.Q2Top.Y;

                            limit.Q2Top.X = x;
                        }
                        else if (x > limit.Q1Top.X)
                        {
                            limit.Q1Top.X = x;
                            limit.Q1Top.Y = y;
                        }
                    }
                }
                else
                {
                    limit.Q2Top.X = x;
                    limit.Q2Top.Y = y;
                }
            }

            // Bottom
            if (y <= limit.Q3Bottom.Y)
            {
                if (y == limit.Q3Bottom.Y) // Special
                {
                    if (y == limit.Q4Bottom.Y)
                    {
                        if (x < limit.Q3Bottom.X)
                        {
                            limit.Q3Bottom.X = x;
                        }
                        else if (x > limit.Q4Bottom.X)
                        {
                            limit.Q4Bottom.X = x;
                        }
                    }
                    else
                    {
                        if (x < limit.Q3Bottom.X)
                        {
                            limit.Q4Bottom.X = limit.Q3Bottom.X;
                            limit.Q4Bottom.Y = limit.Q3Bottom.Y;

                            limit.Q3Bottom.X = x;
                        }
                        else if (x > limit.Q3Bottom.X)
                        {
                            limit.Q4Bottom.X = x;
                            limit.Q4Bottom.Y = y;
                        }
                    }
                }
                else
                {
                    limit.Q3Bottom.X = x;
                    limit.Q3Bottom.Y = y;
                }
            }

            // Right
            if (x >= limit.Q4Right.X)
            {
                if (x == limit.Q4Right.X) // Special
                {
                    if (x == limit.Q1Right.X)
                    {
                        if (y < limit.Q4Right.Y)
                        {
                            limit.Q4Right.Y = y;
                        }
                        else if (y > limit.Q1Right.Y)
                        {
                            limit.Q1Right.Y = y;
                        }
                    }
                    else
                    {
                        if (y < limit.Q4Right.Y)
                        {
                            limit.Q1Right.X = limit.Q4Right.X;
                            limit.Q1Right.Y = limit.Q4Right.Y;

                            limit.Q4Right.Y = y;
                        }
                        else if (y > limit.Q1Right.Y)
                        {
                            limit.Q1Right.X = x;
                            limit.Q1Right.Y = y;
                        }
                    }
                }
                else
                {
                    limit.Q4Right.X = x;
                    limit.Q4Right.Y = y;
                }
            }

            // Left
            if (x <= limit.Q3Left.X)
            {
                if (x == limit.Q3Left.X) // Special
                {
                    if (x == limit.Q2Left.X)
                    {
                        if (y < limit.Q3Left.Y)
                        {
                            limit.Q3Left.Y = y;
                        }
                        else if (y > limit.Q2Left.Y)
                        {
                            limit.Q2Left.Y = y;
                        }
                    }
                    else
                    {
                        if (y < limit.Q3Left.Y)
                        {
                            limit.Q2Left.X = limit.Q3Left.X;
                            limit.Q2Left.Y = limit.Q3Left.Y;

                            limit.Q3Left.Y = y;
                        }
                        else if (y > limit.Q2Left.Y)
                        {
                            limit.Q2Left.X = x;
                            limit.Q2Left.Y = y;
                        }
                    }
                }
                else
                {
                    limit.Q3Left.X = x;
                    limit.Q3Left.Y = y;
                }
            }

            if (limit.Q2Left.X != limit.Q3Left.X)
            {
                limit.Q2Left.X = limit.Q3Left.X;
                limit.Q2Left.Y = limit.Q3Left.Y;
            }

            if (limit.Q1Right.X != limit.Q4Right.X)
            {
                limit.Q1Right.X = limit.Q4Right.X;
                limit.Q1Right.Y = limit.Q4Right.Y;
            }

            if (limit.Q1Top.Y != limit.Q2Top.Y)
            {
                limit.Q1Top.X = limit.Q2Top.X;
                limit.Q1Top.Y = limit.Q2Top.Y;
            }

            if (limit.Q4Bottom.Y != limit.Q3Bottom.Y)
            {
                limit.Q4Bottom.X = limit.Q3Bottom.X;
                limit.Q4Bottom.Y = limit.Q3Bottom.Y;
            }

            return limit;
        }

        private object _findLimitFinalLock = new object();

        private void AggregateLimits(Limit limit)
        {
            lock (this._findLimitFinalLock)
            {
                if (limit.Q1Right.X >= this._limit.Q1Right.X)
                {
                    if (limit.Q1Right.X == this._limit.Q1Right.X)
                    {
                        if (limit.Q1Right.Y > this._limit.Q1Right.Y)
                        {
                            this._limit.Q1Right = limit.Q1Right;
                        }
                    }
                    else
                    {
                        this._limit.Q1Right = limit.Q1Right;
                    }
                }

                if (limit.Q4Right.X > this._limit.Q4Right.X)
                {
                    if (limit.Q4Right.X == this._limit.Q4Right.X)
                    {
                        if (limit.Q4Right.Y < this._limit.Q4Right.Y)
                        {
                            this._limit.Q4Right = limit.Q4Right;
                        }
                    }
                    else
                    {
                        this._limit.Q4Right = limit.Q4Right;
                    }
                }

                if (limit.Q2Left.X < this._limit.Q2Left.X)
                {
                    if (limit.Q2Left.X == this._limit.Q2Left.X)
                    {
                        if (limit.Q2Left.Y > this._limit.Q2Left.Y)
                        {
                            this._limit.Q2Left = limit.Q2Left;
                        }
                    }
                    else
                    {
                        this._limit.Q2Left = limit.Q2Left;
                    }
                }

                if (limit.Q3Left.X < this._limit.Q3Left.X)
                {
                    if (limit.Q3Left.X == this._limit.Q3Left.X)
                    {
                        if (limit.Q3Left.Y > this._limit.Q3Left.Y)
                        {
                            this._limit.Q3Left = limit.Q3Left;
                        }
                    }
                    else
                    {
                        this._limit.Q3Left = limit.Q3Left;
                    }
                }

                if (limit.Q1Top.Y > this._limit.Q1Top.Y)
                {
                    if (limit.Q1Top.Y == this._limit.Q1Top.Y)
                    {
                        if (limit.Q1Top.X > this._limit.Q1Top.X)
                        {
                            this._limit.Q1Top = limit.Q1Top;
                        }
                    }
                    else
                    {
                        this._limit.Q1Top = limit.Q1Top;
                    }
                }

                if (limit.Q2Top.Y > this._limit.Q2Top.Y)
                {
                    if (limit.Q2Top.Y == this._limit.Q2Top.Y)
                    {
                        if (limit.Q2Top.X < this._limit.Q2Top.X)
                        {
                            this._limit.Q2Top = limit.Q2Top;
                        }
                    }
                    else
                    {
                        this._limit.Q2Top = limit.Q2Top;
                    }
                }

                if (limit.Q3Bottom.Y < this._limit.Q3Bottom.Y)
                {
                    if (limit.Q3Bottom.Y == this._limit.Q3Bottom.Y)
                    {
                        if (limit.Q3Bottom.X < this._limit.Q3Bottom.X)
                        {
                            this._limit.Q3Bottom = limit.Q3Bottom;
                        }
                    }
                    else
                    {
                        this._limit.Q3Bottom = limit.Q3Bottom;
                    }
                }

                if (limit.Q4Bottom.Y < this._limit.Q4Bottom.Y)
                {
                    if (limit.Q4Bottom.Y == this._limit.Q4Bottom.Y)
                    {
                        if (limit.Q4Bottom.X > this._limit.Q4Bottom.X)
                        {
                            this._limit.Q4Bottom = limit.Q4Bottom;
                        }
                    }
                    else
                    {
                        this._limit.Q4Bottom = limit.Q4Bottom;
                    }
                }
            }
        }

        public Point2D[] GetResultsAsArrayOfPoint()
        {
            if (this._listOfPoint == null || !this._listOfPoint.Any())
            {
                return new Point2D[0];
            }

            int countOfPoints = this._q1.Count + this._q2.Count + this._q3.Count + this._q4.Count;

            if (this._q1.LastPoint == this._q2.FirstPoint)
            {
                countOfPoints--;
            }

            if (this._q2.LastPoint == this._q3.FirstPoint)
            {
                countOfPoints--;
            }

            if (this._q3.LastPoint == this._q4.FirstPoint)
            {
                countOfPoints--;
            }

            if (this._q4.LastPoint == this._q1.FirstPoint)
            {
                countOfPoints--;
            }

            if (countOfPoints == 0) // Case where there is only one point
            {
                return new Point2D[] { new Point2D(this._q1.FirstPoint.X, this._q1.FirstPoint.Y) };
            }

            if (this._shouldCloseTheGraph)
            {
                countOfPoints++;
            }

            Point2D[] results = new Point2D[countOfPoints];

            int resultIndex = -1;

            if (this._q1.FirstPoint != this._q4.LastPoint)
            {
                foreach (MutablePoint pt in this._q1)
                {
                    results[++resultIndex] = new Point2D(pt.X, pt.Y);
                }
            }
            else
            {
                var enumerator = this._q1.GetEnumerator();
                enumerator.Reset();
                if (enumerator.MoveNext())
                {
                    // Skip first (same as the last one as quadrant 4

                    while (enumerator.MoveNext())
                    {
                        results[++resultIndex] = new Point2D(enumerator.Current.X, enumerator.Current.Y);
                    }
                }
            }

            if (this._q2.Count == 1)
            {
                if (this._q2.FirstPoint != this._q1.LastPoint)
                {
                    results[++resultIndex] = new Point2D(this._q2.FirstPoint.X, this._q2.FirstPoint.Y);
                }
            }
            else
            {
                var enumerator = this._q2.GetEnumerator();
                enumerator.Reset();
                if (enumerator.MoveNext()) // Will always be true
                {
                    if (enumerator.Current != this._q1.LastPoint)
                    {
                        results[++resultIndex] = new Point2D(enumerator.Current.X, enumerator.Current.Y);
                    }

                    while (enumerator.MoveNext())
                    {
                        results[++resultIndex] = new Point2D(enumerator.Current.X, enumerator.Current.Y);
                    }
                }
            }

            if (this._q3.Count == 1)
            {
                if (this._q3.FirstPoint != this._q2.LastPoint)
                {
                    results[++resultIndex] = new Point2D(this._q3.FirstPoint.X, this._q3.FirstPoint.Y);
                }
            }
            else
            {
                var enumerator = this._q3.GetEnumerator();
                enumerator.Reset();
                if (enumerator.MoveNext()) // Will always be true
                {
                    if (enumerator.Current != this._q2.LastPoint)
                    {
                        results[++resultIndex] = new Point2D(enumerator.Current.X, enumerator.Current.Y);
                    }

                    while (enumerator.MoveNext())
                    {
                        results[++resultIndex] = new Point2D(enumerator.Current.X, enumerator.Current.Y);
                    }
                }
            }

            if (this._q4.Count == 1)
            {
                if (this._q4.FirstPoint != this._q3.LastPoint)
                {
                    results[++resultIndex] = new Point2D(this._q4.FirstPoint.X, this._q4.FirstPoint.Y);
                }
            }
            else
            {
                var enumerator = this._q4.GetEnumerator();
                enumerator.Reset();
                if (enumerator.MoveNext()) // Will always be true
                {
                    if (enumerator.Current != this._q3.LastPoint)
                    {
                        results[++resultIndex] = new Point2D(enumerator.Current.X, enumerator.Current.Y);
                    }

                    while (enumerator.MoveNext())
                    {
                        results[++resultIndex] = new Point2D(enumerator.Current.X, enumerator.Current.Y);
                    }
                }
            }

            if (this._shouldCloseTheGraph && results[resultIndex] != results[0])
            {
                results[++resultIndex] = results[0];
            }

            return results;

        }

        private bool IsZeroData()
        {
            return this._listOfPoint == null || !this._listOfPoint.Any();
        }
    }
}
