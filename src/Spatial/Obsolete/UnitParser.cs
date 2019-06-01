using System;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace MathNet.Spatial.Units
{
    [Obsolete("This class will be removed, should not have been public. Made obsolete 2017-12-03.")]
    public static class UnitParser
    {
        public static readonly string UnitValuePattern = $@"^(?: *)(?<Value>{Parser.DoublePattern}) *(?<Unit>.+) *$";

        public static T Parse<T>(string s, Func<double, IAngleUnit, T> creator)
        {
            Match match = Regex.Match(s, UnitValuePattern);
            double d = Parser.ParseDouble(match.Groups["Value"]);
            var unit = ParseUnit(match.Groups["Unit"].Value);
            return creator(d, (IAngleUnit)unit);
        }

        public static object ParseUnit(string s)
        {
            var trim = s.Trim();
            switch (trim)
            {
                case Degrees.Name:
                    return AngleUnit.Degrees;
                case Radians.Name:
                    return AngleUnit.Radians;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
