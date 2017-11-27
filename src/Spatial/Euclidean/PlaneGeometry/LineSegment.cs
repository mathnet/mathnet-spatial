using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathNet.Spatial.Euclidean.PlaneGeometry
{
    public class LineSegment : LineSegment<StraightLine>
    {
        public LineSegment(Point2D start, Point2D end) : base(new StraightLine(start, end), start, end)
        {

        }        
    }
}
