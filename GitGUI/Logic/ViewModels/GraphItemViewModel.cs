using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class GraphItemViewModel : ViewModelBase
    {
        UserControl _control;

        public GraphItemViewModel(UserControl view)
        {
            _control = view;
            InitializeLocation(view);
        }

        void InitializeLocation(UserControl c)
        {
            Canvas.SetLeft(c, 0);
            Canvas.SetTop(c, 0);
        }

        void BackgroundPush()
        {
            Panel.SetZIndex(_control, 0);
        }

        void ForegroundPull()
        {
            Panel.SetZIndex(_control, 1);
        }
    }
}
