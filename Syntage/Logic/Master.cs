using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
    public class Master : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IProcessor
    {
        public VolumeParameter MasterVolume { get; private set; }

        public Master(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            MasterVolume = new VolumeParameter(parameterPrefix + "Vol", "Master Volume", false);

            return new List<Parameter> {MasterVolume};
        }
        
        public void Process(IAudioStream stream)
        {
            var volume = MasterVolume.Value;
            stream.ProcessAllSamples(a => a * volume);
        }
    }
}
