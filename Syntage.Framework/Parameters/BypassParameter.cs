using Jacobi.Vst.Framework;

namespace Syntage.Framework.Parameters
{
    public enum EPowerStatus
    {
        Off,
        On
    }

    public class BypassParameter : EnumParameter<EPowerStatus>
    {
        private readonly IVstPluginBypass _bypassProvider;

        public BypassParameter(string name, string label, IVstPluginBypass bypassProvider, string shortLabel = "",  bool canBeAutomated = true) : 
            base(name, label, shortLabel, canBeAutomated)
        {
            _bypassProvider = bypassProvider;
        }

        public override bool CanSetValueFromUI()
        {
            return !_bypassProvider.Bypass;
        }
    }
}
