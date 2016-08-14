using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Plugin;
using Syntage.Framework.Parameters;
using Syntage.Framework.Tools;

namespace Syntage.Framework
{
    public abstract class SyntagePlugin : VstPluginWithInterfaceManagerBase
    {
        public Action OnOpen;
        public Action OnClose;

        public ParametersManager ParametersManager { get; }

        protected SyntagePlugin(string name, VstProductInfo productInfo, VstPluginCategory category, VstPluginCapabilities capabilities,
            int initialDelay, int pluginID) : base(name, productInfo, category, capabilities, initialDelay, pluginID)
        {
            ParametersManager = new ParametersManager();
        }
        
        protected override IVstPluginParameters CreateParameters(IVstPluginParameters instance)
        {
            return null;
        }

        protected override IVstPluginPrograms CreatePrograms(IVstPluginPrograms instance)
        {
            return null;
        }

        public override void Open(IVstHost host)
        {
            base.Open(host);
            
            ParametersManager.SetHost(host.GetInstance<IVstHostCommands20>());

            OnOpen?.Invoke();

            Log.Print("Open in host : " + host.ProductInfo.Product + " " + DateTime.Now.Second);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                OnClose?.Invoke();

                Log.Print("Close");
            }
        }
    }
}
