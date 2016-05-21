using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace MathNet.Spatial.UnitTests.Euclidean
{
    [TestFixture]
    public class Line3DTests
    {
        [Test]
        public void Ctor()
        {
            Assert.Throws<ArgumentException>(() => new Line3D(Point3D.Origin, Point3D.Origin));
        }

        [TestCase("0, 0, 0", "1, -1, 1", "1, -1, 1")]
        public void DirectionsTest(string p1s, string p2s, string evs)
        {
            Line3D l = Line3D.Parse(p1s, p2s);
            var excpected = UnitVector3D.Parse(evs);
            AssertGeometry.AreEqual(excpected, l.Direction);
        }

        [TestCase("0, 0, 0", "1, -1, 1", "p:{0, 0, 0} v:{1, 0, 0}", "0, 0, 0", "0, -1, 1")]
        public void ProjectOn(string p1s, string p2s, string pls, string ep1s, string ep2s)
        {
            var p1 = Point3D.Parse(p1s);
            var p2 = Point3D.Parse(p2s);
            var line = new Line3D(p1, p2);
            var plane = Plane.Parse(pls);
            var expected = new Line3D(Point3D.Parse(ep1s), Point3D.Parse(ep2s));
            AssertGeometry.AreEqual(expected, line.ProjectOn(plane));
        }

        [TestCase("0, 0, 0", "1, -2, 3", 3.741657)]
        public void Length(string p1s, string p2s, double expected)
        {
            var p1 = Point3D.Parse(p1s);
            var p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            Assert.AreEqual(expected, l.Length, 1e-6);
        }

        [TestCase("0, 0, 0", "1, -1, 1", "0, 0, 0", "1, -1, 1", true)]
        [TestCase("0, 0, 2", "1, -1, 1", "0, 0, 0", "1, -1, 1", false)]
        [TestCase("0, 0, 0", "1, -1, 1", "0, 0, 0", "2, -1, 1", false)]
        public void Equals(string p1s, string p2s, string p3s, string p4s, bool expected)
        {
            var line1 = new Line3D(Point3D.Parse(p1s), Point3D.Parse(p2s));
            var line2 = new Line3D(Point3D.Parse(p3s), Point3D.Parse(p4s));
            Assert.AreEqual(expected, line1.Equals(line2));
            Assert.AreEqual(expected, line1 == line2);
            Assert.AreEqual(!expected, line1 != line2);
        }

        [TestCase("0, 0, 0", "1, 0, 0", "0.5, 1, 0", true, "0.5, 0, 0")]
        [TestCase("0, 0, 0", "1, 0, 0", "0.5, 1, 0", false, "0.5, 0, 0")]
        [TestCase("0, 0, 0", "1, 0, 0", "2, 1, 0", true, "1, 0, 0")]
        [TestCase("0, 0, 0", "1, 0, 0", "2, 1, 0", false, "2, 0, 0")]
        [TestCase("0, 0, 0", "1, 0, 0", "-2, 1, 0", true, "0, 0, 0")]
        [TestCase("0, 0, 0", "1, 0, 0", "-2, 1, 0", false, "-2, 0, 0")]
        public void LineToTest(string p1s, string p2s, string ps, bool mustStartFromLine, string sps)
        {
            var p1 = Point3D.Parse(p1s);
            var p2 = Point3D.Parse(p2s);
            Line3D l = new Line3D(p1, p2);
            var p = Point3D.Parse(ps);
            var actual = l.LineTo(p, mustStartFromLine);
            AssertGeometry.AreEqual(Point3D.Parse(sps), actual.StartPoint, 1e-6);
            AssertGeometry.AreEqual(p, actual.EndPoint, 1e-6);
        }

        [TestCase("1, 2, 3", "4, 5, 6", @"<Line3D><StartPoint X=""1"" Y=""2"" Z=""3"" /><EndPoint X=""4"" Y=""5"" Z=""6"" /></Line3D>")]
        public void XmlTests(string p1s, string p2s, string xml)
        {
            Point3D p1 = Point3D.Parse(p1s);
            Point3D p2 = Point3D.Parse(p2s);
            var l = new Line3D(p1, p2);
            AssertXml.XmlRoundTrips(l, xml, (e, a) => AssertGeometry.AreEqual(e, a));
        }

        [TestCase("0,0,0", "0,0,1", "0,0,0", "0,0,0")]  // start point
        [TestCase("0,0,0", "0,0,1", "0,0,1", "0,0,1")]  // end point
        [TestCase("0,0,0", "0,0,1", "1,0,.25", "0,0,.25")]
        [TestCase("0,0,0", "0,0,1", "0,0,-1", "0,0,0")]
        [TestCase("0,0,0", "0,0,1", "0,0,3", "0,0,1")]
        public void ClosestPointToWithinSegment(string start, string end, string point, string expected)
        {
            var line = Line3D.Parse(start, end);
            var p = Point3D.Parse(point);
            var e = Point3D.Parse(expected);

            Assert.AreEqual(e, line.ClosestPointTo(p, true));
        }

        [TestCase("0,0,0", "0,0,1", "0,0,0", "0,0,0")]  // start point
        [TestCase("0,0,0", "0,0,1", "0,0,1", "0,0,1")]  // end point
        [TestCase("0,0,0", "0,0,1", "1,0,.25", "0,0,.25")]
        [TestCase("0,0,0", "0,0,1", "0,0,-1", "0,0,-1")]
        [TestCase("0,0,0", "0,0,1", "0,0,3", "0,0,3")]
        public void ClosestPointToOutsideSegment(string start, string end, string point, string expected)
        {
            var line = Line3D.Parse(start, end);
            var p = Point3D.Parse(point);
            var e = Point3D.Parse(expected);

            Assert.AreEqual(e, line.ClosestPointTo(p, false));
        }

        [Test]
        public void BinaryRountrip()
        {
            var line = new Line3D(new Point3D(1, 2, 3), new Point3D(4, 5, 6));
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, line);
                ms.Flush();
                ms.Position = 0;
                var roundTrip = (Line3D)formatter.Deserialize(ms);
                AssertGeometry.AreEqual(line, roundTrip);
            }
        }

        [TestCase("0,0,0", "0,0,1", "0,1,1", "0,1,2", true)]
        [TestCase("0,0,0", "0,0,-1", "0,1,1", "0,1,2", true)]
        [TestCase("0,0,0", "0,0.5,-1", "0,1,1", "0,1,2", false)]
        [TestCase("0,0,0", "0,0.00001,-1.0000", "0,1,1", "0,1,2", false)]
        public void IsParallelToWithinDoubleTol(string s1, string e1, string s2, string e2, bool expected)
        {
            var line1 = Line3D.Parse(s1, e1);
            var line2 = Line3D.Parse(s2, e2);

            Assert.AreEqual(expected, line1.IsParallelTo(line2));
        }

        [TestCase("0,0,0", "0,0,1", "0,1,1", "0,1,2", 0.01, true)]
        [TestCase("0,0,0", "0,0,-1", "0,1,1", "0,1,2", 0.01, true)]
        [TestCase("0,0,0", "0,0.5,-1", "0,1,1", "0,1,2", 0.01, false)]
        [TestCase("0,0,0", "0,0.001,-1.0000", "0,1,1", "0,1,2", 0.05, false)]
        [TestCase("0,0,0", "0,0.001,-1.0000", "0,1,1", "0,1,2", 0.06, true)]
        public void IsParallelToWithinAngleTol(string s1, string e1, string s2, string e2, double degreesTol, bool expected)
        {
            var line1 = Line3D.Parse(s1, e1);
            var line2 = Line3D.Parse(s2, e2);

            Assert.AreEqual(expected, line1.IsParallelTo(line2, Angle.FromDegrees(degreesTol)));
        }

        [TestCase("0,0,0", "1,0,0", "1,1,1", "2,1,1", "0,0,0", "0,1,1")] // Parallel case
        [TestCase("0,0,0", "1,0,0", "2,-1,0", "2,0,0", "2,0,0", "2,0,0")] // Intersecting case
        [TestCase("0.3097538,3.0725982,3.9317042", "1.3945620,6.4927958,2.1094821", "2.4486204,5.1947760,6.3369721", "8.4010954,9.5691708,5.1665254", "-0.1891954,1.4995044,4.7698213", "-0.7471721,2.8462306,6.9653670")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("6.7042836,5.1490163,0.3655590", "7.6915457,0.8511235,0.8627290", "0.6890053,6.6207933,3.4147472", "5.6116149,1.7160727,5.4461589", "7.5505158,1.4650754,0.7917085", "5.7356234,1.5925149,5.4973334")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("2.9663973,7.1338954,9.7732130", "6.4625826,8.7716408,3.7015737", "1.8924447,4.9060507,1.8755412", "1.9542505,5.8440396,8.2251863", "1.8254726,6.5994432,11.7545962", "1.9889190,6.3701828,11.7868723")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("7.3107741,0.0835261,3.0516366", "1.0013621,9.6730070,3.6824080", "3.2195097,0.8726049,1.0448256", "4.5620367,8.6714351,3.3311693", "3.7856774,5.4412119,3.4040514", "4.0462563,5.6752319,2.4527875")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("6.8171425,2.0728553,1.9235196", "3.0360117,0.0371047,0.5410118", "0.4798755,2.8536110,0.4402877", "9.4038162,7.5597521,0.3068954", "2.9867513,0.0105830,0.5230005", "1.2671009,3.2687632,0.4285205")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("0.9826196,9.8736629,3.7966845", "3.9607685,8.7036678,0.3921888", "0.2333178,4.9282166,5.6478261", "5.8182917,0.7224496,6.8152397", "-0.7871493,10.5689341,5.8198105", "-3.0149659,7.3743380,4.9688451")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("3.1369768,8.4887731,0.1371429", "4.9427402,3.5584831,9.4250139", "1.0628142,7.4582519,3.7246752", "3.7045709,0.1811190,4.7211734", "3.8813087,6.4565178,3.9655839", "1.7121301,5.6696095,3.9696039")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("8.7598151,0.2378638,3.0353259", "0.5266014,9.6548588,2.7893143", "1.8918146,3.4742242,7.0646539", "7.3796941,7.0882850,4.4899767", "4.5791946,5.0195789,2.9104073", "5.3106500,5.7257093,5.4606833")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("7.6132801,6.8593303,3.7729401", "7.7434088,2.0184714,6.0037938", "4.1910423,0.6022826,6.7846734", "1.6554785,3.0341358,8.7351072", "7.8041730,-0.2419887,7.0455007", "5.8721457,-1.0100597,5.4915169")] // Randomly generated in GOM Inspect Professional V8
        [TestCase("0.3818483,0.4893437,8.1438438", "4.6619509,7.8881811,4.7600744", "0.7792658,4.6454081,8.3927247", "0.2002105,7.4120903,3.5542593", "2.2611504,3.7380159,6.6581026", "0.6866122,5.0880999,7.6185307")] // Randomly generated in GOM Inspect Professional V8
        public void ClosestPointsBetween(string s1, string e1, string s2, string e2, string cp1, string cp2)
        {
            var l1 = Line3D.Parse(s1, e1);
            var l2 = Line3D.Parse(s2, e2);

            var result = l1.ClosestPointsBetween(l2);

            AssertGeometry.AreEqual(Point3D.Parse(cp1), result.Item1);
            AssertGeometry.AreEqual(Point3D.Parse(cp2), result.Item2);
        }
    }
}
