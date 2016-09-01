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
	        return DSPFunctions.AmplitudeToDb(Value);
	    }

		public override double FromStringToValue(string s)
		{
			var volumeDb = DSPFunctions.Clamp(ParseDouble(s), DSPFunctions.KMinADb, DSPFunctions.KMaxADb);
			return (volumeDb - DSPFunctions.KMinADb) / (DSPFunctions.KMaxADb - DSPFunctions.KMinADb);
		}

		public override string FromValueToString(double value)
		{
			return PrintDouble(DSPFunctions.AmplitudeToDb(value));
		}
	}
}
