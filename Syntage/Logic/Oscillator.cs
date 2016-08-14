using System;
using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public enum EOscillatorType
    {
        Sine,
        Triangle,
        Square,
        Saw
    }

    public class Oscillator : AudioProcessorPartWithParameters, IGenerator
	{
		private class Tone
		{
			public int Note;
			public double Time;
			public ADSR.NoteEnvelope Envelope;
		}

		private readonly IAudioStream _stream;
		private readonly List<Tone> _tones = new List<Tone>();
		private readonly ADSR _envelope;

		public VolumeParameter Volume { get; private set; }
		public EnumParameter<EOscillatorType> OscillatorType { get; private set; }
		public RealParameter Fine { get; private set; }
		public RealParameter Panning { get; private set; }

		public Oscillator(AudioProcessor audioProcessor, ADSR envelope = null) :
			base(audioProcessor)
		{
			_envelope = envelope;
			_stream = Processor.CreateAudioStream();
		}

		public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
		{
			Volume = new VolumeParameter(parameterPrefix + "Vol", "Oscillator Volume");
			OscillatorType = new EnumParameter<EOscillatorType>(parameterPrefix + "Osc", "Oscillator Type");
			Fine = new RealParameter(parameterPrefix + "Fine", "Oscillator pitch", "Fine", -2, 2, 0.01);
			Fine.SetDefaultValue(0);
			Panning = new RealParameter(parameterPrefix + "Pan", "Oscillator Panorama", "", 0, 1, 0.01);
			Panning.SetDefaultValue(0.5);

			return new List<Parameter> {Volume, OscillatorType, Fine, Panning};
		}

		public void StartNote(int absoluteNote)
		{
			var tone = new Tone
			{
				Time = 0,
				Note = absoluteNote
			};

			if (_envelope != null)
			{
				var env = _envelope.CreateEnvelope();
				env.Press();

				tone.Envelope = env;
			}

			_tones.Add(tone);
		}

		public void FinishNote(int absoluteNote)
		{
			var tone = _tones.Find(x => x.Note == absoluteNote);
			if (tone != null)
			{
				if (tone.Envelope != null)
				{
					tone.Envelope.Release();
				}
				else
				{
					_tones.RemoveAll(x => x.Note == absoluteNote);
				}
			}
		}

		public IAudioStream Generate()
		{
			_stream.Clear();

			for (int i = _tones.Count - 1; i >= 0; --i)
				GenerateToneToStream(_tones[i]);

			return _stream;
		}

		private void GenerateToneToStream(Tone tone)
		{
			double timeDelta = 1.0 / Processor.SampleRate;
			var count = Processor.CurrentStreamLenght;
			for (int i = 0; i < count; ++i)
			{
				var frequency = DSPFunctions.GetNoteFrequency(tone.Note + Fine.Value);

				var sample = GenerateNextSample(frequency, tone.Time);
				var panR = Panning.Value;
				var panL = 1 - panR;
				var volume = Volume.Value;

				var lc = sample * volume * panL;
				var rc = sample * volume * panR;

				if (tone.Envelope != null)
				{
					var envelopeMultiplier = tone.Envelope.GetNextMultiplier();
					lc *= envelopeMultiplier;
					rc *= envelopeMultiplier;

					if (tone.Envelope.State == ADSR.EState.None)
						_tones.Remove(tone);
				}

				_stream.Channels[0].Samples[i] += lc;
				_stream.Channels[1].Samples[i] += rc;

				tone.Time += timeDelta;
			}
		}

		private double GenerateNextSample(double frequency, double time)
		{
			var ph = time * frequency;
			ph -= (int)ph;

			return GetTableSample(ph);
		}

		private double GetTableSample(double t)
		{
			switch (OscillatorType.Value)
			{
				case EOscillatorType.Sine:
					return Math.Sin(DSPFunctions.Pi2 * t);

				case EOscillatorType.Triangle:
					if (t < 0.25) return 4 * t;
					if (t < 0.75) return 2 - 4 * t;
					return 4 * (t - 1);

				case EOscillatorType.Square:
					return (t < 0.5f) ? 1 : -1;

				case EOscillatorType.Saw:
					return 2 * t - 1;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
