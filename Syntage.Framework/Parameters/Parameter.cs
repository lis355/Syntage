using System;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
	// Любой параметр для плагина. 
	// Минимум 0, максимум 1, NormalizedStep - шаг для интервала [0-1]
	// Например, если шаг 0.5, это значит что у параметра есть 3 значения: 0, 0.5 и 1.

	public abstract class Parameter
    {
        private IParameterModifier _parameterModifier;

        public string Name { get; }
        public string Description { get; }
        public string Label { get; }

	    public double RealValue { get; private set; }

	    public double RealStep { get; protected set; } = 0.01;

        public double? RealDefaultValue { get; protected set; }

        public abstract string GetDisplayValue();

	    public bool CanBeAutomated { get; }

	    // Установка значения из редактора
	    public virtual bool CanSetValueFromUI()
	    {
	        return true;
	    }

	    public void BeginEditValueFromUI()
	    {
            Manger.HostAutomation.BeginEdit(Index);
        }

	    public void SetValueFromUI(double value)
	    {
	        if (CanSetValueFromUI())
	        {
	            SetRealValue(value, EChangeType.UI);

                Manger.HostAutomation.SetParameterAutomated(Index, (float)RealValue);
            }
	    }

	    public void FinishEditValueFromUI()
	    {
	        Manger.HostAutomation.EndEdit(Index);
	    }

	    public void SetValueFromHost(float value)
	    {
            SetRealValue(value, EChangeType.Host);
        }

        public void SetValueFromPlugin(double value)
        {
            SetRealValue(value, EChangeType.Plugin);
        }

        public abstract double Parse(string s);
        
	    public enum EChangeType
	    {
	        Plugin,
	        Host,
	        UI
	    }

	    public event Action<EChangeType> OnValueChange;

        public ParametersManager Manger { get; set; }
        public int Index { get; set; }
        
        public IParameterModifier ParameterModifier
        {
            get { return _parameterModifier; }
            set
            {
                if (_parameterModifier == value)
                    return;

                if (_parameterModifier != null
                    && !CanBeAutomated)
                    throw new ArgumentException();

                _parameterModifier = value;
            }
        }

        public double ProcessedRealValue(int sampleNumber)
        {
            if (_parameterModifier == null)
                return RealValue;

            var modifiedRealValue = _parameterModifier.ModifyRealValue(RealValue, sampleNumber);
            return modifiedRealValue;
        }

        protected Parameter(string systemName, string description, string label, bool canBeAutomated = true)
	    {
	        Name = systemName;
	        Description = description;
	        Label = label;

	        CanBeAutomated = canBeAutomated;
        }

        private void SetRealValue(double value, EChangeType changeType)
        {
            if (!DSPFunctions.IsZero(Math.Abs(RealValue - value)))
            {
                RealValue = value;
                OnValueChange?.Invoke(changeType);
            }
        }
    }
}
