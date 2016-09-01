using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;

namespace Syntage.UI
{
	public partial class Switch : UserControl, IUIParameterController
	{
		private bool _isMouseOn;

		private Parameter _parameter;
		private string _shortLabel;
		private double _value;

		public Color SwitchColor
		{
			get { return (Color)GetValue(SwitchColorProperty); }
			set { SetValue(SwitchColorProperty, value); }
		}

		public static readonly DependencyProperty SwitchColorProperty =
			DependencyProperty.Register("SwitchColor", typeof(Color), typeof(Switch),
				new FrameworkPropertyMetadata(Colors.DeepPink, FrameworkPropertyMetadataOptions.AffectsRender));

		public Switch()
		{
			InitializeComponent();

            UpdateController();
		}

		public void SetParameter(Parameter parameter)
		{
			_parameter = parameter;

			NameLabel.Content = _parameter.Name;
			_shortLabel = _parameter.Label;
        }

		private double GetRealValue()
		{
			return _parameter?.RealValue ?? _value;
		}

		private bool IsRealValueTrue()
		{
			return !DSPFunctions.IsZero(GetRealValue());
		}

		public void UpdateController()
		{
		    ValueLabel.Content = (_parameter != null) ? _parameter.GetDisplayValue() : _value.ToString("F2");

		    VisualStateManager.GoToElementState(this, (!IsRealValueTrue()) ?  "NormalOff" : "NormalOn", true);
		}

	    private void Knob_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
		    if (e.ChangedButton == MouseButton.Left)
		    {
		        ((UIElement)sender).CaptureMouse();
		    }
		}

		private void Knob_OnMouseUp(object sender, MouseButtonEventArgs e)
		{
		    if (e.ChangedButton == MouseButton.Left)
		    {
		        ((UIElement)sender).ReleaseMouseCapture();

		        if (_isMouseOn)
		            VisualStateManager.GoToElementState(this, (!IsRealValueTrue()) ? "MouseOverOff" : "MouseOverOn", true);
		        else
		            VisualStateManager.GoToElementState(this, (!IsRealValueTrue()) ? "NormalOff" : "NormalOn", true);

		        if (_isMouseOn)
		        {
		            _value = 1 - GetRealValue();

		            if (_parameter != null)
		            {
		                _parameter.BeginEditValueFromUI();
			            SetValue();
						_parameter.FinishEditValueFromUI();

		                PluginUI.Instance.Log(string.Format("{0} = {1} {2}", _parameter.Name, _parameter.GetDisplayValue(), _parameter.Label));
		            }
		            else
		            {
		                UpdateController();
		            }
		        }
		    }
		}

		private void Knob_OnMouseMove(object sender, MouseEventArgs e)
		{
		}

		private void Knob_OnMouseEnter(object sender, MouseEventArgs e)
		{
			_isMouseOn = true;

			VisualStateManager.GoToElementState(this, (!IsRealValueTrue()) ? "MouseOverOff" : "MouseOverOn", true);
		}

		private void Knob_OnMouseLeave(object sender, MouseEventArgs e)
		{
			_isMouseOn = false;

			VisualStateManager.GoToElementState(this, (!IsRealValueTrue()) ? "NormalOff" : "NormalOn", true);
		}

		private void SetValue()
		{
			_parameter.SetValueFromUI(_value);

			PluginUI.Instance.Log(string.Format("{0} = {1} {2}", _parameter.Name, _parameter.GetDisplayValue(), _parameter.Label));
		}
	}
}
