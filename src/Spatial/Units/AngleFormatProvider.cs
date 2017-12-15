namespace MathNet.Spatial.Units
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A custom formatter for an Angle
    /// </summary>
    public class AngleFormatProvider : IFormatProvider, ICustomFormatter
    {
        private const string DegreeSymbol = "°";

        private const string RadianSymbol = "\u00A0rad";

        private const string SexagesimalFormat = "{0}° {1}′ {2}″";

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
