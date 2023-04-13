// ReSharper disable InconsistentNaming

using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using System;
using System.Linq;

namespace MathNet.Spatial.Tests.Euclidean
{
    [TestFixture]
    public class PlaneTest
    {
        private const string X = "1; 0 ; 0";
        private const string Y = "0; 1; 0";
        private const string Z = "0; 0; 1";
        private const string NegativeZ = "0; 0; -1";
        private const string ZeroPoint = "0; 0; 0";

        [Test]
        public void Ctor()
        {
            var plane1 = new Plane(new Point3D(0, 0, 3), Direction.ZAxis);
            var plane2 = new Plane(0, 0, 3, 3);
            var plane3 = new Plane(Direction.ZAxis, 3);
            var plane4 = Plane.FromPoints(new Point3D(0, 0, 3), new Point3D(5, 3, 3), new Point3D(-2, 1, 3));
            AssertGeometry.AreEqual(plane1, plane2);
            AssertGeometry.AreEqual(plane1, plane3);
            AssertGeometry.AreEqual(plane1, plane4);
        }

        [Test]
        public void RobustFromPointsIndependentOfPointOrder()
        {
            var p1 = new Point3D(1, 0);
            var p2 = new Point3D(0, 1);
            var p3 = new Point3D(0, 0, 1);
            var plane1 = Plane.FromPoints(p1, p2, p3);
            var plane2 = Plane.FromPoints(p1, p3, p2);
            var plane3 = Plane.FromPoints(p2, p1, p3);
            var plane4 = Plane.FromPoints(p2, p3, p1);
            var plane5 = Plane.FromPoints(p3, p1, p2);
            var plane6 = Plane.FromPoints(p3, p2, p1);
            AssertGeometry.AreEqual(plane1, plane2);
            AssertGeometry.AreEqual(plane1, plane3);
            AssertGeometry.AreEqual(plane1, plane4);
            AssertGeometry.AreEqual(plane1, plane5);
            AssertGeometry.AreEqual(plane1, plane6);
        }

        [TestCase("0, 0, 0", "1, 0, 0", "0, 0, 0", "1, 0, 0")]
        public void Parse(string rootPoint, string unitVector, string pds, string vds)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), Direction.Parse(unitVector));
            AssertGeometry.AreEqual(Point3D.Parse(pds), plane.RootPoint);
            AssertGeometry.AreEqual(Vector3D.Parse(vds), plane.Normal);
        }

        [TestCase(ZeroPoint, X, ZeroPoint, X)]
        [TestCase(ZeroPoint, X, "0, 2, 6", X)]
        [TestCase(X, "1, 1, 1", Z, "1, 1, 1")]
        public void Equals(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), Vector3D.Parse(unitVector1).Normalize());
            var plane2 = new Plane(Point3D.Parse(rootPoint2), Vector3D.Parse(unitVector2).Normalize());
            Assert.IsTrue(plane1 == plane2);
            Assert.IsFalse(plane1 != plane2);
        }

        [TestCase(ZeroPoint, Y, ZeroPoint, X)]
        [TestCase(ZeroPoint, X, "0, 2, 6", Z)]
        [TestCase(X, "1, 1, 1", Z, "1, 0, 1")]
        public void NotEquals(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), Vector3D.Parse(unitVector1).Normalize());
            var plane2 = new Plane(Point3D.Parse(rootPoint2), Vector3D.Parse(unitVector2).Normalize());
            Assert.IsTrue(plane1 != plane2);
            Assert.IsFalse(plane1 == plane2);
        }

        [TestCase("1, 0, 0, 0", "0, 0, 0", "1, 0, 0")]
        public void Parse2(string s, string pds, string vds)
        {
            var plane = GetPlaneFrom4Doubles(s);
            AssertGeometry.AreEqual(Point3D.Parse(pds), plane.RootPoint);
            AssertGeometry.AreEqual(Vector3D.Parse(vds), plane.Normal);
        }

        [TestCase(ZeroPoint, "0, 0, 0", "0, 0, 1", ZeroPoint)]
        [TestCase(ZeroPoint, "0, 0, -1", "0, 0, 1", "0; 0;-1")]
        [TestCase(ZeroPoint, "0, 0, 1", "0, 0, -1", "0; 0; 1")]
        [TestCase("1; 2; 3", "0, 0, 0", "0, 0, 1", "1; 2; 0")]
        [TestCase("8; 0; 0", ZeroPoint, "0.6, 0, 0.8", "8; 0; -6", Z, 1e-15f)]
        public void ProjectPointOn(string ps, string rootPoint, string unitVector, string expectedPoint, string projectionAxis = "", float eps = float.Epsilon)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), Direction.Parse(unitVector));
            Direction? projectionDirection = null;
            if (projectionAxis != "")
            {
                projectionDirection = Direction.Parse(projectionAxis);
            }
            var projectedPoint = plane.Project(Point3D.Parse(ps), projectionDirection);
            var expected = Point3D.Parse(expectedPoint);
            AssertGeometry.AreEqual(expected, projectedPoint, eps);
        }

        [Test, Sequential]
        public void BestFitFrom3RandomPoints(
            [Random(0d, 10d, 3)] double p0x,
            [Random(0d, 10d, 3)] double p0y,
            [Random(0d, 10d, 3)] double p0z,
            [Random(0d, 10d, 3)] double p1x,
            [Random(0d, 10d, 3)] double p1y,
            [Random(0d, 10d, 3)] double p1z,
            [Random(0d, 10d, 3)] double p2x,
            [Random(0d, 10d, 3)] double p2y,
            [Random(0d, 10d, 3)] double p2z
        )
        {
            var ps = new[]
            {
                new Point3D(p0x, p0y, p0z),
                new Point3D(p1x, p1y, p1z),
                new Point3D(p2x, p2y, p2z),
            };
            var actual = Plane.BestFit(ps);

            var expected = Plane.FromPoints(ps[0], ps[1], ps[2]);
            AssertGeometry.AreEqual(expected, actual);
        }

        [Test]
        public void BestFitFromZeroPointsShouldThrowException()
        {
            var points = new Point3D[] { };
            Assert.Throws<ArgumentException>(() => Plane.BestFit(points));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void BestFitFromInsufficientPointsShouldThrowException(int pointCount)
        {
            var p = new Point3D(1, 0);
            var insufficientPoints = Enumerable.Repeat(p, pointCount);
            Assert.Throws<ArgumentException>(() => Plane.BestFit(insufficientPoints));
        }

        [TestCase(ZeroPoint, X, Y)] //all 3 points are on the plane z=0. normal vector is +ez.
        [TestCase(ZeroPoint, Y, Z)] //all 3 points are on the plane x=0. normal vector is +ex.
        [TestCase(ZeroPoint, Z, X)] //all 3 points are on the plane y=0. normal vector is +ey.
        public void CreateFittedPlaneFromPointsShouldBeOnTheFittedPlane(string sP1, string sP2, string sP3)
        {
            var ps = new[]
            {
                Point3D.Parse(sP1),
                Point3D.Parse(sP2),
                Point3D.Parse(sP3),
            }.ToArray();
            var aPlane = Plane.BestFit(ps);

            var tolerance = 1e-16;
            Assert.That(aPlane.SignedDistanceTo(ps[0]), Is.EqualTo(0).Within(tolerance), "ps[0]");
            Assert.That(aPlane.SignedDistanceTo(ps[1]), Is.EqualTo(0).Within(tolerance), "ps[1]");
            Assert.That(aPlane.SignedDistanceTo(ps[2]), Is.EqualTo(0).Within(tolerance), "ps[2]");
        }

        //https://math.stackexchange.com/questions/99299/best-fitting-plane-given-a-set-of-points/1652383#1652383
        // the values in the link example are not correct (for example, it reports xm = 1.8, whereas it clearly is xm=2.0)
        [TestCase("1,1,9;1,2,14;1,3,20;2,1,11;2,2,17;2,3,23;3,1,15;3,2,20;3,3,26", -17.493526463019482, -33.043327763481223, 5.8107314585613725, 1)]
        public void CreateFittedPlane(string sPoints, double eA, double eB, double eC, double eOffset)
        {
            var points = sPoints.Split(';').Select(s => Point3D.Parse(s));
            var actual = Plane.BestFit(points);

            //normalize (eA, eB, eC and eD).
            //both 2 following equations mean the same plane.
            // eq1: Ax + By + Cz + D = 0
            // eq2:-Ax - By - Cz - D = 0
            var normal = new Vector3D(eA, eB, eC);
            var eNormalizedOffset = -eOffset / normal.Length;
            var expected = new Plane(normal.Normalize(), eNormalizedOffset);
            AssertGeometry.AreEqual(expected, actual);
        }

        [TestCase(ZeroPoint, Z, ZeroPoint, 0)]
        [TestCase(ZeroPoint, Z, "1; 2; 0", 0)]
        [TestCase(ZeroPoint, Z, "1; -2; 0", 0)]
        [TestCase(ZeroPoint, Z, "1; 2; 3", 3)]
        [TestCase(ZeroPoint, Z, "-1; 2; -3", -3)]
        [TestCase(ZeroPoint, NegativeZ, ZeroPoint, 0)]
        [TestCase(ZeroPoint, NegativeZ, "1; 2; 1", -1)]
        [TestCase(ZeroPoint, NegativeZ, "1; 2; -1", 1)]
        [TestCase("0; 0; -1", NegativeZ, ZeroPoint, -1)]
        [TestCase("0; 0; 1", NegativeZ, ZeroPoint, -1)]
        [TestCase(ZeroPoint, X, "1; 0; 0", 1)]
        [TestCase("188,6578; 147,0620; 66,0170", Z, "118,6578; 147,0620; 126,1170", 60.1)]
        public void SignedDistanceToPoint(string prps, string pns, string ps, double expected)
        {
            var plane = new Plane(Direction.Parse(pns), Point3D.Parse(prps));
            var p = Point3D.Parse(ps);
            Assert.AreEqual(expected, plane.SignedDistanceTo(p), 1E-6);
        }

        [TestCase(ZeroPoint, Z, ZeroPoint, Z, 0)]
        [TestCase(ZeroPoint, Z, "0;0;1", Z, 1)]
        [TestCase(ZeroPoint, Z, "0;0;-1", Z, -1)]
        [TestCase(ZeroPoint, NegativeZ, "0;0;-1", Z, 1)]
        public void SignedDistanceToOtherPlane(string prps, string pns, string otherPlaneRootPointString, string otherPlaneNormalString, double expectedValue)
        {
            var plane = new Plane(Direction.Parse(pns), Point3D.Parse(prps));
            var otherPlane = new Plane(Direction.Parse(otherPlaneNormalString), Point3D.Parse(otherPlaneRootPointString));
            Assert.AreEqual(expectedValue, plane.SignedDistanceTo(otherPlane), 1E-6);
        }

        [TestCase(ZeroPoint, Z, ZeroPoint, Z, 0)]
        [TestCase(ZeroPoint, Z, ZeroPoint, X, 0)]
        [TestCase(ZeroPoint, Z, "0;0;1", X, 1)]
        public void SignedDistanceToLine(string prps, string pns, string lineThroughPointString, string rayDirectionString, double expectedValue)
        {
            var plane = new Plane(Direction.Parse(pns), Point3D.Parse(prps));
            var otherPlane = new Line(Point3D.Parse(lineThroughPointString), Direction.Parse(rayDirectionString));
            Assert.AreEqual(expectedValue, plane.SignedDistanceTo(otherPlane), 1E-6);
        }

        [Test]
        public void ProjectLineOn()
        {
            var unitVector = Direction.ZAxis;
            var rootPoint = new Point3D(0, 0, 1);
            var plane = new Plane(unitVector, rootPoint);

            var line = new LineSegment(new Point3D(0, 0), new Point3D(1, 0));
            var projectOn = plane.Project(line);
            AssertGeometry.AreEqual(new LineSegment(new Point3D(0, 0, 1), new Point3D(1, 0, 1)), projectOn, float.Epsilon);
        }

        [Test]
        public void ProjectVectorOn()
        {
            var unitVector = Direction.ZAxis;
            var rootPoint = new Point3D(0, 0, 1);
            var plane = new Plane(unitVector, rootPoint);
            var vector = new Vector3D(1, 0);
            var projectOn = plane.Project(vector);
            AssertGeometry.AreEqual(new Vector3D(1, 0), projectOn.Direction, float.Epsilon);
            AssertGeometry.AreEqual(new Point3D(0, 0, 1), projectOn.ThroughPoint, float.Epsilon);
        }

        [TestCase("0, 0, 0", "0, 0, 1", "0, 0, 0", "0, 1, 0", "0, 0, 0", "-1, 0, 0")]
        [TestCase("0, 0, 2", "0, 0, 1", "0, 0, 0", "0, 1, 0", "0, 0, 2", "-1, 0, 0")]
        public void InterSectionWithPlane(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2, string eps, string evs)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), Direction.Parse(unitVector1));
            var plane2 = new Plane(Point3D.Parse(rootPoint2), Direction.Parse(unitVector2));
            var intersections = new[]
            {
                plane1.IntersectionWith(plane2),
                plane2.IntersectionWith(plane1)
            };
            foreach (var intersection in intersections)
            {
                AssertGeometry.AreEqual(Point3D.Parse(eps), intersection.ThroughPoint);
                AssertGeometry.AreEqual(Direction.Parse(evs), intersection.Direction);
            }
        }

        [TestCase("0, 0, 0", "0, 0, 1", "0, 0, 0", "0, 0, 1", "0, 0, 0", "0, 0, 0")]
        public void InterSectionWithPlaneTest_BadArgument(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2, string eps, string evs)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), Direction.Parse(unitVector1));
            var plane2 = new Plane(Point3D.Parse(rootPoint2), Direction.Parse(unitVector2));

            Assert.Throws<ArgumentException>(() => plane1.IntersectionWith(plane2));
            Assert.Throws<ArgumentException>(() => plane2.IntersectionWith(plane1));
        }

        [Test]
        public void MirrorPoint()
        {
            var plane = new Plane(Direction.ZAxis, new Point3D(0, 0));
            var point3D = new Point3D(1, 2, 3);
            var mirrorAbout = plane.MirrorAbout(point3D);
            AssertGeometry.AreEqual(new Point3D(1, 2, -3), mirrorAbout, float.Epsilon);
        }

        [Test]
        public void SignOfD()
        {
            var plane = new Plane(Direction.ZAxis, new Point3D(0, 0, 100));
            Assert.AreEqual(100, plane.D);

            plane = new Plane(Direction.ZAxis, new Point3D(0, 0, -100));
            Assert.AreEqual(100, plane.D);
        }

        [Test]
        public void InterSectionPointDifferentOrder()
        {
            var plane1 = new Plane(Direction.Create(0.8, 0.3, 0.01), new Point3D(20, 0));
            var plane2 = new Plane(Direction.Create(0.002, 1, 0.1), new Point3D(0, 0));
            var plane3 = new Plane(Direction.Create(0.5, 0.5, 1), new Point3D(0, 0, -30));
            var pointFromPlanes1 = Plane.PointFromPlanes(plane1, plane2, plane3);
            var pointFromPlanes2 = Plane.PointFromPlanes(plane2, plane1, plane3);
            var pointFromPlanes3 = Plane.PointFromPlanes(plane3, plane1, plane2);
            AssertGeometry.AreEqual(pointFromPlanes1, pointFromPlanes2, 1E-10);
            AssertGeometry.AreEqual(pointFromPlanes3, pointFromPlanes2, 1E-10);
        }

        [TestCase("0, 0, 0", "1, 0, 0", "0, 0, 0", "0, 1, 0", "0, 0, 0", "0, 0, 1", "0, 0, 0")]
        [TestCase("0, 0, 0", "-1, 0, 0", "0, 0, 0", "0, 1, 0", "0, 0, 0", "0, 0, 1", "0, 0, 0")]
        [TestCase("20, 0, 0", "1, 0, 0", "0, 0, 0", "0, 1, 0", "0, 0, -30", "0, 0, 1", "20, 0, -30")]
        public void PointFromPlanes(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2, string rootPoint3, string unitVector3, string eps)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), Direction.Parse(unitVector1));
            var plane2 = new Plane(Point3D.Parse(rootPoint2), Direction.Parse(unitVector2));
            var plane3 = new Plane(Point3D.Parse(rootPoint3), Direction.Parse(unitVector3));
            var points = new[]
            {
                Plane.PointFromPlanes(plane1, plane2, plane3),
                Plane.PointFromPlanes(plane2, plane1, plane3),
                Plane.PointFromPlanes(plane1, plane3, plane2),
                Plane.PointFromPlanes(plane2, plane3, plane1),
                Plane.PointFromPlanes(plane3, plane2, plane1),
                Plane.PointFromPlanes(plane3, plane1, plane2),
            };
            var expected = Point3D.Parse(eps);
            foreach (var point in points)
            {
                AssertGeometry.AreEqual(expected, point);
            }
        }

        [TestCase("1, 1, 0, 12", "1, -1, 0, -12", "0, 0, 1, 5", "0, 16.970563, 5")]
        public void PointFromPlanes2(string planeString1, string planeString2, string planeString3, string eps)
        {
            var plane1 = GetPlaneFrom4Doubles(planeString1);
            var plane2 = GetPlaneFrom4Doubles(planeString2);
            var plane3 = GetPlaneFrom4Doubles(planeString3);
            var points = new[]
            {
                Plane.PointFromPlanes(plane1, plane2, plane3),
                Plane.PointFromPlanes(plane2, plane1, plane3),
                Plane.PointFromPlanes(plane1, plane3, plane2),
                Plane.PointFromPlanes(plane2, plane3, plane1),
                Plane.PointFromPlanes(plane3, plane2, plane1),
                Plane.PointFromPlanes(plane3, plane1, plane2),
            };
            var expected = Point3D.Parse(eps);
            foreach (var point in points)
            {
                AssertGeometry.AreEqual(expected, point);
            }
        }

        [TestCase("0, 0, 0", "0, 0, 1", "<Plane><RootPoint><X>0</X><Y>0</Y><Z>0</Z></RootPoint><Normal><X>0</X><Y>0</Y><Z>1</Z></Normal></Plane>")]
        public void XmlRoundTrips(string rootPoint, string unitVector, string xml)
        {
            var plane = new Plane(Point3D.Parse(rootPoint), Direction.Parse(unitVector));
            AssertXml.XmlRoundTrips(plane, xml, (e, a) => AssertGeometry.AreEqual(e, a));
        }

        private Plane GetPlaneFrom4Doubles(string inputString)
        {
            var numbers = inputString.Split(',').Select(double.Parse).ToArray();
            return new Plane(numbers[0], numbers[1], numbers[2], numbers[3]);
        }
    }
}
