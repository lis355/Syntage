using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Syntage.Plugin;

namespace Syntage.UI
{
    public class PluginUI : IVstPluginEditor
    {
        private readonly Plugin.PluginController _pluginController;
        private readonly WpfControlWrapper<View> _uiWrapper = new WpfControlWrapper<View>();

        public PluginUI(Plugin.PluginController pluginController)
        {
            _pluginController = pluginController;
        }

        public View Control
        {
            get { return _uiWrapper.Control; }
        }

        public System.Drawing.Rectangle Bounds
        {
            get { return _uiWrapper.Bounds; }
        }

        public void Close()
        {
            _uiWrapper.Close();
        }

        public bool KeyDown(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            // no-op
            return false;
        }

        public bool KeyUp(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            // no-op
            return false;
        }

        public VstKnobMode KnobMode { get; set; }

        public void Open(IntPtr hWnd)
        {
            _uiWrapper.Open(hWnd);
            Control.Controller = new ViewController(_uiWrapper.Control, _pluginController);
        }

        public void ProcessIdle()
        {
            Control.Controller.Update();
        }
    }
}
