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
        public ICommand CloseCommand { get; }

        public TabViewModel(TabModel model)
        {
            CloseCommand = new RelayCommand(() => model.Close(this));
        }
    }
}
