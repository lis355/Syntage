using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Syntage.UI
{
    public partial class Key : UserControl
    {
        private bool _isMouseOn;

        public Key()
        {
            InitializeComponent();
        }

        public event Action OnPressFromUI;
        public event Action OnReleaseFromUI;

		public int KeyNumber
		{
			get { return (int)GetValue(KeyNumberProperty); }
			set { SetValue(KeyNumberProperty, value); }
		}

		public static readonly DependencyProperty KeyNumberProperty =
			DependencyProperty.Register("KeyNumber", typeof(int), typeof(Key),
				new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender));

		public Color KeyBackColor
		{
			get { return (Color)GetValue(KeyBackColorProperty); }
			set { SetValue(KeyBackColorProperty, value); }
		}

		public static readonly DependencyProperty KeyBackColorProperty =
			DependencyProperty.Register("KeyBackColor", typeof(Color), typeof(Key),
				new FrameworkPropertyMetadata(Colors.WhiteSmoke, FrameworkPropertyMetadataOptions.AffectsRender));


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
            if (e.ChangedButton == MouseButton.Left)
            {
                ((UIElement)sender).CaptureMouse();

                OnPressFromUI?.Invoke();
            }
        }

        private void Key_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                OnReleaseFromUI?.Invoke();

                ((UIElement)sender).ReleaseMouseCapture();
            }
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

        private void Key_OnLoaded(object sender, RoutedEventArgs e)
        {
            PluginUI.Instance.RegisterPianoKey(this);
        }
    }
}
