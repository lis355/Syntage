using System;
using Syntage.Framework.Tools;

namespace Syntage.Framework.Parameters
{
	// Любой параметр для плагина. 
	// Минимум 0, максимум 1, NormalizedStep - шаг для интервала [0-1]
	// Например, если шаг 0.5, это значит что у параметра есть 3 значения: 0, 0.5 и 1.

	public abstract class Parameter
	{
	    private double _realValue;

	    public string Name { get; }
        public string Description { get; }
        public string Label { get; }

	    public double RealValue
	    {
	        get { return _realValue; }
	        //set { SetRealValue(value, EChangeType.Plugin); }
	    }

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

        protected Parameter(string systemName, string description, string label, bool canBeAutomated = true)
	    {
	        Name = systemName;
	        Description = description;
	        Label = label;

	        CanBeAutomated = canBeAutomated;
        }

        private void SetRealValue(double value, EChangeType changeType)
        {
            if (!DSPFunctions.IsZero(Math.Abs(_realValue - value)))
            {
                _realValue = value;
                OnValueChange?.Invoke(changeType);
            }
        }
    }
}
