using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathNet.Spatial.Extensions
{
    internal static class DoubleExtensions
    {
        internal static bool IsNearlyEqualTo(this double d, double reference, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }
            return Math.Abs(d-reference) < tolerance;
        }
    }
}
