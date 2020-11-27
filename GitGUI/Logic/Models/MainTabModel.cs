using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GitGUI.Logic
{
    public class MainTabModel : TabModel
    {
        Point _center;
        public event Action UpdateCenter;
        ActionPanelModel _panelModel;
        public event Action<MouseButtonEventArgs> MouseDown;
        public ActionPanelModel PanelModel
        {
            get { return _panelModel; }
            set { _panelModel = value; OnPropertyChanged(); }
        }

        public Point GraphViewCenter { get { UpdateCenter.Invoke(); return _center; } set { _center = value; } }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(e);
        }
    }
}
