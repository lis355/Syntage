using System.Collections.Generic;
using Syntage.Framework.Parameters;

namespace Syntage.Framework.Audio
{
    public abstract class SyntageAudioProcessorComponent<T> where T : SyntageAudioProcessor
    {
        public T Processor { get; }

        protected SyntageAudioProcessorComponent(T audioProcessor)
        {
            Processor = audioProcessor;
        }
    }
    
    public abstract class SyntageAudioProcessorComponentWithParameters<T> : SyntageAudioProcessorComponent<T> where T : SyntageAudioProcessor
    {
        public abstract IEnumerable<Parameter> CreateParameters(string parameterPrefix);

        protected SyntageAudioProcessorComponentWithParameters(T audioProcessor) :
            base(audioProcessor)
        {
        }
    }
}
