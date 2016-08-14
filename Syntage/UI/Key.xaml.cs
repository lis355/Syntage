using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syntage.UI
{
    public partial class Key : UserControl, IKey
    {
	    private bool _isMouseOn;

        public Key()
        {
            InitializeComponent();

            UIDispatcher.Instance.RegisterNextPianoKey(this);
        }

        public event Action OnPressFromUI;
        public event Action OnReleaseFromUI;

        public void Press()
        {
            VisualStateManager.GoToElementState(this, "Pressed", true);
        }

        public void Release()
        {
			VisualStateManager.GoToElementState(this, (_isMouseOn) ? "MouseOver" : "Normal", false);
        }

        private void Key_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
			((UIElement)sender).CaptureMouse();
            
            OnPressFromUI?.Invoke();
        }

        private void Key_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnReleaseFromUI?.Invoke();

			((UIElement)sender).ReleaseMouseCapture();
        }

        private void Key_OnMouseEnter(object sender, MouseEventArgs e)
        {
			_isMouseOn = true;

			VisualStateManager.GoToElementState(this, "MouseOver", true);
        }

        private void Key_OnMouseLeave(object sender, MouseEventArgs e)
        {
			_isMouseOn = false;

			VisualStateManager.GoToElementState(this, "Normal", true);
        }
    }
}
