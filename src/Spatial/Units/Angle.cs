﻿using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MathNet.Spatial.Internals;
using HashCode = MathNet.Spatial.Internals.HashCode;

namespace MathNet.Spatial.Units
{
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

        /// <summary>
        /// Conversion factor for converting Radians to Degrees
        /// </summary>
        private static readonly double RadToDeg = 180.0 / Math.PI;

        /// <summary>
        /// Conversion factor for converting Degrees to Radians
        /// </summary>
        private static readonly double DegToRad = Math.PI / 180.0;

        /// <summary>
        /// The zero angle.
        /// </summary>
        public static readonly Angle Zero = new Angle(0);

        /// <summary>
        /// The 90° angle.
        /// </summary>
        public static readonly Angle HalfPi = new Angle(Math.PI / 2);

        /// <summary>
        /// The 180° angle.
        /// </summary>
        public static readonly Angle Pi = new Angle(Math.PI);

        /// <summary>
        /// The 360° angle.
        /// </summary>
        public static readonly Angle TwoPi = new Angle(2 * Math.PI);

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> struct.
        /// </summary>
        /// <param name="radians">The value in Radians</param>
        private Angle(double radians)
        {
            Radians = radians;
        }

        /// <summary>
        /// Returns the absolute of this angle.
        /// </summary>
        /// <returns></returns>
        public Angle Abs()
        {
            return new Angle(Math.Abs(Radians));
        }

        /// <summary>
        /// Gets the value in degrees
        /// </summary>
        public double Degrees => Radians * RadToDeg;

        /// <summary>
        /// Gets the cosine of this instance
        /// </summary>
        public double Cos => Math.Cos(Radians);

        /// <summary>Returns the angle whose cosine is the specified number.</summary>
        /// <param name="d">A number representing a cosine, where <paramref name="d" /> must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <returns>An angle, θ such that 0 ≤ θ ≤ π</returns>
        public static Angle Acos(double d)
        {
            if (Math.Abs(d) > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(d), "The cosine cannot be greater than 1 in magnitude");
            }

            return new Angle(Math.Acos(d));
        }

        /// <summary>
        /// Gets the sine of this instance
        /// </summary>
        public double Sin => Math.Sin(Radians);

        /// <summary>Returns the angle whose sine is the specified number.</summary>
        /// <param name="d">A number representing a sine, where <paramref name="d" /> must be greater than or equal to -1, but less than or equal to 1.</param>
        /// <returns>An angle, θ such that -π/2 ≤ θ ≤ π/2</returns>
        public static Angle Asin(double d)
        {
            if (Math.Abs(d) > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(d), "The sine cannot be greater than 1 in magnitude");
            }

            return new Angle(Math.Asin(d));
        }

        /// <summary>
        /// Gets the tangent of this instance
        /// </summary>
        public double Tan => Math.Tan(Radians);

        /// <summary>Returns the angle whose tangent is the specified number.</summary>
        /// <param name="d">A number representing a tangent.</param>
        /// <returns>An angle, θ such that -π/2 ≤ θ ≤ π/2.</returns>
        public static Angle Atan(double d)
        {
            return new Angle(Math.Atan(d));
        }

        /// <summary>Returns the angle whose tangent is the quotient of two specified numbers.</summary>
        /// <param name="y">The y coordinate of a point.</param>
        /// <param name="x">The x coordinate of a point.</param>
        /// <returns>An angle, θ such that -π ≤ θ ≤ π, and tan(θ) = <paramref name="y" /> / <paramref name="x" />, where (<paramref name="x" />, <paramref name="y" />) is a point in the Cartesian plane</returns>
        public static Angle Atan2(double y, double x)
        {
            return new Angle(Math.Atan2(y, x));
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
            return FromDegrees(degrees + (minutes / 60.0F) + (seconds / 3600.0F));
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

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString("G15", NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representation of the Angle using the provided format
        /// </summary>
        /// <param name="format">a string indicating the desired format of the double.</param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(string format)
        {
            return ToString(format, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representation of this instance using the provided <see cref="IFormatProvider"/>
        /// </summary>
        /// <param name="provider">A <see cref="IFormatProvider"/></param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(IFormatProvider provider)
        {
            return ToString("G15", NumberFormatInfo.GetInstance(provider));
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider provider)
        {
            return ToString(format, provider, AngleUnit.Radians);
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
                return $"{Radians.ToString(format, provider)}\u00A0{unit?.ShortName ?? AngleUnit.Radians.ShortName}";
            }

            if (unit is Degrees)
            {
                return $"{Degrees.ToString(format, provider)}{unit.ShortName}";
            }

            throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unknown unit");
        }

        /// <inheritdoc />
        public int CompareTo(Angle value)
        {
            return Radians.CompareTo(value.Radians);
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
            return Math.Abs(Radians - other.Radians) < tolerance;
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
            return Math.Abs(Radians - other.Radians) < tolerance.Radians;
        }

        /// <inheritdoc />
        public bool Equals(Angle other) => Radians.Equals(other.Radians);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Angle a && Equals(a);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Radians);

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
            writer.WriteElement("Value", Radians, "G17");
        }
    }
}
