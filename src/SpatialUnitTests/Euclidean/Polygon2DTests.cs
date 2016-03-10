using System.Collections.Generic;
using System;
using System.Linq;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Polygon2DTests
    {
        
        private Polygon2D TestPolygon1()
        {
            var points = from x in new string[] {"0,0", "0.25,0.5", "1,1", "-1,1", "0.5,-0.5"} select Point2D.Parse(x);
            return new Polygon2D(points);
        }

        private Polygon2D TestPolygon2()
        {
            var points = from x in new string[] { "0,0", "0.25,0.5", "1,1", "-1,1", "0.5,-0.5", "0,0" } select Point2D.Parse(x);
            return new Polygon2D(points);
        }

        private Polygon2D TestPolygon3()
        {
            var points = from x in new string[] { "0.25,0", "0.5,1", "1,-1" } select Point2D.Parse(x);
            return new Polygon2D(points);
        }

        private Polygon2D TestPolygon4()
        {
            var points = from x in new string[] { "0.5,1", "1,-1", "0.25,0" } select Point2D.Parse(x);
            return new Polygon2D(points);
        }

        [Test]
        public void ConstructorTest()
        {
            var polygon = this.TestPolygon1();
            var checkList = new List<Point2D> { new Point2D(0, 0), new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5) };
            CollectionAssert.AreEqual(checkList, polygon);
        }

        [Test]
        public void ConstructorTest_ClipsStartOnDuplicate()
        {
            // Test to make sure that if the constructor point list is given to the polygon constructor with the first and last points
            // being duplicates, the point at the beginning of the list is removed
            var polygon = this.TestPolygon2();
            var checkList = new List<Point2D> { new Point2D(0.25, 0.5), new Point2D(1, 1), new Point2D(-1, 1), new Point2D(0.5, -0.5), new Point2D(0, 0) };
            CollectionAssert.AreEqual(checkList, polygon);
        }

        [TestCase(0.5, 0, true)]
        [TestCase(0.35, 0, true)]
        [TestCase(0.5, 0.5, true)]
        [TestCase(0.75, 0.1, false)]
        [TestCase(0.75, -0.1, true)]
        [TestCase(0.5, -0.5, false)]
        [TestCase(0.25, 0.5, false)]
        [TestCase(0.25, -0.5, false)]
        [TestCase(0.0, 0, false)]
        [TestCase(1.5, 0, false)]
        public void IsPointInPolygonTest1(double x, double y, bool outcome)
        {
            var testPoint = new Point2D(x, y);
            var testPoly = this.TestPolygon3();
            
            Assert.AreEqual(outcome, Polygon2D.IsPointInPolygon(testPoint, testPoly));
        }

        [TestCase(0.5, 0, true)]
        [TestCase(0.35, 0, true)]
        [TestCase(0.5, 0.5, true)]
        [TestCase(0.75, 0.1, false)]
        [TestCase(0.75, -0.1, true)]
        [TestCase(0.5, -0.5, false)]
        [TestCase(0.25, 0.5, false)]
        [TestCase(0.25, -0.5, false)]
        [TestCase(0.0, 0, false)]
        [TestCase(1.5, 0, false)]
        public void IsPointInPolygonTest2(double x, double y, bool outcome)
        {
            var testPoint = new Point2D(x, y);
            var testPoly = this.TestPolygon4();

            Assert.AreEqual(outcome, Polygon2D.IsPointInPolygon(testPoint, testPoly));
        }

        // These test cases were generated using scipy.spatial's ConvexHull method
        [TestCase("0.27,0.41;0.87,0.67;0.7,0.33;0.5,0.61;0.04,0.23;0.73,0.14;0.84,0.02;0.25,0.23;0.12,0.2;0.37,0.78", "0.84,0.02;0.87,0.67;0.37,0.78;0.04,0.23;0.12,0.2")]
        [TestCase("0.81,0.25;0.77,0.15;0.17,0.48;0.4,0.58;0.29,0.92;0.37,0.26;0.7,0.91;0.04,0.1;0.39,0.73;0.7,0.12", "0.29,0.92;0.04,0.1;0.7,0.12;0.77,0.15;0.81,0.25;0.7,0.91")]
        [TestCase("0.87,0.39;0.83,0.42;0.75,0.62;0.91,0.49;0.18,0.63;0.17,0.95;0.22,0.5;0.93,0.41;0.66,0.79;0.32,0.42", "0.87,0.39;0.93,0.41;0.91,0.49;0.66,0.79;0.17,0.95;0.18,0.63;0.22,0.5;0.32,0.42")]
        [TestCase("0.18,0.39;0.91,0.3;0.35,0.53;0.91,0.38;0.49,0.28;0.61,0.22;0.27,0.18;0.44,0.06;0.5,0.79;0.78,0.22", "0.5,0.79;0.18,0.39;0.27,0.18;0.44,0.06;0.78,0.22;0.91,0.3;0.91,0.38")]
        [TestCase("0.89,0.55;0.98,0.24;0.03,0.2;0.51,0.99;0.72,0.32;0.56,0.87;0.1,0.75;0.64,0.16;0.82,0.73;0.17,0.46", "0.1,0.75;0.03,0.2;0.64,0.16;0.98,0.24;0.89,0.55;0.82,0.73;0.51,0.99")]
        public void ConvexHullTest(string points, string expected)
        {
            var testPoints = from x in points.Split(';') select Point2D.Parse(x);
            var expectedPoints = from x in expected.Split(';') select Point2D.Parse(x);

            Polygon2D hull = Polygon2D.GetConvexHullFromPoints(testPoints);

            CollectionAssert.AreEquivalent(expectedPoints, hull);
        }

        [TestCase("0,0;0.4,0;0.5,0;0.6,0;1,0;1,.25;1,.75;1,1;0,1;0,0.5", "1,0;1,1;0,1;0,0")]
        public void ReduceComplexity(string points, string reduced)
        {
            var testPoints = from x in points.Split(';') select Point2D.Parse(x);
            var expectedPoints = from x in reduced.Split(';') select Point2D.Parse(x);
            var poly = new Polygon2D(testPoints);
            var expected = new Polygon2D(expectedPoints);
            var thinned = poly.ReduceComplexity(0.00001);

            CollectionAssert.AreEqual(expected, thinned);

        }

    }
}