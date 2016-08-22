using System;
using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public class Noise : AudioProcessorPartWithParameters, IGenerator
    {
        private readonly IAudioStream _stream;
        private readonly Random _random = new Random();

        public VolumeParameter Volume { get; private set; }

        public Noise(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
            _stream = audioProcessor.CreateAudioStream();
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            Volume = new VolumeParameter(parameterPrefix + "Vol", "Noise Volume");
            Volume.SetDefaultValue(0);

            return new List<Parameter> {Volume};
        }

        public IAudioStream Generate()
        {
            if (Processor.Input.PressedNotesCount == 0)
            {
                _stream.Clear();
            }
            else
            {
                var vol = Volume.Value;

                Func<double, double> generator = a => vol * (_random.NextDouble() * 2 - 1);

                _stream.ProcessAllSamples(generator);
                _stream.Mono();                
            }

            return _stream;
        }
    }
}
