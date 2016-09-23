using System.Collections.Generic;
using Jacobi.Vst.Core;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;
using Syntage.Plugin;

namespace Syntage.Logic
{
    public class AudioProcessor : SyntageAudioProcessor
    {
        private readonly AudioStream _mainStream;

        public readonly PluginController PluginController;

        public Oscillator Oscillator { get; }

        public AudioProcessor(PluginController pluginController) :
			base(0, 2, 0)
        {
            _mainStream = (AudioStream)CreateAudioStream();

            PluginController = pluginController;

            Oscillator = new Oscillator(this);
        }
        
        public override IEnumerable<Parameter> CreateParameters()
        {
            var parameters = new List<Parameter>();
            
            parameters.AddRange(Oscillator.CreateParameters("O"));
            
            return parameters;
        }

        public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            base.Process(inChannels, outChannels);

            // генерируем семплы
            var stream = Oscillator.Generate();

            // копируем полученный stream в _mainStream
            _mainStream.Mix(stream, 1, _mainStream, 0);

            // отправляем результат 
            _mainStream.WriteToVstOut(outChannels);
        }
    }
}
