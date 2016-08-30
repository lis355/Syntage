using System.Globalization;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
	public class RealParameter : Parameter<double>
	{
		public RealParameter(string name, string label, string shortLabel, double min, double max,
			double step, bool canBeAutomated = true) :
			base(name, label, shortLabel, min, max, (float)step, canBeAutomated)
		{
		}

		protected static string PrintDouble(double value)
		{
			return value.ToString("F2", CultureInfo.InvariantCulture);
		}

		protected static double ParseDouble(string value)
		{
			return double.Parse(value, CultureInfo.InvariantCulture);
		}

		public override double FromReal(double value)
		{
			return DSPFunctions.Lerp(Min, Max, value);
		}

		public override double ToReal(double value)
		{
			return (value - Min) / (Max - Min);
		}

		public override double FromStringToValue(string s)
		{
			return DSPFunctions.Clamp(ParseDouble(s), Min, Max);
		}

		public override string FromValueToString(double value)
		{
			return PrintDouble(value);
		}
	}
}
