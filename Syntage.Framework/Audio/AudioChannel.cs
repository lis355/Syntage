using System;

namespace Syntage.Framework.Audio
{
    public interface IAudioChannel
    {
        double[] Samples { get; }

        void Clear();
        void ProcessAllSamples(Func<double, double> action);
        void Mix(IAudioChannel a, double aMultiplier, IAudioChannel b, double bMultiplier);
    }

    public class AudioChannel : IAudioChannel
    {
        private IAudioStreamProvider _streamProvider;

        public double[] Samples { get; private set; }

        private int Count { get { return _streamProvider.CurrentStreamLenght; } }

        public void Initialize(int blockSize, IAudioStreamProvider provider)
        {
            Samples = new double[blockSize];
            _streamProvider = provider;
        }
        
        public void Clear()
        {
			Array.Clear(Samples, 0, Count);
        }

        public void ProcessAllSamples(Func<double, double> action)
        {
            int count = Count;
            for (int j = 0; j < count; ++j)
                Samples[j] = action(Samples[j]);
        }

        public void Mix(IAudioChannel a, double aMultiplier, IAudioChannel b, double bMultiplier)
        {
            var aSamples = a.Samples;
            var bSamples = b.Samples;

            int count = Count;
            for (int j = 0; j < count; ++j)
                Samples[j] = aSamples[j] * aMultiplier + bSamples[j] * bMultiplier;
        }
    }
}
