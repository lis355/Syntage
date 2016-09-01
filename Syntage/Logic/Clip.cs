using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public class Clip : AudioProcessorPartWithParameters, IProcessor
    {
        public EnumParameter<EPowerStatus> Power { get; private set; }

        public Clip(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            Power = new EnumParameter<EPowerStatus>(parameterPrefix + "Clip", "Clip", "Clip", false);

            return new List<Parameter> {Power};
        }

        public void Process(IAudioStream stream)
        {
            if (Power.Value == EPowerStatus.Off)
                return;

            stream.ProcessAllSamples(DSPFunctions.Clamp0Db);
        }
    }
}
