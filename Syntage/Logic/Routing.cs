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

                // создаем шум
			    var noiseStream = Processor.NoiseGenerator.Generate();

			    // смешиваем шум и звук с осцилляторов
			    stream.Mix(stream, 1, noiseStream, 1);

			    // эффект дисторшн
				Processor.Distortion.Process(stream);

				// клиппинг перед мастером
				if (Processor.Clip.ClipSample.Value)
					Processor.Clip.Process(stream);

				// мастер-обработка
				Processor.MasterBus.Process(stream);
			}

			// отправляем результат в осциллограф
			Processor.Oscillograph.Process(stream);
		}
	}
}
