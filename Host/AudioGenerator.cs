using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using NAudio.Wave;

namespace SimplyHost
{
    public class AudioGenerator : ISampleProvider
    {
        private readonly VstPluginContext _plugin;

        public WaveFormat WaveFormat { get; }

        public AudioGenerator(IVstHostCommands20 host, VstPluginContext plugin)
        {
            _plugin = plugin;

            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int) (0.5 + 1000 * host.GetSampleRate()), 2);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int channelsCount = WaveFormat.Channels;
            int size = count / channelsCount;
            using (var outputMgr = new VstAudioBufferManager(channelsCount, size))
            {
#pragma warning disable 618
                var outputBuffers = outputMgr.ToArray();
#pragma warning restore 618

                _plugin.PluginCommandStub.SetBlockSize(count / channelsCount);
                _plugin.PluginCommandStub.ProcessReplacing(null, outputBuffers);

				int outIndex = 0;
                for (var i = 0; i < count / channelsCount; ++i)
                    foreach (var outputBuffer in outputBuffers)
                        buffer[outIndex++] = outputBuffer[i];

                _plugin.PluginCommandStub.EditorIdle();
            }

            return count;
        }
    }
}
