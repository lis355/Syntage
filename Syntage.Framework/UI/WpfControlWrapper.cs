using System;
using System.Data;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Syntage.Framework.UI
{
    public class WpfControlWrapper<T> where T : UserControl, new()
    {
        private const int KWsVisible = 0x10000000;
        private const int KWsChild = 0x40000000;

        private HwndSource _hwndSource;

        public Rectangle Bounds { get; private set; }
        public T Control { get; private set; }

        public WpfControlWrapper()
        {
	        ValidateInstance();
        }
        
        public void Open(IntPtr hWnd)
        {
	        ValidateInstance();

			var hwndParams = new HwndSourceParameters("WPFView");
            hwndParams.ParentWindow = hWnd;
            hwndParams.Width = Bounds.Width;
            hwndParams.Height = Bounds.Height;
            hwndParams.WindowStyle = KWsVisible | KWsChild;

            _hwndSource = new HwndSource(hwndParams);
            _hwndSource.RootVisual = Control;
        }

        public void Close()
        {
            if (_hwndSource != null)
            {
                _hwndSource.Dispose();
                _hwndSource = null;
            }

            Control = null;
        }

	    private void ValidateInstance()
	    {
			if (Control != null)
				return;

			Control = new T();
			Bounds = new Rectangle(0, 0, (int)Control.Width, (int)Control.Height);
			if (Bounds.Width * Bounds.Height == 0)
				throw new InvalidConstraintException();
		}
    }
}
