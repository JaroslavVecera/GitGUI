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
    public class ActionPanelViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ActionButtonViewModel> Buttons { get; } = new ObservableCollection<ActionButtonViewModel>();

        public ActionPanelViewModel(ActionPanelModel model, ActionPanelView view)
        {
            Buttons.CollectionChanged += CollectionChanged;
            SubscribeModel(model);
            view.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Buttons"));
        }

        void SubscribeModel(ActionPanelModel model)
        {
            model.Added += Added;
        }

        void Added(ActionButtonViewModel viewModel)
        {
            Buttons.Add(viewModel);
        }
    }
}
