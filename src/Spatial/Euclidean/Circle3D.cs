namespace MathNet.Spatial.Euclidean
{
    using System;

    [Serializable]
    public struct Circle3D
    {
        /// <summary>
        /// The center of the circle
        /// </summary>
        public readonly Point3D CenterPoint;
        /// <summary>
        /// the axis of the circle
        /// </summary>
        public readonly UnitVector3D Axis;
        /// <summary>
        /// the radius of the circle
        /// </summary>
        public readonly double Radius;

        /// <summary>
        /// Constructs a Circle3D with a given <paramref name="radius"/> at a <paramref name="centerPoint"/> orientated to the <paramref name="axis"/>
        /// </summary>
        /// <param name="centerPoint">The center of the circle</param>
        /// <param name="axis">the axis of the circle</param>
        /// <param name="radius">the radius of the circle</param>
        public Circle3D(Point3D centerPoint, UnitVector3D axis, double radius)
        {
            this.CenterPoint = centerPoint;
            this.Axis = axis;
            this.Radius = radius;
        }

        /// <summary>
        /// Create a circle from the midpoint between two points, in a direction along a specified axis
        /// </summary>
        /// <param name="p1">First point on the circumference of the circle</param>
        /// <param name="p2">Second point on the circumference of the circle</param>
        /// <param name="axis">Direction of the plane in which the circle lies</param>
        public Circle3D(Point3D p1, Point3D p2, UnitVector3D axis)
        {
            this.CenterPoint = Point3D.MidPoint(p1, p2);
            this.Axis = axis;
            this.Radius = p1.DistanceTo(CenterPoint);
        }

        /// <summary>
        /// Create a circle from three points which lie along its circumference.
        /// </summary>
        /// <param name="p1">a point on the circle</param>
        /// <param name="p2">a point on the circle</param>
        /// <param name="p3">a point on the circle</param>
        public Circle3D(Point3D p1, Point3D p2, Point3D p3)
        {
            // https://www.physicsforums.com/threads/equation-of-a-circle-through-3-points-in-3d-space.173847/
            Vector3D p1p2 = p2 - p1;
            Vector3D p2p3 = p3 - p2;
            this.Axis = p1p2.CrossProduct(p2p3).Normalize();

            Point3D midPointA = p1 + 0.5 * p1p2;
            Point3D midPointB = p2 + 0.5 * p2p3;

            Vector3D directionA = p1p2.CrossProduct(this.Axis);
            Vector3D directionB = p2p3.CrossProduct(this.Axis);

            Ray3D bisectorA = new Ray3D(midPointA, directionA);
            Plane bisectorB = new Plane(midPointB, midPointB + directionB.Normalize(), midPointB + this.Axis);

            var center = bisectorA.IntersectionWith(bisectorB);
            if (center == null)
                throw new ArgumentException("A circle cannot be created from these points, are they colinear?");

            this.CenterPoint = (Point3D)center;
            this.Radius = this.CenterPoint.DistanceTo(p1);

        }

        /// <summary>
        /// Returns the diameter of the circle
        /// </summary>
        public double Diameter => 2 * Radius;
        /// <summary>
        /// Returns the circumference of the circle
        /// </summary>
        public double Circumference => 2 * Math.PI * Radius;
        /// <summary>
        /// returns the area of the circle
        /// </summary>
        public double Area => Math.PI * Math.Pow(Radius, 2);
    }
}
