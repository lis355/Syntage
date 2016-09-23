using System;
using Jacobi.Vst.Core;

namespace Syntage.Framework.Audio
{
    public interface IAudioStream
    {
        IAudioChannel[] Channels { get; }

        int Length { get; }

        void Clear();
        void ProcessAllSamples(Func<double, double> action);
        void Mix(IAudioStream a, double aMultiplier, IAudioStream b, double bMultiplier);
        void Mono();
    }

    // Нельзя хранить ссылки на AudioStream или AudioChannel
    // нужно их обрабатывать только в функциях Process и Generate
    // для дилеев нужно копировать в буфера
    public class AudioStream : IAudioStream
    {
        private IAudioStreamProvider _streamProvider;
        private int _channelsCount;
        private AudioChannel[] _channels;

        // ReSharper disable once CoVariantArrayConversion
        public IAudioChannel[] Channels { get { return _channels; } }
        public int Length { get { return _streamProvider.CurrentStreamLenght; } }

        public void Initialize(int outputCount, IAudioStreamProvider provider)
        {
            _channelsCount = outputCount;
            _streamProvider = provider;

            _channels = new AudioChannel[_channelsCount];
            for (int i = 0; i < _channelsCount; ++i)
                _channels[i] = new AudioChannel();
        }

        public void SetBlockSize(int blockSize)
        {
            for (int i = 0; i < _channelsCount; ++i)
                _channels[i].Initialize(blockSize, _streamProvider);
        }

        public void Clear()
        {
            for (int i = 0; i < _channelsCount; ++i)
                _channels[i].Clear();
        }

        public void ProcessAllSamples(Func<double, double> action)
        {
            for (int i = 0; i < _channelsCount; ++i)
                _channels[i].ProcessAllSamples(action);
        }

        public void Mix(IAudioStream a, double aMultiplier, IAudioStream b, double bMultiplier)
        {
            for (int i = 0; i < _channelsCount; ++i)
                _channels[i].Mix(a.Channels[i], aMultiplier, b.Channels[i], bMultiplier);
        }

        public void Mono()
        {
            int count = Length;
            for (int j = 0; j < count; ++j)
            {
                double a = 0;
                for (int i = 0; i < _channelsCount; ++i)
                    a += Channels[i].Samples[j];

                a /= _channelsCount;

                for (int i = 0; i < _channelsCount; ++i)
                    Channels[i].Samples[j] = a;
            }
        }

        public void WriteToVstOut(VstAudioBuffer[] outChannels)
        {
            int count = Length;
            for (int i = 0; i < _channelsCount; ++i)
            {
                var ch = outChannels[i];
                var generatedCh = Channels[i];
                for (int j = 0; j < count; ++j)
                    ch[j] = (float)generatedCh.Samples[j];
            }
        }
    }
}
