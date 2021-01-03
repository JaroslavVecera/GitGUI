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
    /// Interakční logika pro BranchLabelView.xaml
    /// </summary>
    public partial class BranchLabelView : UserControl
    {
        public void OnFocusedChanged()
        {

        }

        public void OnMarkedChanged()
        {

        }

        public void OnCheckoutedChanged()
        {

        }

        public bool Focused
        {
            get { return (bool)GetValue(FocusedProperty); }
        }

        public static readonly DependencyProperty FocusedProperty =
            DependencyProperty.Register("Focused", typeof(bool), typeof(BranchLabelView));

        public MouseButtonEventArgs MouseButtonArgs
        {
            get { return (MouseButtonEventArgs)GetValue(MouseButtonArgsProperty); }
            set { SetValue(MouseButtonArgsProperty, value); }
        }

        public static readonly DependencyProperty MouseButtonArgsProperty =
            DependencyProperty.Register("MouseButtonArgs", typeof(MouseButtonEventArgs), typeof(BranchLabelView));

        public MouseEventArgs MouseArgs
        {
            get { return (MouseEventArgs)GetValue(MouseArgsProperty); }
            set { SetValue(MouseArgsProperty, value); }
        }

        public static readonly DependencyProperty MouseArgsProperty =
            DependencyProperty.Register("MouseArgs", typeof(MouseEventArgs), typeof(BranchLabelView));

        public BranchLabelView()
        {
            InitializeComponent();
        }

        public void OnLocationChanged(Point p)
        {
            Canvas.SetTop(this, p.Y);
            Canvas.SetLeft(this, p.X);
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
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            MouseArgs = e;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            MouseArgs = e;
        }
    }
}
