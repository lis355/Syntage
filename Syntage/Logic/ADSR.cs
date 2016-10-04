using System;
using System.Collections.Generic;
using Syntage.Framework.Audio;
using Syntage.Framework.Parameters;

namespace Syntage.Logic
{
    public class ADSR : SyntageAudioProcessorComponentWithParameters<AudioProcessor>, IProcessor
    {
        private class NoteEnvelope
        {
            private enum EState
            {
                None,
                Attack,
                Decay,
                Sustain,
                Release
            }

            private readonly ADSR _ownerEnvelope;
            private double _time;
            private double _multiplier;
            private double _startMultiplier;
            private EState _state;
            private int _sampleNumber;

            public NoteEnvelope(ADSR owner)
            {
                _ownerEnvelope = owner;
            }

            public double GetNextMultiplier(int sampleNumber)
            {
                _sampleNumber = sampleNumber;

                var startMultiplier = GetCurrentStateStartValue();
                var finishMultiplier = GetCurrentStateFinishValue();

                // время уменьшается, поэтому используем обратную величину
                var stateTime = 1 - _time / GetCurrentStateMultiplier();

                // логика интерполяция между startMultiplier и finishMultiplier скрыта в CalculateLevel
                _multiplier = CalculateLevel(startMultiplier, finishMultiplier, stateTime);

                switch (_state)
                {
                    case EState.None:
                        break;

                    case EState.Attack:
                        if (_time < 0)
                            SetState(EState.Decay);
                        break;

                    case EState.Decay:
                        if (_time < 0)
                            SetState(EState.Sustain);
                        break;

                    case EState.Sustain:
                        // сустейн не ограничен по времени
                        break;

                    case EState.Release:
                        if (_time < 0)
                            SetState(EState.None);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // вычитаем время одного семпла
                var timeDelta = 1.0 / _ownerEnvelope.Processor.SampleRate;
                _time -= timeDelta;

                return _multiplier;
            }

            public void Press()
            {
                SetState(EState.Attack);
            }

            public void Release()
            {
                SetState(EState.Release);
            }

            private double CalculateLevel(double a, double b, double t)
            {
                //return DSPFunctions.Lerp(a, b, t);

                switch (_state)
                {
                    case EState.None:
                    case EState.Sustain:
                        return a;

                    case EState.Attack:
                        return Math.Log(1 + t * (Math.E - 1)) * Math.Abs(b - a) + a;

                    case EState.Decay:
                    case EState.Release:
                        return (Math.Exp(1 - t) - 1) / (Math.E - 1) * (a - b) + b;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private void SetState(EState newState)
            {
                // запомним текущее значение огибающей - это будет стартовое значение для новой фазы
                _startMultiplier = (_time > 0) ? _multiplier : GetCurrentStateFinishValue();

                _state = newState;

                // получим время новой фазы
                _time = GetCurrentStateMultiplier();
            }

            private double GetCurrentStateStartValue()
            {
                return _startMultiplier;
            }

            private double GetCurrentStateFinishValue()
            {
                switch (_state)
                {
                    case EState.None:
                        return 0;

                    case EState.Attack:
                        return 1;

                    case EState.Decay:
                    case EState.Sustain:
                        return GetSustainMultiplier();

                    case EState.Release:
                        return 0;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private double GetCurrentStateMultiplier()
            {
                switch (_state)
                {
                    case EState.None:
                        return 1;

                    case EState.Attack:
                        return _ownerEnvelope.Attack.ProcessedValue(_sampleNumber);

                    case EState.Decay:
                        return _ownerEnvelope.Decay.ProcessedValue(_sampleNumber);

                    case EState.Sustain:
                        return GetSustainMultiplier();

                    case EState.Release:
                        return _ownerEnvelope.Release.ProcessedValue(_sampleNumber);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private double GetSustainMultiplier()
            {
                return _ownerEnvelope.Sustain.ProcessedValue(_sampleNumber);
            }
        }

        private readonly NoteEnvelope _noteEnvelope;
        private int _lastPressedNotesCount;

        public RealParameter Attack { get; private set; }
        public RealParameter Decay { get; private set; }
        public RealParameter Sustain { get; private set; }
        public RealParameter Release { get; private set; }

        public ADSR(AudioProcessor audioProcessor) : base(audioProcessor)
        {
            _noteEnvelope = new NoteEnvelope(this);

            Processor.Input.OnPressedNotesChanged += OnPressedNotesChanged;
        }

        public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
        {
            Attack = new RealParameter(parameterPrefix + "Atk", "Envelope Attack", "", 0.01, 1, 0.01);
            Decay = new RealParameter(parameterPrefix + "Dec", "Envelope Decay", "", 0.01, 1, 0.01);
            Sustain = new RealParameter(parameterPrefix + "Stn", "Envelope Sustain", "", 0, 1, 0.01);
            Release = new RealParameter(parameterPrefix + "Rel", "Envelope Release", "", 0.01, 1, 0.01);

            return new List<Parameter> {Attack, Decay, Sustain, Release};
        }

        private void OnPressedNotesChanged(object sender, EventArgs e)
        {
            // если сейчас нажаты какие-то клавиши, а раньше не были - регистрируем нажатие
            // если сейчас нет нажатых клавиш, а раньше были нажаты - значит отжали последнюю клвишу,
            // регистрируем релиз

            var currentPressedNotesCount = Processor.Input.PressedNotesCount;
            if (currentPressedNotesCount > 0
                && _lastPressedNotesCount == 0)
            {
                _noteEnvelope.Press();
            }
            else if (currentPressedNotesCount == 0
                && _lastPressedNotesCount > 0)
            {
                _noteEnvelope.Release();
            }

            _lastPressedNotesCount = currentPressedNotesCount;
        }

        public void Process(IAudioStream stream)
        {
            var lc = stream.Channels[0];
            var rc = stream.Channels[1];

            var count = Processor.CurrentStreamLenght;
            for (int i = 0; i < count; ++i)
            {
                var multiplier = _noteEnvelope.GetNextMultiplier(i);
                lc.Samples[i] *= multiplier;
                rc.Samples[i] *= multiplier;
            }
        }
    }
}
