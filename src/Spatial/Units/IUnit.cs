namespace MathNet.Spatial.Units
{
    public interface IUnit
    {
        double Conversionfactor { get; }
       
        string ShortName { get; }
    }
}