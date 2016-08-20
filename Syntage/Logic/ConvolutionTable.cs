using System;

namespace Syntage.Logic
{
    public class ConvolutionTable
    {
        private double[] _a;
        private double[] _b;
        private double[] _x;
        private double[] _y;
        private int _length;

        public void Clear()
        {
            Array.Clear(_x, 0, _length);
            Array.Clear(_y, 0, _length);
        }

        public void SetCoefficients(double[] a, double[] b)
        {
            _length = a.Length;
            _a = a;
            _b = b;
            _x  = new double[_length];
            _y = new double[_length];
        }

        public double Process(double x)
        {
            for (int i = _length - 1; i >= 1; --i)
            {
                _x[i] = _x[i - 1];
                _y[i] = _y[i - 1];
            }

            _x[0] = x;

            double y = _a[0] * _x[0];
            for (int i = 1; i < _length; ++i)
                y += _a[i] * _x[i] - _b[i] * _y[i];

            _y[0] = y;
            
            return y;
        }
    }
}
