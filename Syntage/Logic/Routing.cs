using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
	public class Routing : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IProcessor
	{
		public BypassParameter Power { get; private set; }
		public RealParameter OscillatorsMix { get; private set; }

		public Routing(AudioProcessor audioProcessor) :
			base(audioProcessor)
		{
		}

		public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
		{
			Power = new BypassParameter(parameterPrefix + "Pwr", "Power", Processor, "Pwr", false);
			OscillatorsMix = new RealParameter(parameterPrefix + "Mix", "Oscillators Mix", "Mix", 0, 1, 0.01);
            OscillatorsMix.SetDefaultValue(0.5);

            return new List<Parameter> {Power, OscillatorsMix};
		}
        
        public void Process(IAudioStream stream)
		{
			if (Power.Value == EPowerStatus.Off)
			{
				stream.Clear();
			}
			else
			{
				// создаем звук на осциляторе А
				var oscAStream = Processor.OscillatorA.Generate();

                // огибающая A
                Processor.EnvelopeA.Process(oscAStream);

				// создаем звук на осциляторе B
				var oscBStream = Processor.OscillatorB.Generate();
               
                // огибающая B
                Processor.EnvelopeB.Process(oscBStream);

                // смешиваем звук с осциллятора А и B
                MixOscillators(stream, oscAStream, oscBStream);
                
                // фильтр
                Processor.Filter.Process(stream);

			    // эффект дисторшн
				Processor.Distortion.Process(stream);

                // дилэй
                Processor.Delay.Process(stream);

				// клиппинг перед мастером
				Processor.Clip.Process(stream);

				// мастер-обработка
				Processor.Master.Process(stream);

                // скажем лфо что был обработан блок
                Processor.LFOModifier.Process(stream);
			}

			// отправляем результат в осциллограф
			Processor.Oscillograph.Process(stream);
		}

	    private void MixOscillators(IAudioStream stream, IAudioStream oscAStream, IAudioStream oscBStream)
	    {
            var count = Processor.CurrentStreamLenght;
	        for (int i = 0; i < count; ++i)
	        {
	            var oscillatorsMix = OscillatorsMix.ProcessedValue(i);
                stream.Channels[0].Samples[i] = oscAStream.Channels[0].Samples[i] * (1 - oscillatorsMix) + oscBStream.Channels[0].Samples[i] * oscillatorsMix;
                stream.Channels[1].Samples[i] = oscAStream.Channels[1].Samples[i] * (1 - oscillatorsMix) + oscBStream.Channels[1].Samples[i] * oscillatorsMix;
            }
        }
	}
}
