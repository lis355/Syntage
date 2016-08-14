using System.Globalization;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
	public class IntegerParameter : Parameter<int>
	{
		public IntegerParameter(string name, string label, string shortLabel, int min, int max,
			int step, bool canBeAutomated = true) :
			base(name, label, shortLabel, min, max, step, canBeAutomated)
		{
		}

		public override int FromReal(double value)
		{
			return (int)DSPFunctions.Lerp(Min, Max, value);
		}

		public override double ToReal(int value)
		{
			return (value - Min) / (Max - Min);
		}
		
		public override int FromStringToValue(string s)
		{
			return (int)DSPFunctions.Clamp(int.Parse(s, CultureInfo.InvariantCulture), Min, Max);
		}

		public override string FromValueToString(int value)
		{
			return value.ToString();
		}
	}
}
