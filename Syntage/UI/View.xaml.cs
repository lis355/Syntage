using System;
using System.Windows.Controls;

namespace Syntage.UI
{
    public partial class View 
    {
        public View()
        {
            InitializeComponent();
		}

        public event Action<TextBlock> LFOParameterChanged;

        private void LFOParamsListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LFOParameterChanged?.Invoke(e.AddedItems[0] as TextBlock);
        }
    }
}
