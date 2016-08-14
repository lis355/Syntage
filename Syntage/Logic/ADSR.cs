using System;
using System.Collections.Generic;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;

namespace Syntage.Logic
{
    public class ADSR : AudioProcessorPartWithParameters
    {
        public enum EState
        {
            None,
            Attack,
            Decay,
            Sustain,
            Release
        }

        public class NoteEnvelope
        {
			private readonly ADSR _ownerEnvelope;
			private readonly double _timeDelta;
			private double _time;
			private double _multiplier;
	        private double _releaseStartMultiplier;
			
	        public EState State { get; private set; }

	        public NoteEnvelope(ADSR owner)
	        {
		        _ownerEnvelope = owner;
		        _timeDelta = 1.0 / _ownerEnvelope.Processor.SampleRate;
	        }

			public double GetNextMultiplier()
			{
				_multiplier = 0;

		        switch (State)
		        {
					case EState.Attack:
						_multiplier = DSPFunctions.Lerp(0, 1, 1 - _time / _ownerEnvelope.Attack.Value);
						if (_time < 0)
							SetState(EState.Decay);
						break;

					case EState.Decay:
						_multiplier = DSPFunctions.Lerp(1, _ownerEnvelope.Sustain.Value, 1 - _time / _ownerEnvelope.Decay.Value);
						if (_time < 0)
							SetState(EState.Sustain);
						break;

					case EState.Sustain:
						_multiplier = _ownerEnvelope.Sustain.Value;
						break;

					case EState.Release:
						_multiplier = DSPFunctions.Lerp(_releaseStartMultiplier, 0, 1 - _time / _ownerEnvelope.Release.Value);
						if (_time < 0)
							SetState(EState.None);
						break;
		        }

				_time -= _timeDelta;
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

			private void SetState(EState newState)
			{
				switch (newState)
				{
					case EState.None:
						break;

					case EState.Attack:
						_time = _ownerEnvelope.Attack.Value;
						break;

					case EState.Decay:
						_time = _ownerEnvelope.Decay.Value;

						break;

					case EState.Sustain:
						break;

					case EState.Release:
						_releaseStartMultiplier = _multiplier;
						_time = _ownerEnvelope.Release.Value;
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
				}

				State = newState;
			}
		}

		public RealParameter Attack { get; private set; }
	    public RealParameter Decay { get; private set; }
	    public RealParameter Sustain { get; private set; }
	    public RealParameter Release { get; private set; }

	    public ADSR(AudioProcessor audioProcessor) : base(audioProcessor)
	    {
	    }

	    public override IEnumerable<Parameter> CreateParameters(string parameterPrefix)
	    {
		    Attack = new RealParameter(parameterPrefix + "Atk", "Envelope Attack", "", 0.01, 1, 0.01);
		    Decay = new RealParameter(parameterPrefix + "Dec", "Envelope Decay", "", 0.01, 1, 0.01);
		    Sustain = new RealParameter(parameterPrefix + "Stn", "Envelope Sustain", "", 0, 1, 0.01);
		    Release = new RealParameter(parameterPrefix + "Rel", "Envelope Release", "", 0.01, 1, 0.01);

		    return new List<Parameter> {Attack, Decay, Sustain, Release};
	    }

	    public NoteEnvelope CreateEnvelope()
	    {
		    return new NoteEnvelope(this);
	    }
    }
}
