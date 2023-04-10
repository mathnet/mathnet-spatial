using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A coordinate system
    /// </summary>
    [Serializable]
    public class CoordinateSystem : DenseMatrix, IEquatable<CoordinateSystem>, IXmlSerializable
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
        public CoordinateSystem()
            : this(new Point3D(0, 0, 0), Direction.XAxis.ToVector3D(), Direction.YAxis.ToVector3D(), Direction.ZAxis.ToVector3D())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="xAxis">The x axis</param>
        /// <param name="yAxis">The y axis</param>
        /// <param name="zAxis">The z axis</param>
        /// <param name="origin">The origin</param>
        public CoordinateSystem(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis, Point3D origin)
            : this(origin, xAxis, yAxis, zAxis)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="xAxis">The x axis</param>
        /// <param name="yAxis">The y axis</param>
        /// <param name="zAxis">The z axis</param>
        public CoordinateSystem(Point3D origin, Direction xAxis, Direction yAxis, Direction zAxis)
            : this(origin, xAxis.ToVector3D(), yAxis.ToVector3D(), zAxis.ToVector3D())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="xAxis">The x axis</param>
        /// <param name="yAxis">The y axis</param>
        /// <param name="zAxis">The z axis</param>
        public CoordinateSystem(Point3D origin, Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
            : base(4)
        {
            SetColumn(0, new[] { xAxis.X, xAxis.Y, xAxis.Z, 0 });
            SetColumn(1, new[] { yAxis.X, yAxis.Y, yAxis.Z, 0 });
            SetColumn(2, new[] { zAxis.X, zAxis.Y, zAxis.Z, 0 });
            SetColumn(3, new[] { origin.X, origin.Y, origin.Z, 1 });
        }

        ////public CoordinateSystem(Vector3D x, Vector3D y, Vector3D z, Vector3D offsetToBase)
        ////    : this(x, y, z, offsetToBase.ToPoint3D())
        ////{
        ////}

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
        /// </summary>
        /// <param name="matrix">A matrix</param>
        public CoordinateSystem(Matrix<double> matrix)
            : base(4, 4, matrix.ToColumnMajorArray())
        {
            if (matrix.RowCount != 4)
            {
                throw new ArgumentException("RowCount must be 4");
            }

            if (matrix.ColumnCount != 4)
            {
                throw new ArgumentException("ColumnCount must be 4");
            }
        }

        /// <summary>
        /// Gets the X Axis
        /// </summary>
        public Vector3D XAxis
        {
            get
            {
                var row = SubMatrix(0, 3, 0, 1).ToRowMajorArray();
                return new Vector3D(row[0], row[1], row[2]);
            }
        }

        /// <summary>
        /// Gets the Y Axis
        /// </summary>
        public Vector3D YAxis
        {
            get
            {
                var row = SubMatrix(0, 3, 1, 1).ToRowMajorArray();
                return new Vector3D(row[0], row[1], row[2]);
            }
        }

        /// <summary>
        /// Gets the z Axis
        /// </summary>
        public Vector3D ZAxis
        {
            get
            {
                var row = SubMatrix(0, 3, 2, 1).ToRowMajorArray();
                return new Vector3D(row[0], row[1], row[2]);
            }
        }

        /// <summary>
        /// Gets the point of origin
        /// </summary>
        public Point3D Origin
        {
            get
            {
                var row = SubMatrix(0, 3, 3, 1).ToRowMajorArray();
                return new Point3D(row[0], row[1], row[2]);
            }
        }

        /// <summary>
        /// Gets the offset to origin
        /// </summary>
        public Vector3D OffsetToBase => Origin.ToVector3D();

        /// <summary>
        /// Gets the base change matrix
        /// </summary>
        public CoordinateSystem BaseChangeMatrix
        {
            get
            {
                var matrix = Build.DenseOfColumnVectors(XAxis.ToVector(), YAxis.ToVector(), ZAxis.ToVector());
                var cs = new CoordinateSystem(this);
                cs.SetRotationSubMatrix(matrix.Transpose());
                return cs;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified coordinate system is equal.
        /// </summary>
        /// <param name="left">The first coordinate system to compare</param>
        /// <param name="right">The second coordinate system to compare</param>
        /// <returns>True if the coordinate system are the same; otherwise false.</returns>
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
            var z = Vector3D.Parse(match.Groups["zv"].Value);
            return new CoordinateSystem(o, x, y, z);
        }

        /// <summary>
        /// Sets to the matrix of rotation that aligns the 'from' vector with the 'to' vector.
        /// The optional Axis argument may be used when the two vectors are perpendicular and in opposite directions to specify a specific solution, but is otherwise ignored.
        /// </summary>
        /// <param name="fromVector3D">Input Vector object to align from.</param>
        /// <param name="toVector3D">Input Vector object to align to.</param>
        /// <param name="axis">Input Vector object. </param>
        /// <returns>A rotated coordinate system </returns>
        public static CoordinateSystem RotateTo(Direction fromVector3D, Direction toVector3D, Direction? axis = null)
        {
            var r = Matrix3D.RotationTo(fromVector3D, toVector3D, axis);
            var coordinateSystem = new CoordinateSystem();
            var cs = SetRotationSubMatrix(r, coordinateSystem);
            return cs;
        }

        /// <summary>
        /// Creates a coordinate system that rotates
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="v">Vector to rotate about</param>
        /// <returns>A rotating coordinate system</returns>
        public static CoordinateSystem Rotation(Angle angle, Direction v)
        {
            var m = Build.Dense(4, 4);
            m.SetSubMatrix(0, 3, 0, 3, Matrix3D.RotationAroundArbitraryVector(v, angle));
            m[3, 3] = 1;
            return new CoordinateSystem(m);
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
            var cs = new CoordinateSystem(); 
            var cosY = yaw.Cos;
            var sinY = yaw.Sin;
            var cosP = pitch.Cos;
            var sinP = pitch.Sin;
            var cosR = roll.Cos;
            var sinR = roll.Sin;

            cs[0, 0] = cosY * cosP;
            cs[1, 0] = sinY * cosP;
            cs[2, 0] = -sinP;

            cs[0, 1] = cosY * sinP * sinR - sinY * cosR;
            cs[1, 1] = sinY * sinP * sinR + cosY * cosR;
            cs[2, 1] = cosP * sinR;

            cs[0, 2] = cosY * sinP * cosR + sinY * sinR;
            cs[1, 2] = sinY * sinP * cosR - cosY * sinR;
            cs[2, 2] = cosP * cosR;

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
            var m = toCs.Multiply(fromCs.Inverse());
            m[3, 3] = 1;
            return new CoordinateSystem(m);
        }

        /// <summary>
        /// Sets this matrix to be the matrix that maps from the 'from' coordinate system to the 'to' coordinate system.
        /// </summary>
        /// <param name="fromOrigin">Input Point3D that defines the origin to map the coordinate system from.</param>
        /// <param name="fromXAxis">Input Vector3D object that defines the X-axis to map the coordinate system from.</param>
        /// <param name="fromYAxis">Input Vector3D object that defines the Y-axis to map the coordinate system from.</param>
        /// <param name="fromZAxis">Input Vector3D object that defines the Z-axis to map the coordinate system from.</param>
        /// <param name="toOrigin">Input Point3D object that defines the origin to map the coordinate system to.</param>
        /// <param name="toXAxis">Input Vector3D object that defines the X-axis to map the coordinate system to.</param>
        /// <param name="toYAxis">Input Vector3D object that defines the Y-axis to map the coordinate system to.</param>
        /// <param name="toZAxis">Input Vector3D object that defines the Z-axis to map the coordinate system to.</param>
        /// <returns>A mapping coordinate system</returns>
        public static CoordinateSystem SetToAlignCoordinateSystems(Point3D fromOrigin, Vector3D fromXAxis, Vector3D fromYAxis, Vector3D fromZAxis, Point3D toOrigin, Vector3D toXAxis, Vector3D toYAxis, Vector3D toZAxis)
        {
            var cs1 = new CoordinateSystem(fromOrigin, fromXAxis, fromYAxis, fromZAxis);
            var cs2 = new CoordinateSystem(toOrigin, toXAxis, toYAxis, toZAxis);
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
            return new CoordinateSystem(translation.ToPoint3D(), Direction.XAxis, Direction.YAxis, Direction.ZAxis);
        }

        /// <summary>
        /// Creates a rotating coordinate system
        /// </summary>
        /// <param name="r">A 3×3 matrix with the rotation portion</param>
        /// <param name="coordinateSystem">A rotated coordinate system</param>
        /// <returns>A rotating coordinate system</returns>
        public static CoordinateSystem SetRotationSubMatrix(Matrix<double> r, CoordinateSystem coordinateSystem)
        {
            if (r.RowCount != 3 || r.ColumnCount != 3)
            {
                throw new ArgumentOutOfRangeException();
            }

            var cs = new CoordinateSystem(coordinateSystem.Origin, coordinateSystem.XAxis, coordinateSystem.YAxis, coordinateSystem.ZAxis);
            cs.SetSubMatrix(0, r.RowCount, 0, r.ColumnCount, r);
            return cs;
        }

        /// <summary>
        /// Gets a rotation submatrix from a coordinate system
        /// </summary>
        /// <param name="coordinateSystem">a coordinate system</param>
        /// <returns>A rotation matrix</returns>
        public static Matrix<double> GetRotationSubMatrix(CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.SubMatrix(0, 3, 0, 3);
        }

        ////public CoordinateSystem SetCoordinateSystem(Matrix<double> matrix)
        ////{
        ////    if (matrix.ColumnCount != 4 || matrix.RowCount != 4)
        ////        throw new ArgumentException("Not a 4x4 matrix!");
        ////    return new CoordinateSystem(matrix);
        ////}

        /// <summary>
        /// Resets rotations preserves scales
        /// </summary>
        /// <returns>A coordinate system with reset rotation</returns>
        public CoordinateSystem ResetRotations()
        {
            var x = XAxis.Length * Direction.XAxis;
            var y = YAxis.Length * Direction.YAxis;
            var z = ZAxis.Length * Direction.ZAxis;
            return new CoordinateSystem(x, y, z, Origin);
        }

        /// <summary>
        /// Rotates a coordinate system around a vector
        /// </summary>
        /// <param name="about">The vector</param>
        /// <param name="angle">An angle</param>
        /// <returns>A rotated coordinate system</returns>
        public CoordinateSystem RotateCoordSysAroundVector(Direction about, Angle angle)
        {
            var rcs = Rotation(angle, about);
            return rcs.Transform(this);
        }

        /// <summary>
        /// Rotate without Reset
        /// </summary>
        /// <param name="yaw">The yaw</param>
        /// <param name="pitch">The pitch</param>
        /// <param name="roll">The roll</param>
        /// <returns>A rotated coordinate system</returns>
        public CoordinateSystem RotateNoReset(Angle yaw, Angle pitch, Angle roll)
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
            return new CoordinateSystem(Origin + v, XAxis, YAxis, ZAxis);
        }

        /// <summary>
        /// Translates a coordinate system
        /// </summary>
        /// <param name="v">a translation vector</param>
        /// <returns>A translated coordinate system</returns>
        public CoordinateSystem OffsetBy(Direction v)
        {
            return new CoordinateSystem(Origin + v, XAxis, YAxis, ZAxis);
        }

        /// <summary>
        /// Transforms a ray according to this change matrix
        /// </summary>
        /// <param name="r">a ray</param>
        /// <returns>a transformed ray</returns>
        public Line TransformToCoordSys(Line r)
        {
            var p = r.ThroughPoint;
            var uv = r.Direction;

            // The position and the vector are transformed
            var baseChangeMatrix = BaseChangeMatrix;
            var point = baseChangeMatrix.Transform(p) + OffsetToBase;
            var direction = uv.TransformBy(baseChangeMatrix);
            return new Line(point, direction);
        }

        /// <summary>
        /// Transforms a point according to this change matrix
        /// </summary>
        /// <param name="p">a point</param>
        /// <returns>a transformed point</returns>
        public Point3D TransformToCoordSys(Point3D p)
        {
            var baseChangeMatrix = BaseChangeMatrix;
            var point = baseChangeMatrix.Transform(p) + OffsetToBase;
            return point;
        }

        /// <summary>
        /// Transforms a ray according to the inverse of this change matrix
        /// </summary>
        /// <param name="r">a ray</param>
        /// <returns>a transformed ray</returns>
        public Line TransformFromCoordSys(Line r)
        {
            var p = r.ThroughPoint;
            var uv = r.Direction;

            // The position and the vector are transformed
            var point = BaseChangeMatrix.Invert().Transform(p) + OffsetToBase;
            var direction = BaseChangeMatrix.Invert().Transform(uv);
            return new Line(point, direction);
        }

        /// <summary>
        /// Transforms a point according to the inverse of this change matrix
        /// </summary>
        /// <param name="p">a point</param>
        /// <returns>a transformed point</returns>
        public Point3D TransformFromCoordSys(Point3D p)
        {
            var point = BaseChangeMatrix.Invert().Transform(p) + OffsetToBase;
            return point;
        }

        /// <summary>
        /// Creates a rotation submatrix
        /// </summary>
        /// <param name="r">a matrix</param>
        /// <returns>a coordinate system</returns>
        public CoordinateSystem SetRotationSubMatrix(Matrix<double> r)
        {
            return SetRotationSubMatrix(r, this);
        }

        /// <summary>
        /// Returns a translation coordinate system
        /// </summary>
        /// <param name="v">a vector</param>
        /// <returns>a coordinate system</returns>
        public CoordinateSystem SetTranslation(Vector3D v)
        {
            return new CoordinateSystem(v.ToPoint3D(), XAxis, YAxis, ZAxis);
        }

        /// <summary>
        /// Returns a rotation sub matrix
        /// </summary>
        /// <returns>a rotation sub matrix</returns>
        public Matrix<double> GetRotationSubMatrix()
        {
            return GetRotationSubMatrix(this);
        }

        /// <summary>
        /// Given a transform from coordinate system A to coordinate system B, and a vector <paramref name="v"/>
        /// expressed in coordinate system B, it returns the vector expressed in coordinate system A
        /// </summary>
        /// <param name="v">Vector whose coordinates are expressed in coordinate system B</param>
        /// <returns>The vector expressed in coordinate system A</returns>
        public Vector3D Transform(Vector3D v)
        {
            var v3 = Vector<double>.Build.Dense(new[] { v.X, v.Y, v.Z });
            GetRotationSubMatrix().Multiply(v3, v3);
            return new Vector3D(v3[0], v3[1], v3[2]);
        }

        /// <summary>
        /// Given a transform from coordinate system A to coordinate system B, and a vector <paramref name="v"/>
        /// expressed in coordinate system B, it returns the vector expressed in coordinate system A
        /// </summary>
        /// <param name="v">Unit vector whose coordinates are expressed in coordinate system B</param>
        /// <returns>The vector expressed in coordinate system A</returns>
        public Vector3D Transform(Direction v)
        {
            var v3 = Vector<double>.Build.Dense(new[] { v.X, v.Y, v.Z });
            GetRotationSubMatrix().Multiply(v3, v3);
            return new Vector3D(v3[0], v3[1], v3[2]);
        }

        /// <summary>
        /// Given a transform from coordinate system A to coordinate system B, and a point <paramref name="p"/>
        /// expressed in coordinate system B, it returns the point expressed in coordinate system A
        /// </summary>
        /// <param name="p">Point whose coordinates are expressed in coordinate system B</param>
        /// <returns>The point expressed in coordinate system A</returns>
        public Point3D Transform(Point3D p)
        {
            var v4 = Vector<double>.Build.Dense(new[] { p.X, p.Y, p.Z, 1 });
            Multiply(v4, v4);
            return new Point3D(v4[0], v4[1], v4[2]);
        }

        /// <summary>
        /// Transforms a coordinate system and returns the transformed
        /// </summary>
        /// <param name="cs">a coordinate system</param>
        /// <returns>A transformed coordinate system</returns>
        public CoordinateSystem Transform(CoordinateSystem cs)
        {
            return new CoordinateSystem(Multiply(cs));
        }

        /// <summary>
        /// Transforms a line segment.
        /// </summary>
        /// <param name="l">A line segment</param>
        /// <returns>The transformed line segment</returns>
        public LineSegment3D Transform(LineSegment3D l)
        {
            return new LineSegment3D(Transform(l.StartPoint), Transform(l.EndPoint));
        }

        /// <summary>
        /// Transforms a ray and returns the transformed.
        /// </summary>
        /// <param name="ray">A ray</param>
        /// <returns>A transformed ray</returns>
        public Line Transform(Line ray)
        {
            return new Line(Transform(ray.ThroughPoint), Transform(ray.Direction));
        }

        /// <summary>
        /// Transforms a coordinate system
        /// </summary>
        /// <param name="matrix">a matrix</param>
        /// <returns>A transformed coordinate system</returns>
        public CoordinateSystem TransformBy(Matrix<double> matrix)
        {
            return new CoordinateSystem(matrix.Multiply(this));
        }

        /// <summary>
        /// Transforms this by the coordinate system and returns the transformed.
        /// </summary>
        /// <param name="cs">a coordinate system</param>
        /// <returns>a transformed coordinate system</returns>
        public CoordinateSystem TransformBy(CoordinateSystem cs)
        {
            return cs.Transform(this);
        }

        /// <summary>
        /// Inverts this coordinate system
        /// </summary>
        /// <returns>An inverted coordinate system</returns>
        public CoordinateSystem Invert()
        {
            return new CoordinateSystem(Inverse());
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
            if (Values.Length != other?.Values.Length)
            {
                return false;
            }

            for (var i = 0; i < Values.Length; i++)
            {
                if (Math.Abs(Values[i] - other.Values[i]) > tolerance)
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        [Pure]
        public bool Equals(CoordinateSystem other)
        {
            if (Values.Length != other?.Values.Length)
            {
                return false;
            }

            for (var i = 0; i < Values.Length; i++)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Values[i] != other.Values[i])
                {
                    return false;
                }
            }

            return true;
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
        public override int GetHashCode() => HashCode.CombineMany(Values);

        /// <summary>
        /// Returns a string representation of the coordinate system
        /// </summary>
        /// <returns>a string</returns>
        public new string ToString()
        {
            return $"Origin: {Origin}, XAxis: {XAxis}, YAxis: {YAxis}, ZAxis: {ZAxis}";
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

            var xAxis = Vector3D.ReadFrom(e.SingleElementReader("XAxis"));
            SetColumn(0, new[] { xAxis.X, xAxis.Y, xAxis.Z, 0 });

            var yAxis = Vector3D.ReadFrom(e.SingleElementReader("YAxis"));
            SetColumn(1, new[] { yAxis.X, yAxis.Y, yAxis.Z, 0 });

            var zAxis = Vector3D.ReadFrom(e.SingleElementReader("ZAxis"));
            SetColumn(2, new[] { zAxis.X, zAxis.Y, zAxis.Z, 0 });

            var origin = Point3D.ReadFrom(e.SingleElementReader("Origin"));
            SetColumn(3, new[] { origin.X, origin.Y, origin.Z, 1 });
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("Origin", Origin);
            writer.WriteElement("XAxis", XAxis);
            writer.WriteElement("YAxis", YAxis);
            writer.WriteElement("ZAxis", ZAxis);
        }
    }
}
