using System;
using System.Diagnostics;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
    [DebuggerDisplay("{Name}")]
    public abstract class Parameter<T> : Parameter where T : struct
	{
        private T? _defaultValue;

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

        public void SetDefaultValue(T? value)
        {
            DefaultValue = value;
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
            get { return _defaultValue; }
            set
            {
                if (!value.HasValue)
                    throw new ArgumentNullException();

                var defaultValueReal = ToReal(value.Value);
                if (defaultValueReal < 0
                    || defaultValueReal > 1)
                    throw new ArgumentNullException();

                _defaultValue = value;
            }
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
	}
}
