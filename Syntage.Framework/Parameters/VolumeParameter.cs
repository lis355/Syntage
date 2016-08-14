using System;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
	public class VolumeParameter : RealParameter
	{
		public VolumeParameter(string name, string label, bool canBeAutomated = true) :
			base(name, label, "dB", 0, 1, 0.01f, canBeAutomated)
		{
            SetDefaultValue(1);
		}

	    public double GetDb()
	    {
	        return DSPFunctions.AmplitudeToDb(Math.Max(DSPFunctions.KMinA, Value));
	    }

		public override string FromValueToString(double value)
		{
			return PrintDouble(GetDb());
		}
	}
}
