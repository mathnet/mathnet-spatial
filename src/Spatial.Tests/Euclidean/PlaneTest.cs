// ReSharper disable InconsistentNaming

using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace MathNet.Spatial.Tests.Euclidean
{
    [TestFixture]
    public class PlaneTest
    {
        private const string X = "1; 0 ; 0";
        private const string Y = "0; 1; 0";
        private const string Z = "0; 0; 1";
        private const string XY = "1; 1; 0";
        private const string YZ = "0; 1; 1";
        private const string ZX = "1; 0; 1";
        private const string NegativeZ = "0; 0; -1";
        private const string ZeroPoint = "0; 0; 0";

        [Test]
        public void Ctor()
        {
            var plane1 = new Plane(new Point3D(0, 0, 3), UnitVector3D.ZAxis);
            var plane2 = new Plane(0, 0, 3, -3);
            var plane3 = new Plane(UnitVector3D.ZAxis, 3);
            var plane4 = Plane.FromPoints(new Point3D(0, 0, 3), new Point3D(5, 3, 3), new Point3D(-2, 1, 3));
            AssertGeometry.AreEqual(plane1, plane2);
            AssertGeometry.AreEqual(plane1, plane3);
            AssertGeometry.AreEqual(plane1, plane4);
        }

        [Test]
        public void RobustFromPointsIndependentOfPointOrder()
        {
            var p1 = new Point3D(1, 0, 0);
            var p2 = new Point3D(0, 1, 0);
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
            var plane = new Plane(Point3D.Parse(rootPoint), UnitVector3D.Parse(unitVector));
            AssertGeometry.AreEqual(Point3D.Parse(pds), plane.RootPoint);
            AssertGeometry.AreEqual(Vector3D.Parse(vds), plane.Normal);
        }

        [TestCase("1, 0, 0, 0", "0, 0, 0", "1, 0, 0")]
        public void Parse2(string s, string pds, string vds)
        {
            var plane = this.GetPlaneFrom4Doubles(s);
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
            var plane = new Plane(Point3D.Parse(rootPoint), UnitVector3D.Parse(unitVector));
            UnitVector3D? projectionDirection = null;
            if (projectionAxis != "")
            {
                projectionDirection = UnitVector3D.Parse(projectionAxis);
            }
            var projectedPoint = plane.Project(Point3D.Parse(ps), projectionDirection);
            var expected = Point3D.Parse(expectedPoint);
            AssertGeometry.AreEqual(expected, projectedPoint, eps);
        }

        [TestCase(ZeroPoint, X, Y)] //all 3 points are on the plane z=0. normal vector is +ez.
        [TestCase(ZeroPoint, Y, Z)] //all 3 points are on the plane x=0. normal vector is +ex.
        [TestCase(ZeroPoint, Z, X)] //all 3 points are on the plane y=0. normal vector is +ey.
        public void CreateFittedPlaneFrom_PointsShouldBeOnTheFittedPlane(string sP1, string sP2, string sP3)
        {
            var ps = new[]
            {
                Point3D.Parse(sP1),
                Point3D.Parse(sP2),
                Point3D.Parse(sP3),
            }.ToArray();
            var aPlane = Plane.CreateFittedPlaneFrom(ps);

            var tolerance = 1e-16;
            Assert.That(aPlane.SignedDistanceTo(ps[0]), Is.EqualTo(0).Within(tolerance), "ps[0]");
            Assert.That(aPlane.SignedDistanceTo(ps[1]), Is.EqualTo(0).Within(tolerance), "ps[1]");
            Assert.That(aPlane.SignedDistanceTo(ps[2]), Is.EqualTo(0).Within(tolerance), "ps[2]");
        }

        [TestCase(ZeroPoint, X, Y, XY, Z, "0,0,+1")] //all 4 points fall on the plane z=+1
        [TestCase(ZeroPoint, X, Y, XY, Z, "0,0,-1")] //all 4 points fall on the plane z=-1
        [TestCase(ZeroPoint, Y, Z, YZ, X, "+1,0,0")] //all 4 points fall on the plane x=+1
        [TestCase(ZeroPoint, Y, Z, YZ, X, "-1,0,0")] //all 4 points fall on the plane x=-1
        [TestCase(ZeroPoint, Z, X, ZX, Y, "0,+1,0")] //all 4 points fall on the plane y=+1
        [TestCase(ZeroPoint, Z, X, ZX, Y, "0,-1,0")] //all 4 points fall on the plane y=-1
        public void CreateFittedPlaneFrom_With4PointsOnAxisAlignedPlane(string sV1, string sV2, string sV3, string sV4, string sENormal, string sRootPoint)
        {
            var rootPoint = Point3D.Parse(sRootPoint);
            var ps = new[]
            {
                Vector3D.Parse(sV1),
                Vector3D.Parse(sV2),
                Vector3D.Parse(sV3),
                Vector3D.Parse(sV4),
            }.Select(deltaV => rootPoint + deltaV);
            var actual = Plane.CreateFittedPlaneFrom(ps);

            var eNormal = UnitVector3D.Parse(sENormal);
            var eRootPoint = Point3D.Parse(sRootPoint);
            var expected = new Plane(eNormal, eRootPoint);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("1,1,9;1,2,14;1,3,20;2,1,11;2,2,17;2,3,23;3,1,15;3,2,20;3,3,26", -21.7240278973, -41.0640090729, 7.3269978417, 1)]
        public void CreateFittedPlane(string sPoints, double eA, double eB, double eC, double eOffset)
        {
            //You can see this example at
            //https://math.stackexchange.com/questions/99299/best-fitting-plane-given-a-set-of-points/1652383#1652383

            var points = sPoints.Split(';').Select(s => Point3D.Parse(s));
            var actual = Plane.CreateFittedPlaneFrom(points);

            //normalize (eA, eB, eC and eD).
            //both 2 following equations mean the same plane.
            // eq1: Ax + By + Cz + D = 0
            // eq2:-Ax - By - Cz - D = 0
            var normal = new Vector3D(eA, eB, eC);
            var eNormalizedOffset = eOffset / normal.Length;
            var expected = new Plane(normal.Negate().Normalize(), eNormalizedOffset);

            //for debugging purpose
            TestContext.WriteLine($"{nameof(actual)}={actual}");
            TestContext.WriteLine($"{nameof(expected)}={expected}");

            var tolerance = 6e-3; // for this example
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
        [TestCase("0; 0; 1", NegativeZ, ZeroPoint, 1)]
        [TestCase(ZeroPoint, X, "1; 0; 0", 1)]
        [TestCase("188,6578; 147,0620; 66,0170", Z, "118,6578; 147,0620; 126,1170", 60.1)]
        public void SignedDistanceToPoint(string prps, string pns, string ps, double expected)
        {
            var plane = new Plane(UnitVector3D.Parse(pns), Point3D.Parse(prps));
            var p = Point3D.Parse(ps);
            Assert.AreEqual(expected, plane.SignedDistanceTo(p), 1E-6);
        }

        [TestCase(ZeroPoint, Z, ZeroPoint, Z, 0)]
        [TestCase(ZeroPoint, Z, "0;0;1", Z, 1)]
        [TestCase(ZeroPoint, Z, "0;0;-1", Z, -1)]
        [TestCase(ZeroPoint, NegativeZ, "0;0;-1", Z, 1)]
        public void SignedDistanceToOtherPlane(string prps, string pns, string otherPlaneRootPointString, string otherPlaneNormalString, double expectedValue)
        {
            var plane = new Plane(UnitVector3D.Parse(pns), Point3D.Parse(prps));
            var otherPlane = new Plane(UnitVector3D.Parse(otherPlaneNormalString), Point3D.Parse(otherPlaneRootPointString));
            Assert.AreEqual(expectedValue, plane.SignedDistanceTo(otherPlane), 1E-6);
        }

        [TestCase(ZeroPoint, Z, ZeroPoint, Z, 0)]
        [TestCase(ZeroPoint, Z, ZeroPoint, X, 0)]
        [TestCase(ZeroPoint, Z, "0;0;1", X, 1)]
        public void SignedDistanceToRay(string prps, string pns, string rayThroughPointString, string rayDirectionString, double expectedValue)
        {
            var plane = new Plane(UnitVector3D.Parse(pns), Point3D.Parse(prps));
            var otherPlane = new Ray3D(Point3D.Parse(rayThroughPointString), UnitVector3D.Parse(rayDirectionString));
            Assert.AreEqual(expectedValue, plane.SignedDistanceTo(otherPlane), 1E-6);
        }

        [Test]
        public void ProjectLineOn()
        {
            var unitVector = UnitVector3D.ZAxis;
            var rootPoint = new Point3D(0, 0, 1);
            var plane = new Plane(unitVector, rootPoint);

            var line = new LineSegment3D(new Point3D(0, 0, 0), new Point3D(1, 0, 0));
            var projectOn = plane.Project(line);
            AssertGeometry.AreEqual(new LineSegment3D(new Point3D(0, 0, 1), new Point3D(1, 0, 1)), projectOn, float.Epsilon);
        }

        [Test]
        public void ProjectVectorOn()
        {
            var unitVector = UnitVector3D.ZAxis;
            var rootPoint = new Point3D(0, 0, 1);
            var plane = new Plane(unitVector, rootPoint);
            var vector = new Vector3D(1, 0, 0);
            var projectOn = plane.Project(vector);
            AssertGeometry.AreEqual(new Vector3D(1, 0, 0), projectOn.Direction, float.Epsilon);
            AssertGeometry.AreEqual(new Point3D(0, 0, 1), projectOn.ThroughPoint, float.Epsilon);
        }

        [TestCase("0, 0, 0", "0, 0, 1", "0, 0, 0", "0, 1, 0", "0, 0, 0", "-1, 0, 0")]
        [TestCase("0, 0, 2", "0, 0, 1", "0, 0, 0", "0, 1, 0", "0, 0, 2", "-1, 0, 0")]
        public void InterSectionWithPlane(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2, string eps, string evs)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), UnitVector3D.Parse(unitVector1));
            var plane2 = new Plane(Point3D.Parse(rootPoint2), UnitVector3D.Parse(unitVector2));
            var intersections = new[]
            {
                plane1.IntersectionWith(plane2),
                plane2.IntersectionWith(plane1)
            };
            foreach (var intersection in intersections)
            {
                AssertGeometry.AreEqual(Point3D.Parse(eps), intersection.ThroughPoint);
                AssertGeometry.AreEqual(UnitVector3D.Parse(evs), intersection.Direction);
            }
        }

        [TestCase("0, 0, 0", "0, 0, 1", "0, 0, 0", "0, 0, 1", "0, 0, 0", "0, 0, 0")]
        public void InterSectionWithPlaneTest_BadArgument(string rootPoint1, string unitVector1, string rootPoint2, string unitVector2, string eps, string evs)
        {
            var plane1 = new Plane(Point3D.Parse(rootPoint1), UnitVector3D.Parse(unitVector1));
            var plane2 = new Plane(Point3D.Parse(rootPoint2), UnitVector3D.Parse(unitVector2));

            Assert.Throws<ArgumentException>(() => plane1.IntersectionWith(plane2));
            Assert.Throws<ArgumentException>(() => plane2.IntersectionWith(plane1));
        }

        [Test]
        public void MirrorPoint()
        {
            var plane = new Plane(UnitVector3D.ZAxis, new Point3D(0, 0, 0));
            var point3D = new Point3D(1, 2, 3);
            var mirrorAbout = plane.MirrorAbout(point3D);
            AssertGeometry.AreEqual(new Point3D(1, 2, -3), mirrorAbout, float.Epsilon);
        }

        [Test]
        public void SignOfD()
        {
            var plane1 = new Plane(UnitVector3D.ZAxis, new Point3D(0, 0, 100));
            Assert.AreEqual(-100, plane1.D);
        }

        [Test]
        public void InterSectionPointDifferentOrder()
        {
            var plane1 = new Plane(UnitVector3D.Create(0.8, 0.3, 0.01), new Point3D(20, 0, 0));
            var plane2 = new Plane(UnitVector3D.Create(0.002, 1, 0.1), new Point3D(0, 0, 0));
            var plane3 = new Plane(UnitVector3D.Create(0.5, 0.5, 1), new Point3D(0, 0, -30));
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
            var plane1 = new Plane(Point3D.Parse(rootPoint1), UnitVector3D.Parse(unitVector1));
            var plane2 = new Plane(Point3D.Parse(rootPoint2), UnitVector3D.Parse(unitVector2));
            var plane3 = new Plane(Point3D.Parse(rootPoint3), UnitVector3D.Parse(unitVector3));
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

        [TestCase("1, 1, 0, -12", "-1, 1, 0, -12", "0, 0, 1, -5", "0, 16.970563, 5")]
        public void PointFromPlanes2(string planeString1, string planeString2, string planeString3, string eps)
        {
            var plane1 = this.GetPlaneFrom4Doubles(planeString1);
            var plane2 = this.GetPlaneFrom4Doubles(planeString2);
            var plane3 = this.GetPlaneFrom4Doubles(planeString3);
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
            var plane = new Plane(Point3D.Parse(rootPoint), UnitVector3D.Parse(unitVector));
            AssertXml.XmlRoundTrips(plane, xml, (e, a) => AssertGeometry.AreEqual(e, a));
        }

        private Plane GetPlaneFrom4Doubles(string inputString)
        {
            var numbers = inputString.Split(',').Select(double.Parse).ToArray();
            return new Plane(numbers[0], numbers[1], numbers[2], numbers[3]);
        }
    }
}
