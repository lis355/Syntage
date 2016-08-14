namespace Syntage.Framework.Parameters
{
	public class BooleanParameter : Parameter<bool>
	{
		public BooleanParameter(string name, string label, string shortLabel,
			bool canBeAutomated = true) :
				base(name, label, shortLabel, 0, 1, 1, canBeAutomated)
		{
		}

		public override bool FromReal(double value)
		{
			return value > 0.5;
		}

		public override double ToReal(bool value)
		{
			return (value) ? 1 : 0;
		}

		public override bool FromStringToValue(string s)
		{
			return bool.Parse(s);
		}

		public override string FromValueToString(bool value)
		{
			return value.ToString();
		}
	}
}
