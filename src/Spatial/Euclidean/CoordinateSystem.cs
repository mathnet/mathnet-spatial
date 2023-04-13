using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A coordinate system
    /// </summary>
    [Serializable]
    public class CoordinateSystem : IEquatable<CoordinateSystem>, IXmlSerializable
    {
        /// <summary>
        /// A local regex pattern for 3D items
        /// </summary>
        private static readonly string Item3DPattern = Parser.Vector3DPattern.Trim('^', '$');

        /// <summary>
        /// A local regex pattern for a coordinate system
        /// </summary>
        private static readonly string CsPattern = string.Format(@"^ *o: *{{(?<op>{0})}} *x: *{{(?<xv>{0})}} *y: *{{(?<yv>{0})}} *z: *{{(?<zv>{0})}} *$", Item3DPattern);

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="matrix">The orientation matrix</param>
        private CoordinateSystem(Point3D origin, RotationMatrix matrix)
        {
            OrientationMatrix = matrix;
            Origin = origin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        public CoordinateSystem()
            : this(Point3D.Origin, RotationMatrix.Identity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="xAxis">The x axis</param>
        /// <param name="yAxis">The y axis</param>
        public CoordinateSystem(Point3D origin, Direction xAxis, Direction yAxis)
            : this(origin, new RotationMatrix(xAxis, yAxis))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="xAxis">The x axis</param>
        /// <param name="yAxis">The y axis</param>
        /// <param name="origin">The origin</param>
        public CoordinateSystem(Vector3D xAxis, Vector3D yAxis, Point3D origin)
            : this(origin, xAxis, yAxis)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="xAxis">The x axis</param>
        /// <param name="yAxis">The y axis</param>
        public CoordinateSystem(Point3D origin, Vector3D xAxis, Vector3D yAxis)
            : this(origin, xAxis.Normalize(), yAxis.Normalize())
        {
        }

        /// <summary>
        /// The rotation matrix which expresses the orientation of this coordinate system
        /// </summary>
        public RotationMatrix OrientationMatrix { get; private set; }

        /// <summary>
        /// Gets the point of origin
        /// </summary>
        public Point3D Origin { get; private set; }

        /// <summary>
        /// The column vector corresponding to the x axis coordinate
        /// </summary>
        public Direction XAxis => OrientationMatrix.XAxis;

        /// <summary>
        /// The column vector corresponding to the y axis coordinate
        /// </summary>
        public Direction YAxis => OrientationMatrix.YAxis;

        /// <summary>
        /// The column vector corresponding to the z axis coordinate
        /// </summary>
        public Direction ZAxis => OrientationMatrix.ZAxis;

        /// <summary>
        /// Gets the offset to origin
        /// </summary>
        public Vector3D OffsetToBase => Origin.ToVector3D();

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified coordinate system is equal.
        /// </summary>
        /// <param name="left">The first coordinate system to compare</param>
        /// <param name="right">The second coordinate system to compare</param>
        /// <returns>True if the coordinate systems are the same; otherwise false.</returns>
        public static bool operator ==(CoordinateSystem left, CoordinateSystem right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified coordinate system is not equal.
        /// </summary>
        /// <param name="left">The first coordinate system to compare</param>
        /// <param name="right">The second coordinate system to compare</param>
        /// <returns>True if the coordinate systems are different; otherwise false.</returns>
        public static bool operator !=(CoordinateSystem left, CoordinateSystem right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Creates a coordinate system from a string
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>A coordinate system</returns>
        public static CoordinateSystem Parse(string s)
        {
            var match = Regex.Match(s, CsPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);
            var o = Point3D.Parse(match.Groups["op"].Value);
            var x = Vector3D.Parse(match.Groups["xv"].Value);
            var y = Vector3D.Parse(match.Groups["yv"].Value);
            return new CoordinateSystem(o, x, y);
        }

        /// <summary>
        /// Sets to the matrix of rotation that aligns the 'from' direction with the 'to' direction.
        /// The optional axis argument may be used when the two directions are perpendicular and in opposite directions to specify a specific solution, but is otherwise ignored.
        /// </summary>
        /// <param name="from">Input direction to align from.</param>
        /// <param name="to">Input direction to align to.</param>
        /// <param name="axis">Input direction. </param>
        /// <returns>A rotated coordinate system </returns>
        public static CoordinateSystem RotateTo(Direction from, Direction to, Direction? axis = null)
        {
            var orientation = RotationMatrix.RotationTo(from, to, axis);
            var coordinateSystem = new CoordinateSystem(Point3D.Origin, orientation);
            return coordinateSystem;
        }

        /// <summary>
        /// Creates a coordinate system that rotates
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="v">Vector to rotate about</param>
        /// <returns>A rotating coordinate system</returns>
        public static CoordinateSystem Rotation(Angle angle, Direction v)
        {
            var orientation = RotationMatrix.AroundAxis(v, angle);
            var coordinateSystem = new CoordinateSystem(Point3D.Origin, orientation);
            return coordinateSystem;
        }

        /// <summary>
        /// Creates a coordinate system that rotates
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="v">Vector to rotate about</param>
        /// <returns>A rotated coordinate system</returns>
        public static CoordinateSystem Rotation(Angle angle, Vector3D v)
        {
            return Rotation(angle, v.Normalize());
        }

        /// <summary>
        /// Successive intrinsic rotations around Z (yaw) then around Y (pitch) and then around X (roll)
        /// Gives an order of magnitude speed improvement.
        /// https://en.wikipedia.org/wiki/Rotation_matrix#General_rotations
        /// </summary>
        /// <param name="yaw">Rotates around Z</param>
        /// <param name="pitch">Rotates around Y</param>
        /// <param name="roll">Rotates around X</param>
        /// <returns>A rotated coordinate system</returns>
        public static CoordinateSystem Rotation(Angle yaw, Angle pitch, Angle roll)
        {
            var rotation = RotationMatrix.Rotation(yaw, pitch, roll);
            var cs = new CoordinateSystem(Point3D.Origin, rotation);
            return cs;
        }

        /// <summary>
        /// Rotates around Z
        /// </summary>
        /// <param name="av">An angle</param>
        /// <returns>A rotated coordinate system</returns>
        public static CoordinateSystem Yaw(Angle av)
        {
            return Rotation(av, Direction.ZAxis);
        }

        /// <summary>
        /// Rotates around Y
        /// </summary>
        /// <param name="av">An angle</param>
        /// <returns>A rotated coordinate system</returns>
        public static CoordinateSystem Pitch(Angle av)
        {
            return Rotation(av, Direction.YAxis);
        }

        /// <summary>
        /// Rotates around X
        /// </summary>
        /// <param name="av">An angle</param>
        /// <returns>A rotated coordinate system</returns>
        public static CoordinateSystem Roll(Angle av)
        {
            return Rotation(av, Direction.XAxis);
        }

        /// <summary>
        /// Creates a coordinate system that maps from the 'from' coordinate system to the 'to' coordinate system.
        /// </summary>
        /// <param name="fromCs">The from coordinate system</param>
        /// <param name="toCs">The to coordinate system</param>
        /// <returns>A mapping coordinate system</returns>
        public static CoordinateSystem CreateMappingCoordinateSystem(CoordinateSystem fromCs, CoordinateSystem toCs)
        {
            var toTranspose = toCs.OrientationMatrix.Transpose();
            var orientationMatrix = toTranspose * fromCs.OrientationMatrix;
            var origin = toTranspose * (fromCs.Origin - toCs.Origin).ToPoint3D();
            return new CoordinateSystem(origin, orientationMatrix);
        }

        /// <summary>
        /// Sets this matrix to be the matrix that maps from the 'from' coordinate system to the 'to' coordinate system.
        /// </summary>
        /// <param name="fromOrigin">Input Point3D that defines the origin to map the coordinate system from.</param>
        /// <param name="fromXAxis">Input Direction that defines the X-axis to map the coordinate system from.</param>
        /// <param name="fromYAxis">Input Direction that defines the Y-axis to map the coordinate system from.</param>
        /// <param name="toOrigin">Input Point3D that defines the origin to map the coordinate system to.</param>
        /// <param name="toXAxis">Input Direction that defines the X-axis to map the coordinate system to.</param>
        /// <param name="toYAxis">Input Direction that defines the Y-axis to map the coordinate system to.</param>
        /// <returns>A mapping coordinate system</returns>
        public static CoordinateSystem SetToAlignCoordinateSystems(
            Point3D fromOrigin,
            Direction fromXAxis,
            Direction fromYAxis,
            Point3D toOrigin,
            Direction toXAxis,
            Direction toYAxis)
        {
            var cs1 = new CoordinateSystem(fromOrigin, fromXAxis, fromYAxis);
            var cs2 = new CoordinateSystem(toOrigin, toXAxis, toYAxis);
            var mcs = CreateMappingCoordinateSystem(cs1, cs2);
            return mcs;
        }

        /// <summary>
        /// Creates a translation
        /// </summary>
        /// <param name="translation">A translation vector</param>
        /// <returns>A translated coordinate system</returns>
        public static CoordinateSystem Translation(Vector3D translation)
        {
            return new CoordinateSystem(translation.ToPoint3D(), RotationMatrix.Identity);
        }

        /// <summary>
        /// Creates a translation
        /// </summary>
        /// <param name="origin">The origin of the translated coordinate system</param>
        /// <returns>A translated coordinate system</returns>
        public static CoordinateSystem Translation(Point3D origin)
        {
            return new CoordinateSystem(origin, RotationMatrix.Identity);
        }

        /// <summary>
        /// Rotates a coordinate system around a vector
        /// </summary>
        /// <param name="about">The vector</param>
        /// <param name="angle">An angle</param>
        /// <returns>A rotated coordinate system</returns>
        public CoordinateSystem Rotate(Direction about, Angle angle)
        {
            var rcs = Rotation(angle, about);
            return rcs.Transform(this);
        }

        /// <summary>
        /// Rotate with successive intrinsic rotations around Z (yaw) then around Y (pitch) and then around X (roll)
        /// </summary>
        /// <param name="yaw">The yaw</param>
        /// <param name="pitch">The pitch</param>
        /// <param name="roll">The roll</param>
        /// <returns>A rotated coordinate system</returns>
        public CoordinateSystem Rotate(Angle yaw, Angle pitch, Angle roll)
        {
            var rcs = Rotation(yaw, pitch, roll);
            return rcs.Transform(this);
        }

        /// <summary>
        /// Translates a coordinate system
        /// </summary>
        /// <param name="v">a translation vector</param>
        /// <returns>A translated coordinate system</returns>
        public CoordinateSystem OffsetBy(Vector3D v)
        {
            return new CoordinateSystem(Origin + v, OrientationMatrix);
        }

        /// <summary>
        /// Translates a coordinate system
        /// </summary>
        /// <param name="v">a translation vector</param>
        /// <returns>A translated coordinate system</returns>
        public CoordinateSystem OffsetBy(Direction v)
        {
            return new CoordinateSystem(Origin + v, OrientationMatrix);
        }

        /// <summary>
        /// Transforms a line according to this change matrix
        /// </summary>
        /// <param name="r">a line</param>
        /// <returns>A transformed line</returns>
        public Line Transform(Line r)
        {
            var p = r.ThroughPoint;
            var uv = r.Direction;
            var point = OrientationMatrix * p + OffsetToBase;
            var direction = OrientationMatrix * uv;
            return new Line(point, direction);
        }

        /// <summary>
        /// Transforms a plane according to this change matrix
        /// </summary>
        /// <param name="p">a plane</param>
        /// <returns>A transformed plane</returns>
        public Plane Transform(Plane p)
        {
            var rootPoint = OrientationMatrix * p.RootPoint + OffsetToBase;
            var normal = OrientationMatrix * p.Normal;
            return new Plane(rootPoint, normal);
        }

        /// <summary>
        /// Transforms a point according to this change matrix
        /// </summary>
        /// <param name="p">a point</param>
        /// <returns>A transformed point</returns>
        public Point3D Transform(Point3D p)
        {
            return OrientationMatrix * p + OffsetToBase;
        }

        /// <summary>
        /// Given a transform from coordinate system A to coordinate system B, and a vector <paramref name="v"/>
        /// expressed in coordinate system B, it returns the vector expressed in coordinate system A
        /// </summary>
        /// <param name="v">Vector whose coordinates are expressed in coordinate system B</param>
        /// <returns>The vector expressed in coordinate system A</returns>
        public Vector3D Transform(Vector3D v)
        {
            return OrientationMatrix * v;
        }

        /// <summary>
        /// Given a transform from coordinate system A to coordinate system B, and a vector <paramref name="v"/>
        /// expressed in coordinate system B, it returns the vector expressed in coordinate system A
        /// </summary>
        /// <param name="v">Unit vector whose coordinates are expressed in coordinate system B</param>
        /// <returns>The direction expressed in coordinate system A</returns>
        public Direction Transform(Direction v)
        {
            return OrientationMatrix * v;
        }

        /// <summary>
        /// Transforms a line segment.
        /// </summary>
        /// <param name="l">A line segment</param>
        /// <returns>The transformed line segment</returns>
        public LineSegment Transform(LineSegment l)
        {
            return new LineSegment(Transform(l.StartPoint), Transform(l.EndPoint));
        }

        /// <summary>
        /// Transforms a coordinate system and returns the transformed
        /// </summary>
        /// <param name="cs">a coordinate system</param>
        /// <returns>A transformed coordinate system</returns>
        public CoordinateSystem Transform(CoordinateSystem cs)
        {
            var orientation = cs.OrientationMatrix * OrientationMatrix.Transpose();
            var origin = cs.Origin + orientation * Origin.ToVector3D().Negate();
            return new CoordinateSystem(origin, orientation);
        }

        /// <summary>
        /// Inverts this coordinate system
        /// </summary>
        /// <returns>An inverted coordinate system</returns>
        public CoordinateSystem Invert()
        {
            var orientation = OrientationMatrix.Inverse();
            var origin = (orientation * Origin).ToVector3D().Negate().ToPoint3D();
            return new CoordinateSystem(origin, orientation);
        }

        /// <summary>
        /// Returns a value to indicate if this CoordinateSystem is equivalent to a another CoordinateSystem
        /// </summary>
        /// <param name="other">The CoordinateSystem to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the CoordinateSystems are equal; otherwise false</returns>
        [Pure]
        public bool Equals(CoordinateSystem other, double tolerance)
        {
            return OrientationMatrix == other.OrientationMatrix
                   && Origin.Equals(other.Origin, tolerance);
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(CoordinateSystem other)
        {
            if (other == null)
            {
                return false;
            }

            return OrientationMatrix == other.OrientationMatrix
                   && Origin.Equals(other.Origin);
        }

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is CoordinateSystem cs && Equals(cs);
        }

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode() => HashCode.Combine(OrientationMatrix, Origin);

        /// <summary>
        /// Returns a string representation of the coordinate system
        /// </summary>
        /// <returns>a string</returns>
        public new string ToString()
        {
            return $"Origin: {Origin}, OrientationMatrix: {OrientationMatrix.ToString()}";
        }

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var e = (XElement)XNode.ReadFrom(reader);

            var xAxis = Vector3D.ReadFrom(e.SingleElementReader("XAxis")).Normalize();
            var yAxis = Vector3D.ReadFrom(e.SingleElementReader("YAxis")).Normalize();

            Origin = Point3D.ReadFrom(e.SingleElementReader("Origin"));
            OrientationMatrix = new RotationMatrix(xAxis, yAxis);
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("Origin", Origin);
            writer.WriteElement("XAxis", OrientationMatrix.XAxis);
            writer.WriteElement("YAxis", OrientationMatrix.YAxis);
        }
    }
}
