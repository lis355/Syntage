using System;
using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.MIDI;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;

namespace Syntage.Logic
{
    public class Oscillator : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IGenerator
    {
        private readonly IAudioStream _stream; // поток, куда будем генерировать семплы
        private double _time;

        public EnumParameter<WaveGenerator.EOscillatorType> OscillatorType { get; private set; }
        public RealParameter Frequency { get; private set; }

        public Oscillator(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
            _stream = Processor.CreateAudioStream(); // запрашиваем поток
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            OscillatorType = new EnumParameter<WaveGenerator.EOscillatorType>(parameterPrefix + "Osc", "Oscillator Type", "Osc", false);
            Frequency = new FrequencyParameter(parameterPrefix + "Frq", "Oscillator Frequency", "Hz");

            return new List<Parameter> { OscillatorType, Frequency };
        }

        public IAudioStream Generate()
        {
            _stream.Clear(); // очищаем все, что было раньше

            GenerateToneToStream(); // самое интересное

            return _stream;
        }

        private void GenerateToneToStream()
        {
            var count = Processor.CurrentStreamLenght; // сколько семплов нужо сгенерировать
            double timeDelta = 1.0 / Processor.SampleRate; // столько времени разделяет два соседних семпла

            // кешируем ссылки на каналы, чтобы было меньше обращений в цикле
            var leftChannel = _stream.Channels[0];
            var rightChannel = _stream.Channels[1];

            for (int i = 0; i < count; ++i)
            {
                // Frequency и OscillatorType лучше не кешировать - это параметры плагина и
                // они могут меняться
                var frequency = DSPFunctions.GetNoteFrequency(Frequency.Value);
                var sample = WaveGenerator.GenerateNextSample(OscillatorType.Value, frequency, _time);

                leftChannel.Samples[i] = sample;
                rightChannel.Samples[i] = sample;

                _time += timeDelta;
            }
        }
    }
}
