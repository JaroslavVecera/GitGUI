using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace GitGUI.Logic
{
    public class ActionPanelViewModel : ViewModelBase
    {
        ActionPanelModel Model { get; set; }
        public List<ActionButtonViewModel> Buttons { get { return Model.ButtonViewModels; } }

        public ActionPanelViewModel(ActionPanelModel model)
        {
            Model = model;
            SubscribeModel(model);
        }

        void SubscribeModel(ActionPanelModel model)
        {
            model.Added += Added;
        }

        void Added(ActionButtonViewModel viewModel)
        {
            OnPropertyChanged("Buttons");
        }
    }
}
