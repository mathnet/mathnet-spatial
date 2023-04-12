using MathNet.Spatial.Units;
using System;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A struct representing a rotation of a coordinate system.
    /// By rotating a coordinate system A so that coordinate system B is created,
    /// this matrix represents the transformation "from B back to A"
    /// </summary>
    [Serializable]
    public readonly struct RotationMatrix : IEquatable<RotationMatrix>
    {
        /// <summary>
        /// The row which is multiplied with all x coordinates (first row in the matrix)
        /// </summary>
        private readonly Direction _xRow;

        /// <summary>
        /// The row which is multiplied with all y coordinates (second row in the matrix)
        /// </summary>
        private readonly Direction _yRow;

        /// <summary>
        /// The row which is multiplied with all z coordinates (third row in the matrix)
        /// </summary>
        private readonly Direction _zRow;

        /// <summary>
        /// The column vector corresponding to the x axis coordinate
        /// </summary>
        public Direction XAxis => Direction.Create(_xRow.X, _yRow.X, _zRow.X);

        /// <summary>
        /// The column vector corresponding to the y axis coordinate
        /// </summary>
        public Direction YAxis => Direction.Create(_xRow.Y, _yRow.Y, _zRow.Y);

        /// <summary>
        /// The column vector corresponding to the z axis coordinate
        /// </summary>
        public Direction ZAxis => Direction.Create(_xRow.Z, _yRow.Z, _zRow.Z);

        /// <summary>
        /// The identity rotation matrix (no rotation)
        /// </summary>
        public static readonly RotationMatrix Identity = new RotationMatrix(
            Direction.XAxis,
            Direction.YAxis,
            Direction.ZAxis);

        /// <summary>
        /// Private constructor for access within the struct
        /// </summary>
        private RotationMatrix(
            double m11,
            double m12,
            double m13,
            double m21,
            double m22,
            double m23,
            double m31,
            double m32,
            double m33)
        {
            _xRow = Direction.Create(m11, m12, m13);
            _yRow = Direction.Create(m21, m22, m23);
            _zRow = Direction.Create(m31, m32, m33);
        }

        /// <summary>
        /// Private constructor for access within the struct
        /// </summary>
        private RotationMatrix(
            Direction xRow,
            Direction yRow,
            Direction zRow)
        {
            _xRow = xRow;
            _yRow = yRow;
            _zRow = zRow;
        }

        internal RotationMatrix(Direction xAxis, Direction yAxis)
        {
            if (!xAxis.IsPerpendicularTo(yAxis))
            {
                throw new ArgumentException($"The axes {xAxis} and {yAxis} are not perpendicular to each other");
            }

            var zAxis = xAxis.CrossProduct(yAxis);
            _xRow = Direction.Create(xAxis.X, yAxis.X, zAxis.X);
            _yRow = Direction.Create(xAxis.Y, yAxis.Y, zAxis.Y);
            _zRow = Direction.Create(xAxis.Z, yAxis.Z, zAxis.Z);
        }

        /// <summary>
        /// Returns the transpose of this rotation matrix, which is also its inverse <seealso cref="Inverse"/>
        /// </summary>
        /// <returns>The transpose of this instance</returns>
        public RotationMatrix Transpose()
        {
            return new RotationMatrix(
                Direction.Create(_xRow.X, _yRow.X, _zRow.X),
                Direction.Create(_xRow.Y, _yRow.Y, _zRow.Y),
                Direction.Create(_xRow.Z, _yRow.Z, _zRow.Z));
        }

        /// <summary>
        /// Returns the inverse of this rotation matrix, which is also its transpose <seealso cref="Transpose"/>
        /// </summary>
        /// <returns>The inverse of this instance</returns>
        public RotationMatrix Inverse()
        {
            return Transpose();
        }

        /// <summary>
        /// Apply a rotation matrix to a given direction
        /// </summary>
        /// <param name="matrix">A matrix from a coordinate system B to a coordinate system A</param>
        /// <param name="direction">A direction expressed in coordinate system B</param>
        /// <returns>The direction expressed in coordinate system A</returns>
        public static Direction operator *(RotationMatrix matrix, Direction direction)
        {
            var v = Multiply(matrix, new Double3(direction));
            return Direction.Create(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Apply a rotation matrix to a given point
        /// </summary>
        /// <param name="matrix">A matrix from a coordinate system B to a coordinate system A</param>
        /// <param name="p">A point expressed in coordinate system B</param>
        /// <returns>The point expressed in coordinate system A</returns>
        public static Point3D operator *(RotationMatrix matrix, Point3D p)
        {
            var v = Multiply(matrix, new Double3(p));
            return new Point3D(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Apply a rotation matrix to a given vector
        /// </summary>
        /// <param name="matrix">A matrix from a coordinate system B to a coordinate system A</param>
        /// <param name="v">A vector expressed in coordinate system B</param>
        /// <returns>The vector expressed in coordinate system A</returns>
        public static Vector3D operator *(RotationMatrix matrix, Vector3D v)
        {
            var vec = Multiply(matrix, new Double3(v));
            return new Vector3D(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Combines two successive rotations into one
        /// </summary>
        /// <param name="matrix1">The rotation matrix from coordinate system B to coordinate system A</param>
        /// <param name="matrix2">The rotation matrix from coordinate system C to coordinate system B</param>
        /// <returns>The rotation matrix from coordinate system C to coordinate system A</returns>
        public static RotationMatrix operator *(RotationMatrix matrix1, RotationMatrix matrix2)
        {
            var transpose = matrix2.Transpose();

            var xColumn = Multiply(matrix1, transpose._xRow);
            var yColumn = Multiply(matrix1, transpose._yRow);
            var zColumn = Multiply(matrix1, transpose._zRow);

            return new RotationMatrix(
                Direction.Create(xColumn.X, yColumn.X, zColumn.X),
                Direction.Create(xColumn.Y, yColumn.Y, zColumn.Y),
                Direction.Create(xColumn.Z, yColumn.Z, zColumn.Z));
        }

        /// <summary>
        /// Creates a rotation matrix around the X axis
        /// </summary>
        /// <param name="angle">The angle to rotate</param>
        /// <returns>A rotation matrix</returns>
        public static RotationMatrix AroundX(Angle angle)
        {
            return AroundAxis(Direction.XAxis, angle);
        }

        /// <summary>
        /// Creates a rotation matrix around the Y axis
        /// </summary>
        /// <param name="angle">The angle to rotate</param>
        /// <returns>A rotation matrix</returns>
        public static RotationMatrix AroundY(Angle angle)
        {
            return AroundAxis(Direction.YAxis, angle);
        }

        /// <summary>
        /// Creates a rotation matrix around the Z axis
        /// </summary>
        /// <param name="angle">The angle to rotate</param>
        /// <returns>A rotation matrix</returns>
        public static RotationMatrix AroundZ(Angle angle)
        {
            return AroundAxis(Direction.ZAxis, angle);
        }

        /// <summary>
        /// Sets to the matrix of rotation that would align the 'from' vector with the 'to' vector.
        /// The optional axis argument may be used when the two vectors are parallel and in opposite directions to specify a specific solution, but is otherwise ignored.
        /// </summary>
        /// <param name="fromVector">Input Vector object to align from.</param>
        /// <param name="toVector">Input Vector object to align to.</param>
        /// <param name="axis">Input Vector object.</param>
        /// <returns>A rotation matrix</returns>
        public static RotationMatrix RotationTo(
            Vector3D fromVector,
            Vector3D toVector,
            Direction? axis = null)
        {
            return RotationTo(fromVector.Normalize(), toVector.Normalize(), axis);
        }

        /// <summary>
        /// Calculates the matrix of rotation that would align the 'from' vector with the 'to' vector.
        /// The optional axis argument may be used when the two vectors are parallel and in opposite directions to specify a specific solution, but is otherwise ignored.
        /// </summary>
        /// <param name="fromVector">Input direction to align from.</param>
        /// <param name="toVector">Input direction to align to.</param>
        /// <param name="axis">Input direction. </param>
        /// <returns>A rotation matrix</returns>
        public static RotationMatrix RotationTo(
            Direction fromVector,
            Direction toVector,
            Direction? axis = null)
        {
            if (fromVector == toVector)
            {
                return Identity;
            }

            if (fromVector.IsParallelTo(toVector))
            {
                if (axis == null)
                {
                    axis = fromVector.Orthogonal;
                }
            }
            else
            {
                axis = fromVector.CrossProduct(toVector);
            }

            var signedAngleTo = fromVector.SignedAngleTo(toVector, axis.Value);
            return AroundAxis(axis.Value, signedAngleTo);
        }

        /// <summary>
        /// Creates a rotation matrix around an arbitrary axis by a given angle
        /// </summary>
        /// <param name="axis">The direction of the rotation axis</param>
        /// <param name="angle">The rotation angle (including sign)</param>
        /// <returns>A rotation matrix</returns>
        public static RotationMatrix AroundAxis(Direction axis, Angle angle)
        {
            // https://en.wikipedia.org/wiki/Rotation_matrix#Rotation_matrix_from_axis_and_angle
            var c = angle.Cos;
            var s = angle.Sin;

            var ux = axis.X;
            var uy = axis.Y;
            var uz = axis.Z;

            var oneMinusCos = 1 - c;

            return new RotationMatrix(
                c + ux * ux * oneMinusCos,
                ux * uy * oneMinusCos - uz * s,
                ux * uz * oneMinusCos + uy * s,
                uy * ux * oneMinusCos + uz * s,
                c + uy * uy * oneMinusCos,
                uy * uz * oneMinusCos - ux * s,
                uz * ux * oneMinusCos - uy * s,
                uz * uy * oneMinusCos + ux * s,
                c + uz * uz * oneMinusCos);
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
        public static RotationMatrix Rotation(Angle yaw, Angle pitch, Angle roll)
        {
            var cosY = yaw.Cos;
            var sinY = yaw.Sin;
            var cosP = pitch.Cos;
            var sinP = pitch.Sin;
            var cosR = roll.Cos;
            var sinR = roll.Sin;

            var xt = Direction.Create(
                cosY * cosP,
                sinY * cosP,
                -sinP);

            var yt = Direction.Create(
                cosY * sinP * sinR - sinY * cosR,
                sinY * sinP * sinR + cosY * cosR,
                cosP * sinR);

            var zt = Direction.Create(
                cosY * sinP * cosR + sinY * sinR,
                sinY * sinP * cosR - cosY * sinR,
                cosP * cosR);

            var m = new RotationMatrix(xt, yt, zt);
            return m.Transpose();
        }

        /// <summary>
        /// Performs the multiplication of this matrix with a direction
        /// </summary>
        private static Double3 Multiply(RotationMatrix matrix, Direction dir)
        {
            return Multiply(matrix, new Double3(dir));
        }

        /// <summary>
        /// Compares this matrix with another rotation matrix
        /// </summary>
        /// <param name="other">The rotation matrix to compare against</param>
        /// <returns>True if matrices are equal, otherwise false</returns>
        public bool Equals(RotationMatrix other)
        {
            return _xRow == other._xRow
                   && _yRow == other._yRow
                   && _zRow == other._zRow;
        }


        /// <summary>
        /// Compares two matrices against each other for equality
        /// </summary>
        /// <param name="left">The first matrix of the comparison</param>
        /// <param name="right">The second matrix of the comparison</param>
        /// <returns>True if matrices are equal, otherwise false</returns>
        public static bool operator ==(RotationMatrix left, RotationMatrix right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two matrices against each other for inequality
        /// </summary>
        /// <param name="left">The first matrix of the comparison</param>
        /// <param name="right">The second matrix of the comparison</param>
        /// <returns>True if matrices are unequal, otherwise false</returns>
        public static bool operator !=(RotationMatrix left, RotationMatrix right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares this matrix with another object
        /// </summary>
        /// <param name="obj">The other object to compare against</param>
        /// <returns>True if the object is a rotation matrix and equal to this instance, otherwise false</returns>
        public override bool Equals(object obj)
        {
            return obj is RotationMatrix other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code of this instance
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _xRow.GetHashCode();
                hashCode = (hashCode * 397) ^ _yRow.GetHashCode();
                hashCode = (hashCode * 397) ^ _zRow.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a string representation of the coordinate system
        /// </summary>
        /// <returns>a string</returns>
        public new string ToString()
        {
            return $"x row: ({_xRow.X:F4}, {_xRow.Y:F4}, {_xRow.Z:F4}), y row: ({_yRow.X:F4}, {_yRow.Y:F4}, {_yRow.Z:F4}), z row: ({_zRow.X:F4}, {_zRow.Y:F4}, {_zRow.Z:F4})";
        }

        /// <summary>
        /// Performs the multiplication of this matrix with a column vector, having coordinates x, y and z
        /// </summary>
        private static Double3 Multiply(RotationMatrix matrix, Double3 v)
        {
            var x2 = matrix._xRow.X * v.X
                     + matrix._xRow.Y * v.Y
                     + matrix._xRow.Z * v.Z;

            var y2 = matrix._yRow.X * v.X
                     + matrix._yRow.Y * v.Y
                     + matrix._yRow.Z * v.Z;

            var z2 = matrix._zRow.X * v.X
                     + matrix._zRow.Y * v.Y
                     + matrix._zRow.Z * v.Z;

            return new Double3(x2, y2, z2);
        }

        // Workaround for .NET Framework 4.6.1 because ValueTuple3 is not defined (used for (x, y, z))
        private readonly struct Double3
        {
            public Double3(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Double3(Point3D p)
                : this(p.X, p.Y, p.Z)
            {
            }

            public Double3(Direction d)
                : this(d.X, d.Y, d.Z)
            {
            }

            public Double3(Vector3D v)
                : this(v.X, v.Y, v.Z)
            {
            }

            public double X { get; }
            public double Y { get; }
            public double Z { get; }
        }
    }
}
