using Syntage.Framework.Parameters;

namespace Syntage.UI
{
    public interface IParameterController
    {
        void SetParameter(Parameter parameter);
        void UpdateController();
    }
}
