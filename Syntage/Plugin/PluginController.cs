using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Syntage.Framework;
using Syntage.Logic;

namespace Syntage.Plugin
{
    public class PluginController : SyntagePlugin
    {
        public AudioProcessor AudioProcessor { get; }

        public PluginController() : base(
            "MyPlugin",
            new VstProductInfo("MyPlugin", "TestCompany", 1000),
            VstPluginCategory.Synth,
            VstPluginCapabilities.None,
            0,
            new FourCharacterCode("TEST").ToInt32())
        {
            AudioProcessor = new AudioProcessor(this);

            ParametersManager.SetParameters(AudioProcessor.CreateParameters());
            ParametersManager.CreateAndSetDefaultProgram();
        }

        protected override IVstPluginAudioProcessor CreateAudioProcessor(IVstPluginAudioProcessor instance)
        {
            return AudioProcessor;
        }
    }
}
