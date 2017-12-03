namespace MathNet.Spatial.Internals
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    internal static class Text
    {
        private const string DoublePatternNullProvider = @"[+-]?\d*(?:[.,]\d+)?(?:[eE][+-]?\d+)?";
        private const string DoublePatternPointProvider = @"[+-]?\d*(?:[.]\d+)?(?:[eE][+-]?\d+)?";
        private const string DoublePatternCommaProvider = @"[+-]?\d*(?:[,]\d+)?(?:[eE][+-]?\d+)?";

        private const string SeparatorPatternNullProvider = @" *[,; ] *";
        private const string SeparatorPatternCommaProvider = @" *[; ] *";
        private const string Pattern2D = @"^ *\(?(?<x>{0}){1}(?<y>{0})?\)? *$";

        // This is for legacy reasons, we allow any culture, not nice.
        private static readonly Regex Regex2DNullProvider = new Regex(
            string.Format(Pattern2D, DoublePatternNullProvider, SeparatorPatternNullProvider),
            RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex Regex2DPointProvider = new Regex(
            string.Format(Pattern2D, DoublePatternPointProvider, SeparatorPatternNullProvider),
            RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex Regex2DCommaProvider = new Regex(
            string.Format(Pattern2D, DoublePatternCommaProvider, SeparatorPatternCommaProvider),
            RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

        internal static bool TryParse2D(string text, IFormatProvider formatProvider, out double x, out double y)
        {
            x = 0;
            y = 0;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var match = Regex2DNullProvider.Match(text);
            if (!match.Success ||
                match.Groups.Count != 3 ||
                match.Groups[0].Captures.Count != 1 ||
                match.Groups[1].Captures.Count != 1 ||
                match.Groups[2].Captures.Count != 1)
            {
                return false;
            }

            return TryParseDouble(match.Groups["x"].Value, formatProvider, out x) &&
                   TryParseDouble(match.Groups["y"].Value, formatProvider, out y);
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

        private static Regex GetRegex3D(IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                return Regex2DNullProvider;
            }

            var formatInfo = NumberFormatInfo.GetInstance(formatProvider);
            if (formatInfo.NumberDecimalSeparator == ".")
            {
                return Regex2DPointProvider;
            }

            if (formatInfo.NumberDecimalSeparator == ",")
            {
                return Regex2DCommaProvider;
            }

            return Regex2DNullProvider;
        }
    }
}
