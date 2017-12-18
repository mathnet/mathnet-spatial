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
        /// The symbol used for appending to Degree strings
        /// </summary>
        private const string DegreeSymbol = "°";

        /// <summary>
        /// The symbol used for appending to Radians strings
        /// </summary>
        private const string RadianSymbol = "\u00A0rad";

        /// <summary>
        /// The symbol used for appending to Mil strings
        /// </summary>
        private const string MilSymbol = "\u00A0mil";

        /// <summary>
        /// The symbol used for appending to arcmin strings
        /// </summary>
        private const string ArcminSymbol = "′";

        /// <summary>
        /// The symbol used for appending to gon strings
        /// </summary>
        private const string GonSymbol = "ᵍ";

        /// <summary>
        /// The string format to use for sexagesimal numbers
        /// </summary>
        private const string SexagesimalFormat = "{0}° {1}′ {2}″";

        /// <summary>
        /// Conversion to Mil from radians
        /// </summary>
        private const double RadToMil = 1018.5916357881;

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
        /// <para>
        /// D:  Formats the angle as degrees e.g. 59°
        /// G:  Formats the angle in Gon or grad e.g. 30ᵍ
        /// M:  Formats the angle in Mil format e.g. 1600 mil
        /// N:  Formats the angle in Minutearc format e.g. 67.343411′
        /// R:  Formats the angle as radians e.g. 34.554 rad
        /// S:  Formats the angle in full sexagesimal format e.g. 40° 11′ 15″
        /// </para>
        /// <para>
        /// Optionally a number can follow the format to indicate precision
        /// </para>
        /// Angle is formatted by default as a radian value
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="arg">An Angle</param>
        /// <param name="provider">A format provider</param>
        /// <returns>A formatted string</returns>
        public string Format(string format, object arg, IFormatProvider provider)
        {
            if (arg is Angle angle)
            {
                if (format == null || format == string.Empty)
                {
                    return angle.Radians.ToString(provider) + RadianSymbol;
                }

                string fmtString = string.Empty;
                if (format.Length > 1)
                {
                    if (int.TryParse(format.Substring(1), out int precision))
                    {
                        fmtString = "N" + precision.ToString();
                    }
                    else
                    {
                        fmtString = format.Substring(1);
                    }
                }

                if (format.Substring(0, 1).Equals("D", StringComparison.OrdinalIgnoreCase))
                {
                    return angle.Degrees.ToString(fmtString, provider) + DegreeSymbol;
                }
                else if (format.Substring(0, 1).Equals("R", StringComparison.OrdinalIgnoreCase))
                {
                    return angle.Radians.ToString(fmtString, provider) + RadianSymbol;
                }
                else if (format.Substring(0, 1).Equals("S", StringComparison.OrdinalIgnoreCase))
                {
                    return ToSexagesimalString(angle.Degrees, fmtString, provider);
                }
                else if (format.Substring(0, 1).Equals("G", StringComparison.OrdinalIgnoreCase))
                {
                    return angle.Gradians.ToString(fmtString, provider) + GonSymbol;
                }
                else if (format.Substring(0, 1).Equals("M", StringComparison.OrdinalIgnoreCase))
                {
                    return ToMilString(angle.Radians, fmtString, provider);
                }
                else if (format.Substring(0, 1).Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    return ToMinutearcString(angle.Degrees, fmtString, provider);
                }
                else
                {
                    return angle.Radians.ToString(format, provider) + RadianSymbol;
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

        /// <summary>
        /// Returns the angle in Mil format
        /// </summary>
        /// <param name="radians">The number to format in radians</param>
        /// <param name="format">a format for the number</param>
        /// <param name="provider">A format provider for the number</param>
        /// <returns>a Mil formatted string of the angle</returns>
        private static string ToMilString(double radians, string format, IFormatProvider provider)
        {
            double mil = radians * RadToMil;
            return mil.ToString(format, provider) + MilSymbol;
        }

        /// <summary>
        /// Returns the arcminute string for the angle
        /// </summary>
        /// <param name="degrees">The number to format in degrees</param>
        /// <param name="format">a format for the number</param>
        /// <param name="provider">A format provider for the number</param>
        /// <returns>An arcminute string of the angle</returns>
        private static string ToMinutearcString(double degrees, string format, IFormatProvider provider)
        {
            double arcmin = degrees * 60;
            return arcmin.ToString(format, provider) + ArcminSymbol;
        }

        /// <summary>
        /// Returns an angle formatted as a sexagesimal number
        /// </summary>
        /// <param name="degrees">The number to format in degrees</param>
        /// <param name="format">a format for the seconds part of the number</param>
        /// <param name="provider">A format provider for the seconds part of the number</param>
        /// <returns>a sexagesimal representation of the angle</returns>
        private static string ToSexagesimalString(double degrees, string format, IFormatProvider provider)
        {
            while (degrees < -180.0F)
            {
                degrees += 360.0F;
            }

            while (degrees > 180.0F)
            {
                degrees -= 360.0F;
            }

            int degint = (int)Math.Truncate(degrees);
            int minutes = (int)Math.Truncate((degrees - degint) * 60);
            double seconds = (((degrees - degint) * 60) - minutes) * 60;
            string secString = seconds.ToString(format, provider);
            return string.Format(SexagesimalFormat, degint, minutes, secString);
        }
    }
}
