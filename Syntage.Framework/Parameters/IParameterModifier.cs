namespace Syntage.Framework.Parameters
{
    public interface IParameterModifier
    {
        double ModifyRealValue(double currentValue, int sampleNumber);
    }
}
