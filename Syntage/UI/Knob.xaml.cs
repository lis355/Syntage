using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;
using Syntage.Framework.UI;

namespace Syntage.UI
{
	public partial class Knob : UserControl, IUIParameterController
	{
		private const double KAngleLimit = 45;
		private const double KAngleMax = 90 - KAngleLimit;
		private const double KAngleMin = -180 - KAngleMax;
		private const double KDeltaFactor = 0.01;

		private bool _isMouseDown;
		private bool _isMouseOn;
		private Point _mouseStartPoint;
		private double _knobStartValue;
		private double _invertedStep = 100;

		private Parameter _parameter;
		private string _shortLabel;
		private double _value;
		
		public Color KnobBackColor
		{
			get { return (Color)GetValue(KnobBackColorProperty); }
			set { SetValue(KnobBackColorProperty, value); }
		}

		public static readonly DependencyProperty KnobBackColorProperty =
			DependencyProperty.Register("KnobBackColor", typeof(Color), typeof(Knob),
				new FrameworkPropertyMetadata(Colors.DeepPink, FrameworkPropertyMetadataOptions.AffectsRender));

		public Knob()
		{
			InitializeComponent();

		    UpdateController();
		}
		
		public void SetParameter(Parameter parameter)
		{
			_parameter = parameter;
			_invertedStep = 1 / _parameter.RealStep;

			NameLabel.Content = _parameter.Name;
			_shortLabel = _parameter.Label;

            UpdateController();

            parameter.OnValueChange += changeType => UIThread.Instance.InvokeUIAction(UpdateController);
        }

	    private double GetRealValue()
		{
			return _parameter?.RealValue ?? _value;
		}

		public void UpdateController()
		{
			RotateTransformKnob.Angle = KnobRealValueToAngle(GetRealValue());

			if (_parameter != null)
				ValueLabel.Content = _parameter.GetDisplayValue() + ((string.IsNullOrEmpty(_shortLabel)) ? "" : " " + _shortLabel);
			else
				ValueLabel.Content = _value.ToString("F2");

			KnobArc.StartAngle = KnobRealValueToAngle(GetRealValue());
			KnobArc.EndAngle = KnobRealValueToAngle(0);
        }

		public static double KnobRealValueToAngle(double value)
		{
			return KAngleMin + (KAngleMax - KAngleMin) * value;
		}

		private void Knob_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).CaptureMouse();

			_isMouseDown = true;
			_mouseStartPoint = e.GetPosition(this);
			_knobStartValue = GetRealValue();

			VisualStateManager.GoToElementState(this, "Pressed", true);

			_parameter?.BeginEditValueFromUI();
		}

		private void Knob_OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).ReleaseMouseCapture();

			_isMouseDown = false;

			VisualStateManager.GoToElementState(this, (_isMouseOn) ? "MouseOver" : "Normal", false);

			_parameter?.FinishEditValueFromUI();
		}

		private void Knob_OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_isMouseDown)
			{
				var position = e.GetPosition(this);
				var delta = position.Y - _mouseStartPoint.Y;

				_value = _knobStartValue - delta * KDeltaFactor;
				_value = DSPFunctions.Clamp01(_value);
				_value = (int)(_value * _invertedStep) / _invertedStep;

				if (_parameter != null)
				{
					_parameter.SetValueFromUI(_value);

                    PluginUI.Instance.Log(string.Format("{0} = {1} {2}", _parameter.Name, _parameter.GetDisplayValue(), _parameter.Label));
				}
				else
				{
                    UpdateController();
				}
			}
		}

		private void Knob_OnMouseEnter(object sender, MouseEventArgs e)
		{
			_isMouseOn = true;

			VisualStateManager.GoToElementState(this, "MouseOver", true);
		}

		private void Knob_OnMouseLeave(object sender, MouseEventArgs e)
		{
			_isMouseOn = false;

			VisualStateManager.GoToElementState(this, "Normal", true);
		}

	    private void Element_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
	    {
	        if (IsMouseOver
                && _parameter?.RealDefaultValue != null)
	        {
	            _isMouseDown = false;

                _value = _parameter.RealDefaultValue.Value;

	            _parameter.BeginEditValueFromUI();
                _parameter.SetValueFromUI(_value);
                _parameter.FinishEditValueFromUI();
            }
	    }
	}
}
