using System;
using Syntage.Framework.Tools;

namespace Syntage.Logic
{
    public static class WaveGenerator
    {
        public enum EOscillatorType
        {
            Sine,
            Triangle,
            Square,
            Saw,
            Noise
        }

        private static readonly Random _random = new Random();

        public static double GenerateNextSample(EOscillatorType oscillatorType, double frequency, double time)
        {
            var ph = time * frequency;
            ph -= (int)ph;

            return GetTableSample(oscillatorType, ph);
        }

        public static double GetTableSample(EOscillatorType oscillatorType, double t)
        {
            switch (oscillatorType)
            {
                case EOscillatorType.Sine:
                    return Math.Sin(DSPFunctions.Pi2 * t);

                case EOscillatorType.Triangle:
                    if (t < 0.25) return 4 * t;
                    if (t < 0.75) return 2 - 4 * t;
                    return 4 * (t - 1);

                case EOscillatorType.Square:
                    return (t < 0.5f) ? 1 : -1;

                case EOscillatorType.Saw:
                    return 2 * t - 1;

                case EOscillatorType.Noise:
                    return _random.NextDouble() * 2 - 1;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
