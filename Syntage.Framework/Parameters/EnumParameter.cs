using System;
using System.Globalization;
using System.Linq;

namespace Syntage.Framework.Parameters
{
	public class EnumParameter<T> : Parameter<T> where T : struct, IConvertible
	{
		public EnumParameter(string name, string label, string shortLabel = "", bool canBeAutomated = true) :
			base(name, label, shortLabel,
				((T[])Enum.GetValues(typeof(T))).First().ToInt32(CultureInfo.InvariantCulture),
				((T[])Enum.GetValues(typeof(T))).Last().ToInt32(CultureInfo.InvariantCulture),
				1, canBeAutomated)
		{
			var values = ((T[])Enum.GetValues(typeof(T)))
				.Select(x => x.ToInt32(CultureInfo.InvariantCulture)).ToArray();
			for (int i = 0; i < values.Length - 1; ++i)
				if (Math.Abs(values[i] - values[i + 1]) != 1)
					throw new ArgumentException();
		}
		
		public override T FromReal(double value)
		{
			return (T)Enum.ToObject(typeof(T), (int)(value * (Max - Min)));
		}

		public override double ToReal(T value)
		{
			return value.ToInt32(CultureInfo.InvariantCulture) / (Max - Min);
		}

		public override T FromStringToValue(string s)
		{
			return (T)Enum.Parse(typeof(T), s);
		}

		public override string FromValueToString(T value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
