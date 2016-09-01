using System;
using System.Collections.Generic;
using Syntage.Framework.MIDI;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
	public class Oscillator : AudioProcessorPartWithParameters, IGenerator
	{
		private class Tone
		{
			public enum EToneState
			{
				None,
				Out,
				Active,
				In
			}

			public int Note;
			public double Time;
			public EToneState State;
		}

		private const int KFadeSamples = 256;
		private readonly IAudioStream _stream;
		private Tone _tone;
		private Tone _lastTone;

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

		private void MidiListenerOnNoteOn(object sender, MidiListener.NoteEventArgs e)
		{
			if (_lastTone == null
				&& _tone != null)
			{
				_lastTone = _tone;
				_lastTone.State = Tone.EToneState.In;
			}

			_tone = new Tone
			{
				Time = 0,
				Note = e.NoteAbsolute,
				State = Tone.EToneState.Out
			};
		}

		public IAudioStream Generate()
		{
			_stream.Clear();

			if (_lastTone != null)
			{
				GenerateToneToStream(_lastTone);
				_lastTone = null;
			}

			if (_tone != null)
				GenerateToneToStream(_tone);

			return _stream;
		}

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
	}
}
