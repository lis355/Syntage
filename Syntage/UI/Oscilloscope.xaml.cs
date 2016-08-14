using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Syntage.Logic;

namespace Syntage.UI
{
    public partial class Oscilloscope : UserControl
    {
        private Oscillograph _oscillograph;

        public Oscilloscope()
        {
            InitializeComponent();
        }

        private void OscilloscopeOnLoaded(object sender, RoutedEventArgs e)
        {
            ClipGeometry.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
        }

        public void SetOscillogpaph(Oscillograph oscillograph)
        {
	        _oscillograph = oscillograph;

            _oscillograph.WindowSize = (int)ActualWidth;
        }
		
        public void AddSample(double l, double r)
        {
            LineL.Points.Add(new Point(LineL.Points.Count, SampleToY(l)));
            LineR.Points.Add(new Point(LineR.Points.Count, SampleToY(r)));
        }

        private double SampleToY(double f)
        {
            return 2 + (ActualHeight - 4) * 0.5 * (1 - f);
        }
		
        public void Update()
        {
            if (!_oscillograph.IsDirty)
                return;

            _oscillograph.BlockBuffers();

            UpdatePolyline(LineL, _oscillograph.LeftBuffer);
            UpdatePolyline(LineR, _oscillograph.RightBuffer);

            _oscillograph.ReleaseBuffers();

			_oscillograph.IsDirty = false;
        }

        private void UpdatePolyline(Polyline polyline, Oscillograph.Buffer buffer)
        {
            var points = polyline.Points;
            points.Clear();

            foreach (var sample in buffer.Values)
                points.Add(new Point(points.Count, SampleToY(sample)));
        }
    }
}
