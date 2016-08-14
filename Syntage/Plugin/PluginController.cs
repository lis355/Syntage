using System;
using System.Collections.Generic;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Syntage.Framework;
using Syntage.Framework.MIDI;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;
using Syntage.Logic;
using Syntage.UI;

namespace Syntage.Plugin
{
    public class PluginController : SyntagePlugin
    {
        public AudioProcessor AudioProcessor { get; }
        public MidiListener MidiListener { get; }
        
        public PluginController() : base(
            "Syntage",
            new VstProductInfo("Study Synth", "lis355", 1000),
            VstPluginCategory.Synth,
            VstPluginCapabilities.None,
            0,
            new FourCharacterCode("SNTG").ToInt32())
        {
            Log.Print("Initializing");

            MidiListener = new MidiListener();
            AudioProcessor = new AudioProcessor(this);
            PluginUI.Instance.PluginController = this;

            ParametersManager.SetParameters(AudioProcessor.CreateParameters());

            var lines = Properties.Resources.InitPreset.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            ParametersManager.SetPrograms(new List<Program>
            {
                ParametersManager.CreateProgramFromSerializedParameters("-", lines)
            });
        }

        protected override IVstPluginAudioProcessor CreateAudioProcessor(IVstPluginAudioProcessor instance)
        {
            return AudioProcessor;
        }

        protected override IVstPluginEditor CreateEditor(IVstPluginEditor instance)
        {
            return PluginUI.Instance;
        }

        protected override IVstPluginBypass CreateBypass(IVstPluginBypass instance)
        {
            return AudioProcessor;
        }

        protected override IVstMidiProcessor CreateMidiProcessor(IVstMidiProcessor instance)
        {
            return MidiListener;
        }
    }
}
