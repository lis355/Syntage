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
            public readonly double[] Data;
            public int Index;

            public Buffer(int length)
            {
                Data = new double[length];
                Index = 0;
            }
        }

        private Buffer _lbuffer;
        private Buffer _rbuffer;

        public EnumParameter<EPowerStatus> Power { get; private set; }
        public RealParameter DryLevel { get; private set; }
        public RealParameter DelaySeconds { get; private set; }
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
            DelaySeconds = new RealParameter(parameterPrefix + "Sec", "Delay Seconds", "Sec", 0, 5, 0.01);
            Feedback = new RealParameter(parameterPrefix + "Fbck", "Feedback", "Feedback", 0, 1, 0.01);

            return new List<Parameter> {Power, DryLevel, DelaySeconds, Feedback};
        }

        public void ClearBuffer()
        {
            if (_rbuffer != null)
                Array.Clear(_rbuffer.Data, 0, _rbuffer.Data.Length);

            if (_lbuffer != null)
                Array.Clear(_lbuffer.Data, 0, _lbuffer.Data.Length);
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
            var size = (int)(e.SampleRate * DelaySeconds.Max);
            _lbuffer = new Buffer(size);
            _rbuffer = new Buffer(size);
        }

        private double ProcessSample(double sample, int sampleNumber, Buffer buffer)
        {
            var dry = DryLevel.Value;
            var wet = 1 - dry;
            var bufSample = buffer.Data[buffer.Index];
            var output = dry * sample + wet * bufSample;

            buffer.Data[buffer.Index] = sample + Feedback.ProcessedValue(sampleNumber) * bufSample;

            int length = (int)(DelaySeconds.ProcessedValue(sampleNumber) * Processor.SampleRate);
            buffer.Index = (buffer.Index + 1) % length;

            return output;
        }

        private void ParametersManagerOnProgramChange(object sender, ParametersManager.ProgramChangeEventArgs e)
        {
            ClearBuffer();
        }
    }
}
