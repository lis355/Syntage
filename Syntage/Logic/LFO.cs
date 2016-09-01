using System.Collections.Generic;
using Syntage.Framework.MIDI;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public class LFO : AudioProcessorPartWithParameters, IProcessor, IParameterModifier
    {
        private double _time;

        public EnumParameter<WaveGenerator.EOscillatorType> OscillatorType { get; private set; }
        public FrequencyParameter Frequency { get; private set; }
        public BooleanParameter MatchKey { get; private set; }
        public RealParameter Gain { get; private set; }
        public Parameter TargetParameter { get; set; }

        public LFO(AudioProcessor audioProcessor) :
			base(audioProcessor)
		{
            audioProcessor.PluginController.MidiListener.OnNoteOn += MidiListenerOnNoteOn;
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            OscillatorType = new EnumParameter<WaveGenerator.EOscillatorType>(parameterPrefix + "Osc", "LFO Type", "Osc", false);
            Frequency = new FrequencyParameter(parameterPrefix + "Frq", "LFO Frequency", "Frq", 0.01, 1000, false);
            MatchKey = new BooleanParameter(parameterPrefix + "Mtch", "LFO Phase Key Link", "Match", false);
            Gain = new RealParameter(parameterPrefix + "Gain", "LFO Gain", "Gain", 0, 1, 0.01, false);

            return new List<Parameter> {OscillatorType, Frequency, MatchKey, Gain};
        }

        private void MidiListenerOnNoteOn(object sender, MidiListener.NoteEventArgs e)
        {
            if (MatchKey.Value)
                _time = 0;
        }
        
        public void Process(IAudioStream stream)
        {
            _time += Processor.CurrentStreamLenght / Processor.SampleRate;
        }

        public double ModifyRealValue(double currentValue, int sampleNumber)
        {
            var gain = Gain.Value;
            if (DSPFunctions.IsZero(gain))
                return currentValue;

            var amplitude = GetCurrentAmplitude(sampleNumber);
            gain *= amplitude * 0.5;

            return DSPFunctions.Clamp01(currentValue + gain);
        }

        private double GetCurrentAmplitude(int sampleNumber)
        {
            var timePass = sampleNumber / Processor.SampleRate;
            var currentTime = _time + timePass;
            var sample = WaveGenerator.GenerateNextSample(OscillatorType.Value, Frequency.Value, currentTime);

            return sample;
        }
    }
}
