using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitGUI.Logic;

namespace GitGUI
{
    public partial class MainWindow : WindowBase
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
            Closing += (object sender, System.ComponentModel.CancelEventArgs e) => CloseRepository(sender, null); ;
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

        private void WindowMouseLeave(object sender, MouseEventArgs e)
        {
            Program.GetInstance().MouseLeaveWindow();
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

        private void CreateRepository(object sender, RoutedEventArgs e)
        {
            Program.GetInstance().CreateRepository();
        }

        private void CloneRepository(object sender, RoutedEventArgs e)
        {
            Program.GetInstance().CloneRepository();
        }

        private void OnChangedUser(object sender, RoutedEventArgs e)
        {
            ChangedUserEventArgs args = (ChangedUserEventArgs)e;
            Program.GetInstance().ChangeUser(args.User);
        }

        private void CloseRepository(object sender, RoutedEventArgs e)
        {
            Program.GetInstance().CloseCurrentRepository();
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Graph.GetInstance().Contains(e.GetPosition(this)))
                WindowMouseUp(sender, e);
        }

        protected override void RefreshMaximizeRestoreButton()
        { 
            if (WindowState == WindowState.Maximized)
            {
                maximizeButton.Visibility = Visibility.Collapsed;
                restoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                maximizeButton.Visibility = Visibility.Visible;
                restoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private void OnChangedRemote(object sender, RoutedEventArgs e)
        {

        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
