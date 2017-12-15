namespace MathNet.Spatial.Units
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A custom formatter for an Angle
    /// </summary>
    public class AngleFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AngleFormatProvider"/> class.
        /// </summary>
        public AngleFormatProvider()
        {
        }

        /// <inheritdoc />
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Formats an angle according to its format string
        ///
        /// D:  Formats the angle as degrees e.g. 59°
        /// R:  Formats the angle as radians e.g. 34.554 rad
        ///
        /// Optionally a number can follow the format to indicate precision
        ///
        /// Angle is formatted by default as a radian value
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="arg">An Angle</param>
        /// <param name="provider">A format provider</param>
        /// <returns>A formatted string</returns>
        public string Format(string format, object arg, IFormatProvider provider)
        {
            if (arg is Angle)
            {
                Angle angle = (Angle)arg;

                if (format == null || format == string.Empty)
                {
                    return angle.Radians.ToString(provider) + "\u00A0rad";
                }

                int precision;
                string fmtString = string.Empty;
                if (format.Length > 1)
                {
                    try
                    {
                        precision = int.Parse(format.Substring(1));
                    }
                    catch (FormatException)
                    {
                        precision = 0;
                    }

                    fmtString = "N" + precision.ToString();
                }

                if (format.Substring(0, 1).Equals("D", StringComparison.OrdinalIgnoreCase))
                {
                    return angle.Degrees.ToString(fmtString, provider) + "°";
                }
                else if (format.Substring(0, 1).Equals("R", StringComparison.OrdinalIgnoreCase))
                {
                    return angle.Radians.ToString(fmtString, provider) + "\u00A0rad";
                }
                else
                {
                    return angle.Radians.ToString(format, provider) + "\u00A0rad";
                }
            }
            else
            {
                if (arg is IFormattable)
                {
                    return ((IFormattable)arg).ToString(format, provider);
                }
                else if (arg != null)
                {
                    return arg.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
