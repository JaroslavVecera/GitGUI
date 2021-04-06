using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GitGUI.Logic
{
    public class ActionButtonViewModel : ViewModelBase
    {
        public ActionButtonModel Model { get; set; }
        public RelayCommand Clicked { get; }
        public string Text { get { return Model.Text; } }
        public double Width { get { return Model.Width; } }
        public Geometry PathData {get {return Model.PathData; } }

        public ActionButtonViewModel(ActionButtonModel model)
        {
            Model = model;
            SubscribeModel(model);
            Clicked = new RelayCommand(OnClicked, () => Model.Active);
        }

        void SubscribeModel(ActionButtonModel m)
        {
            m.PropertyChanged += OnPropertyChanged;
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName == "Active")
                Clicked.RaiseCanExecuteChanged();
        }

        void OnClicked()
        {
            Model.OnClicked(this, null);
        }
    }
}
