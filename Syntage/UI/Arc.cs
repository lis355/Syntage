using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syntage.UI
{
	public class Arc : Shape
	{
		public double StartAngle
		{
			get { return (double)GetValue(StartAngleProperty); }
			set { SetValue(StartAngleProperty, value); }
		}

		public static readonly DependencyProperty StartAngleProperty =
			DependencyProperty.Register("StartAngle", typeof(double), typeof(Arc),
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public double EndAngle
		{
			get { return (double)GetValue(EndAngleProperty); }
			set { SetValue(EndAngleProperty, value); }
		}

		public static readonly DependencyProperty EndAngleProperty =
			DependencyProperty.Register("EndAngle", typeof(double), typeof(Arc),
				new FrameworkPropertyMetadata(Math.PI / 2.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public bool SmallAngle
		{
			get { return (bool)GetValue(SmallAngleProperty); }
			set { SetValue(SmallAngleProperty, value); }
		}

		public static readonly DependencyProperty SmallAngleProperty =
			DependencyProperty.Register("SmallAngle", typeof(bool), typeof(Arc),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		static Arc()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Arc), new FrameworkPropertyMetadata(typeof(Arc)));
		}

		protected override Geometry DefiningGeometry
		{
			get
			{
				var a0 = StartAngle / 180.0 * Math.PI;
				var a1 = EndAngle / 180.0 * Math.PI;

				a0 = a0 < 0 ? a0 + 2 * Math.PI : a0;
				a1 = a1 < 0 ? a1 + 2 * Math.PI : a1;

				if (a1 < a0)
					a1 += Math.PI * 2;

				var d = SweepDirection.Counterclockwise;
				bool large;

				if (SmallAngle)
				{
					large = false;
					d = (a1 - a0) > Math.PI ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
				}
				else
				{
					large = (Math.Abs(a1 - a0) < Math.PI);
				}

				var center = new Point(Width / 2, Height / 2);
				var radius = Math.Min(Width, Height) / 2 - StrokeThickness / 2 - 1;
				var p0 = center + new Vector(Math.Cos(a0), Math.Sin(a0)) * radius;
				var p1 = center + new Vector(Math.Cos(a1), Math.Sin(a1)) * radius;

				var segments = new List<PathSegment>(1);
				segments.Add(new ArcSegment(p1, new Size(radius, radius), 0.0, large, d, true));

				var figures = new List<PathFigure>(1);
				var pf = new PathFigure(p0, segments, true);
				pf.IsClosed = false;
				figures.Add(pf);

				Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);
				return g;
			}
		}
	}
}
