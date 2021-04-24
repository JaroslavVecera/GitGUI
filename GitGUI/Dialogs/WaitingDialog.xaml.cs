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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GitGUI
{
    /// <summary>
    /// Interakční logika pro WaitingDialog.xaml
    /// </summary>
    public partial class WaitingDialog : WindowBase
    {
        public WaitingDialog()
        {
            InitializeComponent();
        }

        public void CloseDialog()
        {
            Close();
            Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        }

        protected override void RefreshMaximizeRestoreButton() { }
    }

    public enum WaitingDialogResult
    {
        OutOfMemory,
        TooMuchCommits,
        Flawless
    }
}
