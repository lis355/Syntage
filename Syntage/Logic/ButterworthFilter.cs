using System;
using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
    public enum EFilterPass
    {
        None,

        LowPass,
        HiPass,
        BandPass
    }

    public class ButterworthFilter : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IProcessor
    {
        private class BiquadConvolutionTable
        {
            public double B0, B1, B2, A1, A2;

            private readonly double[] _x = new double[3];
            private readonly double[] _y = new double[3];

            public double Process(double s)
            {
                // "сдвигаем" предыдущие семплы
                _x[2] = _x[1];
                _x[1] = _x[0];
                _x[0] = s;

                _y[2] = _y[1];
                _y[1] = _y[0];

                // свертка
                _y[0] = B0 * _x[0] + B1 * _x[1] + B2 * _x[2] - A1 * _y[1] - A2 * _y[2];

                return _y[0];
            }
        }

        private readonly BiquadConvolutionTable _tablel;
        private readonly BiquadConvolutionTable _tabler;

        public EnumParameter<EFilterPass> FilterType { get; private set; }
        public FrequencyParameter CutoffFrequency { get; private set; }

        public ButterworthFilter(AudioProcessor audioProcessor) : base(audioProcessor)
        {
            _tablel = new BiquadConvolutionTable();
            _tabler = new BiquadConvolutionTable();
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            FilterType = new EnumParameter<EFilterPass>(parameterPrefix + "Pass", "Filter Type", "Filter", false);
            CutoffFrequency = new FrequencyParameter(parameterPrefix + "Cutoff", "Filter Cutoff Frequency", "Cutoff");

            return new List<Parameter> {FilterType, CutoffFrequency};
        }

        public void Process(IAudioStream stream)
        {
            if (FilterType.Value == EFilterPass.None)
                return;

            var count = Processor.CurrentStreamLenght;
            var lc = stream.Channels[0];
            var rc = stream.Channels[1];
            for (int i = 0; i < count; ++i)
            {
                var cutoff = CutoffFrequency.ProcessedValue(i);
                CalculateCoefficients(cutoff);

                var ls = _tablel.Process(lc.Samples[i]);
                lc.Samples[i] = ls;

                var rs = _tabler.Process(rc.Samples[i]);
                rc.Samples[i] = rs;
            }
        }

        private double TransformFrequency(double w)
        {
            return Math.Tan(Math.PI * w / Processor.SampleRate);
        }

        private void CalculateCoefficients(double cutoff)
        {
            double b0, b1, b2, a0, a1, a2;

            switch (FilterType.Value)
            {
                case EFilterPass.LowPass:
                {
                    var w = TransformFrequency(cutoff);

                    a0 = 1 + Math.Sqrt(2) * w + w * w;
                    a1 = -2 + 2 * w * w;
                    a2 = 1 - Math.Sqrt(2) * w + w * w;

                    b0 = w * w;
                    b1 = 2 * w * w;
                    b2 = w * w;
                }

                    break;

                case EFilterPass.HiPass:
                {
                    var w = TransformFrequency(cutoff);

                    a0 = 1 + Math.Sqrt(2) * w + w * w;
                    a1 = -2 + 2 * w * w;
                    a2 = 1 - Math.Sqrt(2) * w + w * w;

                    b0 = 1;
                    b1 = -2;
                    b2 = 1;
                }

                    break;

                case EFilterPass.BandPass:
                {
                    var w = cutoff;
                    var d = w / 4;

                    // определим полосу фильтра как [w * 3 / 4, w * 5 / 4]
                    var w1 = Math.Max(w - d, CutoffFrequency.Min);
                    var w2 = Math.Min(w + d, CutoffFrequency.Max);
                    w1 = TransformFrequency(w1);
                    w2 = TransformFrequency(w2);

                    var w0Sqr = w2 * w1; // w0^2
                    var wd = w2 - w1; // W

                    a0 = -1 - wd - w0Sqr;
                    a1 = 2 - 2 * w0Sqr;
                    a2 = -1 + wd - w0Sqr;
                    b0 = -wd;
                    b1 = 0;
                    b2 = wd;
                }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _tablel.B0 = _tabler.B0 = b0 / a0;
            _tablel.B1 = _tabler.B1 = b1 / a0;
            _tablel.B2 = _tabler.B2 = b2 / a0;
            _tablel.A1 = _tabler.A1 = a1 / a0;
            _tablel.A2 = _tabler.A2 = a2 / a0;
        }
    }
}
