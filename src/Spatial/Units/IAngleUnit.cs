namespace MathNet.Spatial.Units
{
    public interface IAngleUnit : IUnit
    {
#pragma warning disable CS3005
        /// Identifier differing only in case is not CLS-compliant
        /// <summary>
        /// Gets the value to multiply radians with to get a value in the current unit.
        /// </summary>
        double ConversionFactor { get; }
#pragma warning restore CS3005 // Identifier differing only in case is not CLS-compliant

        /// <summary>
        /// Gets the name of the unit used in ToString
        /// </summary>
        new string ShortName { get; }
    }
}
