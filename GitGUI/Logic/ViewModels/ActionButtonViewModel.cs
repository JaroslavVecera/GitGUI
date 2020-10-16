using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    public class ActionButtonViewModel : ViewModelBase
    {
        public ActionButtonModel Model { get; set; }
        public RelayCommand Clicked { get; }
        public string Text { get { return Model.Text; } }

        public ActionButtonViewModel(ActionButtonModel model)
        {
            Model = model;
            SubscribeModel(model);
            Clicked = new RelayCommand(OnClicked);
        }

        void SubscribeModel(ActionButtonModel m)
        {
            m.PropertyChanged += OnPropertyChanged;
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        void OnClicked()
        {
            Model.OnClicked(this, null);
        }
    }
}
