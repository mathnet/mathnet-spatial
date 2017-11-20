﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Spatial.Euclidean.PlaneGeometry;
using MathNet.Spatial.Euclidean.PlaneGeometry.Lines;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean.PlaneGeometry.Lines
{
    [TestFixture]
    public class LineTests
    {
        [TestCase("0,0", "1,1", 1, 0)]
        [TestCase("1,2", "3,1", -0.5, 2.5)]
        [TestCase("1,0", "1,1", double.PositiveInfinity, double.NaN)]
        [TestCase("0,1", "1,1", 0, 1)]
        public void CreateFromPoints(string p1, string p2, double gradient, double yintercept)
        {
            var first = Point2D.Parse(p1);
            var second = Point2D.Parse(p2);
            Line l = new Line(first, second);
            Assert.AreEqual(gradient, l.Gradient);
            Assert.AreEqual(yintercept, l.YIntercept.Y);
        }

        [TestCase("-1,3", 2, 5)]
        [TestCase("2,2", -2, 6)]
        [TestCase("0,0", 1, 0)]
        public void CreateFromPointGradient(string point, double gradient, double yintercept)
        {
            var p1 = Point2D.Parse(point);
            Line l = new Line(p1, gradient);
            Assert.AreEqual(gradient, l.Gradient);
            Assert.AreEqual(yintercept, l.YIntercept.Y);
        }

        [TestCase("0,0;1,1", "1,0;2,1", true)]
        public void IsParallel(string line1, string line2, bool expected)
        {
            var pointpair1 = line1.Split(';').Select(t => Point2D.Parse(t));
            var pointpair2 = line2.Split(';').Select(t => Point2D.Parse(t));
            Line l1 = new Line(pointpair1);
            Line l2 = new Line(pointpair2);
            Assert.AreEqual(expected, l1.IsParallel(l2));
        }
    }
}