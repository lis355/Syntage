using System;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
    public class FrequencyParameter : RealParameter
    {
        public FrequencyParameter(string name, string label, string shortLabel, double min = 1, double max = 20000, bool canBeAutomated = true) :
            base(name, label, shortLabel, min, max, 1, canBeAutomated)
        {
            if (Min < 0
                || DSPFunctions.IsZero(Min))
                throw new ArgumentException();
        }

        public override double FromReal(double value)
		{
		    return Min * Math.Pow(Max / Min, value);
		}
        
		public override double ToReal(double value)
		{
		    return Math.Log(value / Min, Max / Min);
		}
	}
}
