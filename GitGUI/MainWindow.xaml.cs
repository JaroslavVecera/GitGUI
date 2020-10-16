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
using GitGUI.Logic;

namespace GitGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Resources.MergedDictionaries.Clear();
            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source =
                new Uri(";component/Themes/DarkTheme.xaml", UriKind.RelativeOrAbsolute);
            Resources.MergedDictionaries.Add(myResourceDictionary);
        }

        public void ShowNodePanel()
        {
            nodePanel.Visibility = Visibility.Visible;
        }

        public void HideNodePanel()
        {
            nodePanel.Visibility = Visibility.Collapsed;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {/*
            if (e.Source == graphView)
                Program.OnMouseDown(null, e);*/
        }

        private void WindowPreviewMouseMove(object sender, MouseEventArgs e)
        {
            /*Node n = null;
            if (sender is Node)
                n = (Node)sender;
            Program.OnMouseMove(n, e);*/
        }

        private void WindowPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            /*Node n = null;
            if (sender is Node)
                n = (Node)sender;
            Program.OnMouseUp(n, e);*/
        }

        private void WindowPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private void WindowMouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Resources.MergedDictionaries.Clear();
            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source =
                new Uri(";component/Themes/LightTheme.xaml", UriKind.RelativeOrAbsolute);
            Resources.MergedDictionaries.Add(myResourceDictionary);
        }

        private void ContextMenuMerge(object sender, RoutedEventArgs e)
        {
        }

        private void ContextMenuRebase(object sender, RoutedEventArgs e)
        {
        }
    }
}
