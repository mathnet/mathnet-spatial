namespace MathNet.Spatial.Units
{
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// An angle
    /// </summary>
    [Serializable]
    public struct Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable, IXmlSerializable
    {
        /// <summary>
        /// The value in radians
        /// </summary>
        public readonly double Radians;

        private const double RadToDeg = 180.0 / Math.PI;
        private const double DegToRad = Math.PI / 180.0;

        private Angle(double radians)
        {
            this.Radians = radians;
        }

        /// <summary>
        /// Initializes a new instance of the Angle.
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="unit"></param>
        [Obsolete("This constructor will be removed, use factory method FromRadians. Made obsolete 2017-12-03.")]
        public Angle(double radians, Radians unit)
        {
            this.Radians = radians;
        }

        /// <summary>
        /// Initializes a new instance of the Angle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        [Obsolete("This constructor will be removed, use factory method FromDegrees. Made obsolete 2017-12-03.")]
        public Angle(double value, Degrees unit)
        {
            this.Radians = UnitConverter.ConvertFrom(value, unit);
        }

        /// <summary>
        /// Gets the value in degrees
        /// </summary>
        public double Degrees => this.Radians * RadToDeg;

        /// <summary>
        /// Indicates whether two <see cref="T:MathNet.Spatial.Units.Angle"/> instances are equal.
        /// </summary>
        /// <returns>
        /// true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether two <see cref="T:MathNet.Spatial.Units.Angle"/> instances are not equal.
        /// </summary>
        /// <returns>
        /// true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is less than another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is less than the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator <(Angle left, Angle right)
        {
            return left.Radians < right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is greater than another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is greater than the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator >(Angle left, Angle right)
        {
            return left.Radians > right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is less than or equal to another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is less than or equal to the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator <=(Angle left, Angle right)
        {
            return left.Radians <= right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified <see cref="T:MathNet.Spatial.Units.Angle"/> is greater than or equal to another specified <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is greater than or equal to the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">An <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        public static bool operator >=(Angle left, Angle right)
        {
            return left.Radians >= right.Radians;
        }

        /// <summary>
        /// Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="left"/> and returns the result.
        /// </summary>
        /// <param name="right">An instance of <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        /// <param name="left">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="left"/> and returns the result.</returns>
        public static Angle operator *(double left, Angle right)
        {
            return new Angle(left * right.Radians);
        }

        /// <summary>
        /// Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.
        /// </summary>
        /// <param name="left">An instance of <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        /// <param name="right">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Multiplies an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.</returns>
        public static Angle operator *(Angle left, double right)
        {
            return new Angle(left.Radians * right);
        }

        /// <summary>
        /// Divides an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.
        /// </summary>
        /// <param name="left">An instance of <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        /// <param name="right">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Divides an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> with <paramref name="right"/> and returns the result.</returns>
        public static Angle operator /(Angle left, double right)
        {
            return new Angle(left.Radians / right);
        }

        /// <summary>
        /// Adds two specified <see cref="T:MathNet.Spatial.Units.Angle"/> instances.
        /// </summary>
        /// <returns>
        /// An <see cref="T:MathNet.Spatial.Units.Angle"/> whose value is the sum of the values of <paramref name="left"/> and <paramref name="right"/>.
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/>.</param>
        /// <param name="right">A TimeSpan.</param>
        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left.Radians + right.Radians);
        }

        /// <summary>
        /// Subtracts an angle from another angle and returns the difference.
        /// </summary>
        /// <returns>
        /// An <see cref="T:MathNet.Spatial.Units.Angle"/> that is the difference
        /// </returns>
        /// <param name="left">A <see cref="T:MathNet.Spatial.Units.Angle"/> (the minuend).</param>
        /// <param name="right">A <see cref="T:MathNet.Spatial.Units.Angle"/> (the subtrahend).</param>
        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left.Radians - right.Radians);
        }

        /// <summary>
        /// Returns an <see cref="T:MathNet.Spatial.Units.Angle"/> whose value is the negated value of the specified instance.
        /// </summary>
        /// <returns>
        /// An <see cref="T:MathNet.Spatial.Units.Angle"/> with the same numeric value as this instance, but the opposite sign.
        /// </returns>
        /// <param name="angle">A <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        public static Angle operator -(Angle angle)
        {
            return new Angle(-1 * angle.Radians);
        }

        /// <summary>
        /// Returns the specified instance of <see cref="T:MathNet.Spatial.Units.Angle"/>.
        /// </summary>
        /// <returns>
        /// Returns <paramref name="angle"/>.
        /// </returns>
        /// <param name="angle">A <see cref="T:MathNet.Spatial.Units.Angle"/></param>
        public static Angle operator +(Angle angle)
        {
            return angle;
        }

        /// <summary>
        /// Creates an Angle from its string representation
        /// </summary>
        /// <param name="s">The string representation of the angle</param>
        /// <returns></returns>
        public static Angle Parse(string s)
        {
            return UnitParser.Parse(s, From);
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        [Obsolete("This method will be removed, use factory method FromDegrees or FromRadians. Made obsolete 2017-12-03.")]
        public static Angle From<T>(double value, T unit)
            where T : IAngleUnit
        {
            return new Angle(UnitConverter.ConvertFrom(value, unit));
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value"></param>
        public static Angle FromDegrees(double value)
        {
            return new Angle(value * DegToRad);
        }

        /// <summary>
        /// Creates a new instance of Angle.
        /// </summary>
        /// <param name="value"></param>
        public static Angle FromRadians(double value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Reads an instance of <see cref="T:MathNet.Spatial.Units.Angle"/> from the <paramref name="reader"/>
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/></param>
        /// <returns>An instance of  <see cref="T:MathNet.Spatial.Units.Angle"/></returns>
        public static Angle ReadFrom(XmlReader reader)
        {
            var v = default(Angle);
            v.ReadXml(reader);
            return v;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString((string)null, (IFormatProvider)NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representation of the Angle using the provided format
        /// </summary>
        /// <param name="format">a string indicating the desired format</param>
        public string ToString(string format)
        {
            return this.ToString(format, (IFormatProvider)NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representation of the Angle using the provided formatprovider
        /// </summary>
        /// <param name="provider">a format provider</param>
        public string ToString(IFormatProvider provider)
        {
            return this.ToString((string)null, (IFormatProvider)NumberFormatInfo.GetInstance(provider));
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString(format, formatProvider, AngleUnit.Radians);
        }

        /// <summary>
        /// Returns a string representation of the Angle using the provided formatprovider using the specified format for a given unit
        /// </summary>
        /// <param name="format">A string indicating the desired format</param>
        /// <param name="formatProvider">A formatprovider</param>
        /// <param name="unit">Degrees or Radians</param>
        public string ToString<T>(string format, IFormatProvider formatProvider, T unit)
            where T : IAngleUnit
        {
            var value = UnitConverter.ConvertTo(this.Radians, unit);
            return string.Format("{0}{1}", value.ToString(format, formatProvider), unit.ShortName);
        }

        /// <inheritdoc />
        public int CompareTo(Angle value)
        {
            return this.Radians.CompareTo(value.Radians);
        }

        /// <inheritdoc />
        public bool Equals(Angle other)
        {
            return this.Radians.Equals(other.Radians);
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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Angle && this.Equals((Angle)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Radians.GetHashCode();
        }

        /// <inheritdoc />
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            this = Angle.FromRadians(XmlConvert.ToDouble(e.ReadAttributeOrElementOrDefault("Value")));
        }

        /// <inheritdoc />
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("Value", this.Radians);
        }
    }
}
