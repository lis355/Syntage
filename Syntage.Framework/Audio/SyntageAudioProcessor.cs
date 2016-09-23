using System;
using System.Collections.Generic;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Plugin;
using Syntage.Framework.Parameters;

namespace Syntage.Framework.Audio
{
    public abstract class SyntageAudioProcessor : VstPluginAudioProcessorBase, IVstPluginBypass, IAudioStreamProvider
    {
        private readonly List<AudioStream> _audioStreams = new List<AudioStream>();
        private bool _bypass;

        protected SyntageAudioProcessor(int inputCount, int outputCount, int tailSize) :
            base(inputCount, outputCount, tailSize)
        {
        }

        public override float SampleRate
        {
            get { return base.SampleRate; }
            set
            {
                base.SampleRate = value;
                OnSampleRateChanged?.Invoke(this, new SampleRateEventArgs(SampleRate));
            }
        }

        public class SampleRateEventArgs : EventArgs
        {
            public float SampleRate { get; private set; }

            public SampleRateEventArgs(float sampleRate)
            {
                SampleRate = sampleRate;
            }
        }

        public event EventHandler<SampleRateEventArgs> OnSampleRateChanged;

        public bool Bypass
        {
            get { return _bypass; }
            set
            {
                _bypass = value;
                OnBypassChanged?.Invoke(this, new BypassEventArgs(Bypass));
            }
        }

        public class BypassEventArgs : EventArgs
        {
            public bool Bypass { get; private set; }

            public BypassEventArgs(bool bypass)
            {
                Bypass = bypass;
            }
        }

        public event EventHandler<BypassEventArgs> OnBypassChanged;

        public override int BlockSize
        {
            get { return base.BlockSize; }
            set
            {
                if (base.BlockSize == value)
                    return;

                base.BlockSize = value;

                foreach (var stream in _audioStreams)
                    stream.SetBlockSize(BlockSize);
            }
        }

        public IAudioStream CreateAudioStream()
        {
            var stream = new AudioStream();
            stream.Initialize(OutputCount, this);
            stream.SetBlockSize(BlockSize);

            _audioStreams.Add(stream);

            return stream;
        }

        public void ReleaseAudioStream(IAudioStream stream)
        {
            _audioStreams.Remove(stream as AudioStream);
        }

        public int CurrentStreamLenght { get; private set; }

        public override void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            CurrentStreamLenght = outChannels[0].SampleCount;
        }

        public abstract IEnumerable<Parameter> CreateParameters();
    }
}
