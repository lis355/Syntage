using System;
using System.Collections.Generic;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Plugin;
using Syntage.Framework.Parameters;
using Syntage.Logic.Audio;
using Syntage.Plugin;

namespace Syntage.Logic
{
    public class AudioProcessor : VstPluginAudioProcessorBase, IVstPluginBypass, IAudioStreamProvider
    {
        private readonly List<AudioStream> _audioStreams = new List<AudioStream>();
        private readonly AudioStream _mainStream;
        private bool _bypass;
        
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
            PluginController = pluginController;
            _mainStream = (AudioStream)CreateAudioStream();

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
		}

        public override float SampleRate
        {
            get { return base.SampleRate; }
            set
            {
                base.SampleRate = value;
                OnSampleRateChanged?.Invoke(this, new SampleRateEventArgs(SampleRate));
            }
        }

        public class SampleRateEventArgs : EventArgs
        {
            public float SampleRate { get; private set; }

            public SampleRateEventArgs(float sampleRate)
            {
                SampleRate = sampleRate;
            }
        }

        public event EventHandler<SampleRateEventArgs> OnSampleRateChanged;

        public bool Bypass
        {
            get { return _bypass; }
            set
            {
                _bypass = value;
                Commutator.Power.Value = (_bypass) ? EPowerStatus.Off : EPowerStatus.On;
            }
        }

        public override int BlockSize
        {
            get { return base.BlockSize; }
            set
            {
                if (base.BlockSize == value)
                    return;

                base.BlockSize = value;

                foreach (var stream in _audioStreams)
                    stream.SetBlockSize(BlockSize);
            }
        }

        public IEnumerable<Parameter> CreateParameters()
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

        public IAudioStream CreateAudioStream()
        {
            var stream = new AudioStream();
            stream.Initialize(OutputCount, this);
            stream.SetBlockSize(BlockSize);

            _audioStreams.Add(stream);

            return stream;
        }

        public void ReleaseAudioStream(IAudioStream stream)
        {
            _audioStreams.Remove(stream as AudioStream);
        }

        public int CurrentStreamLenght { get; private set; }
        
        public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            CurrentStreamLenght = outChannels[0].SampleCount;

            Commutator.Process(_mainStream);

            // отправляем результат 
            _mainStream.WriteToVstOut(outChannels);
        }
    }
}
