using System;

namespace MathNet.Spatial.Euclidean
{
    [Serializable]
    public struct Circle3D
    {
        public readonly Point3D CenterPoint;
        public readonly UnitVector3D Axis;
        public readonly double Radius;

        public Circle3D(Point3D centerPoint, UnitVector3D axis, double radius)
        {
            this.CenterPoint = centerPoint;
            this.Axis = axis;
            this.Radius = radius;
        }

        public Circle3D(Point3D p1, Point3D p2, UnitVector3D axis)
        {
            this.CenterPoint = Point3D.MidPoint(p1, p2);
            this.Axis = axis;
            this.Radius = p1.DistanceTo(CenterPoint);
        }

        public double Diameter => 2 * Radius;
        public double Circumference => 2 * Math.PI * Radius;
        public double Area => Math.PI * Math.Pow(Radius, 2);
    }
}
