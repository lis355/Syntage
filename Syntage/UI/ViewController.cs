using System;
using System.Windows;
using Syntage.Framework.Parameters;
using Syntage.Plugin;

namespace Syntage.UI
{
	public class ViewController
    {
		public View View { get; }
		public PluginController PluginController { get; }
		
        public ViewController(View view, Plugin.PluginController pluginController)
        {
            View = view;
	        PluginController = pluginController;

            UIDispatcher.Instance.SetPlugin(PluginController);

            View.Oscilloscope.SetOscillogpaph(PluginController.AudioProcessor.Oscillograph);

            try
            {
                foreach (var parameter in PluginController.ParametersManager.Parameters)
                    BindParameter(parameter);
			}
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

                throw;
            }
        }

        public void BindParameter(Parameter parameter)
        {
            var name = parameter.Name;
            var element = View.FindName(name);
	        var parameterController = element as IParameterController;
            if (parameterController != null)
            {
                parameterController.SetParameter(parameter);
                parameterController.UpdateController();

                parameter.OnValueChange += changeType => UIThread.Instance.InvokeUIAction(() => parameterController.UpdateController());
            }
        }
		
        public void Update()
        {
            View.Oscilloscope.Update();

            UIThread.Instance.Update();
        }
    }
}
