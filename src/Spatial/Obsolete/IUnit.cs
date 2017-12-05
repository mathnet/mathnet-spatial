#pragma warning disable SA1600 // Elements must be documented
namespace MathNet.Spatial.Units
{
    using System;

    [Obsolete("This should not have been public, will be removed in a future version. Made obsolete 2017-12-02")]
    public interface IUnit
    {
        double Conversionfactor { get; }

        string ShortName { get; }
    }
}
