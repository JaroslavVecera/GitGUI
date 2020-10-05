using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class ActionButtonViewModel
    {
        public ActionButtonModel Model { get; set; }

        public ActionButtonViewModel(ActionButtonModel model, Button view)
        {
            view.DataContext = this;
            Model = model;
        }
    }
}
