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

        private void OpenStashingMenu(object sender, MouseButtonEventArgs e)
        {
            popup.IsOpen = true;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RefreshMaximizeRestoreButton()
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

        private void WindowStateChanged(object sender, EventArgs e)
        {
            RefreshMaximizeRestoreButton();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
        }

        public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                // We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
                // including the task bar.
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                // Adjust the maximized size and position to fit the work area of the correct monitor
                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                    GetMonitorInfo(monitor, ref monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            return IntPtr.Zero;
        }

        private const int WM_GETMINMAXINFO = 0x0024;

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            RefreshMaximizeRestoreButton();
        }
    }
}
