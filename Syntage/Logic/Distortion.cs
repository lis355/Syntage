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

            var treshold = Treshold.Value;
            stream.ProcessAllSamples(f => ((f > 0) ? Math.Min(f, treshold) : Math.Max(f, -treshold)) / treshold);
        }
    }
}
