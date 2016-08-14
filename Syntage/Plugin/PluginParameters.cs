using System;
using System.Collections.Generic;
using System.Linq;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Plugin;
using Syntage.Framework.Parameters;

namespace Syntage.Plugin
{/*
	public class PluginParameters : VstPluginProgramsBase
	{
		private readonly PluginController _pluginController;

		public readonly Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
        
		public PluginParameters(PluginController pluginController)
		{
			_pluginController = pluginController;

			_pluginController.OnOpen += PluginOnOpen;
		}

		private void PluginOnOpen()
		{
			var hostAutomation = _pluginController.Host.GetInstance<IVstHostAutomation>();
			foreach (var parameter in Parameters)
				parameter.Value.Manager.HostAutomation = hostAutomation;
		}

	    protected override VstProgramCollection CreateProgramCollection()
	    {
	        foreach (var parameter in _pluginController.AudioProcessor.CreateParameters())
	            Parameters.Add(parameter.Name, parameter);

	        var lines = Properties.Resources.InitPreset.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
	        var defaultProgram = LoadProgramsFromSerializedParameters("-", lines);
	        return new VstProgramCollection {defaultProgram};
	    }

	    
    }*/
}
