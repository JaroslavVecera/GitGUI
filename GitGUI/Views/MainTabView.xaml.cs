using GitGUI.Logic;
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
    /// Interakční logika pro MainTab.xaml
    /// </summary>
    public partial class MainTabView : UserControl
    {
        bool _first = true;
        public MainTabView()
        {
            InitializeComponent();
            AddHandler(MouseDownEvent, (MouseButtonEventHandler)OnMouseDown, true);
        }

        public MouseButtonEventArgs MouseButtonArgs
        {
            get { return (MouseButtonEventArgs)GetValue(MouseButtonArgsProperty); }
            set { SetValue(MouseButtonArgsProperty, value); }
        }

        public static readonly DependencyProperty MouseButtonArgsProperty =
            DependencyProperty.Register("MouseButtonArgs", typeof(MouseButtonEventArgs), typeof(MainTabView));

        public RelayCommand MouseDownCommand
        {
            get { return (RelayCommand)GetValue(MouseDownCommandProperty); }
        }

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register("MouseDown", typeof(RelayCommand), typeof(MainTabView));

        public double WidthP
        {
            set { Console.Write(value); }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Grid))
                return;
            MouseButtonArgs = e;
            MouseDownCommand.Execute(null);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_first)
                return;
            _first = false;
            Binding b = new Binding("MouseButtonArgs");
            b.Mode = BindingMode.OneWayToSource;
            b.Source = DataContext;
            SetBinding(MouseButtonArgsProperty, b);
            Binding b2 = new Binding("MouseDown");
            b2.Mode = BindingMode.OneWay;
            b2.Source = DataContext;
            if (DataContext != null)
            ((MainTabViewModel)DataContext).ScrollViewer = graphView;
            SetBinding(MouseDownCommandProperty, b2);
        }
    }
}
