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
        public event Action ShownChanged;
        ActionPanelModel _panelModel;
        public event Action<MouseButtonEventArgs> MouseDown;
        public ActionPanelModel PanelModel
        {
            get { return _panelModel; }
            set { _panelModel = value; OnPropertyChanged(); }
        }
        GraphItemModel _shown = null;
        public GraphItemModel Shown { get { return _shown; } set { _shown = value; ShownChanged?.Invoke(); } }

        public Point GraphViewCenter { get { UpdateCenter.Invoke(); return _center; } set { _center = value; } }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(e);
        }
    }
}
