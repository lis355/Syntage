using System;
using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.MIDI;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;

namespace Syntage.Logic
{
    public class LFO : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IProcessor, IParameterModifier
    {
        private double _time;
        private Parameter _target;

        private class ParameterName : IntegerParameter
        {
            private readonly ParametersManager _parametersManager;

            public ParameterName(string parameterPrefix, ParametersManager parametersManager) :
                base(parameterPrefix + "Num", "LFO Parameter Number", "Num", -1, 34, 1, false)
            {
                _parametersManager = parametersManager;
            }

            public override int FromStringToValue(string s)
            {
                var parameter = _parametersManager.FindParameter(s);
                return (parameter == null) ? -1 : _parametersManager.GetParameterIndex(parameter);
            }

            public override string FromValueToString(int value)
            {
                return (value >= 0) ? _parametersManager.GetParameter(value).Name : "--";
            }
        }

        public EnumParameter<WaveGenerator.EOscillatorType> OscillatorType { get; private set; }
        public FrequencyParameter Frequency { get; private set; }
        public BooleanParameter MatchKey { get; private set; }
        public RealParameter Gain { get; private set; }
        public IntegerParameter TargetParameter { get; private set; }

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
            TargetParameter = new ParameterName(parameterPrefix, Processor.PluginController.ParametersManager);

            TargetParameter.OnValueChange += TargetParameterNumberOnValueChange;

            return new List<Parameter> {OscillatorType, Frequency, MatchKey, Gain, TargetParameter};
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
            gain *= amplitude * Math.Min(currentValue, 1 - currentValue);

            return DSPFunctions.Clamp01(currentValue + gain);
        }

        private double GetCurrentAmplitude(int sampleNumber)
        {
            var timePass = sampleNumber / Processor.SampleRate;
            var currentTime = _time + timePass;
            var sample = WaveGenerator.GenerateNextSample(OscillatorType.Value, Frequency.Value, currentTime);

            return sample;
        }

        private void MidiListenerOnNoteOn(object sender, MidiListener.NoteEventArgs e)
        {
            if (MatchKey.Value)
                _time = 0;
        }

        private void TargetParameterNumberOnValueChange(Parameter.EChangeType obj)
        {
            var number = TargetParameter.Value;
            var parameter = (number >= 0) ? Processor.PluginController.ParametersManager.GetParameter(number) : null;

            if (_target != null)
                _target.ParameterModifier = null;

            _target = parameter;

            if (_target != null)
                _target.ParameterModifier = this;
        }
    }
}
