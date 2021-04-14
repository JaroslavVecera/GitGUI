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

        public RelayCommand MouseUpCommand
        {
            get { return (RelayCommand)GetValue(MouseUpCommandProperty); }
        }

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.Register("MouseUp", typeof(RelayCommand), typeof(MainTabView));

        public double WidthP
        {
            set { Console.Write(value); }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            MouseButtonArgs = e;
            MouseDownCommand.Execute(null);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            MouseButtonArgs = e;
            MouseUpCommand.Execute(null);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null)
                ((MainTabViewModel)DataContext).ScrollViewer = graphView;
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
            SetBinding(MouseDownCommandProperty, b2);
            Binding b3 = new Binding("MouseUp");
            b3.Mode = BindingMode.OneWay;
            b3.Source = DataContext;
            SetBinding(MouseUpCommandProperty, b3);
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Program.GetInstance().OnMouseWheel(e);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Graph.GetInstance().Size = e.NewSize;
        }

        private void OnGraphViewLayoutUpdated(object sender, EventArgs e)
        {
            if (VisualTreeHelper.GetParent(this) == null)
                return;
            Graph.GetInstance().Position = graphView.TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new Point(0, 0));
            Graph.GetInstance().CheckBoundaries();
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = LibGitService.GetInstance().CurrentRepositoryPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
