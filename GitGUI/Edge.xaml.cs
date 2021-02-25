using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitGUI
{
    /// <summary>
    /// Interakční logika pro Edge.xaml
    /// </summary>
    public partial class Edge : UserControl
    {
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
          "Stroke", typeof(Brush), typeof(Edge), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnStrokeChanged));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
          "StrokeThickness", typeof(double), typeof(Edge), new PropertyMetadata(1.0, OnStrokeThicknessChanged));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
          "Source", typeof(Point), typeof(Edge), new PropertyMetadata(new Point(20, 0), OnPropertiesChanged));

        public static readonly DependencyProperty SinkProperty = DependencyProperty.Register(
          "Sink", typeof(Point), typeof(Edge), new PropertyMetadata(new Point(100, 0), OnPropertiesChanged));

        public static readonly DependencyProperty MaxArcRadiusProperty = DependencyProperty.Register(
          "MaxArcRadius", typeof(double), typeof(Edge), new PropertyMetadata(30.0, OnPropertiesChanged));

        public static readonly DependencyProperty MaxUpperArcOffsetProperty = DependencyProperty.Register(
          "MaxUpperArcOffset", typeof(double), typeof(Edge), new PropertyMetadata(20.0, OnPropertiesChanged));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double MaxUpperArcOffset
        {
            get { return (double)GetValue(MaxUpperArcOffsetProperty); }
            set { SetValue(MaxUpperArcOffsetProperty, value); }
        }

        public double MaxArcRadius
        {
            get { return (double)GetValue(MaxArcRadiusProperty); }
            set { SetValue(MaxArcRadiusProperty, value); }
        }

        public Point Source
        {
            get { return (Point)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public Point Sink
        {
            get { return (Point)GetValue(SinkProperty); }
            set { SetValue(SinkProperty, value); }
        }

        Point Left { get { return (Source.X < Sink.X) ? Source : Sink; } }

        Point Right { get { return (Source.X < Sink.X) ? Sink : Source; } }

        double EdgeHeight { get { return Math.Abs(Source.Y - Sink.Y); } }

        double EdgeWidth { get { return Math.Abs(Source.X - Sink.X); } }

        double ArcRadius { get { return Math.Min(Math.Min(MaxArcRadius, EdgeHeight / 2), EdgeWidth / 2); } }

        bool IsAscending { get { return Right.Y > Left.Y; } }

        double UpperLineLength { get { return Math.Min(MaxUpperArcOffset, (EdgeWidth - 2 * ArcRadius) / 2); } }

        Point LeftLineSink
        {
            get
            {
                double x = Left.X;
                if (IsAscending)
                    x += EdgeWidth - UpperLineLength - 2 * ArcRadius;
                else
                    x += UpperLineLength;
                return new Point(x, Left.Y);
            }
        }

        Size ArcSize { get { double s = ArcRadius; return new Size(s, s); } }

        Point LeftArcSink { get { Point lls = LeftLineSink; return new Point(lls.X + ArcRadius, Left.Y + (IsAscending ? ArcRadius : -ArcRadius)); } }

        Point MidLineSink { get { Point las = LeftArcSink; return new Point(las.X, Right.Y + (IsAscending ? -ArcRadius : ArcRadius)); } }

        Point RightArcSink { get { return new Point(LeftLineSink.X + 2 * ArcRadius, Right.Y); } }

        public Edge()
        {
            InitializeComponent();
        }

        void OnPropertiesChanged()
        {
            SetSegmentSinkPoints();
            SetArcs();
        }

        void SetArcs()
        {
            rightArc.Size = leftArc.Size = ArcSize;
            if (IsAscending)
            {
                leftArc.SweepDirection = SweepDirection.Clockwise;
                rightArc.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {
                leftArc.SweepDirection = SweepDirection.Counterclockwise;
                rightArc.SweepDirection = SweepDirection.Clockwise;
            }
        }

        void SetSegmentSinkPoints()
        {
            left.StartPoint = Left;
            leftLine.Point = LeftLineSink;
            leftArc.Point = LeftArcSink;
            midLine.Point = MidLineSink;
            rightArc.Point = RightArcSink;
            rightLine.Point = Right;
        }

        static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Edge)d).OnPropertiesChanged();
        }

        static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Edge)d).path.StrokeThickness = (double)(e.NewValue);
        }

        static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Edge)d).path.Stroke = (Brush)(e.NewValue);
        }
    }
}
