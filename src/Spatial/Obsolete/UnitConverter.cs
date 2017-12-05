#pragma warning disable SA1600 // Elements must be documented
// ReSharper disable once CheckNamespace
namespace MathNet.Spatial.Units
{
    using System;

    [Obsolete("This should not have been public, will be removed in a future version. Made obsolete 2017-12-03")]
    public static class UnitConverter
    {
        public static double ConvertTo<TUnit>(double value, TUnit toUnit)
            where TUnit : IUnit
        {
            return value / toUnit.Conversionfactor;
        }

        public static double ConvertFrom<TUnit>(double value, TUnit toUnit)
            where TUnit : IUnit
        {
            return value * toUnit.Conversionfactor;
        }
    }
}
