using System;
using System.Windows.Forms;
using Jacobi.Vst.Core.Host;

namespace SimplyHost
{
    public partial class ParameterEditor : UserControl
    {
        private readonly int _i;
        private readonly IVstPluginCommandStub _pluginCommandStub;

        public ParameterEditor(int i, IVstPluginCommandStub pluginCommandStub)
        {
            InitializeComponent();

            _i = i;
            _pluginCommandStub = pluginCommandStub;

            NameLabel.Text = _pluginCommandStub.GetParameterName(i);
            UpdateValueDisplay();
            ValueSlider.Value = (int) (ValueSlider.Minimum + (ValueSlider.Maximum - ValueSlider.Minimum) * _pluginCommandStub.GetParameter(_i));

            ValueSlider.ValueChanged += ValueSliderValueChanged;
        }

        private void UpdateValueDisplay()
        {
            ValueLabel.Text = _pluginCommandStub.GetParameterDisplay(_i);
        }

        private void ValueSliderValueChanged(object sender, EventArgs e)
        {
            _pluginCommandStub.SetParameter(_i, ValueSlider.Value / (float)(ValueSlider.Maximum - ValueSlider.Minimum));
            UpdateValueDisplay();
        }
    }
}
