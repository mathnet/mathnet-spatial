using MathNet.Spatial.Internals;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Numerics;
using HashCode = MathNet.Spatial.Internals.HashCode;
using MathNet.Spatial.Extensions;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// Describes a standard 2 dimensional circle
    /// </summary>
    [Serializable]
    public struct Circle2D : IEquatable<Circle2D>, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Circle2D"/> struct.
        /// Creates a Circle of a given radius from a center point
        /// </summary>
        /// <param name="center">The location of the center</param>
        /// <param name="radius">The radius of the circle</param>
        public Circle2D(Point2D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Gets the center point of the circle
        /// </summary>
        public Point2D Center { get; private set; }

        /// <summary>
        /// Gets the radius of the circle
        /// </summary>
        public double Radius { get; private set; }

        /// <summary>
        /// Gets the circumference of the circle
        /// </summary>
        [Pure]
        public double Circumference => 2 * Radius * Math.PI;

        /// <summary>
        /// Gets the diameter of the circle
        /// </summary>
        [Pure]
        public double Diameter => 2 * Radius;

        /// <summary>
        /// Gets the area of the circle
        /// </summary>
        [Pure]
        public double Area => Radius * Radius * Math.PI;

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified circles is equal.
        /// </summary>
        /// <param name="left">The first circle to compare.</param>
        /// <param name="right">The second circle to compare.</param>
        /// <returns>True if the circles are the same; otherwise false.</returns>
        public static bool operator ==(Circle2D left, Circle2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified circles is not equal.
        /// </summary>
        /// <param name="left">The first circle to compare.</param>
        /// <param name="right">The second circle to compare</param>
        /// <returns>True if the circles are different; otherwise false.</returns>
        public static bool operator !=(Circle2D left, Circle2D right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Creates a <see cref="Circle2D"/> circle from three points which lie along its circumference.
        /// Points may not be collinear
        /// </summary>
        /// <param name="pointA">The first point on the circle.</param>
        /// <param name="pointB">The second point on the circle.</param>
        /// <param name="pointC">The third point on the circle.</param>
        /// <returns>A Circle which is defined by the three specified points</returns>
        /// <exception cref="ArgumentException">An exception is thrown if no possible circle can be formed from the points</exception>
        public static Circle2D FromPoints(Point2D pointA, Point2D pointB, Point2D pointC)
        {
            // https://mathworld.wolfram.com/Circle.html
            //
            // The equation of a circle passing through A={x1, y1}, B={x2,y2}, and C={x3,y3} is
            //    | x^2+y^2   x  y  1 | = 0
            //    | x1^2+y1^2 x1 y1 1 |
            //    | x2^2+y2^2 x2 y2 1 |
            //    | x3^2+y3^2 x3 y3 1 |
            //
            // By using the cofactor expansion,
            //    (x^2+y^2)*a + x*d + y*e + f = 0
            //    where a = | x1 y1 1 |, d = -| x1^2+y1^2 y1 1 |, e = | x1^2+y1^2 x1 1 |, and f = -| x1^2+y1^2 x1 y1 |
            //              | x2 y2 1 |       | x2^2+y2^2 y2 1 |      | x2^2+y2^2 x2 1 |           | x2^2+y2^2 x2 y2 |
            //              | x3 y3 1 |       | x3^2+y3^2 y3 1 |      | x3^2+y3^2 x3 1 |           | x3^2+y3^2 x3 y3 |
            //
            // In the center-radius form,
            //    (x - x0)^2 + (y - y0^2) = r^2
            //    where x0 = -d/(2a), y0 = -e/(2a), and r = sqrt((d^2+e^2)/(4a^2) - f/a))

            var x1 = pointA.X; var y1 = pointA.Y;
            var x2 = pointB.X; var y2 = pointB.Y;
            var x3 = pointC.X; var y3 = pointC.Y;

            var a = x1 * y2 - x1 * y3 - x2 * y1 + x2 * y3 + x3 * y1 - x3 * y2;
            if (a == 0)
            {
                throw new ArgumentException("Points cannot form a circle, are they collinear?");
            }

            var sq1 = x1 * x1 + y1 * y1;
            var sq2 = x2 * x2 + y2 * y2;
            var sq3 = x3 * x3 + y3 * y3;

            var d = -(sq1 * y2 - sq1 * y3 - sq2 * y1 + sq2 * y3 + sq3 * y1 - sq3 * y2);
            var e = sq1 * x2 - sq1 * x3 - sq2 * x1 + sq2 * x3 + sq3 * x1 - sq3 * x2;
            var f = -(sq1 * x2 * y3 - sq1 * x3 * y2 - sq2 * x1 * y3 + sq2 * x3 * y1 + sq3 * x1 * y2 - sq3 * x2 * y1);

            var centerX = -d / (2 * a);
            var centerY = -e / (2 * a);
            var center = new Point2D(centerX, centerY);
            var radius = Math.Sqrt((d * d + e * e) / (4 * a * a) - f / a); // or radius = center.DistanceTo(pointA);

            return new Circle2D(center, radius);            
        }

        /// <summary>
        /// Returns intersection a point2D array if this circle and the given line have the intersections
        /// </summary>
        /// <param name="line">the given line</param>
        /// <returns>intersections as a Point2D Array, depending on the count.</returns>
        public Point2D[] IntersectWith(Line2D line)
        {
            var ts = this.findParameterTs(line);
            var result = ts
                .Select(t => line.StartPoint + t * line.Direction)
                .ToArray();
            return result;
        }

        private double[] findParameterTs(Line2D line)
        {
            // These 2 equations in vector form can be described
            // (p-cc)^2=r^2 (eq1)
            // p=s+t*d     (eq2)
            // , where p is the point on the line and/or circle,
            // cc is the center of the circle and
            // r is the radius of the circle.
            // Substituting (eq2) into (eq1) yields:
            // ((s+t*d)-c)^2=r^2 (eq3)
            // (eq3) reduces to the following quadratic equation: a*t^2 + b*t + c==0

            var cc = this.Center.ToVector2D(); //center of circle
            var s = line.StartPoint.ToVector2D();
            var d = line.Direction;
            var r = this.Radius;

            var a = d.DotProduct(d);
            var b = 2 * (s.DotProduct(d) - d.DotProduct(cc));
            var c = (s - cc).DotProduct(s - cc) - r * r;

            var soluions = FindRoots.Polynomial(new[] { c, b, a });
            var ts = soluions
                .Where(z => z.IsReal())
                .Select(z => z.Real)
                .ToArray();
            return ts;
        }

            if (discriminant.IsNearlyEqualTo(0, 1e-6))
            {
                var t = -b / (2 * a);
                return new[] { Point2D.OfVector((s + t * d).ToVector()) };
            }


            var t1 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            var t2 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            var ts = new double[] { t1, t2 };
            var result = ts.Select(t => Point2D.OfVector((s + t * d).ToVector())).ToArray();
            return result;
        }

        /// <summary>
        /// Returns a value to indicate if a pair of circles are equal
        /// </summary>
        /// <param name="c">The circle to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the points are equal; otherwise false</returns>
        [Pure]
        public bool Equals(Circle2D c, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(c.Radius - Radius) < tolerance && Center.Equals(c.Center, tolerance);
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(Circle2D c) => Radius.Equals(c.Radius) && Center.Equals(c.Center);

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj) => obj is Circle2D c && Equals(c);

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(Center, Radius);

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc/>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            try
            {
                reader.ReadToFirstDescendant();
                var center = reader.ReadElementAs<Point2D>();
                if (reader.TryReadElementContentAsDouble("Radius", out var radius))
                {
                    Center = center;
                    Radius = radius;
                    reader.Skip();
                    return;
                }
            }
            catch
            {
                // ignored
            }

            throw new XmlException($"Could not read a {GetType()}");
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("Center", Center);
            writer.WriteElement("Radius", Radius, "G17");
        }
    }
}
