using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class StashMenuViewModel : ViewModelBase
    {
        Tuple<string, string> _selected = null;
        StashMenuModel Model { get; set; }

        public Tuple<string, string> Selected { get { return _selected; } set { _selected = value; SelectedChanged(); } }

        public RelayCommand Apply { get; private set; }
        public RelayCommand Pop { get; private set; }
        public RelayCommand Delete { get; private set; }

        public IEnumerable<Tuple<string, string>> Stashes { get { return Model.Stashes; } }

        public StashMenuViewModel(StashMenuModel model)
        {
            Model = model;
            Model.PropertyChanged += OnPropertyChanged;
            InitializeCommands();
        }

        void SelectedChanged()
        {
            Apply.RaiseCanExecuteChanged();
            Pop.RaiseCanExecuteChanged();
            Delete.RaiseCanExecuteChanged();
        }

        void InitializeCommands()
        {
             Apply = new RelayCommand(() => { Model.Apply(Selected.Item2); }, () => Selected != null);
             Delete = new RelayCommand(() => { Model.Delete(Selected.Item2); }, () => Selected != null);
             Pop = new RelayCommand(() => { Model.Pop(Selected.Item2); }, () => Selected != null);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
    }
}
