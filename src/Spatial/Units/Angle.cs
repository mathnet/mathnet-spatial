namespace MathNet.Spatial.Units
{
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using MathNet.Spatial.Internals;

    /// <summary>
    /// An angle
    /// </summary>
    [Serializable]
    public struct Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable, IXmlSerializable
    {
        /// <summary>
        /// An angle representing 180 degrees or PI radians
        /// </summary>
        public static readonly Angle Degrees180 = Angle.FromRadians(Math.PI);

        /// <summary>
        /// An angle representing 90 degrees or PI / 2 radians
        /// </summary>
        public static readonly Angle Degrees90 = Angle.FromRadians(Math.PI / 2);

        /// <summary>
        /// An angle representing 360 degrees or 2 * PI radians
        /// </summary>
        public static readonly Angle Degrees360 = Angle.FromRadians(2 * Math.PI);

        /// <summary>
        /// The value in radians
        /// </summary>
        public readonly double Radians;

        /// <summary>
        /// Conversion factor for converting Radians to Degrees
        /// </summary>
        private const double RadToDeg = 180.0 / Math.PI;

        /// <summary>
        /// Conversion factor for converting Degrees to Radians
        /// </summary>
        private const double DegToRad = Math.PI / 180.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> struct.
        /// </summary>
        /// <param name="radians">The value in radians.</param>
        /// <param name="unit">The radians unit.</param>
        [Obsolete("This constructor will be removed, use factory method FromRadians. Made obsolete 2017-12-03.")]
        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable once UnusedParameter.Local
        public Angle(double radians, Radians unit)
        {
            this.Radians = radians;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> struct.
        /// </summary>
        /// <param name="value">The value in degrees.</param>
        /// <param name="unit">The radians unit.</param>
        [Obsolete("This constructor will be removed, use factory method FromDegrees. Made obsolete 2017-12-03.")]
        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable once UnusedParameter.Local
        public Angle(double value, Degrees unit)
        {
            this.Radians = value * DegToRad;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> struct.
        /// </summary>
        /// <param name="radians">The value in Radians</param>
        private Angle(double radians)
        {
            this.Radians = radians;
        }

        /// <summary>
        /// Gets the value in degrees
        /// </summary>
        public double Degrees => this.Radians * RadToDeg;

        /// <summary>
        /// Explicit conversion from a numeric angle in radians to an Angle object
        /// </summary>
        /// <param name="radians">The angle in radians</param>
        public static explicit operator Angle(double radians)
        {
            return Angle.FromRadians(radians);
        }

        /// <summary>
        /// Returns a value that indicates whether two specified Angles are equal.
        /// </summary>
        /// <param name="left">The first angle to compare</param>
        /// <param name="right">The second angle to compare</param>
        /// <returns>True if the angles are the same; otherwise false.</returns>
        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether two specified Angles are not equal.
        /// </summary>
        /// <param name="left">The first angle to compare</param>
        /// <param name="right">The second angle to compare</param>
        /// <returns>True if the angles are different; otherwise false.</returns>
        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates if a specified Angles is less than another.
        /// </summary>
        /// <param name="left">The first angle to compare</param>
        /// <param name="right">The second angle to compare</param>
        /// <returns>True if the first angle is less than the second angle; otherwise false.</returns>
        public static bool operator <(Angle left, Angle right)
        {
            return left.Radians < right.Radians;
        }

        /// <summary>
        /// Returns a value that indicates if a specified Angles is greater than another.
        /// </summary>
        /// <param name="left">The first angle to compare</param>
        /// <param name="right">The second angle to compare</param>
        /// <returns>True if the first angle is greater than the second angle; otherwise false.</returns>
        public static bool operator >(Angle left, Angle right)
        {
            return left.Radians > right.Radians;
        }

        /// <summary>
        /// Returns a value that indicates if a specified Angles is less than or equal to another.
        /// </summary>
        /// <param name="left">The first angle to compare</param>
        /// <param name="right">The second angle to compare</param>
        /// <returns>True if the first angle is less than or equal to the second angle; otherwise false.</returns>
        public static bool operator <=(Angle left, Angle right)
        {
            return left.Radians <= right.Radians;
        }

        /// <summary>
        /// Returns a value that indicates if a specified Angles is greater than or equal to another.
        /// </summary>
        /// <param name="left">The first angle to compare</param>
        /// <param name="right">The second angle to compare</param>
        /// <returns>True if the first angle is greater than or equal to the second angle; otherwise false.</returns>
        public static bool operator >=(Angle left, Angle right)
        {
            return left.Radians >= right.Radians;
        }

        /// <summary>
        /// Multiplies an Angle by a scalar
        /// </summary>
        /// <param name="left">The scalar.</param>
        /// <param name="right">The angle.</param>
        /// <returns>A new angle equal to the product of the angle and the scalar.</returns>
        public static Angle operator *(double left, Angle right)
        {
            return new Angle(left * right.Radians);
        }

        /// <summary>
        /// Multiplies an Angle by a scalar
        /// </summary>
        /// <param name="left">The angle.</param>
        /// <param name="right">The scalar.</param>
        /// <returns>A new angle equal to the product of the angle and the scalar.</returns>
        public static Angle operator *(Angle left, double right)
        {
            return new Angle(left.Radians * right);
        }

        /// <summary>
        /// Divides an Angle by a scalar
        /// </summary>
        /// <param name="left">The angle.</param>
        /// <param name="right">The scalar.</param>
        /// <returns>A new angle equal to the division of the angle by the scalar.</returns>
        public static Angle operator /(Angle left, double right)
        {
            return new Angle(left.Radians / right);
        }

        /// <summary>
        /// Adds two angles together
        /// </summary>
        /// <param name="left">The first angle.</param>
        /// <param name="right">The second angle.</param>
        /// <returns>A new Angle equal to the sum of the provided angles.</returns>
        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left.Radians + right.Radians);
        }

        /// <summary>
        /// Subtracts the second angle from the first
        /// </summary>
        /// <param name="left">The first angle.</param>
        /// <param name="right">The second angle.</param>
        /// <returns>A new Angle equal to the difference of the provided angles.</returns>
        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left.Radians - right.Radians);
        }

        /// <summary>
        /// Negates the angle
        /// </summary>
        /// <param name="angle">The angle to negate.</param>
        /// <returns>The negated angle.</returns>
        public static Angle operator -(Angle angle)
        {
            return new Angle(-1 * angle.Radians);
        }

        /// <summary>
        /// No function.
        /// </summary>
        /// <param name="angle">An angle</param>
        /// <returns>The angle passed</returns>
        [Obsolete("Unnecessary, made obsolete on 2017-12-07")]
        public static Angle operator +(Angle angle)
        {
            return angle;
        }

        /// <summary>
        /// Attempts to convert a string into an <see cref="Angle"/>
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="result">Am <see cref="Angle"/></param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, out Angle result)
        {
            return TryParse(text, null, out result);
        }

        /// <summary>
        /// Attempts to convert a string into an <see cref="Angle"/>
        /// </summary>
        /// <param name="text">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <param name="result">An <see cref="Angle"/></param>
        /// <returns>True if <paramref name="text"/> could be parsed.</returns>
        public static bool TryParse(string text, IFormatProvider formatProvider, out Angle result)
        {
            if (Text.TryParseAngle(text, formatProvider, out result))
            {
                return true;
            }

            result = default(Angle);
            return false;
        }

        /// <summary>
        /// Attempts to convert a string into an <see cref="Angle"/>
        /// </summary>
        /// <param name="value">The string to be converted</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/></param>
        /// <returns>An <see cref="Angle"/></returns>
        public static Angle Parse(string value, IFormatProvider formatProvider = null)
        {
            if (TryParse(value, formatProvider, out var p))
            {
                return p;
            }

            throw new FormatException($"Could not parse an Angle from the string {value}");
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <typeparam name="T">The type of the unit</typeparam>
        /// <param name="value">The value</param>
        /// <param name="unit">the unit</param>
        /// <returns> A new instance of the <see cref="Angle"/> struct.</returns>
        [Obsolete("This method will be removed, use factory method FromDegrees or FromRadians. Made obsolete 2017-12-03.")]
        public static Angle From<T>(double value, T unit)
            where T : IAngleUnit
        {
            return new Angle(value * unit.ConversionFactor);
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value">The value in degrees.</param>
        /// <returns> A new instance of the <see cref="Angle"/> struct.</returns>
        public static Angle FromDegrees(double value)
        {
            return new Angle(value * DegToRad);
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value">The value in radians.</param>
        /// <returns> A new instance of the <see cref="Angle"/> struct.</returns>
        public static Angle FromRadians(double value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Creates a new instance of Angle from the sexagesimal format of the angle in degrees, minutes, seconds
        /// </summary>
        /// <param name="degrees">The degrees of the angle</param>
        /// <param name="minutes">The minutes of the angle</param>
        /// <param name="seconds">The seconds of the angle</param>
        /// <returns>A new instance of the <see cref="Angle"/> struct.</returns>
        public static Angle FromSexagesimal(int degrees, int minutes, double seconds)
        {
            return Angle.FromDegrees(degrees + (minutes / 60.0F) + (seconds / 3600.0F));
        }

        /// <summary>
        /// Creates an <see cref="Angle"/> from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> positioned at the node to read into this <see cref="Angle"/>.</param>
        /// <returns>An <see cref="Angle"/> that contains the data read from the reader.</returns>
        public static Angle ReadFrom(XmlReader reader)
        {
            return reader.ReadElementAs<Angle>();
        }

        /// <summary>
        /// Gets the sine of the angle
        /// </summary>
        /// <returns>The sine of the angle</returns>
        public double Sin()
        {
            return Math.Sin(this.Radians);
        }

        /// <summary>
        /// Gets the cosine of the angle
        /// </summary>
        /// <returns>The cosine of the angle</returns>
        public double Cos()
        {
            return Math.Cos(this.Radians);
        }

        /// <summary>
        /// Gets the tangent of the angle
        /// </summary>
        /// <returns>The tangent of the angle</returns>
        public double Tan()
        {
            return Math.Tan(this.Radians);
        }

        /// <summary>
        /// Normalizes an angle to be constrained within start and end.
        /// </summary>
        /// <param name="start">The start angle</param>
        /// <param name="end">The end angle</param>
        /// <returns>The normalized angle</returns>
        public Angle Normalize(Angle start, Angle end)
        {
            return this.Normalize(start.Radians, end.Radians);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString(null, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representation of the Angle using the provided format
        /// </summary>
        /// <param name="format">a string indicating the desired format of the double.</param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(string format)
        {
            return this.ToString(format, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representation of this instance using the provided <see cref="IFormatProvider"/>
        /// </summary>
        /// <param name="provider">A <see cref="IFormatProvider"/></param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(IFormatProvider provider)
        {
            return this.ToString(null, NumberFormatInfo.GetInstance(provider));
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider provider)
        {
            return this.ToString(format, provider, AngleUnit.Radians);
        }

        /// <summary>
        /// Returns a string representation of the Angle using the provided <see cref="IFormatProvider"/> using the specified format for a given unit
        /// </summary>
        /// <typeparam name="T">The unit type, generic to avoid boxing.</typeparam>
        /// <param name="format">a string indicating the desired format of the double.</param>
        /// <param name="provider">A <see cref="IFormatProvider"/></param>
        /// <param name="unit">Degrees or Radians</param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString<T>(string format, IFormatProvider provider, T unit)
            where T : IAngleUnit
        {
            if (unit == null ||
                unit is Radians)
            {
                return $"{this.Radians.ToString(format, provider)}\u00A0{unit?.ShortName ?? AngleUnit.Radians.ShortName}";
            }

            if (unit is Degrees)
            {
                return $"{this.Degrees.ToString(format, provider)}{unit.ShortName}";
            }

            throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unknown unit");
        }

        /// <inheritdoc />
        public int CompareTo(Angle value)
        {
            return this.Radians.CompareTo(value.Radians);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="T:MathNet.Spatial.Units.Angle"/> object within the given tolerance.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> represents the same angle as this instance; otherwise, false.
        /// </returns>
        /// <param name="other">An <see cref="T:MathNet.Spatial.Units.Angle"/> object to compare with this instance.</param>
        /// <param name="tolerance">The maximum difference for being considered equal</param>
        public bool Equals(Angle other, double tolerance)
        {
            return Math.Abs(this.Radians - other.Radians) < tolerance;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="T:MathNet.Spatial.Units.Angle"/> object within the given tolerance.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> represents the same angle as this instance; otherwise, false.
        /// </returns>
        /// <param name="other">An <see cref="T:MathNet.Spatial.Units.Angle"/> object to compare with this instance.</param>
        /// <param name="tolerance">The maximum difference for being considered equal</param>
        public bool Equals(Angle other, Angle tolerance)
        {
            return Math.Abs(this.Radians - other.Radians) < tolerance.Radians;
        }

        /// <inheritdoc />
        public bool Equals(Angle other) => this.Radians.Equals(other.Radians);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Angle a && this.Equals(a);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(this.Radians);

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.TryReadAttributeAsDouble("Value", out var value) ||
                reader.TryReadAttributeAsDouble("Radians", out value))
            {
                reader.Skip();
                this = FromRadians(value);
                return;
            }

            if (reader.TryReadAttributeAsDouble("Degrees", out value))
            {
                reader.Skip();
                this = FromDegrees(value);
                return;
            }

            if (reader.Read())
            {
                if (reader.HasValue)
                {
                    this = FromRadians(reader.ReadContentAsDouble());
                    reader.Skip();
                    return;
                }

                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    if (reader.TryReadElementContentAsDouble("Value", out value) ||
                        reader.TryReadElementContentAsDouble("Radians", out value))
                    {
                        reader.Skip();
                        this = FromRadians(value);
                        return;
                    }

                    if (reader.TryReadElementContentAsDouble("Degrees", out value))
                    {
                        reader.Skip();
                        this = FromDegrees(value);
                        return;
                    }
                }
            }

            throw new XmlException("Could not read an Angle");
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("Value", this.Radians);
        }

        /// <summary>
        /// Normalizes an angle to be constrained within start and end.
        /// </summary>
        /// <param name="start">The start angle in radians</param>
        /// <param name="end">The end angle in radians</param>
        /// <returns>The normalized angle</returns>
        internal Angle Normalize(double start, double end)
        {
            double width = end - start;
            double offset = this.Radians - start;

            return Angle.FromRadians((offset - (Math.Floor(offset / width) * width)) + start);
        }
    }
}
