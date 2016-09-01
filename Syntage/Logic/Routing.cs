using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
	public class Routing : AudioProcessorPartWithParameters, IProcessor
	{
		public BypassParameter Power { get; private set; }
		public RealParameter OscillatorsMix { get; private set; }

		public Routing(AudioProcessor audioProcessor) :
			base(audioProcessor)
		{
		}

		public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
		{
			Power = new BypassParameter(parameterPrefix + "Pwr", "Power", Processor);
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

				// создаем звук на осциляторе B
				var oscBStream = Processor.OscillatorB.Generate();

				// смешиваем звук с осциллятора А и B
				stream.Mix(oscAStream, 1 - OscillatorsMix.Value, oscBStream, OscillatorsMix.Value);
                
                // огибающая
                Processor.Envelope.Process(stream);

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
	}
}
