using System;
using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public class Distortion : AudioProcessorPartWithParameters, IProcessor
    {
        public RealParameter Treshold { get; private set; }

        public Distortion(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            Treshold = new RealParameter(parameterPrefix + "Trshd", "Treshold", "Trshd", 0, 1, 0.01);

            return new List<Parameter> {Treshold};
        }

        public void Process(IAudioStream stream)
        {
            var treshold = Treshold.Value;
            stream.ProcessAllSamples(f => ((f > 0) ? Math.Min(f, treshold) : Math.Max(f, -treshold)));
        }
    }
}
