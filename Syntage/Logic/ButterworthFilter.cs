using System;
using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Logic.Audio;

namespace Syntage.Logic
{
    public enum EFilterPass
    {
        None,

        LowPass,
        HiPass,
        BandPass
    }

    public class ButterworthFilter : AudioProcessorPartWithParameters, IProcessor
    {
        private class BiquadConvolutionTable
        {
            public double A0;
            public double A1;
            public double A2;
            public double B1;
            public double B2;

            private readonly double[] _x = new double[3];
            private readonly double[] _y = new double[3];
            
            public double Process(double s)
            {
                _x[2] = _x[1];
                _x[1] = _x[0];
                _x[0] = s;

                _y[2] = _y[1];
                _y[1] = _y[0];
                _y[0] = A0 * _x[0] + A1 * _x[1] + A2 * _x[2] - B1 * _y[1] - B2 * _y[2];

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
            double a0, a1, a2, b0, b1, b2;
            
            var filterType = FilterType.Value;
            if (filterType == EFilterPass.BandPass)
            {
                var w = cutoff;
                var d = w / 4;
                var w1 = Math.Max(w - d, CutoffFrequency.Min);
                var w2 = Math.Min(w + d, CutoffFrequency.Max);
                w1 = TransformFrequency(w1);
                w2 = TransformFrequency(w2);
                var w0Sqr = w2 * w1;
                var wd = w2 - w1;
                
                b0 = -1 - wd - w0Sqr;
                b1 = (2 - 2 * w0Sqr) / b0;
                b2 = (-1 + wd - w0Sqr) / b0;
                a0 = -wd / b0;
                a1 = 0;
                a2 = wd / b0;
            }
            else
            {
                var w = TransformFrequency(cutoff);

                b0 = 1 + Math.Sqrt(2) * w + w * w;
                b1 = (-2 + 2 * w * w) / b0;
                b2 = (1 - Math.Sqrt(2) * w + w * w) / b0;

                if (filterType == EFilterPass.LowPass)
                {
                    a0 = w * w / b0;
                    a1 = 2 * w * w / b0;
                    a2 = w * w / b0;
                }
                else // EFilterPass.HiPass
                {
                    a0 = 1 / b0;
                    a1 = -2 / b0;
                    a2 = 1 / b0;
                }
            }

            _tablel.A0 = _tabler.A0 = a0;
            _tablel.A1 = _tabler.A1 = a1;
            _tablel.A2 = _tabler.A2 = a2;
            _tablel.B1 = _tabler.B1 = b1;
            _tablel.B2 = _tabler.B2 = b2;
        }
    }
}
