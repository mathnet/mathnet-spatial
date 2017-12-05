namespace MathNet.Spatial.Internals
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using MathNet.Spatial.Units;

    internal static class Text
    {
        private const string DoublePatternPointProvider = "[+-]?\\d*(?:[.]\\d+)?(?:[eE][+-]?\\d+)?";
        private const string DoublePatternCommaProvider = "[+-]?\\d*(?:[,]\\d+)?(?:[eE][+-]?\\d+)?";

        private const string SeparatorPatternPointProvider = " ?[,;]?( |\u00A0)?";
        private const string SeparatorPatternCommaProvider = " ?[;]?( |\u00A0)?";

        internal static bool TryParse2D(string text, IFormatProvider provider, out double x, out double y)
        {
            x = 0;
            y = 0;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (Regex2D.TryMatch(text, provider, out var match) &&
                match.Groups.Count == 3 ||
                match.Groups[0].Captures.Count == 1 ||
                match.Groups[1].Captures.Count == 1 ||
                match.Groups[2].Captures.Count == 1)
            {
                return TryParseDouble(match.Groups["x"].Value, provider, out x) &&
                       TryParseDouble(match.Groups["y"].Value, provider, out y);
            }

            return false;
        }

        internal static bool TryParseAngle(string text, IFormatProvider provider, out Angle a)
        {
            a = default(Angle);
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (RegexAngle.TryMatchDegrees(text, provider, out var value))
            {
                a = Angle.FromDegrees(value);
                return true;
            }

            if (RegexAngle.TryMatchRadians(text, provider, out value))
            {
                a = Angle.FromRadians(value);
                return true;
            }

            return false;
        }

        private static bool TryParseDouble(string s, IFormatProvider formatProvider, out double result)
        {
            if (formatProvider == null)
            {
                // This is for legacy reasons, we allow any culture, not nice.
                // Fixing would break
                return double.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            }

            return double.TryParse(s, NumberStyles.Float, formatProvider, out result);
        }

        private static class Regex2D
        {
            private const string Pattern2D = "^ *\\(?(?<x>{0}){1}(?<y>{0})\\)? *$";
            private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline;

            private static readonly Regex Point = new Regex(
                string.Format(Pattern2D, DoublePatternPointProvider, SeparatorPatternPointProvider),
                RegexOptions);

            private static readonly Regex Comma = new Regex(
                string.Format(Pattern2D, DoublePatternCommaProvider, SeparatorPatternCommaProvider),
                RegexOptions);

            internal static bool TryMatch(string text, IFormatProvider formatProvider, out Match match)
            {
                if (formatProvider != null &&
                    NumberFormatInfo.GetInstance(formatProvider) is NumberFormatInfo formatInfo)
                {
                    if (formatInfo.NumberDecimalSeparator == ".")
                    {
                        match = Point.Match(text);
                        return match.Success;
                    }

                    if (formatInfo.NumberDecimalSeparator == ",")
                    {
                        match = Comma.Match(text);
                        return match.Success;
                    }
                }

                match = Point.Match(text);
                if (match.Success)
                {
                    return true;
                }

                match = Comma.Match(text);
                return match.Success;
            }
        }

        private static class RegexAngle
        {
            private const string RadiansPattern = "^(?<value>{0})( |\u00A0)?(°|rad|radians) *$";
            private const string DegreesPattern = "^(?<value>{0})( |\u00A0)?(°|deg|degrees) *$";
            private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase;

            private static readonly Regex RadiansPoint = new Regex(
                string.Format(RadiansPattern, DoublePatternPointProvider),
                RegexOptions);

            private static readonly Regex RadiansComma = new Regex(
                string.Format(RadiansPattern, DoublePatternCommaProvider),
                RegexOptions);

            private static readonly Regex DegreesPoint = new Regex(
                string.Format(DegreesPattern, DoublePatternPointProvider),
                RegexOptions);

            private static readonly Regex DegreesComma = new Regex(
                string.Format(DegreesPattern, DoublePatternCommaProvider),
                RegexOptions);

            internal static bool TryMatchDegrees(string text, IFormatProvider provider, out double value)
            {
                if (TryMatchDegrees(text, provider, out Match match))
                {
                    return TryParseDouble(match.Groups["value"].Value, provider, out value);
                }

                value = 0;
                return false;
            }

            internal static bool TryMatchRadians(string text, IFormatProvider provider, out double value)
            {
                if (TryMatchRadians(text, provider, out Match match))
                {
                    return TryParseDouble(match.Groups["value"].Value, provider, out value);
                }

                value = 0;
                return false;
            }

            private static bool TryMatchRadians(string text, IFormatProvider formatProvider, out Match match)
            {
                if (formatProvider != null &&
                    NumberFormatInfo.GetInstance(formatProvider) is NumberFormatInfo formatInfo)
                {
                    if (formatInfo.NumberDecimalSeparator == ".")
                    {
                        match = RadiansPoint.Match(text);
                        return match.Success;
                    }

                    if (formatInfo.NumberDecimalSeparator == ",")
                    {
                        match = RadiansComma.Match(text);
                        return match.Success;
                    }
                }

                match = RadiansPoint.Match(text);
                if (match.Success)
                {
                    return true;
                }

                match = RadiansComma.Match(text);
                return match.Success;
            }

            private static bool TryMatchDegrees(string text, IFormatProvider provider, out Match match)
            {
                if (provider != null && NumberFormatInfo.GetInstance(provider) is NumberFormatInfo formatInfo)
                {
                    if (formatInfo.NumberDecimalSeparator == ".")
                    {
                        match = DegreesPoint.Match(text);
                        return match.Success;
                    }

                    if (formatInfo.NumberDecimalSeparator == ",")
                    {
                        match = DegreesComma.Match(text);
                        return match.Success;
                    }
                }

                match = DegreesPoint.Match(text);
                if (match.Success)
                {
                    return true;
                }

                match = DegreesComma.Match(text);
                return match.Success;
            }
        }
    }
}
