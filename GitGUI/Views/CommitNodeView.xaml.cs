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
        public void OnLocationChanged(Point p)
        {
            Canvas.SetTop(this, p.Y);
            Canvas.SetLeft(this, p.X);
        }

        public void OnFocusedChanged()
        {
            plusButton.Visibility = (PlusButton && Focused) ? Visibility.Visible : Visibility.Collapsed;
        }

        public void OnMarkedChanged()
        {

        }

        public void OnCheckoutedChanged()
        {

        }

        public double TextWidth
        {
            get { return (double)GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(CommitNodeView), new PropertyMetadata((double)0));

        public double TextStartDist
        {
            get { return (double)GetValue(TextStartDistProperty); }
            set { SetValue(TextStartDistProperty, value); }
        }

        public static readonly DependencyProperty TextStartDistProperty =
            DependencyProperty.Register("TextStartDist", typeof(double), typeof(CommitNodeView));

        public double LeftContactDist
        {
            get { return (double)GetValue(LeftContactDistProperty); }
            set { SetValue(LeftContactDistProperty, value); }
        }

        public static readonly DependencyProperty LeftContactDistProperty =
            DependencyProperty.Register("LeftContactDist", typeof(double), typeof(CommitNodeView));

        public double RightContactDist
        {
            get { return (double)GetValue(RightContactDistProperty); }
            set { SetValue(RightContactDistProperty, value); }
        }

        public static readonly DependencyProperty RightContactDistProperty =
            DependencyProperty.Register("RightContactDist", typeof(double), typeof(CommitNodeView));


        public static readonly DependencyProperty PlusButtonProperty =
            DependencyProperty.Register("PlusButton", typeof(bool), typeof(CommitNodeView), new PropertyMetadata(true));

        public bool PlusButton
        {
            get { return (bool)GetValue(PlusButtonProperty); }
            set { SetValue(PlusButtonProperty, value); }
        }

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

        public CommitNodeView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            OnFocusedChanged();
            OnMarkedChanged();
            OnCheckoutedChanged();
            UpdateShape();
            UpdateMargins();
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
            Binding b5 = new Binding("PlusButton");
            b5.Source = DataContext;
            SetBinding(PlusButtonProperty, b5);
            Binding b6 = new Binding("TextStartDist");
            b6.Source = DataContext;
            SetBinding(TextStartDistProperty, b6);
            Binding b7 = new Binding("LeftContactDist");
            b7.Source = DataContext;
            SetBinding(LeftContactDistProperty, b7);
            Binding b8 = new Binding("RightContactDist");
            b8.Source = DataContext;
            SetBinding(RightContactDistProperty, b8);
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
