namespace MathNet.Spatial.Units
{
    public interface IAngleUnit : IUnit
    {
        /// <summary>
        /// The value to multiply radians with to get a value in the current unit.
        /// </summary>
        double ConversionFactor { get; }

        /// <summary>
        /// The name of the unit used in ToString
        /// </summary>
        string ShortName { get; }
    }
}
