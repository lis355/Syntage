using System.Windows.Controls;
using System.Windows.Media;

namespace Syntage.UI
{
    public partial class Piano : UserControl
	{
		int _number;

		public Piano()
        {
            InitializeComponent();

			KeysPanel.Children.Clear();

	        const int kOctavs = 8;

	        int left = -1;
	        for (int i = 0; i < kOctavs; ++i)
	        {
	            var key = CreateWhiteKey(); Canvas.SetLeft(key, left +  0 + 0 * 13); KeysPanel.Children.Add(key);
				    key = CreateBlackKey(); Canvas.SetLeft(key, left +  8 + 0 * 13); KeysPanel.Children.Add(key);
				    key = CreateWhiteKey(); Canvas.SetLeft(key, left +  0 + 1 * 13); KeysPanel.Children.Add(key);
				    key = CreateBlackKey(); Canvas.SetLeft(key, left +  9 + 1 * 14); KeysPanel.Children.Add(key);
				    key = CreateWhiteKey(); Canvas.SetLeft(key, left +  0 + 2 * 13); KeysPanel.Children.Add(key);
				    key = CreateWhiteKey(); Canvas.SetLeft(key, left +  0 + 3 * 13); KeysPanel.Children.Add(key);
				    key = CreateBlackKey(); Canvas.SetLeft(key, left +  8 + 3 * 13); KeysPanel.Children.Add(key);
				    key = CreateWhiteKey(); Canvas.SetLeft(key, left +  0 + 4 * 13); KeysPanel.Children.Add(key);
				    key = CreateBlackKey(); Canvas.SetLeft(key, left +  9 + 4 * 13); KeysPanel.Children.Add(key);
				    key = CreateWhiteKey(); Canvas.SetLeft(key, left +  0 + 5 * 13); KeysPanel.Children.Add(key);
				    key = CreateBlackKey(); Canvas.SetLeft(key, left + 10 + 5 * 13); KeysPanel.Children.Add(key);
				    key = CreateWhiteKey(); Canvas.SetLeft(key, left +  0 + 6 * 13); KeysPanel.Children.Add(key);

				left += 91;
	        }
        }

	    private Key CreateWhiteKey()
	    {
		    var key = new Key();
		    key.KeyBackColor = Colors.WhiteSmoke;
		    key.Width = 15;
		    key.Height = 80;
			key.KeyNumber = _number++;
			Panel.SetZIndex(key, 0);

			return key;
	    }

		private Key CreateBlackKey()
		{
			var key = new Key();
			key.KeyBackColor = Colors.Black;
			key.Width = 10;
			key.Height = 50;
			key.KeyNumber = _number++;
			Panel.SetZIndex(key, 1);

			return key;
		}
	}
}
