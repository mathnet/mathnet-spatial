namespace MathNet.Spatial.Internals
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using MathNet.Spatial.Units;

    internal static class Text
    {
        internal const string DoublePatternNullProvider = @"[+-]?\d*(?:[.,]\d+)?(?:[eE][+-]?\d+)?";
        internal const string DoublePatternPointProvider = @"[+-]?\d*(?:[.]\d+)?(?:[eE][+-]?\d+)?";
        internal const string DoublePatternCommaProvider = @"[+-]?\d*(?:[,]\d+)?(?:[eE][+-]?\d+)?";

        private const string SeparatorPatternNullProvider = @"[,; ] *";
        private const string SeparatorPatternCommaProvider = @"[; ] *";

        internal static bool TryParse2D(string text, IFormatProvider provider, out double x, out double y)
        {
            x = 0;
            y = 0;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var match = Regex2D.Get(provider).Match(text);
            if (!match.Success ||
                match.Groups.Count != 3 ||
                match.Groups[0].Captures.Count != 1 ||
                match.Groups[1].Captures.Count != 1 ||
                match.Groups[2].Captures.Count != 1)
            {
                return false;
            }

            return TryParseDouble(match.Groups["x"].Value, provider, out x) &&
                   TryParseDouble(match.Groups["y"].Value, provider, out y);
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
            private const string Pattern2D = @"^ *\(?(?<x>{0}){1}(?<y>{0})?\)? *$";
            private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline;

            // This is for legacy reasons, we allow any culture, not nice.
            private static readonly Regex NullProvider = new Regex(
                string.Format(Pattern2D, DoublePatternNullProvider, SeparatorPatternNullProvider),
                RegexOptions);

            private static readonly Regex PointProvider = new Regex(
                string.Format(Pattern2D, DoublePatternPointProvider, SeparatorPatternNullProvider),
                RegexOptions);

            private static readonly Regex CommaProvider = new Regex(
                string.Format(Pattern2D, DoublePatternCommaProvider, SeparatorPatternCommaProvider),
                RegexOptions);

            internal static Regex Get(IFormatProvider formatProvider)
            {
                if (formatProvider == null)
                {
                    return NullProvider;
                }

                var formatInfo = NumberFormatInfo.GetInstance(formatProvider);
                if (formatInfo.NumberDecimalSeparator == ".")
                {
                    return PointProvider;
                }

                if (formatInfo.NumberDecimalSeparator == ",")
                {
                    return CommaProvider;
                }

                return NullProvider;
            }
        }

        private static class RegexAngle
        {
            private const string RadiansPattern = @"^(?<value>{0})( |\u00A0)?(°|rad|radians) *$";
            private const string DegreesPattern = @"^(?<value>{0})( |\u00A0)?(°|deg|degrees) *$";
            private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase;

            // This is for legacy reasons, we allow any culture, not nice.
            private static readonly Regex RadiansNullProvider = new Regex(
                string.Format(RadiansPattern, DoublePatternNullProvider),
                RegexOptions);

            private static readonly Regex RadiansPointProvider = new Regex(
                string.Format(RadiansPattern, DoublePatternPointProvider),
                RegexOptions);

            private static readonly Regex RadiansCommaProvider = new Regex(
                string.Format(RadiansPattern, DoublePatternCommaProvider),
                RegexOptions);

            // This is for legacy reasons, we allow any culture, not nice.
            private static readonly Regex DegreesNullProvider = new Regex(
                string.Format(DegreesPattern, DoublePatternNullProvider),
                RegexOptions);

            private static readonly Regex DegreesPointProvider = new Regex(
                string.Format(DegreesPattern, DoublePatternPointProvider),
                RegexOptions);

            private static readonly Regex DegreesCommaProvider = new Regex(
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
                if (formatProvider == null)
                {
                    match = RadiansNullProvider.Match(text);
                    return match.Success;
                }

                var formatInfo = NumberFormatInfo.GetInstance(formatProvider);
                if (formatInfo.NumberDecimalSeparator == ".")
                {
                    match = RadiansPointProvider.Match(text);
                    return match.Success;
                }

                if (formatInfo.NumberDecimalSeparator == ",")
                {
                    match = RadiansCommaProvider.Match(text);
                    return match.Success;
                }

                match = RadiansNullProvider.Match(text);
                return match.Success;
            }


            private static bool TryMatchDegrees(string text, IFormatProvider provider, out Match match)
            {
                if (provider == null)
                {
                    match = DegreesNullProvider.Match(text);
                    return match.Success;
                }

                var formatInfo = NumberFormatInfo.GetInstance(provider);
                if (formatInfo.NumberDecimalSeparator == ".")
                {
                    match = DegreesPointProvider.Match(text);
                    return match.Success;
                }

                if (formatInfo.NumberDecimalSeparator == ",")
                {
                    match = DegreesCommaProvider.Match(text);
                    return match.Success;
                }

                match = DegreesNullProvider.Match(text);
                return match.Success;
            }
        }
    }
}
