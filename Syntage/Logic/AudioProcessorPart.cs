using System.Collections.Generic;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
    public abstract class AudioProcessorPart
    {
        public AudioProcessor Processor { get; }

        protected AudioProcessorPart(AudioProcessor audioProcessor)
        {
            Processor = audioProcessor;
        }
    }
    
    public abstract class AudioProcessorPartWithParameters : AudioProcessorPart
    {
        public abstract IEnumerable<Parameter> CreateParameters(string parameterPrefix);

        protected AudioProcessorPartWithParameters(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
        }
    }
}
