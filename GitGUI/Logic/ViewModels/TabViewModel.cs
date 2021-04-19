using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitGUI.Logic
{
    public abstract class TabViewModel : ViewModelBase
    { 
        public string Header { get { return new string(Model.Header.Take(30).ToArray()); } }
        public ICommand CloseCommand { get; }
        public virtual TabModel Model { get; private set; }

        public virtual bool CloseButton { get { return true; } }

        public TabViewModel(TabModel model)
        {
            Model = model;
            CloseCommand = new RelayCommand(() => model.Close(this));
        }
    }
}
