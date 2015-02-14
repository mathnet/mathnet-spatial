using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    [Serializable]
    public class CoordinateSystem : Numerics.LinearAlgebra.Double.DenseMatrix, IEquatable<CoordinateSystem>, IXmlSerializable
    {
        static string _item3DPattern = Parser.Vector3DPattern.Trim('^', '$');

        public static readonly string CsPattern = string.Format(@"^ *o: *{{(?<op>{0})}} *x: *{{(?<xv>{0})}} *y: *{{(?<yv>{0})}} *z: *{{(?<zv>{0})}} *$", _item3DPattern);

        public CoordinateSystem()
            : this(new Point3D(0, 0, 0), UnitVector3D.XAxis.ToVector3D(), UnitVector3D.YAxis.ToVector3D(), UnitVector3D.ZAxis.ToVector3D())
        {
        }

        public CoordinateSystem(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis, Point3D origin)
            : this(origin, xAxis, yAxis, zAxis)
        {
        }

        public CoordinateSystem(Point3D origin, UnitVector3D xAxis, UnitVector3D yAxis, UnitVector3D zAxis)
            : this(origin, xAxis.ToVector3D(), yAxis.ToVector3D(), zAxis.ToVector3D())
        {
        }

        public CoordinateSystem(Point3D origin, Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
            : base(4)
        {
            this.SetColumn(0, new[] { xAxis.X, xAxis.Y, xAxis.Z, 0 });
            this.SetColumn(1, new[] { yAxis.X, yAxis.Y, yAxis.Z, 0 });
            this.SetColumn(2, new[] { zAxis.X, zAxis.Y, zAxis.Z, 0 });
            this.SetColumn(3, new[] { origin.X, origin.Y, origin.Z, 1 });
        }

        ////public CoordinateSystem(Vector3D x, Vector3D y, Vector3D z, Vector3D offsetToBase)
        ////    : this(x, y, z, offsetToBase.ToPoint3D())
        ////{
        ////}
        public CoordinateSystem(Matrix<double> matrix)
            : base(4, 4, matrix.ToColumnWiseArray())
        {
            if (matrix.RowCount != 4)
            {
                throw new ArgumentException("Rowcount must be 4");
            }

            if (matrix.ColumnCount != 4)
            {
                throw new ArgumentException("Rowcount must be 4");
            }
        }

        public Vector3D XAxis
        {
            get { return new Vector3D(this.SubMatrix(0, 3, 0, 1).ToRowWiseArray()); }
        }

        public Vector3D YAxis
        {
            get { return new Vector3D(this.SubMatrix(0, 3, 1, 1).ToRowWiseArray()); }
        }

        public Vector3D ZAxis
        {
            get { return new Vector3D(this.SubMatrix(0, 3, 2, 1).ToRowWiseArray()); }
        }

        public Point3D Origin
        {
            get { return new Point3D(this.SubMatrix(0, 3, 3, 1).ToRowWiseArray()); }
        }

        public Vector3D OffsetToBase
        {
            get { return this.Origin.ToVector3D(); }
        }

        /// <summary>
        ///
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
        public static CoordinateSystem RotateTo(UnitVector3D fromVector3D, UnitVector3D toVector3D, UnitVector3D? axis = null)
        {
            Matrix<double> r = Matrix3D.RotationTo(fromVector3D, toVector3D, axis);
            var coordinateSystem = new CoordinateSystem();
            CoordinateSystem cs = SetRotationSubMatrix(r, coordinateSystem);
            return cs;
        }

        /// <summary>
        /// Creates a coordinate system that rotates 
        /// </summary>
        /// <param name="a">Angle to rotate</param>
        /// <param name="unit">The unit of the angle</param>
        /// <param name="v">Vector to rotate about</param>
        /// <returns></returns>
        public static CoordinateSystem Rotation<T>(double a, T unit, UnitVector3D v) where T : IAngleUnit
        {
            return Rotation(Angle.From(a, unit), v);
        }

        /// <summary>
        /// Creates a coordinate system that rotates 
        /// </summary>
        /// <param name="a">Angle to rotate</param>
        /// <param name="unit">The unit of the angle</param>
        /// <param name="v">Vector to rotate about</param>
        /// <returns></returns>
        public static CoordinateSystem Rotation<T>(double a, T unit, Vector3D v) where T : IAngleUnit
        {
            return Rotation(Angle.From(a, unit), v.Normalize());
        }

        /// <summary>
        /// Creates a coordinate system that rotates 
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="v">Vector to rotate about</param>
        /// <returns></returns>
        public static CoordinateSystem Rotation(Angle angle, UnitVector3D v)
        {
            var m = Matrix<double>.Build.Dense(4, 4);
            m.SetSubMatrix(0, 3, 0, 3, Matrix3D.RotationAroundArbitraryVector(v, angle));
            m[3, 3] = 1;
            return new CoordinateSystem(m);
        }

        /// <summary>
        /// Creates a coordinate system that rotates
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="v">Vector to rotate about</param>
        /// <returns></returns>
        public static CoordinateSystem Rotation(Angle angle, Vector3D v)
        {
            return Rotation(angle, v.Normalize());
        }

        /// <summary>
        /// Rotation around Z (yaw) then around Y (pitch) and then around X (roll)
        /// http://en.wikipedia.org/wiki/Aircraft_principal_axes
        /// </summary>
        /// <param name="yaw">Rotates around Z</param>
        /// <param name="pitch">Rotates around Y</param>
        /// <param name="roll">Rotates around X</param>
        /// <param name="unit"></param>
        public static CoordinateSystem Rotation<T>(double yaw, double pitch, double roll, T unit) where T : IAngleUnit
        {
            var ya = Angle.From(yaw, unit);
            var ra = Angle.From(roll, unit);
            var pa = Angle.From(pitch, unit);
            return Rotation(ya, pa, ra);
        }

        /// <summary>
        /// Rotation around Z (yaw) then around Y (pitch) and then around X (roll)
        /// http://en.wikipedia.org/wiki/Aircraft_principal_axes
        /// </summary>
        /// <param name="yaw">Rotates around Z</param>
        /// <param name="pitch">Rotates around Y</param>
        /// <param name="roll">Rotates around X</param>
        public static CoordinateSystem Rotation(Angle yaw, Angle pitch, Angle roll)
        {
            var cs = new CoordinateSystem();
            var yt = Yaw(yaw);
            var pt = Pitch(pitch);
            var rt = Roll(roll);
            return rt.Transform(pt.Transform(yt.Transform(cs)));
        }

        /// <summary>
        /// Rotates around Z
        /// </summary>
        /// <param name="a"></param>
        /// <param name="unit"></param>
        public static CoordinateSystem Yaw<T>(double a, T unit) where T : IAngleUnit
        {
            return Yaw(Angle.From(a, unit));
        }

        /// <summary>
        /// Rotates around Z
        /// </summary>
        /// <param name="av"></param>
        public static CoordinateSystem Yaw(Angle av)
        {
            return Rotation(av, UnitVector3D.ZAxis);
        }

        /// <summary>
        /// Rotates around Y
        /// </summary>
        /// <param name="a"></param>
        /// <param name="unit"></param>
        public static CoordinateSystem Pitch<T>(double a, T unit) where T : IAngleUnit
        {
            return Pitch(Angle.From(a, unit));
        }

        /// <summary>
        /// Rotates around Y
        /// </summary>
        /// <param name="av"></param>
        public static CoordinateSystem Pitch(Angle av)
        {
            return Rotation(av, UnitVector3D.YAxis);
        }

        /// <summary>
        /// Rotates around X
        /// </summary>
        /// <param name="a"></param>
        /// <param name="unit"></param>
        public static CoordinateSystem Roll<T>(double a, T unit) where T : IAngleUnit
        {
            return Roll(Angle.From(a, unit));
        }

        /// <summary>
        /// Rotates around X
        /// </summary>
        /// <param name="av"></param>
        public static CoordinateSystem Roll(Angle av)
        {
            return Rotation(av, UnitVector3D.XAxis);
        }

        /// <summary>
        /// Creates a coordinate system that maps from the 'from' coordinate system to the 'to' coordinate system.
        /// </summary>
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
        public static CoordinateSystem SetToAlignCoordinateSystems(Point3D fromOrigin, Vector3D fromXAxis, Vector3D fromYAxis, Vector3D fromZAxis, Point3D toOrigin, Vector3D toXAxis, Vector3D toYAxis, Vector3D toZAxis)
        {
            var cs1 = new CoordinateSystem(fromOrigin, fromXAxis, fromYAxis, fromZAxis);
            var cs2 = new CoordinateSystem(toOrigin, toXAxis, toYAxis, toZAxis);
            CoordinateSystem mcs = CreateMappingCoordinateSystem(cs1, cs2);
            return mcs;
        }

        /// <summary>
        /// Creates a translation
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public static CoordinateSystem Translation(Vector3D translation)
        {
            return new CoordinateSystem(translation.ToPoint3D(), UnitVector3D.XAxis, UnitVector3D.YAxis, UnitVector3D.ZAxis);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="r">A 3×3 matrix with the rotation portion</param>
        /// <param name="coordinateSystem"></param>
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

        public static Matrix<double> GetRotationSubMatrix(CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.SubMatrix(0, 3, 0, 3);
        }

        public static bool operator ==(CoordinateSystem left, CoordinateSystem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoordinateSystem left, CoordinateSystem right)
        {
            return !Equals(left, right);
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
        public CoordinateSystem ResetRotations()
        {
            var x = this.XAxis.Length * UnitVector3D.XAxis;
            var y = this.YAxis.Length * UnitVector3D.YAxis;
            var z = this.ZAxis.Length * UnitVector3D.ZAxis;
            return new CoordinateSystem(x, y, z, this.Origin);
        }

        public CoordinateSystem RotateCoordSysAroundVector<T>(Vector3D aboutVector3D, double angle, T angleUnit)
            where T : IAngleUnit
        {
            var rcs = Rotation(Angle.From(angle, angleUnit), aboutVector3D.Normalize());
            return rcs.Transform(this);
        }

        public CoordinateSystem RotateNoReset<T>(double yaw, double pitch, double roll, T angleUnit) where T : IAngleUnit
        {
            var rcs = Rotation(yaw, pitch, roll, angleUnit);
            return rcs.Transform(this);
        }

        public CoordinateSystem OffsetBy(Vector3D v)
        {
            return new CoordinateSystem(this.Origin + v, this.XAxis, this.YAxis, this.ZAxis);
        }

        public CoordinateSystem OffsetBy(UnitVector3D v)
        {
            return new CoordinateSystem(this.Origin + v, this.XAxis, this.YAxis, this.ZAxis);
        }

        public Ray3D TransformToCoordSys(Ray3D r)
        {
            var p = r.ThroughPoint;
            var uv = r.Direction;

            // positionen och vektorn transformeras
            var baseChangeMatrix = this.BaseChangeMatrix;
            var point = baseChangeMatrix.Transform(p) + this.OffsetToBase;
            var direction = uv.TransformBy(baseChangeMatrix);
            return new Ray3D(point, direction);
        }

        public Point3D TransformToCoordSys(Point3D p)
        {
            var baseChangeMatrix = this.BaseChangeMatrix;
            var point = baseChangeMatrix.Transform(p) + this.OffsetToBase;
            return point;
        }

        public Ray3D TransformFromCoordSys(Ray3D r)
        {
            var p = r.ThroughPoint;
            var uv = r.Direction;

            // positionen och vektorn transformeras
            var point = this.BaseChangeMatrix.Invert().Transform(p) + this.OffsetToBase;
            var direction = this.BaseChangeMatrix.Invert().Transform(uv);
            return new Ray3D(point, direction);
        }

        public Point3D TransformFromCoordSys(Point3D p)
        {
            var point = this.BaseChangeMatrix.Invert().Transform(p) + this.OffsetToBase;
            return point;
        }

        public CoordinateSystem SetRotationSubMatrix(Matrix<double> r)
        {
            return SetRotationSubMatrix(r, this);
        }

        public CoordinateSystem SetTranslation(Vector3D v)
        {
            return new CoordinateSystem(v.ToPoint3D(), this.XAxis, this.YAxis, this.ZAxis);
        }

        public Matrix<double> GetRotationSubMatrix()
        {
            return GetRotationSubMatrix(this);
        }

        /// <summary>
        /// Transforms a vector and returns the transformed vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3D Transform(Vector3D v)
        {
            var v3 = Vector<double>.Build.Dense(new[] { v.X, v.Y, v.Z });
            this.GetRotationSubMatrix().Multiply(v3, v3);
            return new Vector3D(v3[0], v3[1], v3[2]);
        }

        /// <summary>
        /// Transforms a vector and returns the transformed vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3D Transform(UnitVector3D v)
        {
            var v3 = Vector<double>.Build.Dense(new[] { v.X, v.Y, v.Z });
            this.GetRotationSubMatrix().Multiply(v3, v3);
            return new Vector3D(v3[0], v3[1], v3[2]);
        }

        /// <summary>
        /// Transforms a point and returns the transformed point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point3D Transform(Point3D p)
        {
            var v4 = Vector<double>.Build.Dense(new[] { p.X, p.Y, p.Z, 1 });
            Multiply(v4, v4);
            return new Point3D(v4[0], v4[1], v4[2]);
        }

        /// <summary>
        /// Transforms a coordinate system and returns the transformed
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public CoordinateSystem Transform(CoordinateSystem cs)
        {
            return new CoordinateSystem(this.Multiply(cs));
        }

        /// <summary>
        /// Transforms a line and returns the transformed.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public Line3D Transform(Line3D l)
        {
            return new Line3D(this.Transform(l.StartPoint), this.Transform(l.EndPoint));
        }

        /// <summary>
        /// Transforms a ray and returns the transformed.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public Ray3D Transform(Ray3D ray)
        {
            return new Ray3D(this.Transform(ray.ThroughPoint), this.Transform(ray.Direction));
        }

        public CoordinateSystem TransformBy(Matrix<double> matrix)
        {
            return new CoordinateSystem(matrix.Multiply(this));
        }

        /// <summary>
        /// Transfomes this by the coordinate system and returns the tranformed.
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public CoordinateSystem TransformBy(CoordinateSystem cs)
        {
            return cs.Transform(this);
        }

        public CoordinateSystem Invert()
        {
            return new CoordinateSystem(this.Inverse());
        }

        public bool Equals(CoordinateSystem other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.Values.Length != this.Values.Length)
            {
                return false;
            }

            return !this.Values.Where((t, i) => Math.Abs(other.Values[i] - t) > 1E-15).Any();
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(CoordinateSystem))
            {
                return false;
            }

            return this.Equals((CoordinateSystem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.XAxis.GetHashCode();
                result = (result * 397) ^ this.YAxis.GetHashCode();
                result = (result * 397) ^ this.ZAxis.GetHashCode();
                result = (result * 397) ^ this.OffsetToBase.GetHashCode();
                return result;
            }
        }

        public new string ToString()
        {
            return string.Format("Origin: {0}, XAxis: {1}, YAxis: {2}, ZAxis: {3}", this.Origin, this.XAxis, this.YAxis, this.ZAxis);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var e = (XElement)XNode.ReadFrom(reader);

            var xAxis = new Vector3D(double.NaN, double.NaN, double.NaN);
            xAxis.ReadXml(e.SingleElementReader("XAxis"));
            this.SetColumn(0, new[] { xAxis.X, xAxis.Y, xAxis.Z, 0 });

            var yAxis = new Vector3D(double.NaN, double.NaN, double.NaN);
            yAxis.ReadXml(e.SingleElementReader("YAxis"));
            this.SetColumn(1, new[] { yAxis.X, yAxis.Y, yAxis.Z, 0 });

            var zAxis = new Vector3D(double.NaN, double.NaN, double.NaN);
            zAxis.ReadXml(e.SingleElementReader("ZAxis"));
            this.SetColumn(2, new[] { zAxis.X, zAxis.Y, zAxis.Z, 0 });

            var origin = new Point3D(double.NaN, double.NaN, double.NaN);
            origin.ReadXml(e.SingleElementReader("Origin"));
            this.SetColumn(3, new[] { origin.X, origin.Y, origin.Z, 1 });
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElement("Origin", this.Origin);
            writer.WriteElement("XAxis", this.XAxis);
            writer.WriteElement("YAxis", this.YAxis);
            writer.WriteElement("ZAxis", this.ZAxis);
        }
    }
}
