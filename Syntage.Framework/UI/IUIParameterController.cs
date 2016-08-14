using Syntage.Framework.Parameters;

namespace Syntage.Framework.UI
{
    public interface IUIParameterController
    {
        void SetParameter(Parameter parameter);
        void UpdateController();
    }
}
