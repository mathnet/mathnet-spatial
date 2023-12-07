using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.Tests.Euclidean
{
    [TestFixture]
    public class Circle2DTests
    {
        [TestCase("0, 0", 2.5)]
        [TestCase("2, -4", 4.7)]
        public void CircleCenterRadius(string p1s, double radius)
        {
            var center = Point2D.Parse(p1s);
            var circle = new Circle2D(center, radius);
            Assert.AreEqual(2 * radius, circle.Diameter, double.Epsilon);
            Assert.AreEqual(2 * Math.PI * radius, circle.Circumference, double.Epsilon);
            Assert.AreEqual(Math.PI * radius * radius, circle.Area, double.Epsilon);
        }

        [TestCase("0, 0", 1)]
        [TestCase("2, -4", 4.7)]
        public void CircleEquality(string center, double radius)
        {
            var cp = Point2D.Parse(center);
            var c = new Circle2D(cp, radius);
            var c2 = new Circle2D(cp, radius);
            Assert.True(c == c2);
            Assert.True(c.Equals(c2));
        }

        [TestCase("-7,4", "-4,5", "0,3", "-4,0", 5)]
        [TestCase("1,1", "2,4", "5,3", "3,2", 2.2360679775)]
        [TestCase("-1,0", "0,1", "1,0", "0,0", 1)]
        [TestCase("0,0", "0,1", "1,0", "0.5,0.5", 0.70710678118654752440)] // radius = sqrt(2)
        [TestCase("0,0", "0,1", "1,1", "0.5,0.5", 0.70710678118654752440)]
        [TestCase("0,0", "1,0", "0,1", "0.5,0.5", 0.70710678118654752440)]
        [TestCase("0,0", "1,1", "1,0", "0.5,0.5", 0.70710678118654752440)]
        public void CircleFromThreePoints(string p1s, string p2s, string p3s, string centers, double radius)
        {
            var p1 = Point2D.Parse(p1s);
            var p2 = Point2D.Parse(p2s);
            var p3 = Point2D.Parse(p3s);
            var center = Point2D.Parse(centers);

            var circle = Circle2D.FromPoints(p1, p2, p3);

            AssertGeometry.AreEqual(center, circle.Center);
            Assert.AreEqual(radius, circle.Radius, 1e-6);
        }

        [Test]
        public void CircleFromThreePointsArgumentException()
        {
            var p1 = new Point2D(0, 0);
            var p2 = new Point2D(-1, 0);
            var p3 = new Point2D(1, 0);

            Assert.Throws<ArgumentException>(() => { Circle2D.FromPoints(p1, p2, p3); });
        }

        //parallel to the X-axis
        [TestCase("0,0", 1, "-10,-10", "+10,-10", "empty")]
        [TestCase("0,0", 1, "-10,-1", "+10,-1", "0,-1")]
        [TestCase("0,0", 1, "-10,0", "+10,0", "+1,0;-1,0")]
        [TestCase("0,0", 1, "-10,+1", "+10,+1", "0,+1")]
        [TestCase("0,0", 1, "-10,+10", "+10,+10", "empty")]
        //parallel to the Y-axis
        [TestCase("0,0", 1, "-10,-10", "-10,+10", "empty")]
        [TestCase("0,0", 1, "-1,-10", "-1,+10", "-1,0")]
        [TestCase("0,0", 1, "0,-10", "0,+10", "0,+1;0,-1")]
        [TestCase("0,0", 1, "+1,-10", "+1,+10", "+1,0")]
        [TestCase("0,0", 1, "+10,-10", "+10,+10", "empty")]
        //general cases
        [TestCase("0,0", 1, "-10,+10", "+10,+10", "empty")]
        [TestCase("0,0", 1, "-1.414213562373095,0", "0,+1.414213562373095", "-0.707,0.707")]
        [TestCase("0,0", 1, "-10,-10", "+10,+10", "+0.707,+0.707;-0.707,-0.707")]
        [TestCase("0,0", 1, "0,-1.41421356", "+1.41421356,0", "+0.707,-0.707")]
        [TestCase("0,0", 1, "0,-10", "+10,0", "empty")]
        public void CircleIntersectWithLine2D(string sc, double radius, string sps, string spe, string expectedIntersectionsAsString)
        {
            var circle = new Circle2D(Point2D.Parse(sc), radius);
            var line = new Line2D(Point2D.Parse(sps), Point2D.Parse(spe));

            var actual = circle.IntersectWith(line);

            var expected = parseToPointsArray(expectedIntersectionsAsString);
            for (int i = 0; i < Math.Min(actual.Length, expected.Length); i++)
            {
                var a = actual[i];
                var e = expected[i];
                AssertGeometry.AreEqual(a, e, 1e-3); //needs to fix for the default tolerance
            }
        }
        //parallel to X-axis
        ////segment contains the all intersections(same to the cases of circle and line)
        [TestCase("0,0", 1, "-10,+10", "+10,+10", "empty")]
        [TestCase("0,0", 1, "-10,+1", "+10,+1", "0,+1")]
        [TestCase("0,0", 1, "-10,0", "+10,0", "+1,0;-1,0")]
        [TestCase("0,0", 1, "-10,-1", "+10,-1", "0,-1")]
        [TestCase("0,0", 1, "-10,-10", "+10,-10", "empty")]
        ////segments cross the circle's contour just 1 time
        [TestCase("0,0", 1, "+0,+10", "+10,+10", "empty")]
        [TestCase("0,0", 1, "+0,+1", "+10,+1", "0,1")]
        [TestCase("0,0", 1, "+0,+0", "+10,+0", "1,0")]
        [TestCase("0,0", 1, "+0,-1", "+10,-1", "0,-1")]
        [TestCase("0,0", 1, "+0,-10", "+10,-10", "empty")]
        ////segment contains no intersections(px of the startingPoint is too big to intersect with the circle)
        [TestCase("0,0", 1, "+10,+10", "+100,+10", "empty")]
        [TestCase("0,0", 1, "+10,+1", "+100,+1", "empty")]
        [TestCase("0,0", 1, "+10,+0", "+100,0", "empty")]
        [TestCase("0,0", 1, "+10,-1", "+100,-1", "empty")]
        [TestCase("0,0", 1, "+10,-10", "+100,-10", "empty")]
        //parallel to Y-axis
        ////segment contains the all intersections(same to the cases of circle and line)
        [TestCase("0,0", 1, "-10,-10", "-10,+10", "empty")]
        [TestCase("0,0", 1, "-1,-10", "-1,+10", "-1,0")]
        [TestCase("0,0", 1, "+0,-10", "+0,+10", "0,+1;0,-1")]
        [TestCase("0,0", 1, "+1,-10", "+1,+10", "+1,0")]
        [TestCase("0,0", 1, "+10,-10", "+10,+10", "EMPTY")]
        ////segments cross the circle's contour just 1 time
        [TestCase("0,0", 1, "+10,0", "+10,+10", "empty")]
        [TestCase("0,0", 1, "+1,0", "+1,+10", "+1,0")]
        [TestCase("0,0", 1, "+0,0", "+0,+10", "0,+1")]
        [TestCase("0,0", 1, "-1,0", "-1,+10", "-1,0")]
        [TestCase("0,0", 1, "-10,0", "-10,+10", "empty")]
        ////segment contains no intersections(py of the startingPoint is too big to intersect with the circle)
        [TestCase("0,0", 1, "+10,+10", "+10,+100", "empty")]
        [TestCase("0,0", 1, "+1,+10", "+1,+100", "empty")]
        [TestCase("0,0", 1, "+0,+10", "+0,+100", "empty")]
        [TestCase("0,0", 1, "-1,+10", "-1,+100", "empty")]
        [TestCase("0,0", 1, "-10,+10", "-10,+100", "empty")]
        //general cases
        ////segment contains the all intersections(same to the cases of circle and line)
        [TestCase("0,0", 1, "-10,+10", "+10,+10", "empty")]
        [TestCase("0,0", 1, "-1.414213562373095,0", "0,+1.414213562373095", "-0.707,0.707")]
        [TestCase("0,0", 1, "-10,-10", "+10,+10", "+0.707,+0.707;-0.707,-0.707")]
        [TestCase("0,0", 1, "0,-1.41421356", "+1.41421356,0", "+0.707,-0.707")]
        [TestCase("0,0", 1, "0,-10", "+10,0", "empty")]
        ////segments cross the circle's contour just 1 time
        [TestCase("0,0", 1, "+10,0", "+10,+10", "empty")]
        [TestCase("0,0", 1, "+1,0", "+1,+10", "+1,0")]
        [TestCase("0,0", 1, "+0,0", "+0,+10", "0,+1")]
        [TestCase("0,0", 1, "-1,0", "-1,+10", "-1,0")]
        [TestCase("0,0", 1, "-10,0", "-10,+10", "empty")]
        ////segment contains no intersections(py of the startingPoint is too big to intersect with the circle)
        [TestCase("0,0", 1, "+10,+10", "+10,+100", "empty")]
        [TestCase("0,0", 1, "+1,+10", "+1,+100", "empty")]
        [TestCase("0,0", 1, "+0,+10", "+0,+100", "empty")]
        [TestCase("0,0", 1, "-1,+10", "-1,+100", "empty")]
        [TestCase("0,0", 1, "-10,+10", "-10,+100", "empty")]
        public void CircleIntersectWithLineSegment2D(string sCenter, double radius, string sStart, string sEnd, string expectedIntersectionsAsString)
        {
            var circle = new Circle2D(Point2D.Parse(sCenter), radius);
            var segment = new LineSegment2D(Point2D.Parse(sStart), Point2D.Parse(sEnd));

            var actual = circle.IntersectWith(segment);

            var expected = parseToPointsArray(expectedIntersectionsAsString);
            for (int i = 0; i < Math.Min(actual.Length, expected.Length); i++)
            {
                var a = actual[i];
                var e = expected[i];
                AssertGeometry.AreEqual(a, e, 1e-3); //FIXME!
            }
        }

        private Point2D[] parseToPointsArray(string input)
        {
            if (input.ToLower().Contains("empty"))
            {
                return new Point2D[] { };
            }

            var result = input.Split(';')
                .Select(s => Point2D.Parse(s))
                .ToArray();
            return result;
        }
    }
}
