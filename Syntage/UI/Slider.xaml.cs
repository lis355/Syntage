using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;

namespace Syntage.UI
{
	public partial class Slider : UserControl, IUIParameterController
	{
		private const double KDeltaFactor = 0.015;

		private bool _isMouseDown;
		private bool _isMouseOn;
		private Point _mouseStartPoint;
		private double _knobStartValue;
		private double _invertedStep = 100;

		private Parameter _parameter;
		private string _shortLabel;
		private double _value;

		public Color SliderBackColor
		{
			get { return (Color)GetValue(SliderBackColorProperty); }
			set { SetValue(SliderBackColorProperty, value); }
		}

		public static readonly DependencyProperty SliderBackColorProperty =
			DependencyProperty.Register("SliderBackColor", typeof(Color), typeof(Slider),
				new FrameworkPropertyMetadata(Colors.DeepPink, FrameworkPropertyMetadataOptions.AffectsRender));

		public Slider()
		{
			InitializeComponent();
		}
		
		private void Slider_OnLoaded(object sender, RoutedEventArgs e)
		{
            UpdateController();
		}

		public void SetParameter(Parameter parameter)
		{
			_parameter = parameter;
			_invertedStep = 1 / _parameter.RealStep;

			NameLabel.Content = _parameter.Name;
			_shortLabel = _parameter.Label;
        }
        
	    public void UpdateController()
		{
			var height = SliderGrid.ActualHeight;
			var heightS = height - 10;
			var margin = EllipseButton.Margin;
			
			EllipseButton.Margin = new Thickness(margin.Left, heightS - GetRealValue() * 2 * heightS, margin.Right, margin.Bottom);

			if (_parameter != null)
				ValueLabel.Content = _parameter.GetDisplayValue() + ((string.IsNullOrEmpty(_shortLabel)) ? "" : " " + _shortLabel);
			else
				ValueLabel.Content = _value.ToString("F2");

			margin = RectangleBackgroundSlider.Margin;
			RectangleBackgroundSlider.Margin = new Thickness(margin.Left, (1 - GetRealValue()) * height, margin.Right, margin.Bottom);
		}

		private double GetRealValue()
		{
			return _parameter?.RealValue ?? _value;
		}

		private void Knob_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
		    if (e.ChangedButton == MouseButton.Left)
		    {
		        ((UIElement)sender).CaptureMouse();

		        _isMouseDown = true;
		        _mouseStartPoint = e.GetPosition(this);
		        _knobStartValue = GetRealValue();

		        VisualStateManager.GoToElementState(this, "Pressed", true);

		        _parameter?.BeginEditValueFromUI();
		    }
		}

		private void Knob_OnMouseUp(object sender, MouseButtonEventArgs e)
		{
		    if (e.ChangedButton == MouseButton.Left)
		    {
		        ((UIElement)sender).ReleaseMouseCapture();

		        _isMouseDown = false;

		        VisualStateManager.GoToElementState(this, (_isMouseOn) ? "MouseOver" : "Normal", false);

		        _parameter?.FinishEditValueFromUI();
		    }
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
					SetValue();
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
	            SetValue();
				_parameter.FinishEditValueFromUI();
            }
        }

		private void SetValue()
		{
			_parameter.SetValueFromUI(_value);

			PluginUI.Instance.Log(string.Format("{0} = {1} {2}", _parameter.Name, _parameter.GetDisplayValue(), _parameter.Label));
		}
	}
}
