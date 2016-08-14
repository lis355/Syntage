using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public class Clip : AudioProcessorPartWithParameters, IProcessor
    {
        public BooleanParameter ClipSample { get; private set; }

        public Clip(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            ClipSample = new BooleanParameter(parameterPrefix + "Clip", "Clip", "Clip");

            return new List<Parameter> { ClipSample };
        }

        public void Process(IAudioStream stream)
        {
            stream.ProcessAllSamples(DSPFunctions.Clamp0Db);
        }
    }
}
