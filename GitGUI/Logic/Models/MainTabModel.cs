using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class MainTabModel : TabModel
    {
        ActionPanelModel _panelModel;
        public ActionPanelModel PanelModel
        {
            get { return _panelModel; }
            set { _panelModel = value; OnPropertyChanged(); }
        }
    }
}
