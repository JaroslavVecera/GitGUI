﻿using System;
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
        public void OpenContextMenu()
        {
            menu.IsOpen = true;
        }

        public void OpenNoAggregationContextMenu()
        {
            MessageBox.Show("Cannot merge or rebase when there are conflicts in repository.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public MainWindow()
        {
        }

        private void WindowPreviewMouseMove(object sender, MouseEventArgs e)
        {
            Program.GetInstance().OnMouseMove(null, e);
        }

        private void WindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            Program.GetInstance().OnWindowMouseUp(null, e);
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.GetInstance().OnWindowMouseDown(null, e);
        }

        private void WindowPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Program.GetInstance().OnMouseWheel(e);
        }

        private void WindowMouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void ContextMenuMerge(object sender, RoutedEventArgs e)
        {
            Program.GetInstance().Merge();
        }

        private void ContextMenuRebase(object sender, RoutedEventArgs e)
        {
            Program.GetInstance().Rebase();
        }

        private void OpenRepository(object sender, RoutedEventArgs e)
        {
            Program.GetInstance().OpenRepository();
        }

        private void OnChangedUser(object sender, RoutedEventArgs e)
        {
            ChangedUserEventArgs args = (ChangedUserEventArgs)e;
            Program.GetInstance().ChangeUser(args.User);
        }
    }
}
