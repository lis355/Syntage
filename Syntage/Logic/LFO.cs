using System.Collections.Generic;
using Syntage.Framework.MIDI;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
    public class LFO : AudioProcessorPartWithParameters
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
        /*
        private void GenerateToneToStream(Tone tone)
        {
            if (tone.State == Tone.EToneState.None)
                return;

            var leftChannel = _stream.Channels[0];
            var rightChannel = _stream.Channels[1];

            double timeDelta = 1.0 / Processor.SampleRate;
            var count = Processor.CurrentStreamLenght;
            for (int i = 0; i < count; ++i)
            {
                var frequency = DSPFunctions.GetNoteFrequency(tone.Note + Fine.Value);
                var sample = WaveGenerator.GenerateNextSample(OscillatorType.Value, frequency, tone.Time);

                if (tone.State == Tone.EToneState.Out
                    || tone.State == Tone.EToneState.In)
                {
                    double fadeSamplesCount = Math.Min(KFadeSamples, count);
                    double fadeMultiplier = (i < fadeSamplesCount) ? i / fadeSamplesCount : 1;

                    if (tone.State == Tone.EToneState.In)
                        fadeMultiplier = 1 - fadeMultiplier;

                    sample *= fadeMultiplier;
                }

                sample *= Volume.Value;

                var panR = Panning.Value;
                var panL = 1 - panR;

                leftChannel.Samples[i] += sample * panL;
                rightChannel.Samples[i] += sample * panR;

                tone.Time += timeDelta;
            }

            switch (tone.State)
            {
                case Tone.EToneState.Out:
                    tone.State = Tone.EToneState.Active;
                    break;

                case Tone.EToneState.In:
                    tone.State = Tone.EToneState.None;
                    break;
            }
        }
        */
        public void ProcessModification()
        {
            if (TargetParameter == null)
                return;


        }
    }
}
