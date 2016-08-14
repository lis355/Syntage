using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Syntage.Framework.Parameters;

namespace Syntage.Framework.UI
{
    public abstract class PluginWpfUI<T> : IVstPluginEditor where T : UserControl, new()
    {
        private readonly WpfControlWrapper<T> _uiWrapper = new WpfControlWrapper<T>();

        public T Control
        {
            get { return _uiWrapper.Control; }
        }

        public Rectangle Bounds
        {
            get { return _uiWrapper.Bounds; }
        }

        public bool KeyDown(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            return false;
        }

        public bool KeyUp(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            return false;
        }

        public VstKnobMode KnobMode { get; set; }

        public virtual void Open(IntPtr hWnd)
        {
            _uiWrapper.Open(hWnd);
        }

        public virtual void Close()
        {
            _uiWrapper.Close();
        }

        public virtual void ProcessIdle()
        {
        }

        public void BindParameters(IEnumerable<Parameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                var name = parameter.Name;
                var element = Control.FindName(name);
                var parameterController = element as IUIParameterController;
                parameterController?.SetParameter(parameter);
            }
        }
    }
}
