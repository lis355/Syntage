using Syntage.Framework.Parameters;

namespace Syntage.UI
{
    public interface IUIParameterController
    {
        void SetParameter(Parameter parameter);
        void UpdateController();
    }
}
