using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.MIDI;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;

namespace Syntage.Logic
{
	public class Oscillator : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IGenerator
	{
        private class Tone
        {
            public int Note;
            public double Time;
        }

        private readonly IAudioStream _stream;
        private Tone _tone;

        public VolumeParameter Volume { get; private set; }
		public EnumParameter<WaveGenerator.EOscillatorType> OscillatorType { get; private set; }
		public RealParameter Fine { get; private set; }
		public RealParameter Panning { get; private set; }

		public Oscillator(AudioProcessor audioProcessor) :
			base(audioProcessor)
		{
			_stream = Processor.CreateAudioStream();

			audioProcessor.PluginController.MidiListener.OnNoteOn += MidiListenerOnNoteOn;
        }

        private void MidiListenerOnNoteOn(object sender, MidiListener.NoteEventArgs e)
        {
            var newNote = e.NoteAbsolute;

            // если уже была нажата нота, нужно скопировать ее фазу, чтобы не было щелчка
            double time = 0;
            if (_tone != null)
            {
                // фаза от 0 до 1
                var tonePhase = DSPFunctions.Frac(_tone.Time * GetToneFrequency(_tone.Note, 0));

                // фаза второй ноты должна быть такой же, определим время по частоте
                time = tonePhase / GetToneFrequency(newNote, 0);
            }

            _tone = new Tone
            {
                Time = time,
                Note = e.NoteAbsolute
            };
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
		{
			Volume = new VolumeParameter(parameterPrefix + "Vol", "Oscillator Volume");
			OscillatorType = new EnumParameter<WaveGenerator.EOscillatorType>(parameterPrefix + "Osc", "Oscillator Type", "Osc", false);

			Fine = new RealParameter(parameterPrefix + "Fine", "Oscillator pitch", "Fine", -2, 2, 0.01);
			Fine.SetDefaultValue(0);

			Panning = new RealParameter(parameterPrefix + "Pan", "Oscillator Panorama", "", 0, 1, 0.01);
			Panning.SetDefaultValue(0.5);

			return new List<Parameter> {Volume, OscillatorType, Fine, Panning};
		}
        
        public IAudioStream Generate()
		{
			_stream.Clear();
            
			if (_tone != null)
				GenerateToneToStream(_tone);

			return _stream;
		}

		private void GenerateToneToStream(Tone tone)
		{
            var leftChannel = _stream.Channels[0];
            var rightChannel = _stream.Channels[1];

            double timeDelta = 1.0 / Processor.SampleRate;
			var count = Processor.CurrentStreamLenght;
			for (int i = 0; i < count; ++i)
			{
			    var frequency = GetToneFrequency(tone.Note, i);
                var sample = WaveGenerator.GenerateNextSample(OscillatorType.Value, frequency, tone.Time);
                
				sample *= Volume.ProcessedValue(i);

				var panR = Panning.ProcessedValue(i);
				var panL = 1 - panR;

                leftChannel.Samples[i] += sample * panL;
                rightChannel.Samples[i] += sample * panR;

                tone.Time += timeDelta;
			}
		}

	    private double GetToneFrequency(int note, int sampleNumber)
	    {
	        return DSPFunctions.GetNoteFrequency(note + Fine.ProcessedValue(sampleNumber));
        }
    }
}
