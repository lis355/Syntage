using System;

namespace Syntage.Framework.Parameters
{
    public class FrequencyParameter : RealParameter
    {
        public FrequencyParameter(string name, string label, string shortLabel, bool canBeAutomated = true) :
            base(name, label, shortLabel, 1, 20000, 1, canBeAutomated)
        {
        }

		public override double FromReal(double value)
		{
			return Math.Pow(2, Math.Log(Max, 2) * value);
		}

		public override double ToReal(double value)
		{
			throw new NotImplementedException();
		}
	}
}
