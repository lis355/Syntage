using System;
using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public class Distortion : AudioProcessorPartWithParameters, IProcessor
    {
		public EnumParameter<EPowerStatus> Power { get; private set; }
		
		public RealParameter Treshold { get; private set; }

        public Distortion(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
        }

	    public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
	    {
		    Power = new EnumParameter<EPowerStatus>(parameterPrefix + "Pwr", "Power", "", false);
		    Treshold = new RealParameter(parameterPrefix + "Trshd", "Treshold", "Trshd", 0.1, 1, 0.01);

		    return new List<Parameter> {Power, Treshold};
	    }

	    public void Process(IAudioStream stream)
        {
			if (Power.Value == EPowerStatus.Off)
				return;

            var count = Processor.CurrentStreamLenght;
	        for (int i = 0; i < count; ++i)
	        {
	            var treshold = Treshold.ProcessedValue(i);
                stream.Channels[0].Samples[i] = DistortSample(stream.Channels[0].Samples[i], treshold);
                stream.Channels[1].Samples[i] = DistortSample(stream.Channels[1].Samples[i], treshold);
            }
        }

        private double DistortSample(double sample, double treshold)
        {
            return ((sample > 0) ? Math.Min(sample, treshold) : Math.Max(sample, -treshold)) / treshold;
        }
    }
}
