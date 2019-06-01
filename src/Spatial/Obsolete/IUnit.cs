using System;

namespace MathNet.Spatial.Units
{
    [Obsolete("This should not have been public, will be removed in a future version. Made obsolete 2017-12-02")]
    public interface IUnit
    {
        double Conversionfactor { get; }

        string ShortName { get; }
    }
}
