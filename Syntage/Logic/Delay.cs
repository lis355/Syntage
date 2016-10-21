using System;
using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
    public class Delay : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IProcessor
    {
        private class Buffer
        {
            private int _index;
            private readonly double[] _data;

            public Buffer(int length)
            {
                _data = new double[length];
                _index = 0;
            }

            public double Current
            {
                get { return _data[_index]; }
                set { _data[_index] = value; }
            }

            public void Increment(int currentLength)
            {
                _index = (_index + 1) % currentLength;
            }

            public void Clear()
            {
                Array.Clear(_data, 0, _data.Length);
            }
        }

        private Buffer _lbuffer;
        private Buffer _rbuffer;

        public EnumParameter<EPowerStatus> Power { get; private set; }
        public RealParameter DryLevel { get; private set; }
        public RealParameter Time { get; private set; }
        public RealParameter Feedback { get; private set; }

        public Delay(AudioProcessor audioProcessor) :
            base(audioProcessor)
        {
            audioProcessor.OnSampleRateChanged += OnSampleRateChanged;
            audioProcessor.PluginController.ParametersManager.OnProgramChange += ParametersManagerOnProgramChange;
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            Power = new EnumParameter<EPowerStatus>(parameterPrefix + "Pwr", "Power", "", false);
            DryLevel = new RealParameter(parameterPrefix + "Dry", "Dry Level", "Dry", 0, 1, 0.01);
            Time = new RealParameter(parameterPrefix + "Sec", "Delay Time", "Time", 0, 5, 0.01);
            Feedback = new RealParameter(parameterPrefix + "Fbck", "Feedback", "Feedback", 0, 1, 0.01);

            return new List<Parameter> {Power, DryLevel, Time, Feedback};
        }

        public void ClearBuffer()
        {
            _rbuffer?.Clear();
            _lbuffer?.Clear();
        }

        public void Process(IAudioStream stream)
        {
            if (Power.Value == EPowerStatus.Off)
                return;

            var leftChannel = stream.Channels[0];
            var rightChannel = stream.Channels[1];

            var count = Processor.CurrentStreamLenght;
            for (int i = 0; i < count; ++i)
            {
                leftChannel.Samples[i] = ProcessSample(leftChannel.Samples[i], i, _lbuffer);
                rightChannel.Samples[i] = ProcessSample(rightChannel.Samples[i], i, _rbuffer);
            }
        }
        
        private void OnSampleRateChanged(object sender, SyntageAudioProcessor.SampleRateEventArgs e)
        {
            var size = (int)(e.SampleRate * Time.Max);
            _lbuffer = new Buffer(size);
            _rbuffer = new Buffer(size);
        }

        private void ParametersManagerOnProgramChange(object sender, ParametersManager.ProgramChangeEventArgs e)
        {
            ClearBuffer();
        }

        private double ProcessSample(double sample, int sampleNumber, Buffer buffer)
        {
            var dry = DryLevel.Value;
            var wet = 1 - dry;

            var output = dry * sample + wet * buffer.Current;
            buffer.Current = sample + Feedback.ProcessedValue(sampleNumber) * buffer.Current;

            int length = (int)(Time.ProcessedValue(sampleNumber) * Processor.SampleRate);
            buffer.Increment(length);

            return output;
        }
    }
}
