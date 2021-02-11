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
        public string Header { get { return Model.Header; } }
        public ICommand CloseCommand { get; }
        public virtual TabModel Model { get; private set; }

        public TabViewModel(TabModel model)
        {
            Model = model;
            CloseCommand = new RelayCommand(() => model.Close(this));
        }
    }
}
