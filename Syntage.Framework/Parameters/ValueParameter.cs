using System;
using System.Diagnostics;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
    [DebuggerDisplay("{Name}")]
    public abstract class Parameter<T> : Parameter where T : struct
    {
        protected Parameter(string name, string description, string label,
			double min, double max, double step, bool canBeAutomated = true) :
            base(name, description, label, canBeAutomated)
	    {
			if (max <= min
				|| step > (max - min)) throw new ArgumentException();
            
		    Min = min;
		    Max = max;

            Step = Range / Math.Max(1, (int)(1 / DSPFunctions.Clamp01(step / Range)));
            RealStep = Step / Range;
	    }

        public void SetDefaultValue(T value)
        {
            RealDefaultValue = ToReal(value);
        }

        public double Min { get; }
		public double Max { get; }
        public double Range { get { return Max - Min; } }
        public double Step { get; }

        public T Value
	    {
		    get { return FromReal(RealValue); }
		    set { SetValueFromPlugin(ToReal(value)); }
	    }

        public T? DefaultValue
        {
            get { return (RealDefaultValue.HasValue) ? FromReal(RealDefaultValue.Value) : (T?)null; }
        }

        public abstract T FromReal(double value);
		public abstract double ToReal(T value);

		public abstract T FromStringToValue(string s);
	    public abstract string FromValueToString(T value);

        public override double Parse(string s)
        {
            return ToReal(FromStringToValue(s));
	    }

        public override string GetDisplayValue()
        {
            return FromValueToString(Value);
        }

        public T ProcessedValue(int sampleNumber)
        {
            return FromReal(ProcessedRealValue(sampleNumber));
        }
    }
}
