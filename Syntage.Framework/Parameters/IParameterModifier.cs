namespace Syntage.Framework.Parameters
{
    public interface IParameterModifier
    {
        Parameter Target { get; }
        double ModifyRealValue(double currentValue, int sampleNumber);
    }
}
