using System.Windows;

namespace Syntage.UI.App
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

            Width = View.Width + 50;
            Height = View.Height + 50;
        }
	}
}
