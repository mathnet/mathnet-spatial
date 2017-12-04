namespace MathNet.Spatial.Units
{
    public interface IAngleUnit : IUnit
    {
        /// <summary>
        /// Gets the value to multiply radians with to get a value in the current unit.
        /// </summary>
        double ConversionFactor { get; }

        /// <summary>
        /// Gets the name of the unit used in ToString
        /// </summary>
        new string ShortName { get; }
    }
}
