using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class MainTabViewModel : TabViewModel
    {
        MainTabModel Model { get; set; }

        public string Header { get { return "Main"; } }

        public ActionPanelViewModel ActionPanel
        { get { return new ActionPanelViewModel(Model.PanelModel); } }

        public MainTabViewModel(MainTabModel model) : base(model)
        {
            Model = model;
            SubscribeModel(model);
        }

        void SubscribeModel(MainTabModel model)
        {

        }
    }
}
