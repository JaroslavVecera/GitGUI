using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interakční logika pro CommitNode.xaml
    /// </summary>
    public partial class CommitNodeView : UserControl
    {
        protected double _margin = 8;

        double LeftContactDist { get { return Height / 2; } }
        double TextStartDist { get { return LeftContactDist + _margin + (EnabledPhoto ? Height / 2 : 0); } }
        double TextEndDist { get { return TextStartDist + TextWidth; } }

        public void OnLocationChanged(Point p)
        {
            Canvas.SetTop(this, p.Y);
            Canvas.SetLeft(this, p.X);
        }

        public void OnFocusedChanged()
        {
            plusButton.Visibility = Focused ? Visibility.Visible : Visibility.Collapsed;
        }

        public void OnMarkedChanged()
        {

        }

        public void OnCheckoutedChanged()
        {

        }

        double RightContactDist { get { return TextEndDist + _margin; } }

        public double TextWidth
        {
            get { return (double)GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(CommitNodeView), new PropertyMetadata((double)0));

        public RelayCommand PlusCommand
        {
            get { return (RelayCommand)GetValue(PlusCommandProperty); }
            set { SetValue(PlusCommandProperty, value); }
        }

        public static readonly DependencyProperty PlusCommandProperty =
            DependencyProperty.Register("PlusCommand", typeof(RelayCommand), typeof(CommitNodeView));

        public bool Focused
        {
            get { return (bool)GetValue(FocusedProperty); }
        }

        public static readonly DependencyProperty FocusedProperty =
            DependencyProperty.Register("Focused", typeof(bool), typeof(CommitNodeView));

        public bool EnabledPhoto
        {
            get { return (bool)GetValue(EnabledPhotoProperty); }
            set { SetValue(EnabledPhotoProperty, value); ellipse.IsEnabled = value; UpdateProperties(); }
        }

        public static readonly DependencyProperty EnabledPhotoProperty =
            DependencyProperty.Register("EnabledPhoto", typeof(bool), typeof(CommitNodeView), new PropertyMetadata(true));

        public MouseButtonEventArgs MouseButtonArgs
        {
            get { return (MouseButtonEventArgs)GetValue(MouseButtonArgsProperty); }
            set { SetValue(MouseButtonArgsProperty, value); }
        }

        public static readonly DependencyProperty MouseButtonArgsProperty =
            DependencyProperty.Register("MouseButtonArgs", typeof(MouseButtonEventArgs), typeof(CommitNodeView));

        public MouseEventArgs MouseArgs
        {
            get { return (MouseEventArgs)GetValue(MouseArgsProperty); }
            set { SetValue(MouseArgsProperty, value); }
        }

        public static readonly DependencyProperty MouseArgsProperty =
            DependencyProperty.Register("MouseArgs", typeof(MouseEventArgs), typeof(CommitNodeView));


        public double MaxW { get { return 150; } }
        
        protected void MeasureTextWidth(TextBlock b)
        {
            b.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (b.DesiredSize.Width <= MaxW)
                TextWidth = b.DesiredSize.Width;
            else
                TextWidth = Math.Min(MaxW, b.DesiredSize.Width / 2);
        }

        public CommitNodeView()
        {
            InitializeComponent();
            OnFocusedChanged();
            OnMarkedChanged();
            OnCheckoutedChanged();
        }

        void UpdateProperties()
        {
            MeasureTextWidth(message);
            UpdateShape();
            UpdateMargins();
            UpdateMaxWidth();
        }

        void UpdateShape()
        {
            geometry.Figures = new PathFigureCollection() { new PathFigure()
            {
                StartPoint = new Point(LeftContactDist, 0),
                Segments = new PathSegmentCollection { Top(), RightArc(), Bottom(), LeftArc() },
                IsClosed = true
            } };
        }

        void UpdateMargins()
        {
            message.Margin = new Thickness(TextStartDist, 0, 0, 0);

        }

        void UpdateMaxWidth()
        {
            MaxWidth = 2 * _margin + TextWidth + Height * 3 / 2;
        }

        LineSegment Top()
        {
            return new LineSegment(new Point(RightContactDist, 0), true);
        }

        ArcSegment RightArc()
        {
            return new ArcSegment()
            {
                Point = new Point(RightContactDist, Height),
                Size = new Size(Height / 2, Height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }

        LineSegment Bottom()
        {
            return new LineSegment(new Point(LeftContactDist, Height), true);
        }

        ArcSegment LeftArc()
        {
            return new ArcSegment()
            {
                Point = new Point(LeftContactDist, 0),
                Size = new Size(Height / 2, Height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }

        private void MessageUpdated(object sender, DataTransferEventArgs e)
        {
            UpdateProperties();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseButtonArgs = e;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseButtonArgs = e;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Binding b = new Binding("MouseButtonArgs");
            b.Mode = BindingMode.OneWayToSource;
            b.Source = DataContext;
            SetBinding(MouseButtonArgsProperty, b);
            Binding b2 = new Binding("MouseArgs");
            b2.Mode = BindingMode.OneWayToSource;
            b2.Source = DataContext;
            SetBinding(MouseArgsProperty, b2);
            Binding b3 = new Binding("Focused");
            b3.Source = DataContext;
            SetBinding(FocusedProperty, b3);
            Binding b4 = new Binding("PlusCommand");
            b4.Source = DataContext;
            SetBinding(PlusCommandProperty, b4);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            MouseArgs = e;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            MouseArgs = e;
        }

        private void ButtonUp(object sender, MouseButtonEventArgs e)
        {
            PlusCommand.Execute(null);
        }
    }
}
