using Jacobi.Vst.Framework;
using Syntage.Framework;

namespace Syntage.Plugin
{
    public class PluginCommandStub : SyntagePluginCommandStub<PluginController>
    {
        protected override IVstPlugin CreatePluginInstance()
        {
            return new PluginController();
        }
    }
}
