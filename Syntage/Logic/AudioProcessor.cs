using System.Collections.Generic;
using Jacobi.Vst.Core;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;
using Syntage.Plugin;

namespace Syntage.Logic
{
    public class AudioProcessor : SyntageAudioProcessor
    {
        private readonly AudioStream _mainStream;

        public readonly PluginController PluginController;

        public Input Input { get; }

        public Routing Commutator { get; }
        public Oscillator OscillatorA { get; }
        public ADSR EnvelopeA { get; }
        public Oscillator OscillatorB { get; }
        public ADSR EnvelopeB { get; }
        public ButterworthFilter Filter { get; }
        public Distortion Distortion { get; }
        public Delay Delay { get; }
        public Clip Clip { get; }
        public LFO LFOModifier { get; }
        public Master Master { get; }
		public Oscillograph Oscillograph { get; }

        public AudioProcessor(PluginController pluginController) :
			base(0, 2, 0)
        {
            _mainStream = (AudioStream)CreateAudioStream();

            PluginController = pluginController;

            Input = new Input(this);

            Commutator = new Routing(this);
            OscillatorA = new Oscillator(this);
            EnvelopeA = new ADSR(this);
            OscillatorB = new Oscillator(this);
            EnvelopeB = new ADSR(this);
            Filter = new ButterworthFilter(this);
            Distortion = new Distortion(this);
            Clip = new Clip(this);
            Delay = new Delay(this);
            LFOModifier = new LFO(this);
            Master = new Master(this);
			Oscillograph = new Oscillograph(this);

            OnBypassChanged += (sender, args) =>
            {
                Commutator.Power.Value = (Bypass) ? EPowerStatus.Off : EPowerStatus.On;
            };
        }
        
        public override IEnumerable<Parameter> CreateParameters()
        {
            var parameters = new List<Parameter>();

            parameters.AddRange(Commutator.CreateParameters("C"));
            parameters.AddRange(OscillatorA.CreateParameters("A"));
            parameters.AddRange(EnvelopeA.CreateParameters("EA"));
            parameters.AddRange(OscillatorB.CreateParameters("B"));
            parameters.AddRange(EnvelopeB.CreateParameters("EB"));
            parameters.AddRange(Filter.CreateParameters("F"));
            parameters.AddRange(Distortion.CreateParameters("D"));
            parameters.AddRange(Delay.CreateParameters("Dly"));
            parameters.AddRange(Clip.CreateParameters("K"));
            parameters.AddRange(LFOModifier.CreateParameters("G"));
            parameters.AddRange(Master.CreateParameters("M"));
			parameters.AddRange(Oscillograph.CreateParameters("O"));
            
            return parameters;
        }

        public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            base.Process(inChannels, outChannels);

            Commutator.Process(_mainStream);

            // отправляем результат 
            _mainStream.WriteToVstOut(outChannels);
        }
    }
}
